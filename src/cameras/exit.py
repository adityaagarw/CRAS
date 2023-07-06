import argparse
import configparser
import cv2
import os
import io
import threading
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
EXIT_EXPIRY_TIME = 15.0

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

def insert_record_to_incomplete_mem(face_encoding, inmem_db):
    new_record = InMemIncomplete(
        customer_id = str(Utils.generate_unique_id()),
        encoding = face_encoding.tobytes()
    )
    inmem_db.insert_record(new_record, type="incomplete")

def insert_record_to_exited_mem(face_encoding, inmem_db):
    new_record = InMemExited(
        customer_id = str(Utils.generate_unique_id()),
        encoding = face_encoding.tobytes()
    )
    inmem_db.insert_record(new_record, type="exited")
#################################################################################
def create_new_record_and_insert_to_localdb(face_encoding, face_pixels, in_mem_db, local_db):
    date_format = "%Y-%m-%d %H:%M:%S"
    new_id = Utils.generate_unique_id()
    face_img = get_face_image(face_pixels)
    current_location = in_mem_db.fetch_store_location()
    store_id = in_mem_db.fetch_store_id()
    time_now = datetime.now().strftime(date_format)
    face_encoding_bytes = face_encoding.tobytes()
    loc_list = "{" + str(current_location) + "}"

    encoding = np.frombuffer(face_encoding_bytes, dtype=np.float32).tolist()

    new_customer_record = LocalCustomer(
        customer_id = str(new_id),
        name = "",
        phone_number = "",
        encoding = encoding,
        image = face_img,
        return_customer = "0",
        last_visit = "",
        average_time_spent = "",
        average_bill_value = "",
        average_bill_per_visit = "",
        average_bill_per_billed_visit="",
        maximum_purchase = "",
        remarks = "New Customer",
        loyalty_level = "",
        num_bills = "",
        num_visits = "1",
        num_billed_visits = "",
        last_location = loc_list,
        location_list = "",
        category = "",
        creation_date = str(time_now),
        group_id = "",
    )

    new_visit_record = LocalVisit(
        customer_id = str(new_id),
        visit_id = str(Utils.generate_unique_id()),
        store_id = str(store_id),
        entry_time = "",
        exit_time = str(time_now),
        billed = "0",
        bill_no = "",
        bill_date = "",
        bill_amount = "0",
        return_amount = "0",
        time_spent = "",
        visit_remark = "New incomplete customer",
        customer_rating = "",
        customer_feedback = "",
        incomplete = "1"
    )

    local_db.insert_customer_record(new_customer_record)
    local_db.insert_visit_record(new_visit_record)
    print("New customer record created and inserted to local db")

def insert_existing_record_to_visit(record, in_mem_db, local_db):
    # Delete exisitng record
    date_format = "%Y-%m-%d %H:%M:%S"
    customer_id = record[0]
    exit_time = datetime.now().strftime(date_format)

    new_visit_record = LocalVisit(
        customer_id = customer_id,
        visit_id = str(Utils.generate_unique_id()),
        store_id = str(in_mem_db.fetch_store_id()),
        entry_time = "",
        exit_time = exit_time,
        billed = "0",
        bill_no = "",
        bill_date = "",
        bill_amount = "0",
        return_amount = "0",
        time_spent = "",
        visit_remark = "Incomplete visit",
        customer_rating = "",
        customer_feedback = "",
        incomplete = "1"
    )
    local_db.insert_visit_record(new_visit_record)
    print("Customer not found in memory but found in local db, recording visit")
#################################################################################
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

def get_face_record_from_incomplete_mem(face_encoding, threshold, in_mem_db):
    records = in_mem_db.connection.keys('incomplete_inmem_db:*')

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

def get_face_record_from_exited_mem(face_encoding, threshold, in_mem_db):
    records = in_mem_db.connection.keys('exited_inmem_db:*')

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

def commit_record(customer_id):
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')

    in_mem_db.connect()
    local_db.connect()

    customer_record = in_mem_db.connection.hgetall("customer_inmem_db:" + str(customer_id))
    visit_record = in_mem_db.connection.hgetall("visit_inmem_db:" + str(customer_id))
    if customer_record and visit_record:
        face_encoding = np.frombuffer(customer_record.get(b'encoding'), dtype=np.float32).tolist()
        ins_customer_record = LocalCustomer(
            customer_id = customer_record.get(b'customer_id').decode(),
            name = customer_record.get(b'name').decode(),
            phone_number = customer_record.get(b'phone_number').decode(),
            encoding = face_encoding,
            image = customer_record.get(b'image'),
            return_customer = int(customer_record.get(b'return_customer').decode()),
            last_visit = customer_record.get(b'last_visit').decode(),
            average_time_spent = int(float(customer_record.get(b'average_time_spent').decode())),
            average_bill_value = customer_record.get(b'average_bill_value').decode(),
            average_bill_per_visit = customer_record.get(b'average_bill_per_visit').decode(),
            average_bill_per_billed_visit = customer_record.get(b'average_bill_per_billed_visit').decode(),
            maximum_purchase = customer_record.get(b'maximum_purchase').decode(),
            remarks = customer_record.get(b'remarks').decode(),
            loyalty_level = customer_record.get(b'loyalty_level').decode(),
            num_bills = int(customer_record.get(b'num_bills').decode()),
            num_visits = int(customer_record.get(b'num_visits').decode()),
            num_billed_visits = int(customer_record.get(b'num_billed_visits').decode()),
            last_location = customer_record.get(b'last_location').decode(),
            location_list = customer_record.get(b'location_list').decode(),
            category = customer_record.get(b'category').decode(),
            creation_date = customer_record.get(b'creation_date').decode(),
            group_id = customer_record.get(b'group_id').decode(),
        )

        if (ins_customer_record.average_bill_value == ""):
            ins_customer_record.average_bill_value = "0"

        if (ins_customer_record.average_bill_per_visit == ""):
            ins_customer_record.average_bill_per_visit = "0"

        if (ins_customer_record.average_bill_per_billed_visit == ""):
            ins_customer_record.average_bill_per_billed_visit = "0"

        if (ins_customer_record.maximum_purchase == ""):
            ins_customer_record.maximum_purchase = "0"

        ins_visit_record = LocalVisit(
            customer_id = visit_record.get(b'customer_id').decode(),
            visit_id = visit_record.get(b'visit_id').decode(),
            store_id = visit_record.get(b'store_id').decode(),
            entry_time = visit_record.get(b'entry_time').decode(),
            exit_time = visit_record.get(b'exit_time').decode(),
            billed = int(visit_record.get(b'billed').decode()),
            bill_no = visit_record.get(b'bill_no').decode(),
            bill_date = visit_record.get(b'bill_date').decode(),
            bill_amount = visit_record.get(b'bill_amount').decode(),
            return_amount = visit_record.get(b'return_amount').decode(),
            time_spent = int(float(visit_record.get(b'time_spent').decode())),
            visit_remark = visit_record.get(b'visit_remark').decode(),
            customer_rating = visit_record.get(b'customer_rating').decode(),
            customer_feedback = visit_record.get(b'customer_feedback').decode(),
            incomplete = int(visit_record.get(b'incomplete').decode())
        )

        if (customer_record.get(b'return_customer').decode() == "1"):
            local_db.update_customer_record(ins_customer_record)
        else:
            local_db.insert_customer_record(ins_customer_record)

        local_db.insert_visit_record(ins_visit_record)
        in_mem_db.delete_record(str(customer_id), type="customer")
        in_mem_db.delete_record(str(customer_id), type="visit")
        in_mem_db.delete_record(str(customer_id), type="exited")
        in_mem_db.connection.publish(Channel.Backend.value, BackendMessage.DeleteCustomer.value + ":" + str(customer_id))

    in_mem_db.disconnect()
    local_db.disconnect()

def start_expiry_timer(customer_id):
    timer = threading.Timer(EXIT_EXPIRY_TIME, commit_record, args=(customer_id,)) #DEBUG increase time
    timer.start()
    return timer

def update_record_inmem(record, in_mem_db):

    date_format = "%Y-%m-%d %H:%M:%S"
    exit_time = datetime.strptime(datetime.now().strftime(date_format), date_format)
    entry_time = datetime.strptime(record.get(b'entry_time').decode(), date_format)

    time_spent = (exit_time - entry_time).total_seconds()

    num_visits = int(record.get(b'num_visits').decode())
    customer_id = str(record.get(b'customer_id').decode())

    # Update number of visits
    updated_num_visits = num_visits + 1

    # Update average time spent
    old_avg_time_spent = float(record.get(b'average_time_spent').decode())
    if old_avg_time_spent == "":
        updated_avg_time_spent = str(time_spent)
    else:
        # Convert existing avg time to datetime

        existing_interval_seconds = old_avg_time_spent
        
        exisitng_total_seconds = existing_interval_seconds * num_visits

        total_time = exisitng_total_seconds + time_spent

        updated_avg_time_spent = (total_time) / updated_num_visits

    # Update location list and last location
    current_location = str(in_mem_db.fetch_store_location())
    updated_last_location = current_location
    if record.get(b'location_list').decode() == "":
        updated_location_list = "{" + current_location + "}"
    else:
        location_list_string = record.get(b'location_list').decode().strip("{}")
        location_list_string = location_list_string + "," + current_location
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
    remarks = record.get(b'remarks').decode()
    num_bills = record.get(b'num_bills').decode()
    num_billed_visits = record.get(b'num_billed_visits').decode() # DEBUG
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
        exited = "1",
        visit_time = str(time_spent),
        exit_time = str(exit_time)
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
        time_spent = str(time_spent),
        visit_remark = v_visit_remark,
        customer_rating = v_customer_rating,
        customer_feedback = v_customer_feedback,
        incomplete = "0"
    )
    in_mem_db.delete_record(customer_id, type="customer")
    in_mem_db.delete_record(customer_id, type="visit")
    in_mem_db.insert_record(new_customer_record)
    in_mem_db.insert_record(new_visit_record, type="visit")
    messsage = BackendMessage.ExitingCustomer.value + ":" + str(customer_id)
    in_mem_db.connection.publish(Channel.Backend.value, messsage)
    return new_customer_record.customer_id

def get_face_record_from_localdb(face_encoding, threshold, local_db):
    # Query to get nearest similarity face record
    face_encoding_str = f"{face_encoding.tolist()}"
    face_record_query = """
                        SELECT * FROM local_customer_db WHERE encoding <=> %(face_encoding)s > %(threshold)s LIMIT 1; 
                        """
    local_db.cursor.execute(face_record_query, {'face_encoding': face_encoding_str, 'threshold': threshold})
    record = local_db.cursor.fetchone()

    return record

def pipe_stream_process(camera, parameters, pipe_q, camfeed_break_flag):
    fp = FacePipe("exit")
    
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

def search_face_data(parameters, search_q, camfeed_break_flag):
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
            face_encoding = obj[0]
            face_pixels = obj[1]
        except:
            continue

        # Check if we have the record in localdb i.e. the customer has visited before
        record_from_localdb = get_face_record_from_localdb(face_encoding, parameters.threshold, local_db)
        if record_from_localdb:
            # Overwrite everything
            # Delete new record and add existing record
            insert_existing_record_to_visit(record_from_localdb, in_mem_db, local_db)
        elif not record_from_localdb:
            create_new_record_and_insert_to_localdb(face_encoding, face_pixels, in_mem_db, local_db)

def dequeue_messages(in_mem_db):
    messages = []
    while True:
        message = in_mem_db.connection.lpop("CancelQueue")
        if message:
            messages.append(message.decode())
        else:
            break
    return messages

def cancel_timer(timer_dict):
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    in_mem_db.connect()
    p = in_mem_db.connection.pubsub()
    p.subscribe(Channel.CancelTimer.value)

    messages = dequeue_messages(in_mem_db)
    for message in messages:
        timer = timer_dict.get(message)
        if timer:
            timer.cancel()
            timer_dict.pop(message)
            print("Timer cancelled for customer: ", message)

def consume_face_data(parameters, q, search_q, camfeed_break_flag):
    # DB objects
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)

    in_mem_db.connect()
    if not in_mem_db.connection:
        print("Redis db connection failed!")
    else:
        print("Connected to redis db: 1")

    timer_dict = {}

    r = Recognition(parameters)
    p = Predictor(parameters)
    while True:
        messages = dequeue_messages(in_mem_db)
        for message in messages:
            timer = timer_dict.get(message)
            if timer:
                timer.cancel()
                timer_dict.pop(message)
                print("Timer cancelled for customer: ", message)

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
            if face_encoding is None:
                continue

            record_from_mem = get_face_record_from_mem(face_encoding, parameters.threshold, in_mem_db)

            if record_from_mem:
                record_from_exited_mem = get_face_record_from_exited_mem(face_encoding, parameters.threshold, in_mem_db)
                if not record_from_exited_mem:
                    id = update_record_inmem(record_from_mem, in_mem_db)
                    timer = start_expiry_timer(id) #DEBUG Remove exited entry on timer expire
                    timer_dict[id] = timer
                    insert_record_to_exited_mem(face_encoding, in_mem_db)

            elif not record_from_mem:
                record_from_incomplete_mem = get_face_record_from_incomplete_mem(face_encoding, parameters.threshold, in_mem_db)
                # Add new record id and face encoding to search queue for local db search
                if not record_from_incomplete_mem:
                    insert_record_to_incomplete_mem(face_encoding, in_mem_db)
                    send_faces_to_search_queue(face_encoding, face_pixels, search_q)

def send_faces_to_search_queue(face_encoding, face_pixels, search_q):
    item = (face_encoding, face_pixels)
    search_q.put(item)

def send_faces_to_pipe_queue(faces, frame, pipe_q):
    item = (faces, frame)
    pipe_q.put(item)

def send_faces_to_queue(faces, frame, q):
    item = (faces, frame)
    q.put(item)
    # Concerning if it keeps rising
    print("Exit Queue size:", q.qsize())

# Start entry camera
def start_exit_cam(parameters, camera, q, pipe_q, search_q, stop):

    # Choose source
    cap = cv2.VideoCapture(camera)
    detector = Detection(parameters)

    stream_process = Process(target = pipe_stream_process, args = (camera, parameters, pipe_q, stop,))
    stream_process.name = "Camera_stream_exit"
    stream_process.start()

    num_consumers = NUM_CONSUMER_PROCESSES
    consumers = []
    for _ in range(num_consumers):
        consumer_process = Process(target = consume_face_data, args = (parameters, q, search_q, stop,))
        consumer_process.name = "Frame_iterator_exit"
        consumer_process.start()
        consumers.append(consumer_process)

    num_search_process = NUM_SEARCH_PROCESSES
    search_processes = []
    for _ in range(num_search_process):
        search_process = Process(target = search_face_data, args = (parameters, search_q, stop,))
        search_process.name = "Face_search_exit"
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

def write_exit_pid():
    with open("exit_pid", "w") as f:
        f.write(str(os.getpid()))

if __name__ == "__main__":

    parser = argparse.ArgumentParser()
    parser.add_argument("-camera", type=int, help="Camera number for exit", required = True)
    args = parser.parse_args()

    parameters = build_parameters("config.ini")

    write_exit_pid()

    camfeed_break_flag = multiprocessing.Event()

    manager = Manager()
    message_queue = manager.Queue(maxsize=QUEUE_MAX_SIZE)
    pipe_queue = manager.Queue(maxsize = QUEUE_MAX_SIZE)
    search_queue = manager.Queue(maxsize = SEARCH_QUEUE_SIZE)

    start_exit_cam(parameters, args.camera, message_queue, pipe_queue, search_queue, camfeed_break_flag)