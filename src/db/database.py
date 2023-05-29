import psycopg2

class Database:
    def __init__(self, dbname, user, password, host, port):
        self.dbname = dbname
        self.user = user
        self.password = password
        self.host = host
        self.port = port
        self.conn = None
        self.cursor = None

    def connect(self):
        raise NotImplementedError("Subclasses must implement the connect method.")

    def insert_vector(self, vector):
        raise NotImplementedError("Subclasses must implement the insert_vector method.")

    def query_vectors(self, query_vector, threshold):
        raise NotImplementedError("Subclasses must implement the query_vectors method.")

    def close(self):
        if self.cursor:
            self.cursor.close()
        if self.conn:
            self.conn.close()


class PostgreSQLDatabase(Database):
    def initial_setup(self):
        # Create user
        # Grant privilges
        # Create db

    def connect(self):
        self.conn = psycopg2.connect(database=self.dbname, 
                                     user=self.user,
                                     password=self.password,
                                     host=self.host,
                                     port=self.port)
        self.cursor = self.conn.cursor()

    def insert_vector(self, vector):
        self.cursor.execute("INSERT INTO face_encodings (encoding) VALUES (%s);", (vector,))
        self.conn.commit()

    def query_vectors(self, query_vector, threshold):
        self.cursor.execute("SELECT id, encoding FROM face_encodings WHERE cos_distance(encoding, %s) < %s;",
                            (query_vector, threshold))
        return self.cursor.fetchall()
