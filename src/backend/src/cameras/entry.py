import argparse
import configparser
import cv2
import os
import io
import time
import numpy as np
import multiprocessing
import fasteners
import threading
from datetime import datetime, timedelta
from PIL import Image
from multiprocessing import Process
from multiprocessing import Manager

from sklearn.metrics.pairwise import cosine_similarity

# Private packages
from face.face import Detection, Recognition, Rectangle, Predictor
from face.pipe import FacePipe
from utils.utils import Utils
from config.params import Parameters
from db.database import *
from db.redis_pubsub import *
from db.log import *

QUEUE_MAX_SIZE = 100000
SEARCH_QUEUE_SIZE = 100000
NUM_CONSUMER_PROCESSES = 1
NUM_SEARCH_PROCESSES = 1

def get_face_image(face_pixels, target_size=(160, 160)):
    #face_8bit = np.clip(face_pixels, 0, 255).astype(np.uint8)
    #face_image = Image.fromarray(face_8bit)
    try:
        face_pixels_rgb = cv2.cvtColor(face_pixels, cv2.COLOR_BGR2RGB)
    except:
        face_pixels_rgb = face_pixels

    #if len(face_pixels_rgb.shape) not in [2, 3] or (len(face_pixels_rgb.shape) == 3 and face_pixels_rgb.shape[2] not in [3, 4]):
    try:
        face_image = Image.fromarray(face_pixels_rgb)
    except:
        return None
    
    face_image = face_image.resize(target_size)
    img_bytes = io.BytesIO()
    face_image.save(img_bytes, format='PNG')
    img_data = img_bytes.getvalue()
    return img_data

def detect_faces_in_frame(detector, frame):
    faces = detector.detect(frame, 1)
    return faces

def get_face_image_encoding(r, face, frame):
    face_pixels = r.get_face_pixels_for_image(face, frame)
    embedding = r.get_encodings(face, frame)
    return embedding, face_pixels

def fetch_all_employee_records(local_db):
    fetch_query = """
                        SELECT * FROM local_employee_db; 
                        """
    local_db.cursor.execute(fetch_query)
    records = local_db.cursor.fetchall()
    return records

# Load employee data from local db to in mem db
def load_employee_data():
    print("Loading employee data")
    local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)

    local_db.connect()
    if not local_db.connection:
        print("Local db connection failed while loading employee data!")
    else:
        print("Connected to localdb while loading employee data")

    in_mem_db.connect()
    if not in_mem_db.connection:
        print("Redis db connection failed while loading employee data!")
    else:
        print("Connected to redis db while loading employee data")

    # Get all employee records from localdb
    employee_records = fetch_all_employee_records(local_db)
    for record in employee_records:

        # Check if employee already in in-mem db
        employee_id = record[0]
        employee_record = in_mem_db.connection.hgetall("employee_inmem_db:" + employee_id)
        if employee_record:
            continue
        # Format encoding from localdb to inmemdb
        encoding = "" if record[4] is None else record[4]
        encoding_str_list = encoding.strip("[]").split()
        encoding_str = ",".join(encoding_str_list)
        face_encoding_np = np.fromstring(encoding_str, sep=",", dtype=np.float32)

        # Insert employee record in-mem
        new_employee_record = InMemEmployee(
            employee_id = "" if record[0] is None else record[0],
            name = "" if record[1] is None else record[1],
            phone_number = "" if record[2] is None else record[2],
            face_image = "" if record[3] is None else record[3],
            face_encoding = face_encoding_np.tobytes(),
            entry_time = "",
            exit_time = "",                                                                      
            num_exits = "0",
            in_store = "0"
        )
        in_mem_db.insert_record(new_employee_record, "employee")

    print_log(in_mem_db, "Backend", datetime.now(), "entry", "load_employee_data", "Success", "Loaded employee data", line_number(), "DEBUG")
    local_db.disconnect()
    in_mem_db.disconnect()

def update_employee_inmem(in_mem_db, record):
    date_format = "%Y-%m-%d %H:%M:%S"
    employee_id = record.get(b'employee_id').decode()
    name = record.get(b'name').decode()
    phone_number = record.get(b'phone_number').decode()
    face_image = record.get(b'face_image')
    face_encoding = record.get(b'face_encoding')
    entry_time = datetime.now().strftime(date_format)
    exit_time = record.get(b'exit_time').decode()
    num_exits = record.get(b'num_exits').decode()

    new_employee_record = InMemEmployee(
        employee_id = employee_id,
        name = name,
        phone_number = phone_number,
        face_image = face_image,
        face_encoding = face_encoding,
        entry_time = entry_time,
        exit_time = exit_time,
        num_exits = num_exits,
        in_store = "1"
    )

    in_mem_db.delete_record(employee_id, type="employee")
    in_mem_db.insert_record(new_employee_record, type="employee")

def update_exit_entry_customer(in_mem_db, customer_id):
    record = in_mem_db.connection.hgetall("customer_inmem_db:" + customer_id)
    date_format = "%Y-%m-%d %H:%M:%S"
    exit_time = datetime.strptime(datetime.now().strftime(date_format), date_format)
    entry_time = datetime.strptime(record.get(b'entry_time').decode(), date_format)

    time_spent = (exit_time - entry_time).total_seconds()
    num_visits = int(record.get(b'num_visits').decode())
    customer_id = str(record.get(b'customer_id').decode())

    # Update number of visits
    updated_num_visits = num_visits - 1

    # Update average time spent
    old_avg_time_spent = record.get(b'average_time_spent').decode()
    if updated_num_visits == 0:
        updated_avg_time_spent = ""
    else:
        # Convert existing avg time to datetime

        existing_interval_seconds = old_avg_time_spent
        
        exisitng_total_seconds = existing_interval_seconds * num_visits

        total_time = exisitng_total_seconds - time_spent.total_seconds()

        updated_avg_time_spent = (total_time) / updated_num_visits

    # Update location list and last location
    current_location = str(in_mem_db.fetch_store_location())
    updated_last_location = current_location
    if record.get(b'location_list').decode() == "{" + current_location + "}":
        updated_location_list = ""
    else:
        location_list_string_list = record.get(b'location_list').decode().strip("{}").split(",")
        location_list_string_list = location_list_string_list.pop()
        location_list_string = ",".join(location_list_string_list)
        updated_location_list = "{" + location_list_string + "}"

    name = record.get(b'name').decode()
    phone_number = record.get(b'phone_number').decode()
    encoding = record.get(b'encoding')
    image = record.get(b'image')
    return_customer = record.get(b'return_customer').decode()
    average_bill_value = record.get(b'average_bill_value').decode()
    average_bill_per_visit = record.get(b'average_bill_per_visit').decode()
    average_bill_per_billed_visit = record.get(b'average_bill_per_billed_visit').decode()
    maximum_purchase = record.get(b'maximum_purchase').decode()
    num_bills = record.get(b'num_bills').decode()
    num_billed_visits = record.get(b'num_billed_visits').decode()
    remarks = record.get(b'remarks').decode()
    loyalty_level = record.get(b'loyalty_level').decode()
    category = record.get(b'category').decode()
    creation_date = record.get(b'creation_date').decode()
    group_id = record.get(b'group_id').decode()

    new_customer_record = InMemCustomer(
        customer_id = customer_id,
        name = name,
        phone_number = phone_number,
        encoding = encoding,
        image = image,
        return_customer = return_customer,
        last_visit = str(exit_time),
        average_time_spent = updated_avg_time_spent,
        average_bill_value = average_bill_value,
        average_bill_per_visit = average_bill_per_visit,
        average_bill_per_billed_visit = average_bill_per_billed_visit,
        maximum_purchase = maximum_purchase,
        remarks = remarks,
        loyalty_level = loyalty_level,
        num_bills = str(num_bills),
        num_visits = str(updated_num_visits),
        num_billed_visits = str(num_billed_visits),
        last_location = str(updated_last_location),
        location_list = updated_location_list,
        category = category,
        creation_date = creation_date,
        group_id = group_id,
        incomplete = "0",
        entry_time = str(entry_time),
        exited = "0",
        visit_time = "",
        exit_time = ""
    )

    vrecord = in_mem_db.connection.hgetall("visit_inmem_db:" + customer_id)
    visit_id = str(vrecord.get(b'visit_id').decode())
    v_entry_time = vrecord.get(b'entry_time').decode()
    v_billed = vrecord.get(b'billed').decode()
    v_bill_no = vrecord.get(b'bill_no').decode()
    v_bill_date = vrecord.get(b'bill_date').decode()
    v_bill_amount = vrecord.get(b'bill_amount').decode()
    v_return_amount = vrecord.get(b'return_amount').decode()
    v_visit_remark = vrecord.get(b'visit_remark').decode()
    v_customer_rating = vrecord.get(b'customer_rating').decode()
    v_customer_feedback = vrecord.get(b'customer_feedback').decode()

    new_visit_record = InMemVisit(
        customer_id = customer_id,
        visit_id = visit_id,
        store_id = str(in_mem_db.fetch_store_id()),
        entry_time = v_entry_time,
        exit_time = str(exit_time),
        billed = v_billed,
        bill_no = v_bill_no,
        bill_date = v_bill_date,
        bill_amount = v_bill_amount,
        return_amount = v_return_amount,
        time_spent = "",
        visit_remark = v_visit_remark,
        customer_rating = v_customer_rating,
        customer_feedback = v_customer_feedback,
        incomplete = "0",
        return_customer = "1"
    )
    in_mem_db.delete_record(customer_id, type="customer")
    in_mem_db.delete_record(customer_id, type="visit")
    in_mem_db.insert_record(new_customer_record)
    in_mem_db.insert_record(new_visit_record, type="visit")
    print("Welcome back noob: ", customer_id)
    print_log(in_mem_db, "Backend", datetime.now(), "entry", "update_exit_entry_customer", customer_id, "Exited customer re-entered", line_number(), "DEBUG")
    return new_customer_record.customer_id

def insert_initial_record_inmem(face_encoding, face_pixels, in_mem_db):
    date_format = "%Y-%m-%d %H:%M:%S"
    new_id = Utils.generate_unique_id()
    face_img = get_face_image(face_pixels)
    if not face_img:
        print("Empty face image.")
        face_img = np.zeros((1,1), dtype=np.uint8)
        face_img = face_img.tobytes()
        return None

    current_location = in_mem_db.fetch_store_location()
    store_id = in_mem_db.fetch_store_id()
    time_now = datetime.now().strftime(date_format)
    face_encoding_bytes = face_encoding.tobytes()

    # Reconstruct bytes as follows:
    # face_encoding = np.frombuffer(face_encoding_bytes, dtype=np.float64)
    # face_img = np.frombuffer(face_img_bytes, dtype=np.uint8)

    new_customer_record = InMemCustomer(
        customer_id = str(new_id),
        name = "",
        phone_number = "",
        encoding = face_encoding_bytes,
        image = face_img,
        return_customer = "0",
        last_visit = "",
        average_time_spent = "",
        average_bill_value = "",
        average_bill_per_visit = "",
        average_bill_per_billed_visit= "",
        maximum_purchase = "",
        remarks = "New Customer",
        loyalty_level = "",
        num_bills = "0",
        num_visits = "0",
        num_billed_visits = "0",
        last_location = current_location,
        location_list = "",
        category = "",
        creation_date = time_now,
        group_id = "",
        incomplete = "1",
        entry_time = time_now,
        exited = "0",
        visit_time = "",
        exit_time = ""
    )

    new_visit_record = InMemVisit(
        customer_id = str(new_id),
        visit_id = str(Utils.generate_unique_id()),
        store_id = str(store_id),
        entry_time = time_now,
        exit_time = "",
        billed = "0",
        bill_no = "",
        bill_date = "",
        bill_amount = "0",
        return_amount = "0",
        time_spent = "",
        visit_remark = "New customer",
        customer_rating = "",
        customer_feedback = "",
        incomplete = "1",
        return_customer = "0"
    )

    in_mem_db.insert_record(new_customer_record)
    in_mem_db.insert_record(new_visit_record, type="visit")

    print("Welcome new customer: " + new_customer_record.customer_id)
    print_log(in_mem_db, "Backend", datetime.now(), "entry", "insert_initial_record_inmem", new_id, "New customer entered the store", line_number(), "DEBUG")
    message = BackendMessage.NewCustomer.value + ":" + str(new_id)
    in_mem_db.connection.publish(Channel.Backend.value, message)
    return(new_customer_record)

def insert_existing_record_inmem(new_record, record, in_mem_db):
    # Delete exisitng record

    existing_visit_id = in_mem_db.fetch_visit_id(new_record.customer_id)

    entry_time = new_record.entry_time

    customer_id = record[0]
    
    # name = record[1]
    # phone_number = record[2]
    # encoding = record[3]
    # image = record[4]
    # return_customer = record[5]
    # last_visit = record[6]
    # average_time_spent = record[7]
    # average_bill_value = record[8]
    # average_bill_per_visit = record[9]
    # average_bill_per_billed_visit = record[10]
    # maximum_purchase = record[11]
    # remarks = record[12]
    # loyalty_level = record[13]
    # num_bills = record[14]
    # num_visits = record[15]
    # num_billed_visits = record[16]
    # last_location = record[17]
    # location_list = record[18]
    # category = record[19]
    # creation_date = record[20]
    # group_id = record[21]
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

    face_encoding_np = np.fromstring(encoding_str, sep=",", dtype=np.float32)

    # Insert exisitng record to in-mem   #DEBUG will face problems later
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

    modified_visit_record = InMemVisit(
        customer_id=str(customer_id),
        visit_id=str(existing_visit_id),
        store_id=in_mem_db.fetch_store_id(),
        entry_time=str(new_record.entry_time),
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

    print("Inserting existing customer in memory")
    print("Welcome exisitng customer: " + str(existing_customer_record.customer_id))
    print_log(in_mem_db, "Backend", datetime.now(), "entry", "insert_existing_record_inmem", existing_customer_record.customer_id, "Customer found!", line_number(), "DEBUG")
    in_mem_db.insert_record(existing_customer_record)
    in_mem_db.insert_record(modified_visit_record, type="visit")
    #in_mem_db.delete_record(new_record.customer_id)
    #in_mem_db.delete_record(new_record.customer_id, type="visit")

    # UpdateCustomer:<>,TempCustomer:<>
    message = BackendMessage.UpdateCustomer.value + ":" + str(new_record.customer_id) + "," + str(record[0])
    print(message)
    in_mem_db.connection.publish(Channel.Backend.value, message)

def get_face_record_from_mem(face_encoding, threshold, in_mem_db):
    # Get all customer records from the in-memory Redis database
    records = in_mem_db.connection.keys('customer_inmem_db:*')

    # Initialize variables to track the closest record and similarity
    closest_record = None
    closest_similarity = -1.0

    # Iterate over each record
    for record_key in records:
        # Retrieve the face encoding from the record
        record_data = in_mem_db.connection.hgetall(record_key)
        record_encoding_bytes = record_data.get(b'encoding')

        if record_encoding_bytes is None or len(record_encoding_bytes) == 0:
            continue
        if face_encoding is None or len(face_encoding) == 0:
            continue

        # Convert the face encodings to numpy arrays
        face_encoding_np = np.frombuffer(face_encoding, dtype=np.float32)
        
        record_encoding_np = np.frombuffer(record_encoding_bytes, dtype=np.float32)

        # Calculate the cosine similarity between the face encodings
        similarity = cosine_similarity(face_encoding_np.reshape(1, -1), record_encoding_np.reshape(1, -1))

        # Check if the similarity exceeds the threshold and is closer than the previous closest
        if similarity > float(threshold) and similarity > closest_similarity:
            closest_record = record_data
            closest_similarity = similarity

    return closest_record

def get_employee_face_record_from_mem(face_encoding, threshold, in_mem_db):
    # Get all customer records from the in-memory Redis database
    records = in_mem_db.connection.keys('employee_inmem_db:*')

    # Initialize variables to track the closest record and similarity
    closest_record = None
    closest_similarity = -1.0

    # Iterate over each record
    for record_key in records:
        # Retrieve the face encoding from the record
        record_data = in_mem_db.connection.hgetall(record_key)
        record_encoding_bytes = record_data.get(b'face_encoding')

        if record_encoding_bytes is None or len(record_encoding_bytes) == 0:
            continue

        if face_encoding is None or len(face_encoding) == 0:
            continue

        # Convert the face encodings to numpy arrays
        face_encoding_np = np.frombuffer(face_encoding, dtype=np.float32)
        
        record_encoding_np = np.frombuffer(record_encoding_bytes, dtype=np.float32)

        # Calculate the cosine similarity between the face encodings
        similarity = cosine_similarity(face_encoding_np.reshape(1, -1), record_encoding_np.reshape(1, -1))

        # Check if the similarity exceeds the threshold and is closer than the previous closest
        if similarity > float(threshold) and similarity > closest_similarity:
            closest_record = record_data
            closest_similarity = similarity

    return closest_record

def get_face_record_from_localdb(face_encoding, threshold, local_db):
    # Query to get nearest similarity face record
    face_encoding_str = f"{face_encoding.tolist()}"
    face_record_query = """
                        SELECT * FROM local_customer_db WHERE (1 - (encoding <=> %(face_encoding)s)) > %(threshold)s LIMIT 1; 
                        """
    local_db.cursor.execute(face_record_query, {'face_encoding': face_encoding_str, 'threshold': threshold})
    record = local_db.cursor.fetchone()

    return record

def get_employee_face_record_from_localdb(face_encoding, threshold, local_db):
    # Query to get nearest similarity face record
    face_encoding_str = f"{face_encoding.tolist()}"
    face_record_query = """
                        SELECT * FROM local_employee_db WHERE (1 - (encoding <=> %(face_encoding)s)) > %(threshold)s LIMIT 1; 
                        """
    local_db.cursor.execute(face_record_query, {'face_encoding': face_encoding_str, 'threshold': threshold})
    record = local_db.cursor.fetchone()

    return record

def check_if_employee_instore(record):
    is_in_store = record.get(b'in_store').decode('utf-8')
    if is_in_store == '1':
        return True
    return False

def pipe_stream_process(camera, parameters, pipe_q, camfeed_break_flag):
    fp = FacePipe("entry")
    
    fp.create_named_pipe()

    while True:
        if camfeed_break_flag is True:
            break
        try:
            obj = pipe_q.get()
            faces = obj[0] # Faces
            frame = obj[1] # Frame
        except:
            continue

        try:
            fp.send_faces_to_pipe(parameters, faces, frame)
        except:
            print("Problem sending faces to pipe, continuing")
            continue
        time.sleep(0.100)

    fp.destroy_pipe()

def search_face_data(parameters, search_q, lock, camfeed_break_flag):
    local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)

    local_db.connect()
    if not local_db.connection:
        print("Local db connection failed!")
    else:
        print("Connected to localdb")

    in_mem_db.connect()
    if not in_mem_db.connection:
        print("Redis db connection failed!")
    else:
        print("Connected to redis db: 2")
    
    while True:
        if camfeed_break_flag is True:
            print("Camera feed stopped ending search face data process")
            break
 
        try:
            obj = search_q.get()
            record = obj[0] # Record
            face_encoding = obj[1] # Face encoding
        except:
            continue

        current_threshold = in_mem_db.get_threshold()
        # Check if we have the record in localdb i.e. the customer has visited before
        record_from_localdb = get_face_record_from_localdb(face_encoding, current_threshold, local_db)
        if record_from_localdb:
            # Overwrite everything
            # Delete new record and add existing record
            with lock:
                insert_existing_record_inmem(record, record_from_localdb, in_mem_db)
        time.sleep(0.100)

def enqueue_message(in_mem_db, message):
    in_mem_db.connection.rpush(BackendMessage.CancelQueue.value, message)

def consume_face_data(parameters, q, search_q, lock, camfeed_break_flag):
    # DB objects
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)

    in_mem_db.connect()
    if not in_mem_db.connection:
        print("Redis db connection failed!")
    else:
        print("Connected to redis db: 1")

    r = Recognition(parameters)
    p = Predictor(parameters)
    while True:
        if camfeed_break_flag is True:
            print("Camera feed stopped ending message queue consumer")
            break
        try:
            obj = q.get()
            faces = obj[0] # Faces
            frame = obj[1] # Frame
        except:
            continue

        # For each face, first see if it exists in mem otherwise try and fetch it from localdb
        for face in faces:
            # Constraints start
            yaw, pitch, roll = r.calculate_yaw_pitch_roll(frame, face, p)
            current_yaw = in_mem_db.get_yaw_threshold()
            current_pitch = in_mem_db.get_pitch_threshold()
            current_area = in_mem_db.get_area_threshold()
            current_threshold = in_mem_db.get_threshold()

            if abs(yaw) > float(current_yaw) or abs(pitch) < float(current_pitch):
                continue
            area = (face.right() - face.left()) * (face.bottom() - face.top())
            if area < float(current_area):
                continue
            # Constraints end

            face_encoding, face_pixels = get_face_image_encoding(r, face, frame)
            if face_encoding is None or face_pixels is None:
                continue

            with lock:
                # First check employee db
                record_from_mem = get_employee_face_record_from_mem(face_encoding, current_threshold, in_mem_db)
                if record_from_mem:
                    # Update employee record
                    if check_if_employee_instore(record_from_mem):
                        print("Employee  " + record_from_mem.get(b'name').decode('utf-8') + " already in store and standing infront of entry cam, what a noob!")
                        print_log(in_mem_db, "Backend", datetime.now(), "entry", "consume_face_data", "Success", "Employee already in store and standing infront of entry cam", line_number(), "INFO")
                        continue
                    print("Employee " + record_from_mem.get(b'name').decode('utf-8') + " entered the store, welcome back!")
                    print_log(in_mem_db, "Backend", datetime.now(), "entry", "consume_face_data", "Success", "Employee entered the store", line_number(), "INFO")
                    update_employee_inmem(in_mem_db, record_from_mem)
                    # Publish message employee entered
                    message = BackendMessage.EmployeeEntered.value + ":" + record_from_mem.get(b'employee_id').decode('utf-8')
                    in_mem_db.connection.publish(Channel.Employee.value, message)
                    continue

                record_from_mem = get_face_record_from_mem(face_encoding, current_threshold, in_mem_db)
                if record_from_mem:
                    exited = record_from_mem.get(b'exited').decode('utf-8')
                    if exited == '1':
                        cust_id = record_from_mem.get(b'customer_id').decode('utf-8')
                        # Publish cancel event for timer
                        update_exit_entry_customer(in_mem_db, cust_id)
                        enqueue_message(in_mem_db, cust_id)

                elif not record_from_mem:
                    # Create a record < Assign an ID < treat as new
                    new_record = insert_initial_record_inmem(face_encoding, face_pixels, in_mem_db)
                    if not new_record:
                        continue
                    # Add new record id and face encoding to search queue for local db search
                    send_faces_to_search_queue(new_record, face_encoding, search_q)
        time.sleep(0.100)

def send_faces_to_search_queue(record, face_encoding, search_q):
    item = (record, face_encoding)
    search_q.put(item)

def send_faces_to_pipe_queue(faces, frame, pipe_q):
    item = (faces, frame)
    pipe_q.put(item)

def send_faces_to_queue(faces, frame, q):
    item = (faces, frame)
    q.put(item)
    # Concerning if it keeps rising
    #print("Entry Queue size:", q.qsize())

class CameraThread(threading.Thread):
    def __init__(self, camera, name='CameraThread'):
        self.camera = camera
        self.last_frame = None
        super(CameraThread, self).__init__(name=name)
        self.start()

    def run(self):
        while True:
            ret, self.last_frame = self.camera.read()

# Start entry camera
def start_entry_cam(parameters, camera, cam_type, q, pipe_q, search_q, stop):

    # Load employee data
    load_employee_data()

    # Choose source
    if cam_type == "Index":
        camera = int(camera)
    cap = cv2.VideoCapture(camera)
    #cap = cv2.VideoCapture("rtsp://crasadmin:strongpass123@192.168.2.109:554/cam/realmonitor?channel=7&subtype=0")
    #cap = cv2.VideoCapture('rtsp://crasadmin:lol12345@192.168.2.108:554/cam/realmonitor?channel=4&subtype=0&proto=tcp')
    #cap.set(cv2.CAP_PROP_FOURCC, cv2.VideoWriter_fourcc('H', '2', '6', '4'))

    detector = Detection(parameters)

    lock = multiprocessing.Lock()

    stream_process = Process(target = pipe_stream_process, args = (camera, parameters, pipe_q, stop,), daemon = True)
    stream_process.name = "Camera_stream_entry"
    stream_process.start()

    num_consumers = NUM_CONSUMER_PROCESSES
    consumers = []

    for _ in range(num_consumers):
        consumer_process = Process(target = consume_face_data, args = (parameters, q, search_q, lock, stop,), daemon = True)
        consumer_process.name = "Frame_iterator_entry"
        consumer_process.start()
        consumers.append(consumer_process)

    num_search_process = NUM_SEARCH_PROCESSES
    search_processes = []
    for _ in range(num_search_process):
        search_process = Process(target = search_face_data, args = (parameters, search_q, lock, stop,), daemon = True)
        search_process.name = "Face_search_entry"
        search_process.start()
        search_processes.append(search_process)

    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    in_mem_db.connect()

    #Publish message that entry cam is up
    in_mem_db.connection.publish(Channel.Status.value, Status.EntryCamUp.value)
    # Acquire status.lock
    with fasteners.InterProcessLock(Utils.lock_file):
        Utils.entry_up()

    cam_thread = CameraThread(cap)
    while True:
        # ret, frame = cap.read()
        # if not ret:
        #     continue
        frame = cam_thread.last_frame
        if frame is None:
            continue

        roi = in_mem_db.get_entry_roi()
        roi = [float(value.decode('utf-8')) for value in roi]
        resize_factor = in_mem_db.get_resize_factor()
        cropped_frame = frame[int(roi[1]):int(roi[1])+int(roi[3]), int(roi[0]):int(roi[0])+int(roi[2])]
        new_width = int(float(resize_factor) * int(roi[2]))
        new_height = int(float(resize_factor) * int(roi[3]))
        frame = cv2.resize(cropped_frame, (new_width, new_height))

        if camfeed_break_flag is True:
            break

        faces = detect_faces_in_frame(detector, frame)

        # Send faces to pipe_queue for streaming
        send_faces_to_pipe_queue(faces, frame, pipe_q)
        if not faces:
            continue
        # Send faces to main queue for detection
        send_faces_to_queue(faces, frame, q)
        time.sleep(0.100)

    camfeed_break_flag.set()
    
    stream_process.terminate()
    stream_process.join()
    
    for consumer in consumers:
        consumer.terminate()
        consumer.join()
    
    for sp in search_processes:
        sp.terminate()
        sp.join()

    cap.release()
    cv2.destroyAllWindows()

def write_entry_pid():
    with open("entry_pid", "w") as f:
        f.write(str(os.getpid()))

if __name__ == "__main__":

    parser = argparse.ArgumentParser()
    parser.add_argument("-camera", type=str, help="Camera number for entry", required = True)
    parser.add_argument("-cam_type", type=str, help="Camera Type: Stream/Index", required = True)
    args = parser.parse_args()

    parameters = Parameters.build_parameters("config.ini")

    write_entry_pid()

    camfeed_break_flag = multiprocessing.Event()

    manager = Manager()
    message_queue = manager.Queue(maxsize=QUEUE_MAX_SIZE)
    pipe_queue = manager.Queue(maxsize = QUEUE_MAX_SIZE)
    search_queue = manager.Queue(maxsize = SEARCH_QUEUE_SIZE)

    start_entry_cam(parameters, args.camera, args.cam_type, message_queue, pipe_queue, search_queue, camfeed_break_flag)