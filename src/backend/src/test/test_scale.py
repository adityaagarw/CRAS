import argparse
import configparser
import cv2
import os
import io
import random
import string
import time
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
def exising_inmem_db_store(record, local_db, face_encoding, face_image):
    customer_id = record[0]
    name = record[1]
    phone_number = record[2]
    encoding = record[3]
    image = record[4]
    return_customer = record[5]
    last_visit = record[6]
    average_time_spent = record[7]
    average_purchase = record[8]
    maximum_purchase = record[9]
    remarks = record[10]
    loyalty_level = record[11]
    num_visits = record[12]
    last_location = record[13]
    location_list = record[14]
    category = record[15]
    creation_date = record[16]
    group_id = record[17]
    cos_distance = record[18]
    # TBD Write this function later
    pass

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

def build_local_database(parameters):
    dataset_path = r"C:\Users\adity\OneDrive\Documents\Aditya\_CRAS_\face_dataset"

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
                if face_encoding is None:
                    continue
                local_db_store(local_db, face_encoding, face_image)

def build_parameters(file):
    config = configparser.ConfigParser()
    config.read(file)
    args = config['general']
    parameters = Parameters(args['detection'], \
                            args['library'], \
                            args['model'], \
                            args['threshold'], \
                            args['yaw_threshold'], \
                            args['pitch_threshold'], \
                            args['area_threshold'], \
                            args['billing_cam_time'], \
                            args['sim_method'], \
                            args['debug_mode'], \
                            args['username'], \
                            args['password'], \
                            args['db_link'], \
                            args['db_name'], \
                            args['input_type'], \
                            args['video_path'], \
                            args['model_dir'])
    return parameters

if __name__ == '__main__':
    # Use argparse to parse command line arguments
    parser = argparse.ArgumentParser(description='Test system at scale')
    parser.add_argument('build_db', type=str, help='Load all images to local database') # Most time consuming
    
    # Parse the command line arguments
    args = parser.parse_args()

    parameters = build_parameters("config.ini")

    if (args.build_db):
        build_local_database(parameters)

