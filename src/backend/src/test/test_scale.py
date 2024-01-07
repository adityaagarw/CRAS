import argparse
import configparser
import os
import random
import string
import numpy as np

from datetime import datetime, timedelta
from PIL import Image
from face.face import Detection, Recognition, Rectangle, Predictor
from face_utils.imagetoface import ImageToFace
from utils.utils import Utils
from config.params import Parameters
from db.database import *

# Purpose of this script is to test the system at scale.
# This script has multiple options to test different scenarios
# The primary input is a dataset containing about 23000 images of customers
# The first option is to test the system with all images loaded to local database.
# The second option is to test the system with all images loaded to local database and a quarter of them loaded to in memory database.
# The third option is to test the system with all images loaded to local database and in memory database.
# To accomplish this we need to create fake customer records and visit records and store them in the database.

def generate_random_name():
    length = random.randint(6, 7)
    characters = string.ascii_letters + string.digits
    return ''.join(random.choice(characters) for _ in range(length))

def generate_random_phone_number():
    length = 10
    characters = string.digits  # Use only digits
    return ''.join(random.choice(characters) for _ in range(length))

def local_db_store(local_db, face_encoding, face_image):
    new_id = Utils.generate_unique_id()
    visit_id = Utils.generate_unique_id()
    name = generate_random_name()
    phone_number = generate_random_phone_number()
    # TBD: Process encoding and image for storage
    encoding = np.frombuffer(face_encoding, dtype=np.float32).tolist()
    image = face_image

    ins_customer_record = LocalCustomer(
            customer_id = str(new_id),
            name = name,
            phone_number = phone_number,
            encoding = encoding,
            image = image,
            return_customer = "1",
            last_visit = "2023-06-10 22:49:41.793134",
            average_time_spent = "",
            average_bill_value = "2000",
            average_bill_per_visit = "2000",
            average_bill_per_billed_visit = "2000",
            maximum_purchase = "20000",
            remarks = "TestCustomer",
            loyalty_level = "Gold",
            num_bills = "1",
            num_visits = "1",
            num_billed_visits = "1",
            last_location = "Gurgaon",
            location_list = "{Gurgaon}",
            category = "Customer",
            creation_date = "2023-06-10 22:49:41.793134",
            group_id = ""
    )

    ins_visit_record = LocalVisit(
        customer_id = str(new_id),
        visit_id = str(visit_id),
        store_id = "2cc25f36-8c1d-4fd9-a6c7-b6d4e791bb30",
        entry_time = "2023-06-10 22:19:41.793134",
        exit_time = "2023-06-10 22:49:41.793134",
        billed = "1",
        bill_no = "123",
        bill_date = "2023-06-10 22:19:41.793134",
        bill_amount = "2000",
        return_amount = "0",
        time_spent = "",
        visit_remark = "TestVisit",
        customer_rating = "5",
        customer_feedback = "5",
        incomplete = "0",
        return_customer = "1"
    )
    print("Added: " + str(new_id) + " " + name + " " + phone_number)
    local_db.insert_customer_record(ins_customer_record)
    local_db.insert_visit_record(ins_visit_record)

# Retrieve exising record from local database and store it in memory
def existing_inmem_db_store(record, in_mem_db):
    date_format = "%Y-%m-%d %H:%M:%S"
    entry_time = datetime.now().strftime(date_format)

    customer_id = '' if record[0] is None else record[0]
    name = '' if record[1] is None else record[1]
    phone_number = '' if record[2] is None else record[2]
    encoding = '' if record[3] is None else record[3]
    image = '' if record[4] is None else record[4]
    return_customer = '' if record[5] is None else record[5]
    last_visit = '' if record[6] is None else record[6]
    average_time_spent = '' if record[7] is None else record[7]
    average_bill_value = '' if record[8] is None else record[8]
    average_bill_per_visit = '' if record[9] is None else record[9]
    average_bill_per_billed_visit = '' if record[10] is None else record[10]
    maximum_purchase = '' if record[11] is None else record[11]
    remarks = '' if record[12] is None else record[12]
    loyalty_level = '' if record[13] is None else record[13]
    num_bills = '' if record[14] is None else record[14]
    num_visits = '' if record[15] is None else record[15]
    num_billed_visits = '' if record[16] is None else record[16]
    last_location = '' if record[17] is None else record[17]
    location_list = '' if record[18] is None else record[18]
    category = '' if record[19] is None else record[19]
    creation_date = '' if record[20] is None else record[20]
    group_id = '' if record[21] is None else record[21]
    encoding_str_list = encoding.strip("[]").split()
    encoding_str = ",".join(encoding_str_list)

    # If image type is memoryview convert it to bytes else keep it as is
    if (type(image) == memoryview):
        image = image.tobytes()
    else:
        print("Failed for customer id: " + str(customer_id) + " " + name + " " + phone_number)
        return

    face_encoding_np = np.fromstring(encoding_str, sep=",", dtype=np.float32)
    existing_customer_record = InMemCustomer(
        customer_id = str(customer_id),
        name = name,
        phone_number = str(phone_number),
        encoding = face_encoding_np.tobytes(),
        image = image,
        return_customer = "1",
        last_visit = str(last_visit),
        average_time_spent = str(average_time_spent),
        average_bill_value= str(average_bill_value),
        average_bill_per_visit= str(average_bill_per_visit),
        average_bill_per_billed_visit= str(average_bill_per_billed_visit),
        maximum_purchase = str(maximum_purchase),
        remarks = str(remarks),
        loyalty_level = str(loyalty_level),
        num_bills = str(num_bills),
        num_visits = str(num_visits),
        num_billed_visits= str(num_billed_visits),
        last_location = last_location,
        location_list = str(location_list),
        category = str(category),
        creation_date = str(creation_date),
        group_id = str(group_id),
        incomplete="1",
        entry_time=str(entry_time),
        exited="0",
        visit_time="",
        exit_time=""
    )

    new_visit_record = InMemVisit(
        customer_id=str(customer_id),
        visit_id=str(Utils.generate_unique_id()),
        store_id="2cc25f36-8c1d-4fd9-a6c7-b6d4e791bb30",
        entry_time=str(entry_time),
        exit_time="",
        billed="0",
        bill_no="",
        bill_date="",
        bill_amount="0",
        return_amount="0",
        time_spent="",
        visit_remark="",
        customer_rating="",
        customer_feedback="",
        incomplete="1",
        return_customer="1"
    )
    in_mem_db.insert_record(existing_customer_record)
    in_mem_db.insert_record(new_visit_record, type="visit")
    #print("Added: " + str(customer_id) + " " + name + " " + phone_number)

# Create a new record and store it in memory
def new_inmem_db_store(in_mem_db, face_encoding, face_image):
    date_format = "%Y-%m-%d %H:%M:%S"
    new_id = Utils.generate_unique_id()
    visit_id = Utils.generate_unique_id()
    name = generate_random_name()
    phone_number = generate_random_phone_number()
    entry_time = datetime.now().strftime(date_format)
    # TBD: Process encoding and image for storage
    encoding = face_encoding
    image = face_image

    new_customer_record = InMemCustomer(
        customer_id = str(new_id),
        name = name,
        phone_number = phone_number,
        encoding = encoding,
        image = image,
        return_customer = "1",
        last_visit = "2023-06-10 22:49:41.793134",
        average_time_spent = "00:30:00.000000",
        average_bill_value = "2000",
        average_bill_per_visit = "2000",
        average_bill_per_billed_visit = "2000",
        maximum_purchase = "20000",
        remarks = "TestCustomer",
        loyalty_level = "Gold",
        num_bills = "1",
        num_visits = "1",
        num_billed_visits = "1",
        last_location = "Gurgaon",
        location_list = "\{Gurgaon\}",
        category = "Customer",
        creation_date = "2023-06-10 22:49:41.793134",
        group_id = "",
        incomplete = "0",
        entry_time = entry_time,
        exited = "0",
        visit_time = "",
        exit_time = ""
    )
    new_visit_record = InMemVisit(
        customer_id = str(new_id),
        visit_id = str(visit_id),
        store_id = "2cc25f36-8c1d-4fd9-a6c7-b6d4e791bb30",
        entry_time = entry_time,
        exit_time = "",
        billed = "1",
        bill_amount = "2000",
        time_spent = "",
        visit_remark = "TestVisit",
        customer_rating = "5",
        customer_feedback = "5",
        incomplete = "0",
        return_customer = "1"
    )

    in_mem_db.insert_record(new_customer_record)
    in_mem_db.insert_record(new_visit_record, type="visit")

def fetch_customer_record(in_mem_db, local_db, customer_id):
    customer_record = in_mem_db.connection.hgetall('customer_inmem_db:' + customer_id)

    face_encoding = np.frombuffer(customer_record.get(b'encoding'), dtype=np.float32)

    face_encoding_str = f"{face_encoding.tolist()}"
    face_record_query = """
                        SELECT *, (encoding <=> %(face_encoding)s) AS distance FROM local_customer_db WHERE 1 - (encoding <=> %(face_encoding)s) > 0.65 LIMIT 1;
                        """

    local_db.cursor.execute(face_record_query, {'face_encoding': face_encoding_str})
    record = local_db.cursor.fetchone()
    return record

def load_into_inmem(parameters, num_records):
    # Connect to local database
    local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
    local_db.connect()

    # Connect to in memory database
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    in_mem_db.connect()

    query = """
            SELECT * FROM local_customer_db LIMIT %(num_records)s;
            """
    local_db.cursor.execute(query, {'num_records': num_records})
    records = local_db.cursor.fetchall()

    for record in records:
        existing_inmem_db_store(record, in_mem_db)

    # Disconnect from local database
    local_db.disconnect()

def build_local_database(parameters):
    dataset_path = r"C:\Users\adity\OneDrive\Documents\Aditya\CRAS_main\face_dataset"

    local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
    local_db.connect()

    detector = Detection(parameters)
    r = Recognition(parameters)

    imgToFace = ImageToFace()

    # Iterate over all images in the dataset
    for root, dirs, files in os.walk(dataset_path):
        for file in files:
            if file.endswith(".jpg"):
                image_path = os.path.join(root, file)
                face_encoding, face_pixels = imgToFace.fullImagetoEncoding(detector, r, image_path)
                face_image = imgToFace.get_face_image(face_pixels)
                if face_image is None or face_image.size == 0:
                    continue
                if face_encoding is None:
                    continue
                local_db_store(local_db, face_encoding, face_image)

if __name__ == '__main__':
    # Use argparse to parse command line arguments
    parser = argparse.ArgumentParser(description='Test system at scale')
    parser.add_argument('-build_db', dest="build_db", type=str, required=False, help='Load all images to local database') # Most time consuming
    parser.add_argument("-load_db_inmem", dest="load_db_inmem", type=str, required=False, help="Load x images to in memory database")
    parser.add_argument("-num_records", dest="num_records", type=int, required=False, help="Number of records to load to in memory database")

    # Parse the command line arguments
    args = parser.parse_args()

    parameters = Parameters.build_parameters("config.ini")

    if (args.build_db):
        build_local_database(parameters)
    
    if (args.load_db_inmem):
        if (args.num_records == None):
            print("No num_records argument provided")
            exit(1)
        load_into_inmem(parameters, args.num_records)


