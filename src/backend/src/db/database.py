import abc
import ast
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
                 average_bill_value=None, average_bill_per_visit=None, average_bill_per_billed_visit = None, maximum_purchase=None, remarks=None,
                 loyalty_level=None, num_bills=0, num_visits=0,  num_billed_visits = 0, last_location=None,
                 location_list=None, category=None, creation_date=None, group_id=None, incomplete=True):
        self.customer_id = customer_id
        self.name = name
        self.phone_number = phone_number
        self.encoding = encoding
        self.image = image
        self.return_customer = return_customer
        self.last_visit = last_visit
        self.average_time_spent = average_time_spent
        self.average_bill_value = average_bill_value
        self.average_bill_per_visit = average_bill_per_visit
        self.average_bill_per_billed_visit = average_bill_per_billed_visit 
        self.maximum_purchase = maximum_purchase
        self.remarks = remarks
        self.loyalty_level = loyalty_level
        self.num_bills = num_bills
        self.num_visits = num_visits
        self.num_billed_visits = num_billed_visits
        self.last_location = last_location
        self.location_list = location_list
        self.category = category
        self.creation_date = creation_date
        self.group_id = group_id

class LocalEmployee:
    def __init__(self, employee_id, name, phone_number, face_image, face_encoding):
        self.employee_id = employee_id
        self.name = name
        self.phone_number = phone_number
        self.face_image = face_image
        self.face_encoding = face_encoding

class LocalStore:
    def __init__(self, store_id, location, name, num_entry_cams, num_billing_cams, num_exit_cams,
                 entry_cam, billing_cam, exit_cam):
        self.store_id = store_id
        self.location = location
        self.name = name
        self.num_entry_cams = num_entry_cams
        self.num_billing_cams = num_billing_cams
        self.num_exit_cams = num_exit_cams
        self.entry_cam = entry_cam
        self.billing_cam = billing_cam
        self.exit_cam = exit_cam

class LocalVisit:
    def __init__(self, customer_id, visit_id, store_id, entry_time, exit_time, billed, bill_no, bill_date, bill_amount, return_amount, time_spent, 
                 visit_remark, customer_rating, customer_feedback, incomplete, return_customer):
        self.store_id = store_id
        self.customer_id = customer_id
        self.visit_id = visit_id
        self.entry_time = entry_time
        self.exit_time = exit_time
        self.billed = billed
        self.bill_no = bill_no
        self.bill_date = bill_date
        self.bill_amount = bill_amount
        self.return_amount = return_amount
        self.time_spent = time_spent
        self.visit_remark = visit_remark
        self.customer_rating = customer_rating
        self.customer_feedback = customer_feedback
        self.incomplete = incomplete
        self.return_customer = return_customer

class Session:
    def __init__(self, start_time, end_time, detection, model, threshold, yaw_threshold, pitch_threshold, area_threshold, billing_cam_time, similarity_method,
                 total_faces, same_faces, misidentified_faces, unidentified_faces):
        self.start_time = start_time
        self.end_time = end_time
        self.detection = detection
        self.model = model
        self.threshold = threshold
        self.yaw_threshold = yaw_threshold
        self.pitch_threshold = pitch_threshold
        self.area_threshold = area_threshold
        self.billing_cam_time = billing_cam_time
        self.similarity_method = similarity_method
        self.total_faces = total_faces
        self.same_faces = same_faces
        self.misidentified_faces = misidentified_faces
        self.unidentified_faces = unidentified_faces

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
            return_customer INTEGER DEFAULT 0,
            last_visit TIMESTAMP,
            average_time_spent INTEGER,
            average_bill_value NUMERIC(10, 2),
            average_bill_per_visit NUMERIC(10,2),
            average_bill_per_billed_visit NUMERIC(10,2),
            maximum_purchase NUMERIC(10, 2),
            remarks TEXT,
            loyalty_level VARCHAR(50),
            num_bills INTEGER DEFAULT 0,
            num_visits INTEGER DEFAULT 0,
            num_billed_visits INTEGER DEFAULT 0,
            last_location VARCHAR(255),
            location_list TEXT[],
            category VARCHAR(255),
            creation_date TIMESTAMP,
            group_id NUMERIC
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

        create_table_query = """
        CREATE TABLE IF NOT EXISTS local_store_db (
            store_id UUID PRIMARY KEY,
            location VARCHAR(255),
            name VARCHAR(255),
            num_entry_cams INTEGER,
            num_billing_cams INTEGER,
            num_exit_cams INTEGER,
            entry_cam INTEGER[],
            billing_cam INTEGER[],
            exit_cam INTEGER[]
        )"""
        self.cursor.execute(create_table_query)
        self.connection.commit()

        create_table_query = """
        CREATE TABLE IF NOT EXISTS local_visit_db (
            customer_id UUID,
            visit_id UUID PRIMARY KEY,
            store_id UUID,
            entry_time TIMESTAMP,
            exit_time TIMESTAMP,
            billed INTEGER DEFAULT 0,
            bill_no VARCHAR(255),
            bill_date TIMESTAMP,
            bill_amount NUMERIC(10, 2),
            return_amount NUMERIC(10, 2),
            time_spent INTEGER,
            visit_remark TEXT,
            customer_rating INTEGER,
            customer_feedback INTEGER,
            incomplete INTEGER DEFAULT 1,
            return_customer INTEGER DEFAULT 0
        )"""
        self.cursor.execute(create_table_query)
        self.connection.commit()

        create_table_query = """
        CREATE TABLE IF NOT EXISTS local_billing_db (
            bill_no VARCHAR(255),
            bill_date TIMESTAMP,
            bill_amount NUMERIC(10, 2),
            return_amount NUMERIC(10, 2),
            quantity INTEGER,
            name VARCHAR(255),
            phone_number VARCHAR(10),
            customer_id VARCHAR(255),
            visit_id VARCHAR(255),
            customer_list TEXT[],
            visit_list TEXT[],
            PRIMARY KEY (bill_no, bill_date)
        )
        """
        self.cursor.execute(create_table_query)
        self.connection.commit()

        create_table_query = """
        CREATE TABLE IF NOT EXISTS Log (
            sessionid   INTEGER,
            logtime     TIMESTAMP,
            source      VARCHAR(255),
            module      VARCHAR(255),
            actiontype  VARCHAR(255),
            actionvalue VARCHAR(255),
            logmessage  VARCHAR(2048), 
            line        VARCHAR(255)
        )
        """
        self.cursor.execute(create_table_query)
        self.connection.commit()

        create_table_query = """
        CREATE TABLE IF NOT EXISTS Session (
            sessionid           SERIAL PRIMARY KEY,
            start_time          TIMESTAMP,
            end_time            TIMESTAMP,
            detection           VARCHAR(255),
            model               VARCHAR(255),
            threshold           NUMERIC(10, 2),
            yaw_threshold       INTEGER,
            pitch_threshold     INTEGER,
            area_threshold      INTEGER,
            billing_cam_time    INTEGER,
            similarity_method   VARCHAR(255),
            total_faces         INTEGER DEFAULT 0,
            same_faces          INTEGER DEFAULT 0,
            misidentified_faces INTEGER DEFAULT 0,
            unidentified_faces  INTEGER DEFAULT 0,
            junk_image          INTEGER DEFAULT 0,
            accuracy            NUMERIC(10, 2),
            boot_up_time        NUMERIC(10, 2)
        )
        """
        self.cursor.execute(create_table_query)
        self.connection.commit()

        create_table_query = """
        CREATE TABLE IF NOT EXISTS SubSession (
            sessionid           INTEGER,
            sub_sessionid       INTEGER,
            start_time          TIMESTAMP,
            end_time            TIMESTAMP
        )
        """
        self.cursor.execute(create_table_query)
        self.connection.commit()

    def insert_customer_record_old(self, record):
        with self.connection.cursor() as cursor:
            insert_query = """
            INSERT INTO local_customer_db (
                customer_id, name, phone_number, encoding, image, return_customer, last_visit,
                average_time_spent, average_purchase, maximum_purchase, remarks,
                loyalty_level, num_visits, last_location, location_list, category, creation_date, group_id
            )
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
            """
            cursor.execute(insert_query, (
                record.customer_id, record.name, record.phone_number, record.encoding, record.image,
                record.return_customer, record.last_visit, record.average_time_spent,
                record.average_purchase, record.maximum_purchase, record.remarks,
                record.loyalty_level, record.num_visits, record.last_location,
                record.location_list, record.category, record.creation_date, record.group_id
            ))
            self.connection.commit()

    def insert_customer_record(self, record):
        fields = [
            'customer_id', 'name', 'phone_number', 'encoding', 'image',
            'return_customer', 'last_visit', 'average_time_spent',
            'average_bill_value', 'average_bill_per_visit', 'average_bill_per_billed_visit', 'maximum_purchase', 'remarks',
            'loyalty_level', 'num_bills', 'num_visits', 'num_billed_visits' ,'last_location',
            'location_list', 'category', 'creation_date', 'group_id'
        ]

        with self.connection.cursor() as cursor:
            insert_query = """
            INSERT INTO local_customer_db ({})
            VALUES ({})
            """.format(','.join(fields), ','.join(['%s']*len(fields)))

            values = []
            for field in fields:
                value = getattr(record, field)
                values.append(value if value != "" else None)
            
            cursor.execute(insert_query, tuple(values))
            self.connection.commit()
            #print(insert_query)

    def print_insert_query(self, record):
        insert_query = """
        INSERT INTO local_customer_db (
            customer_id, name, phone_number, encoding, image, return_customer, last_visit,
            average_time_spent, average_purchase, maximum_purchase, remarks,
            loyalty_level, num_visits, last_location, location_list, category, creation_date, group_id
        )
        VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
        """
        query_parameters = (
            record.customer_id, record.name, record.phone_number, record.encoding, record.image,
            record.return_customer, record.last_visit, record.average_time_spent,
            record.average_purchase, record.maximum_purchase, record.remarks,
            record.loyalty_level, record.num_visits, record.last_location,
            record.location_list, record.category, record.creation_date, record.group_id
        )

        print("SQL Query:", self.cursor.mogrify(insert_query, query_parameters))

    def insert_employee_record_old(self, record):
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

    def insert_employee_record(self, record):
        fields = [
            'employee_id', 'name', 'phone_number', 'face_image', 'face_encoding'
        ]

        with self.connection.cursor() as cursor:
            insert_query = """
            INSERT INTO local_employee_db ({})
            VALUES ({})
            """.format(','.join(fields), ','.join(['%s']*len(fields)))

            values = []
            for field in fields:
                value = getattr(record, field)
                values.append(value if value != "" else None)
            
            cursor.execute(insert_query, tuple(values))
            self.connection.commit()
            print(insert_query)

    def insert_store_record(self, record):
        insert_query = """
        INSERT INTO local_store_db (
            store_id, location, name, num_entry_cams, num_billing_cams, num_exit_cams,
            entry_cam, billing_cam, exit_cam
        )
        VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)
        """
        self.cursor.execute(insert_query, (
            str(record.store_id), record.location, record.name, record.num_entry_cams, record.num_billing_cams,
            record.num_exit_cams, record.entry_cam, record.billing_cam, record.exit_cam
        ))
        self.connection.commit()

    def insert_visit_record_old(self, record):
        insert_query = """
        INSERT INTO local_visit_db (
            customer_id, visit_id, store_id, entry_time, exit_time, billed, bill_amount, time_spent, visit_remark,
            customer_rating, customer_feedback, incomplete, return_customer
        )
        VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
        """
        self.cursor.execute(insert_query, (
            record.customer_id, record.visit_id, record.store_id, record.entry_time, record.exit_time,
            record.billed, record.bill_amount, record.time_spent, record.visit_remark, record.customer_rating,
            record.customer_feedback, record.incomplete
        ))
        self.connection.commit()

    def insert_visit_record(self, record):
        fields = [
            'customer_id', 'visit_id', 'store_id', 'entry_time', 'exit_time', 'billed', 'bill_amount',
            'time_spent', 'visit_remark', 'customer_rating', 'customer_feedback', 'incomplete', 'return_customer'
        ]

        with self.connection.cursor() as cursor:
            insert_query = """
            INSERT INTO local_visit_db ({})
            VALUES ({})
            """.format(','.join(fields), ','.join(['%s']*len(fields)))

            values = []
            for field in fields:
                value = getattr(record, field)
                values.append(value if value != "" else None)
            
            cursor.execute(insert_query, tuple(values))
            self.connection.commit()
            #print(insert_query)

    def insert_session_record(self, record):
        fields = [
            'start_time', 'end_time', 'detection', 'model', 'threshold', 'yaw_threshold',
            'pitch_threshold', 'area_threshold', 'billing_cam_time', 'similarity_method'
        ]

        with self.connection.cursor() as cursor:
            insert_query = """
            INSERT INTO Session ({})
            VALUES ({})
            """.format(','.join(fields), ','.join(['%s']*len(fields)))

            values = []
            for field in fields:
                value = getattr(record, field)
                values.append(value if value != "" else None)
            
            cursor.execute(insert_query, tuple(values))
            self.connection.commit()

    def update_customer_record_old(self, record):
        with self.connection.cursor() as cursor:
            update_query = """
            UPDATE local_customer_db
            SET name = %s, phone_number = %s, encoding = %s, image = %s,
                return_customer = %s, last_visit = %s, average_time_spent = %s,
                average_purchase = %s, maximum_purchase = %s, remarks = %s,
                loyalty_level = %s, num_visits = %s, last_location = %s,
                location_list = %s, category = %s, creation_date = %s, group_id = %s
            WHERE customer_id = %s
            """
            cursor.execute(update_query, (
                record.name, record.phone_number, record.encoding, record.image,
                record.return_customer, record.last_visit, record.average_time_spent,
                record.average_purchase, record.maximum_purchase, record.remarks,
                record.loyalty_level, record.num_visits, record.last_location,
                record.location_list, record.category, record.customer_id, record.creation_date, record.group_id
            ))
            self.connection.commit()

    def update_customer_record(self, record):
        fields = [
            'name', 'phone_number', 'encoding', 'image',
            'return_customer', 'last_visit', 'average_time_spent',
            'average_bill_value','average_bill_per_visit', 'average_bill_per_billed_visit', 'maximum_purchase', 'remarks',
            'loyalty_level', 'num_bills', 'num_visits', 'num_billed_visits', 'last_location',
            'location_list', 'category', 'creation_date', 'group_id'
        ]

        with self.connection.cursor() as cursor:
            update_query = """
            UPDATE local_customer_db
            SET {}
            WHERE customer_id = %s
            """.format(', '.join('{} = %s'.format(f) for f in fields))

            values = []
            for field in fields:
                value = getattr(record, field)
                values.append(value if value != "" else None)

            values.append(record.customer_id)

            cursor.execute(update_query, tuple(values))
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

    def update_store_record(self, record):
        update_query = """
        UPDATE local_store_db
        SET location = %s, name = %s, num_entry_cams = %s, num_billing_cams = %s, num_exit_cams = %s,
            entry_cam = %s, billing_cam = %s, exit_cam = %s
        WHERE store_id = %s
        """
        self.cursor.execute(update_query, (
            record.location, record.name, record.num_entry_cams, record.num_billing_cams,
            record.num_exit_cams, record.entry_cam, record.billing_cam, record.exit_cam, record.store_id
        ))
        self.connection.commit()

    def update_visit_record(self, record):
        update_query = """
        UPDATE local_visit_db
        SET customer_id = %s, visit_id = %s, store_id = %s, entry_time = %s, exit_time = %s, billed = %s, bill_amount = %s,
            time_spent = %s, visit_remark = %s, customer_rating = %s, customer_feedback = %s, incomplete = %s, return_customer = %s
        WHERE visit_id = %s
        """
        self.cursor.execute(update_query, (
            record.customer_id, record.visit_id, record.store_id, record.entry_time, record.exit_time,
            record.billed, record.bill_amount, record.time_spent, record.visit_remark, record.customer_rating,
            record.customer_feedback, record.incomplete, record.visit_id
        ))
        self.connection.commit()

    def update_shutdown_time(self, shutdown_time):
        update_query = """
        UPDATE Session
        SET end_time = %s
        WHERE sessionid = (SELECT MAX(sessionid) FROM Session)
        """
        self.cursor.execute(update_query, (shutdown_time,))
        self.connection.commit()

    def update_boot_up_time(self, boot_up_time):
        update_query = """
        UPDATE Session
        SET boot_up_time = %s
        WHERE sessionid = (SELECT MAX(sessionid) FROM Session)
        """
        self.cursor.execute(update_query, (boot_up_time,))
        self.connection.commit()

    def get_latest_session_date(self):
        select_query = """
        SELECT MAX(start_time) FROM Session
        """
        self.cursor.execute(select_query)
        return self.cursor.fetchone()

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
        WHERE employee_id = %s
        """
        self.cursor.execute(delete_query, (record_name,))
        self.connection.commit()

    def delete_store_record(self, record_id):
        delete_query = """
        DELETE FROM local_store_db
        WHERE store_id = %s
        """
        self.cursor.execute(delete_query, (record_id,))
        self.connection.commit()

    def delete_visit_record(self, record_id):
        delete_query = """
        DELETE FROM local_visit_db
        WHERE visit_id = %s
        """
        self.cursor.execute(delete_query, (record_id,))
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

    def fetch_customer_record(self, customer_id):
        with self.connection.cursor() as cursor:
            select_query = """
            SELECT * FROM local_customer_db
            WHERE customer_id = %s
            """
            cursor.execute(select_query, (customer_id,))
            return cursor.fetchone()
        
    def fetch_employee_record(self, employee_id):
        with self.connection.cursor() as cursor:
            select_query = """
            SELECT * FROM local_employee_db
            WHERE employee_id = %s
            """
            cursor.execute(select_query, (employee_id,))
            return cursor.fetchone()

    def fetch_store_record(self):
        with self.connection.cursor() as cursor:
            select_query = """
            SELECT * FROM local_store_db
            """
            cursor.execute(select_query)
            return cursor.fetchone()
        
    def fetch_store_location(self):
        with self.connection.cursor() as cursor:
            select_query = """
            SELECT location FROM local_store_db
            """
            cursor.execute(select_query)
            return cursor.fetchone()
    
    def fetch_visit_record(self, visit_id):
        with self.connection.cursor() as cursor:
            select_query = """
            SELECT * FROM local_visit_db
            WHERE visit_id = %s
            """
            cursor.execute(select_query, (visit_id,))
            return cursor.fetchone()

####################################################################################################################################################
# Customer data structure for in-memory Redis database
class InMemCustomer:
    def __init__(self, customer_id, name, phone_number, encoding, image,
                 return_customer=False, last_visit=None, average_time_spent=None,
                 average_bill_value=None, average_bill_per_visit=None, average_bill_per_billed_visit = None, maximum_purchase=None, remarks=None,
                 loyalty_level=None, num_bills = 0, num_visits=0, num_billed_visits = 0, last_location=None,
                 location_list=None, category=None, creation_date=None, group_id=None, incomplete=None,
                 entry_time=None, billed=False, exited=None, visit_time=None, exit_time=None,
                 ):
        self.customer_id = customer_id
        self.name = name
        self.phone_number = phone_number
        self.encoding = encoding
        self.image = image
        self.return_customer = return_customer
        self.last_visit = last_visit
        self.average_time_spent = average_time_spent
        self.average_bill_value = average_bill_value
        self.average_bill_per_visit = average_bill_per_visit
        self.average_bill_per_billed_visit = average_bill_per_billed_visit
        self.maximum_purchase = maximum_purchase
        self.remarks = remarks
        self.loyalty_level = loyalty_level
        self.num_bills = num_bills
        self.num_visits = num_visits
        self.num_billed_visits = num_billed_visits
        self.last_location = last_location
        self.location_list = location_list
        self.category = category
        self.creation_date = creation_date
        self.group_id = group_id
        #############################
        self.incomplete = incomplete
        self.entry_time = entry_time
        self.exited = exited
        self.visit_time = visit_time
        self.exit_time = exit_time

# Employee data structure for in-memory Redis database
class InMemEmployee:
    def __init__(self, employee_id, name, phone_number, face_image, face_encoding,
                 entry_time=None, exit_time=None, num_exits=0, in_store = False):
        self.employee_id = employee_id
        self.name = name
        self.phone_number = phone_number
        self.face_image = face_image
        self.face_encoding = face_encoding
        #################################
        self.entry_time = entry_time
        self.exit_time = exit_time
        self.num_exits = num_exits
        self.in_store = in_store

class InMemStore:
    def __init__(self, store_id, location, name, num_entry_cams, num_billing_cams, num_exit_cams,
                 entry_cam, billing_cam, exit_cam):
        self.store_id = store_id
        self.location = location
        self.name = name
        self.num_entry_cams = num_entry_cams
        self.num_billing_cams = num_billing_cams
        self.num_exit_cams = num_exit_cams
        self.entry_cam = entry_cam
        self.billing_cam = billing_cam
        self.exit_cam = exit_cam

class InMemVisit:
    def __init__(self, customer_id, visit_id, store_id, entry_time, exit_time, billed, bill_no, bill_date, bill_amount, return_amount, time_spent, 
                 visit_remark, customer_rating, customer_feedback, incomplete, return_customer):
        self.store_id = store_id
        self.customer_id = customer_id
        self.visit_id = visit_id
        self.entry_time = entry_time
        self.exit_time = exit_time
        self.billed = billed
        self.bill_no = bill_no
        self.bill_date = bill_date
        self.bill_amount = bill_amount
        self.return_amount = return_amount
        self.time_spent = time_spent
        self.visit_remark = visit_remark
        self.customer_rating = customer_rating
        self.customer_feedback = customer_feedback
        self.incomplete = incomplete
        self.return_customer = return_customer

class InMemIncomplete:
    def __init__(self, customer_id, encoding):
        self.customer_id = customer_id
        self.encoding = encoding

class InMemExited:
    def __init__(self, customer_id):
        self.customer_id = customer_id

class InMemParams:
    def __init__(self, detection, model, threshold, yaw_threshold, pitch_threshold, area_threshold, billing_cam_time, similarity_method, periodic_sleep_time,
                 num_threads_per_process, frames_per_second):
        self.detection = detection
        self.model = model
        self.threshold = threshold
        self.yaw_threshold = yaw_threshold
        self.pitch_threshold = pitch_threshold
        self.area_threshold = area_threshold
        self.billing_cam_time = billing_cam_time
        self.similarity_method = similarity_method
        self.periodic_sleep_time = periodic_sleep_time
        self.num_threads_per_process = num_threads_per_process
        self.frames_per_second = frames_per_second

class InMemParamNames:
    detection = "param_detection"
    model = "param_model"
    threshold = "param_threshold"
    yaw_threshold = "param_yaw_threshold"
    pitch_threshold = "param_pitch_threshold"
    area_threshold = "param_area_threshold"
    billing_cam_time = "param_billing_cam_time"
    similarity_method = "param_similarity_method"
    periodic_sleep_time = "param_periodic_sleep_time"
    num_threads_per_process = "param_num_threads_per_process"
    frames_per_second = "param_frames_per_second"

BOOT_TIME_START = "inmem_boot_time_start"
BOOT_TIME_END = "inmem_boot_time_end"
INMEM_PARAMS = "inmem_parameters"

# Class for the in-memory Redis database
class InMemoryRedisDB(Database):
    def __init__(self, host, port):
        super().__init__(host, port)
        self.connection = None

    ################################### SET & GET PARAM METHODS #########################################
    def set_detection(self, detection):
        self.connection.set(InMemParamNames.detection, str(detection))

    def get_detection(self):
        return self.connection.get(InMemParamNames.detection).decode()
    
    def set_model(self, model):
        self.connection.set(InMemParamNames.model, str(model))

    def get_model(self):
        return self.connection.get(InMemParamNames.model).decode()
    
    def set_threshold(self, threshold):
        self.connection.set(InMemParamNames.threshold, str(threshold))

    def get_threshold(self):
        return self.connection.get(InMemParamNames.threshold).decode()

    def set_yaw_threshold(self, yaw_threshold):
        self.connection.set(InMemParamNames.yaw_threshold, str(yaw_threshold))

    def get_yaw_threshold(self):
        return self.connection.get(InMemParamNames.yaw_threshold).decode()
    
    def set_pitch_threshold(self, pitch_threshold):
        self.connection.set(InMemParamNames.pitch_threshold, str(pitch_threshold))

    def get_pitch_threshold(self):
        return self.connection.get(InMemParamNames.pitch_threshold).decode()
    
    def set_area_threshold(self, area_threshold):
        self.connection.set(InMemParamNames.area_threshold, str(area_threshold))

    def get_area_threshold(self):
        return self.connection.get(InMemParamNames.area_threshold).decode()
    
    def set_billing_cam_time(self, billing_cam_time):
        self.connection.set(InMemParamNames.billing_cam_time, str(billing_cam_time))

    def get_billing_cam_time(self):
        return self.connection.get(InMemParamNames.billing_cam_time).decode()
    
    def set_similarity_method(self, similarity_method):
        self.connection.set(InMemParamNames.similarity_method, str(similarity_method))

    def get_similarity_method(self):
        return self.connection.get(InMemParamNames.similarity_method).decode()
    
    def set_periodic_sleep_time(self, periodic_sleep_time):
        self.connection.set(InMemParamNames.periodic_sleep_time, str(periodic_sleep_time))

    def get_periodic_sleep_time(self):
        return self.connection.get(InMemParamNames.periodic_sleep_time).decode()
    
    def set_num_threads_per_process(self, num_threads_per_process):
        self.connection.set(InMemParamNames.num_threads_per_process, str(num_threads_per_process))

    def get_num_threads_per_process(self):
        return self.connection.get(InMemParamNames.num_threads_per_process).decode()
    
    def set_frames_per_second(self, frames_per_second):
        self.connection.set(InMemParamNames.frames_per_second, str(frames_per_second))

    def get_frames_per_second(self):
        return self.connection.get(InMemParamNames.frames_per_second).decode()
    
    def set_boot_time_start(self, boot_time_start):
        self.connection.set(BOOT_TIME_START, str(boot_time_start))

    def get_boot_time_start(self):
        return self.connection.get(BOOT_TIME_START).decode()
    
    def set_boot_time_end(self, boot_time_end):
        self.connection.set(BOOT_TIME_END, str(boot_time_end))

    def get_boot_time_end(self):
        return self.connection.get(BOOT_TIME_END).decode()
    ####################################################################################################

    def insert_record(self, record, type='customer'):
        with self.connection.pipeline() as pipe:
            if type == 'customer':
                pipe.hmset(f'customer_inmem_db:{record.customer_id}', vars(record))
            elif type == 'employee':
                pipe.hmset(f'employee_inmem_db:{record.employee_id}', vars(record))
            elif type == 'store':
                pipe.hmset(f'store_inmem_db:{record.store_id}', vars(record))
            elif type == 'visit':
                pipe.hmset(f'visit_inmem_db:{record.customer_id}', vars(record)) # Every visit in memory is identified by customer id
            elif type == 'cust_id_list':
                pipe.lpush('cust_id_list', record)
            elif type == 'incomplete':
                pipe.hmset(f'incomplete_inmem_db:{record.customer_id}', vars(record))
            elif type == 'exited':
                pipe.hmset(f'exited_inmem_db:{record.customer_id}', vars(record))
            pipe.execute()

    def update_record(self, record, type='customer'):
        with self.connection.pipeline() as pipe:
            if type == 'customer':
                pipe.hmset(f'customer_inmem_db:{record.customer_id}', vars(record))
            elif type == 'employee':
                pipe.hmset(f'employee_inmem_db:{record.employee_id}', vars(record))
            elif type == 'store':
                pipe.hmset(f'store_inmem_db:{record.store_id}', vars(record))
            elif type == 'visit':
                pipe.hmset(f'visit_inmem_db:{record.customer_id}', vars(record))
            elif type == 'incomplete':
                pipe.hmset(f'incomplete_inmem_db:{record.customer_id}', vars(record))
            elif type == 'exited':
                pipe.hmset(f'exited_inmem_db:{record.customer_id}', vars(record))
            pipe.execute()

    def delete_record(self, record_id, type='customer'):
        # Delete the record by its key
        if type == 'customer':
            self.connection.delete(f'customer_inmem_db:{record_id}')
        elif type == 'employee':
            self.connection.delete(f'employee_inmem_db:{record_id}')
        elif type == 'store':
            self.connection.delete(f'store_inmem_db:{record_id}')
        elif type == 'visit':
            self.connection.delete(f'visit_inmem_db:{record_id}')
        elif type == 'incomplete':
            self.connection.delete(f'incomplete_inmem_db:{record_id}')
        elif type == 'exited':
            self.connection.delete(f'exited_inmem_db:{record_id}')

    def fetch_customer_records(self, customer_id):
        record = self.connection.hgetall(f'customer_inmem_db:{customer_id}')
        if record:
            return {key.decode(): ast.literal_eval(value.decode()) for key, value in record.items()}
        return None

    def fetch_employee_record(self, employee_id):
        record = self.connection.hgetall(f'employee_inmem_db:{employee_id}')
        if record:
            return {key.decode(): ast.literal_eval(value.decode()) for key, value in record.items()}
        return None
    
    def fetch_store_record(self):
        keys = self.connection.keys(f'store_inmem_db:*')
        for key in keys:
            record = self.connection.hgetall(key)
            if record:
                return {key.decode(): ast.literal_eval(value.decode()) for key, value in record.items()}
        return None
    
    def fetch_store_location(self):
        keys = self.connection.keys(f'store_inmem_db:*')
        for key in keys:
            record = self.connection.hgetall(key)
            if record:
                loc = record.get(b'location')
                return loc.decode()
        return None
    
    def fetch_store_id(self):
        keys = self.connection.keys(f'store_inmem_db:*')
        for key in keys:
            record = self.connection.hgetall(key)
            if record:
                store_id = record.get(b'store_id')
                return store_id.decode()
        return None
    
    def fetch_visit_record(self, customer_id):
        record = self.connection.hgetall(f'visit_inmem_db:{customer_id}')
        if record:
            return {key.decode(): ast.literal_eval(value.decode()) for key, value in record.items()}
        return None
    
    def fetch_visit_id(self, customer_id):
        record = self.connection.hgetall(f'visit_inmem_db:{customer_id}')
        if record:
            visit_id = record.get(b'visit_id')
            return visit_id.decode()
        return None

    def insert_params(self, params):
        with self.connection.pipeline() as pipe:
            pipe.hmset(INMEM_PARAMS, vars(params))
            pipe.execute()

    def insert_boot_time_start(self, boot_time_start):
        with self.connection.pipeline() as pipe:
            pipe.hmset(BOOT_TIME_START, vars(boot_time_start))
            pipe.execute()

    def connect(self):
        self.connection = redis.Redis(host=self.host, port=self.port)

##############################################################################################################################

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
            category=inmem_customer.category,
            creation_date=inmem_customer.creation_date,
            group_id=inmem_customer.group_id,
            incomplete=inmem_customer.incomplete,
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

    def map_store_inmem_to_local(self, inmem_store):
        local_store = LocalStore(
            store_id=inmem_store.store_id,
            location=inmem_store.location,
            name=inmem_store.name,
            num_entry_cams=inmem_store.num_entry_cams,
            num_billing_cams=inmem_store.num_billing_cams,
            num_exit_cams=inmem_store.num_exit_cams,
            entry_cam=inmem_store.entry_cam,
            billing_cam=inmem_store.billing_cam,
            exit_cam=inmem_store.exit_cam
        )
        self.local_db.insert_record(local_store)

    def map_visit_inmem_to_local(self, inmem_visit):
        local_visit = LocalVisit(
            customer_id=inmem_visit.customer_id,
            visit_id=inmem_visit.visit_id,
            store_id=inmem_visit.store_id,
            entry_time=inmem_visit.entry_time,
            exit_time=inmem_visit.exit_time,
            billed=inmem_visit.billed,
            bill_amount=inmem_visit.bill_amount,
            time_spent=inmem_visit.time_spent,
            visit_remark=inmem_visit.visit_remark,
            customer_rating=inmem_visit.customer_rating,
            customer_feedback=inmem_visit.customer_feedback,
            incomplete=inmem_visit.incomplete,
            return_customer=inmem_visit.return_customer
        )
        self.local_db.insert_record(local_visit)

class MapLocaltoInMem:
    def __init__(self, local_db):
        self.local_db = local_db
    
    def map_customer_local_to_inmem(self, local_customer):
        inmem_customer = InMemCustomer(
            customer_id=local_customer[0],
            name=local_customer[1],
            phone_number=local_customer[2],
            encoding=local_customer[3],
            image=local_customer[4],
            return_customer=local_customer[5],
            last_visit=local_customer[6],
            average_time_spent=local_customer[7],
            average_purchase=local_customer[8],
            maximum_purchase=local_customer[9],
            remarks=local_customer[10],
            loyalty_level=local_customer[11],
            num_visits=local_customer[12],
            last_location=local_customer[13],
            location_list=local_customer[14],
            category=local_customer[15],
            creation_date=local_customer[16],
            group_id=local_customer[17],
        )
        return inmem_customer

    def map_employee_local_to_inmem(self, local_employee):
        inmem_employee = InMemEmployee(
            employee_id = local_employee[0],
            name=local_employee[1],
            phone_number=local_employee[2],
            face_image=local_employee.face_image[3],
            face_encoding=local_employee.face_encoding[4]
        )
        return inmem_employee
    
    def map_store_local_to_inmem(self, local_store):
        inmem_store = InMemStore(
            store_id=local_store[0],
            location=local_store[1],
            name=local_store[2],
            num_entry_cams=local_store[3],
            num_billing_cams=local_store[4],
            num_exit_cams=local_store[5],
            entry_cam=str(local_store[6]),
            billing_cam=str(local_store[7]),
            exit_cam=str(local_store[8])
        )
        return inmem_store
    
    def map_visit_local_to_inmem(self, local_visit):
        inmem_visit = InMemVisit(
            customer_id=local_visit[0],
            visit_id=local_visit[1],
            store_id=local_visit[2],
            entry_time=local_visit[3],
            exit_time=local_visit[4],
            billed=local_visit[5],
            bill_amount=local_visit[6],
            time_spent=local_visit[7],
            visit_remark=local_visit[8],
            customer_rating=local_visit[9],
            customer_feedback=local_visit[10],
            incomplete=local_visit[11],
            return_customer=local_visit[12]
        )
        return inmem_visit
    
