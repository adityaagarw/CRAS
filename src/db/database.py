import abc
import psycopg2
from psycopg2.extensions import adapt, register_adapter
import redis
import uuid

# Register UUID adapter for psycopg2
register_adapter(uuid.UUID, lambda u: adapt(str(u)).getquoted())

# Abstract class for the database
class Database(abc.ABC):
    def __init__(self, host, port):
        self.host = host
        self.port = port
        self.connection = None

    def __exit__(self, exc_type, exc_value, traceback):
        self.disconnect()

    def connect(self):
        pass

    def disconnect(self):
        if self.connection is not None:
            self.connection.close()

    # Add other common methods here...


# Customer data structure for local PostgreSQL database
class LocalCustomer:
    def __init__(self, customer_id, name, phone_number, encoding, image,
                 return_customer=False, last_visit=None, average_time_spent=None,
                 average_purchase=None, maximum_purchase=None, remarks=None,
                 loyalty_level=None, num_visits=0, last_location=None,
                 location_list=None, category=None):
        self.customer_id = customer_id
        self.name = name
        self.phone_number = phone_number
        self.encoding = encoding
        self.image = image
        self.return_customer = return_customer
        self.last_visit = last_visit
        self.average_time_spent = average_time_spent
        self.average_purchase = average_purchase
        self.maximum_purchase = maximum_purchase
        self.remarks = remarks
        self.loyalty_level = loyalty_level
        self.num_visits = num_visits
        self.last_location = last_location
        self.location_list = location_list
        self.category = category

class LocalEmployee:
    def __init__(self, name, phone_number, face_image, face_encoding):
        self.name = name
        self.phone_number = phone_number
        self.face_image = face_image
        self.face_encoding = face_encoding

# Class for the local PostgreSQL database
class LocalPostgresDB(Database):
    def __init__(self, host, port, database, user, password):
        super().__init__(host, port)
        self.database = database
        self.user = user
        self.password = password
        self.cursor = None

    def create_table_if_not_exists(self):

        create_pgvector_extension_query = """
        CREATE EXTENSION IF NOT EXISTS vector
        """
        self.cursor.execute(create_pgvector_extension_query)
        self.connection.commit()

        create_table_query = """
        CREATE TABLE IF NOT EXISTS local_customer_db (
            customer_id UUID PRIMARY KEY,
            name VARCHAR(255),
            phone_number VARCHAR(20),
            encoding vector(2048),
            image BYTEA,
            return_customer BOOLEAN DEFAULT FALSE,
            last_visit TIMESTAMP,
            average_time_spent INTERVAL,
            average_purchase NUMERIC(10, 2),
            maximum_purchase NUMERIC(10, 2),
            remarks TEXT,
            loyalty_level VARCHAR(50),
            num_visits INTEGER DEFAULT 0,
            last_location VARCHAR(255),
            location_list TEXT[],
            category VARCHAR(255)
        )
        """
        self.cursor.execute(create_table_query)
        self.connection.commit()

        create_table_query = """
        CREATE TABLE IF NOT EXISTS local_employee_db (
            employee_id UUID PRIMARY KEY,
            name VARCHAR(255),
            phone_number VARCHAR(20),
            face_image BYTEA,
            face_encoding vector(2048)
        )
        """
        self.cursor.execute(create_table_query)
        self.connection.commit()


    def insert_customer_record(self, record):
        with self.connection.cursor() as cursor:
            insert_query = """
            INSERT INTO local_customer_db (
                customer_id, name, phone_number, encoding, image, return_customer, last_visit, 
                average_time_spent, average_purchase, maximum_purchase, remarks, 
                loyalty_level, num_visits, last_location, location_list, category
            )
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
            """
            cursor.execute(insert_query, (
                record.customer_id, record.name, record.phone_number, record.encoding, record.image,
                record.return_customer, record.last_visit, record.average_time_spent,
                record.average_purchase, record.maximum_purchase, record.remarks,
                record.loyalty_level, record.num_visits, record.last_location,
                record.location_list, record.category
            ))
            self.connection.commit()

    def insert_employee_record(self, record):
        insert_query = """
        INSERT INTO local_employee_db (
            employee_id, name, phone_number, face_image, face_encoding
        )
        VALUES (%s, %s, %s, %s, %s)
        """
        self.cursor.execute(insert_query, (
            record.employee_id, record.name, record.phone_number, record.face_image, record.face_encoding
        ))
        self.connection.commit()

    def update_customer_record(self, record):
        with self.connection.cursor() as cursor:
            update_query = """
            UPDATE local_customer_db
            SET name = %s, phone_number = %s, encoding = %s, image = %s, 
                return_customer = %s, last_visit = %s, average_time_spent = %s, 
                average_purchase = %s, maximum_purchase = %s, remarks = %s, 
                loyalty_level = %s, num_visits = %s, last_location = %s, 
                location_list = %s, category = %s
            WHERE customer_id = %s
            """
            cursor.execute(update_query, (
                record.name, record.phone_number, record.encoding, record.image,
                record.return_customer, record.last_visit, record.average_time_spent,
                record.average_purchase, record.maximum_purchase, record.remarks,
                record.loyalty_level, record.num_visits, record.last_location,
                record.location_list, record.category, record.customer_id
            ))
            self.connection.commit()

    def update_employee_record(self, record):
        update_query = """
        UPDATE local_employee_db
        SET name = %s, phone_number = %s, face_image = %s, face_encoding = %s
        WHERE name = %s
        """
        self.cursor.execute(update_query, (
            record.name, record.phone_number, record.face_image, record.face_encoding, record.name
        ))
        self.connection.commit()


    def delete_customer_record(self, record_id):
        with self.connection.cursor() as cursor:
            delete_query = """
            DELETE FROM local_customer_db
            WHERE customer_id = %s
            """
            cursor.execute(delete_query, (record_id,))
            self.connection.commit()

    def delete_employee_record(self, record_name):
        delete_query = """
        DELETE FROM local_employee_db
        WHERE name = %s
        """
        self.cursor.execute(delete_query, (record_name,))
        self.connection.commit()

    def connect(self):
        self.connection = psycopg2.connect(
            host=self.host,
            port=self.port,
            database=self.database,
            user=self.user,
            password=self.password
        )
        self.cursor = self.connection.cursor()

#####################################################################################################
# Customer data structure for in-memory Redis database
class InMemCustomer:
    def __init__(self, customer_id, name, phone_number, encoding, image,
                 return_customer=False, last_visit=None, average_time_spent=None,
                 average_purchase=None, maximum_purchase=None, remarks=None,
                 loyalty_level=None, num_visits=0, last_location=None,
                 location_list=None, category=None, entry_time=None,
                 billed=False, exited=None, visit_time=None, exit_time=None):
        self.customer_id = customer_id
        self.name = name
        self.phone_number = phone_number
        self.encoding = encoding
        self.image = image
        self.return_customer = return_customer
        self.last_visit = last_visit
        self.average_time_spent = average_time_spent
        self.average_purchase = average_purchase
        self.maximum_purchase = maximum_purchase
        self.remarks = remarks
        self.loyalty_level = loyalty_level
        self.num_visits = num_visits
        self.last_location = last_location
        self.location_list = location_list
        self.category = category
        self.entry_time = entry_time
        self.billed = billed
        self.exited = exited
        self.visit_time = visit_time
        self.exit_time = exit_time


# Class for the in-memory Redis database
class InMemoryRedisDB(Database):
    def __init__(self, host, port):
        super().__init__(host, port)
        self.connection = None

    def insert_record(self, record):
        with self.connection.pipeline() as pipe:
            pipe.hmset(f'customer_inmem_db:{record.customer_id}', vars(record))
            pipe.execute()

    def update_record(self, record):
        with self.connection.pipeline() as pipe:
            pipe.hmset(f'customer_inmem_db:{record.customer_id}', vars(record))
            pipe.execute()

    def delete_record(self, record_id):
        # Delete the record by its key
        self.connection.delete(f'customer_inmem_db:{record_id}')

    def connect(self):
        self.connection = redis.Redis(host=self.host, port=self.port)


# Employee data structure for in-memory Redis database
class InMemEmployee:
    def __init__(self, employee_id, name, phone_number, face_image, face_encoding,
                 entry_time=None, exit_time=None, num_exits=0):
        self.employee_id = employee_id
        self.name = name
        self.phone_number = phone_number
        self.face_image = face_image
        self.face_encoding = face_encoding
        self.entry_time = entry_time
        self.exit_time = exit_time
        self.num_exits = num_exits

# Class for the in-memory Redis database for employees
class InMemoryRedisEmployeeDB(Database):
    def __init__(self, host, port):
        super().__init__(host, port)
        self.connection = None

    def create_table_if_not_exists(self):
        # Redis is schema-less, so no explicit table creation is required
        pass

    def insert_record(self, record):
        with self.connection.pipeline() as pipe:
            pipe.hmset(f'employee_inmem_db:{record.employee_id}', vars(record))
            pipe.execute()

    def update_record(self, record):
        with self.connection.pipeline() as pipe:
            pipe.hmset(f'employee_inmem_db:{record.employee_id}', vars(record))
            pipe.execute()

    def delete_record(self, record_id):
        # Delete the record by its key
        self.connection.delete(f'employee_inmem_db:{record_id}')

    def connect(self):
        self.connection = redis.Redis(host=self.host, port=self.port)


# Class for mapping and inserting in-memory records to the local database
class MapInMemtoLocal:
    def __init__(self, local_db):
        self.local_db = local_db

    def map_customer_inmem_to_local(self, inmem_customer):
        local_customer = LocalCustomer(
            customer_id=inmem_customer.customer_id,
            name=inmem_customer.name,
            phone_number=inmem_customer.phone_number,
            encoding=inmem_customer.encoding,
            image=inmem_customer.image,
            return_customer=inmem_customer.return_customer,
            last_visit=inmem_customer.last_visit,
            average_time_spent=inmem_customer.average_time_spent,
            average_purchase=inmem_customer.average_purchase,
            maximum_purchase=inmem_customer.maximum_purchase,
            remarks=inmem_customer.remarks,
            loyalty_level=inmem_customer.loyalty_level,
            num_visits=inmem_customer.num_visits,
            last_location=inmem_customer.last_location,
            location_list=inmem_customer.location_list,
            category=inmem_customer.category
        )
        self.local_db.insert_record(local_customer)

    def map_employee_inmem_to_local(self, inmem_employee):
        local_employee = LocalEmployee(
            employee_id = inmem_employee.employee_id,
            name=inmem_employee.name,
            phone_number=inmem_employee.phone_number,
            face_image=inmem_employee.face_image,
            face_encoding=inmem_employee.face_encoding
        )
        self.local_db.insert_record(local_employee)

    def fetch_customer_inmem_from_redis(self, customer_id):
        record = self.redis_db.connection.hgetall(f'customer_inmem_db:{customer_id}')
        if record:
            return {key.decode(): ast.literal_eval(value.decode()) for key, value in record.items()}
        return None

    def fetch_employee_inmem_from_redis(self, employee_id):
        record = self.redis_db.connection.hgetall(f'employee_inmem_db:{employee_id}')
        if record:
            return {key.decode(): ast.literal_eval(value.decode()) for key, value in record.items()}
        return None
