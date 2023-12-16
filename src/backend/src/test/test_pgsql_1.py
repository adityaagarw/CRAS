from tkinter import *
from tkinter import ttk, simpledialog
from tkinter.ttk import Combobox
import psycopg2
from psycopg2.extras import RealDictCursor

class SQLTableEditor:
    def __init__(self, root):
        self.root = root
        self.table = None
        self.db_conn = psycopg2.connect(database="localdb", user="cras_admin", password="admin", host="127.0.0.1", cursor_factory=RealDictCursor)

        self.root.grid_columnconfigure(0, weight=1)
        self.root.grid_rowconfigure(0, weight=1)

        self.table_frame = Frame(root)
        self.table_frame.grid(row=0, column=0, sticky="nsew")

        self.vsb = ttk.Scrollbar(self.table_frame, orient="vertical")
        self.hsb = ttk.Scrollbar(self.table_frame, orient="horizontal")

        self.choose_table()

        self.query_entry = Entry(self.root)
        self.query_entry.grid(row=1, column=0, sticky="ew")

        self.query_btn = Button(self.root, text='Execute Query', command=self.execute_query)
        self.query_btn.grid(row=2, column=0, sticky="ew")

        self.commit_btn = Button(self.root, text='Commit', command=self.commit_changes)
        self.commit_btn.grid(row=3, column=0, sticky="ew")

        self.refresh_btn = Button(self.root, text='Refresh', command=self.refresh_table)
        self.refresh_btn.grid(row=4, column=0, sticky="ew")

        self.add_row_btn = Button(self.root, text='Add Row', command=self.add_row)
        self.add_row_btn.grid(row=5, column=0, sticky="ew")

        self.delete_row_btn = Button(self.root, text='Delete Row', command=self.delete_row)
        self.delete_row_btn.grid(row=6, column=0, sticky="ew")

    def choose_table(self):
        cursor = self.db_conn.cursor()
        cursor.execute("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'")
        tables = [table['table_name'] for table in cursor.fetchall()]
        self.table_combobox = Combobox(self.root, values=tables)
        self.table_combobox.grid(row=7, column=0, sticky="ew")
        self.table_combobox.bind("<<ComboboxSelected>>", self.initialize_table)
        cursor.close()

    def initialize_table(self, event=None):
        self.table_name = self.table_combobox.get()
        self.table_combobox.grid_forget()

        cursor = self.db_conn.cursor()
        cursor.execute(f"SELECT * FROM {self.table_name} LIMIT 0")
        self.columns = [desc[0] for desc in cursor.description]
        cursor.execute(f"SELECT * FROM {self.table_name}")
        self.data = cursor.fetchall()
        cursor.close()

        # Ask user to select the primary key
        self.primary_key_combobox = Combobox(self.root, values=self.columns)
        self.primary_key_combobox.grid(row=7, column=0, sticky="ew")
        self.primary_key_combobox.bind("<<ComboboxSelected>>", self.set_primary_key_and_create_table)

    def set_primary_key_and_create_table(self, event=None):
        self.primary_key = self.primary_key_combobox.get()
        self.primary_key_combobox.grid_forget()
        self.create_table()

    def create_table(self):
        if self.table is not None:
            self.table.grid_forget()
            self.vsb.grid_forget()
            self.hsb.grid_forget()

        self.table = ttk.Treeview(self.table_frame, columns=self.columns, show='headings', yscrollcommand=self.vsb.set, xscrollcommand=self.hsb.set)
        for column in self.columns:
            self.table.heading(column, text=column)
            self.table.column(column, width=100)

        for item in self.data:
            self.table.insert('', 'end', values=list(item.values()))

        self.vsb.config(command=self.table.yview)
        self.hsb.config(command=self.table.xview)

        self.table.grid(row=0, column=0, sticky="nsew")
        self.vsb.grid(row=0, column=1, sticky="ns")
        self.hsb.grid(row=1, column=0, sticky="ew")

        self.table.bind('<Double-1>', self.OnDoubleClick)

    def execute_query(self):
        cursor = self.db_conn.cursor()
        query = self.query_entry.get()
        cursor.execute(query)
        result = cursor.fetchall()
        print(result)
        cursor.close()

    def commit_changes(self):
        for item in self.table.get_children():
            values = list(self.table.item(item)['values'])
            for i in range(len(values)):
                if isinstance(values[i], str) and values[i].startswith('{') and values[i].endswith('}'):
                    values[i] = values[i].strip("{}").split(",")
            update_values = ", ".join(f"{col} = '{val}'" for col, val in zip(self.columns, values))
            query = f"UPDATE {self.table_name} SET {update_values} WHERE {self.primary_key} = '{values[0]}'"
            cursor = self.db_conn.cursor()
            cursor.execute(query)
        self.db_conn.commit()
        self.refresh_table()

    def refresh_table(self):
        self.initialize_table()

    def add_row(self):
        self.table.insert('', 'end')

    def delete_row(self):
        selected_items = self.table.selection()
        for selected_item in selected_items:
            cursor = self.db_conn.cursor()
            query = f"DELETE FROM {self.table_name} WHERE {self.primary_key} = '{self.table.item(selected_item)['values'][0]}'"
            cursor.execute(query)
        self.db_conn.commit()
        self.refresh_table()

    def OnDoubleClick(self, event):
        item = self.table.identify('item',event.x,event.y)
        column = self.table.identify('column', event.x, event.y)
        value = simpledialog.askstring("Input", f"Enter new value for {self.columns[int(column[1:])-1]}",
                                        parent=self.root)
        self.table.set(item, column, value)


root = Tk()
app = SQLTableEditor(root)
root.mainloop()