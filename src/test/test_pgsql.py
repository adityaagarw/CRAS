import psycopg2
from tkinter import *
from tkinter import ttk
from tkinter import simpledialog

class SQLTableEditor:
    def __init__(self, root):
        self.root = root
        self.table = None
        self.db_conn = psycopg2.connect(database="localdb", user="cras_admin", password="admin", host="127.0.0.1")

        self.vsb = ttk.Scrollbar(self.root, orient="vertical")
        self.hsb = ttk.Scrollbar(self.root, orient="horizontal")

        self.initialize_table()

        self.query_entry = Entry(self.root)
        self.query_entry.pack()

        self.query_btn = Button(self.root, text='Execute Query', command=self.execute_query)
        self.query_btn.pack()

        self.commit_btn = Button(self.root, text='Commit', command=self.commit_changes)
        self.commit_btn.pack()

        self.refresh_btn = Button(self.root, text='Refresh', command=self.refresh_table)
        self.refresh_btn.pack()

    def get_table_name(self):
        self.table_name = simpledialog.askstring("Table Name", "Enter table name:")

    def get_primary_key(self):
        self.primary_key = simpledialog.askstring("Primary Key", "Enter primary key column name:")
        cursor = self.db_conn.cursor()
        cursor.execute(f"SELECT kcu.column_name \
                        FROM information_schema.table_constraints tco \
                        JOIN information_schema.key_column_usage kcu \
                            ON kcu.constraint_name = tco.constraint_name \
                        WHERE kcu.table_name = '{self.table_name}' AND tco.constraint_type = 'PRIMARY KEY';")
        primary_keys = [col[0] for col in cursor.fetchall()]
        cursor.close()
        if self.primary_key not in primary_keys:
            raise Exception("Invalid primary key")

    def get_columns_and_types(self):
        cursor = self.db_conn.cursor()
        cursor.execute(f"SELECT column_name, data_type FROM information_schema.columns WHERE table_name = '{self.table_name}' ORDER BY ordinal_position;")
        self.columns, self.types = zip(*cursor.fetchall())
        print(self.columns, self.types) # DEBUG
        cursor.close()

    def get_data(self):
        cursor = self.db_conn.cursor()
        cursor.execute(f"SELECT * FROM {self.table_name};")
        self.data = cursor.fetchall()
        cursor.close()

    def initialize_table(self):
        self.get_table_name()
        self.get_primary_key()
        self.create_table()

    def create_table(self):
        self.get_columns_and_types()
        self.get_data()

        if self.table is not None:
            self.table.destroy()

        self.table = ttk.Treeview(self.root, columns=self.columns, show='headings', yscrollcommand=self.vsb.set, xscrollcommand=self.hsb.set)

        for column in self.columns:
            self.table.heading(column, text=column)
            self.table.column(column, width=100)

        for row in self.data:
            values = []
            for value, type_ in zip(row, self.types):
                if "ARRAY" in type_ and value is not None:
                    value = "{" + ",".join(map(str, value)) + "}"
                elif isinstance(value, str) and len(value) > 100:
                    value = value[:100] + '...'
                values.append(value)
            self.table.insert('', 'end', values=values)

        self.table.bind('<Double-1>', self.edit_item)

        self.table.pack()
        self.vsb.pack(side='right', fill='y')
        self.hsb.pack(side='bottom', fill='x')

    def refresh_table(self):
        self.create_table()

    def edit_item(self, event):
        item = self.table.selection()[0]
        column = self.table.identify_column(event.x)
        value = simpledialog.askstring("Edit item", "Enter new value:")
        self.table.set(item, column, value)

    def commit_changes(self):
        item = self.table.selection()[0]
        original_values = self.table.item(item, 'values')
        values = list(original_values)  # convert tuple to list for modification

        query = f"UPDATE {self.table_name} SET "

        for i, (column, type_) in enumerate(zip(self.columns, self.types)):
            query += f"{column} = '{values[i]}'"
            if i != len(self.columns) - 1:
                query += ", "

        query += f" WHERE {self.primary_key} = '{values[0]}'"

        cursor = self.db_conn.cursor()
        try:
            cursor.execute(query)
            self.db_conn.commit()
        except Exception as e:
            print(f"Commit failed: {e}")
            self.db_conn.rollback()

        cursor.close()

        self.refresh_table()

    def execute_query(self):
        query = self.query_entry.get()
        cursor = self.db_conn.cursor()

        try:
            cursor.execute(query)
            self.columns = [desc[0] for desc in cursor.description]
            self.data = cursor.fetchall()
            self.create_table()
        except Exception as e:
            print(f"Query failed: {e}")
            self.db_conn.rollback()

        cursor.close()

root = Tk()
app = SQLTableEditor(root)
root.mainloop()