import argparse
import configparser
import cv2
import os
import io
import numpy as np
import multiprocessing
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

QUEUE_MAX_SIZE = 100000
SEARCH_QUEUE_SIZE = 100000
NUM_CONSUMER_PROCESSES = 1
NUM_SEARCH_PROCESSES = 1

def get_face_image(face_pixels, target_size=(160, 160)):
    #face_8bit = np.clip(face_pixels, 0, 255).astype(np.uint8)
    #face_image = Image.fromarray(face_8bit)
    face_pixels_rgb = cv2.cvtColor(face_pixels, cv2.COLOR_BGR2RGB)
    
    face_image = Image.fromarray(face_pixels_rgb)
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

def update_exit_entry_customer(in_mem_db, customer_id):
    record = in_mem_db.connection.hgetall("customer_inmem_db:" + customer_id)
    date_format = "%Y-%m-%d %H:%M:%S.%f"
    base_datetime = datetime(1900, 1, 1)
    exit_time = datetime.now()
    entry_time = datetime.strptime(record.get(b'entry_time').decode(), date_format)

    time_spent = exit_time - entry_time
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
        o_time = datetime.strptime(old_avg_time_spent, "%H:%M:%S.%f")
        existing_interval = o_time - base_datetime
        existing_interval_seconds = existing_interval.total_seconds()
        
        exisitng_total_seconds = existing_interval_seconds * num_visits

        total_time = exisitng_total_seconds - time_spent.total_seconds()

        updated_avg_time_spent_seconds = (total_time) / updated_num_visits
        delta = timedelta(seconds=updated_avg_time_spent_seconds)
        result_datetime = base_datetime + delta
        updated_avg_time_spent = result_datetime.strftime("%H:%M:%S.%f")

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
    
    print(updated_location_list)
    print(updated_avg_time_spent)


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
        incomplete = "0"
    )
    in_mem_db.delete_record(customer_id, type="customer")
    in_mem_db.delete_record(customer_id, type="visit")
    in_mem_db.insert_record(new_customer_record)
    in_mem_db.insert_record(new_visit_record, type="visit")
    return new_customer_record.customer_id

def insert_initial_record_inmem(face_encoding, face_pixels, in_mem_db):
    new_id = Utils.generate_unique_id()
    face_img = get_face_image(face_pixels)
    current_location = in_mem_db.fetch_store_location()
    store_id = in_mem_db.fetch_store_id()
    time_now = str(datetime.now())
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
        incomplete = "1"
    )

    in_mem_db.insert_record(new_customer_record)
    in_mem_db.insert_record(new_visit_record, type="visit")

    print("Welcome new customer")
    message = BackendMessage.NewCustomer.value + ":" + str(new_id)
    in_mem_db.connection.publish(Channel.Backend.value, message)
    return(new_customer_record)

def insert_existing_record_inmem(new_record, record, in_mem_db):
    # Delete exisitng record

    existing_visit_id = in_mem_db.fetch_visit_id(new_record.customer_id)

    entry_time = new_record.entry_time

    customer_id = record[0]
    name = record[1]
    phone_number = record[2]
    encoding = record[3]
    image = record[4]
    return_customer = record[5]
    last_visit = record[6]
    average_time_spent = record[7]
    average_bill_value = record[8]
    average_bill_per_visit = record[9]
    average_bill_per_billed_visit = record[10]
    maximum_purchase = record[11]
    remarks = record[12]
    loyalty_level = record[13]
    num_bills = record[14]
    num_visits = record[15]
    num_billed_visits = record[16]
    last_location = record[17]
    location_list = record[18]
    category = record[19]
    creation_date = record[20]
    group_id = record[21]

    encoding_str_list = encoding.strip("[]").split()
    encoding_str = ",".join(encoding_str_list)
    face_encoding_np = np.fromstring(encoding_str, sep=",", dtype=np.float32)

    # Insert exisitng record to in-mem   #DEBUG will face problems later
    existing_customer_record = InMemCustomer(
        customer_id = customer_id,
        name = name,
        phone_number = str(phone_number),
        encoding = face_encoding_np.tobytes(),
        image = image.tobytes(),
        return_customer = str(return_customer),
        last_visit = str(last_visit),
        average_time_spent = str(average_time_spent),
        average_bill_value= str(average_bill_value),
        average_bill_per_visit= str(average_bill_per_visit),
        average_bill_per_billed_visit= str(average_bill_per_billed_visit),
        maximum_purchase = str(maximum_purchase),
        remarks = remarks,
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
        customer_id=customer_id,
        visit_id=existing_visit_id,
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
        incomplete="1"
    )

    print("Inserting existing customer in memory")
    print("Welcome exisitng customer")
    in_mem_db.insert_record(existing_customer_record)
    in_mem_db.insert_record(modified_visit_record, type="visit")
    in_mem_db.delete_record(new_record.customer_id)
    in_mem_db.delete_record(new_record.customer_id, type="visit")

    message = BackendMessage.UpdateCustomer.value + ":" + str(record[0]) + "," + BackendMessage.TempCustomer.value + ":" + str(new_record.customer_id)
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

def pipe_stream_process(camera, parameters, pipe_q, camfeed_break_flag):
    fp = FacePipe(camera)
    
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

        # Check if we have the record in localdb i.e. the customer has visited before
        record_from_localdb = get_face_record_from_localdb(face_encoding, parameters.threshold, local_db)
        if record_from_localdb:
            # Overwrite everything
            # Delete new record and add existing record
            with lock:
                insert_existing_record_inmem(record, record_from_localdb, in_mem_db)

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
            if abs(yaw) > float(parameters.yaw_threshold) or abs(pitch) < float(parameters.pitch_threshold):
                continue
            area = (face.right() - face.left()) * (face.bottom() - face.top())
            if area < float(parameters.area_threshold):
                continue
            # Constraints end

            face_encoding, face_pixels = get_face_image_encoding(r, face, frame)
            if face_encoding is None or face_pixels is None:
                continue

            with lock:
                record_from_mem = get_face_record_from_mem(face_encoding, parameters.threshold, in_mem_db)
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

                    # Add new record id and face encoding to search queue for local db search
                    send_faces_to_search_queue(new_record, face_encoding, search_q)

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

# Start entry camera
def start_entry_cam(parameters, camera, q, pipe_q, search_q, stop):

    # Choose source
    cap = cv2.VideoCapture(camera)
    detector = Detection(parameters)

    lock = multiprocessing.Lock()

    stream_process = Process(target = pipe_stream_process, args = (camera, parameters, pipe_q, stop,))
    stream_process.name = "Camera_stream_entry"
    stream_process.start()

    num_consumers = NUM_CONSUMER_PROCESSES
    consumers = []
    for _ in range(num_consumers):
        consumer_process = Process(target = consume_face_data, args = (parameters, q, search_q, lock, stop,))
        consumer_process.name = "Frame_iterator_entry"
        consumer_process.start()
        consumers.append(consumer_process)

    num_search_process = NUM_SEARCH_PROCESSES
    search_processes = []
    for _ in range(num_search_process):
        search_process = Process(target = search_face_data, args = (parameters, search_q, lock, stop,))
        search_process.name = "Face_search_entry"
        search_process.start()
        search_processes.append(search_process)

    while True:
        ret, frame = cap.read()
        if not ret:
            break

        if camfeed_break_flag is True:
            break

        faces = detect_faces_in_frame(detector, frame)

        # Send faces to pipe_queue for streaming
        send_faces_to_pipe_queue(faces, frame, pipe_q)
        if not faces:
            continue
        # Send faces to main queue for detection
        send_faces_to_queue(faces, frame, q)

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

def write_entry_pid():
    with open("entry_pid", "w") as f:
        f.write(str(os.getpid()))

if __name__ == "__main__":

    parser = argparse.ArgumentParser()
    parser.add_argument("-camera", type=int, help="Camera number for entry", required = True)
    args = parser.parse_args()

    parameters = build_parameters("config.ini")

    write_entry_pid()

    camfeed_break_flag = multiprocessing.Event()

    manager = Manager()
    message_queue = manager.Queue(maxsize=QUEUE_MAX_SIZE)
    pipe_queue = manager.Queue(maxsize = QUEUE_MAX_SIZE)
    search_queue = manager.Queue(maxsize = SEARCH_QUEUE_SIZE)

    start_entry_cam(parameters, args.camera, message_queue, pipe_queue, search_queue, camfeed_break_flag)