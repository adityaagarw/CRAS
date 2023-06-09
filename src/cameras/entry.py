import argparse
import configparser
import cv2
import time
import threading
import os
import io
import queue
import signal
import sys
import numpy as np
import multiprocessing
from datetime import datetime
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

def get_face_image(face_pixels):
    face_8bit = np.clip(face_pixels, 0, 255).astype(np.uint8)
    face_image = Image.fromarray(face_8bit)
    img_bytes = io.BytesIO()
    face_image.save(img_bytes, format='PNG')
    img_data = img_bytes.getvalue()
    return img_data

def detect_faces_in_frame(detector, parameters, frame):
    faces = detector.detect(frame, 1)

    return faces

def get_face_image_encoding(r, parameters, face, frame):
    rect = Rectangle(face, parameters)
    x, y, width, height = rect.get_coordinates()
    embedding = r.get_encodings(face, frame)
    face_pixels = r.get_face_pixels(face, frame)

    return embedding, face_pixels

def insert_initial_record_inmem(face_encoding, face_pixels, in_mem_db):
    new_id = Utils.generate_unique_id()
    face_img = get_face_image(face_pixels)
    current_location = in_mem_db.fetch_store_location()
    store_id = in_mem_db.fetch_store_id()
    time_now = str(datetime.now())
    face_encoding_bytes = face_encoding.tobytes()

    # Reconstruct bytes as follows:
    #face_encoding = np.frombuffer(face_encoding_bytes, dtype=np.float64)
    #face_img = np.frombuffer(face_img_bytes, dtype=np.uint8)

    new_customer_record = InMemCustomer(
        customer_id = str(new_id),
        name = "",
        phone_number = "",
        encoding = face_encoding_bytes,
        image = face_img,
        return_customer = 0,
        last_visit = "",
        average_time_spent = "",
        average_purchase = "",
        maximum_purchase = "",
        remarks = "New Customer",
        loyalty_level = "",
        num_visits = 1,
        last_location = current_location,
        location_list = "",
        category = "",
        creation_date = time_now,
        group_id = "",
        incomplete = 1,
        entry_time = time_now,
        billed = 0,
        exited = 0,
        visit_time = "",
        exit_time = ""
    )

    new_visit_record = InMemVisit(
        customer_id = str(new_id),
        visit_id = str(Utils.generate_unique_id()),
        store_id = str(store_id),
        entry_time = time_now,
        exit_time = "",
        billed = 0,
        bill_amount = 0,
        time_spent = "",
        visit_remark = "New customer",
        customer_rating = "",
        customer_feedback = "",
        incomplete = 1
    )

    in_mem_db.insert_record(new_customer_record)
    in_mem_db.insert_record(new_visit_record, type="visit")

    print("Welcome new customer")
    in_mem_db.connection.publish(Channel.Backend.value, BackendMessage.NewCustomer.value)
    return(new_customer_record)

def insert_existing_record_inmem(new_record, record, in_mem_db):
    # Delete exisitng record
    in_mem_db.delete_record(new_record.customer_id)

    existing_visit_id = in_mem_db.fetch_visit_id(new_record.customer_id)
    in_mem_db.delete_record(new_record.customer_id, type="visit")

    # Insert exisitng record to in-mem   #DEBUG will face problems later
    existing_customer_record = InMemCustomer(
        customer_id=record[0],
        name=record[1],
        phone_number=record[2],
        encoding=record[3],
        image=record[4],
        return_customer=1,
        last_visit=record[6],
        average_time_spent=record[7],
        average_purchase=record[8],
        maximum_purchase=record[9],
        remarks=record[10],
        loyalty_level=record[11],
        num_visits=record[12],
        last_location=record[13],
        location_list=record[14],
        category=record[15],
        creation_date=record[16],
        group_id=record[17],
        incomplete=1,
        entry_time=str(new_record.entry_time),
        billed=0,
        exited=0,
        visit_time="",
        exit_time=""
    )

    modified_visit_record = InMemVisit(
        customer_id=record[0],
        visit_id=existing_visit_id,
        store_id=in_mem_db.fetch_store_id(),
        entry_time=str(new_record.entry_time),
        exit_time="",
        billed=0,
        bill_amount=0,
        time_spent="",
        visit_remark="",
        customer_rating="",
        customer_feedback="",
        incomplete=1
    )

    in_mem_db.insert_record(existing_customer_record)
    in_mem_db.insert_record(modified_visit_record, type="visit")
    in_mem_db.connection.publish(Channel.Backend.value, BackendMessage.ExisitingCustomer.value)

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
                        SELECT * FROM local_customer_db WHERE encoding <=> %(face_encoding)s > %(threshold)s LIMIT 1; 
                        """
    local_db.cursor.execute(face_record_query, {'face_encoding': face_encoding_str, 'threshold': threshold})
    record = local_db.cursor.fetchone()

    return record

def pipe_stream_process(camera, parameters, pipe_q, camfeed_break_flag):
    fp = FacePipe(camera)
    
    pipe = fp.create_named_pipe()

    while True:
        if camfeed_break_flag is True:
            break
        try:
            obj = pipe_q.get(timeout = 1)
            faces = obj[0] # Faces
            frame = obj[1] # Frame
        except:
            continue

        fp.send_faces_to_pipe(parameters, faces, frame, pipe)

    fp.destroy_pipe(pipe)

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
            obj = search_q.get(timeout = 1)
            record = obj[0] # Faces
            face_encoding = obj[1] # Frame
        except:
            continue

        # Check if we have the record in localdb i.e. the customer has visited before
        record_from_localdb = get_face_record_from_localdb(face_encoding, parameters.threshold, local_db)
        if record_from_localdb:
            # Overwrite everything
            # Delete new record and add existing record
            insert_existing_record_inmem(record, record_from_localdb, in_mem_db)

def consume_face_data(parameters, q, search_q, camfeed_break_flag):
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
            obj = q.get(timeout = 1)
            faces = obj[0] # Faces
            frame = obj[1] # Frame
        except:
            continue

        # For each face, first see if it exists in mem otherwise try and fetch it from localdb
        for face in faces:
            yaw, pitch, roll = r.calculate_yaw_pitch_roll(frame, face, p)
            if abs(yaw) > float(parameters.yaw_threshold) or abs(pitch) < float(parameters.pitch_threshold):
                continue
            #start_time = time.perf_counter()
            face_encoding, face_pixels = get_face_image_encoding(r, parameters, face, frame)
            if face_encoding is None:
                continue
            #elapsed_time = time.perf_counter() - start_time
            #print("Elapsed time:", elapsed_time, "seconds")
            record_from_mem = get_face_record_from_mem(face_encoding, parameters.threshold, in_mem_db)

            if not record_from_mem:
                # Create a record < Assign an ID < treat as new
                #start_time = time.perf_counter()
                new_record = insert_initial_record_inmem(face_encoding, face_pixels, in_mem_db)
                #elapsed_time = time.perf_counter() - start_time
                #print("INSERTION elapsed time:", elapsed_time, "seconds")

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
    print("Queue size:", q.qsize())

# Start entry camera
def start_entry_cam(parameters, camera, q, pipe_q, search_q, stop):

    # Choose source
    cap = cv2.VideoCapture(camera)
    detector = Detection(parameters)

    stream_process = Process(target = pipe_stream_process, args = (camera, parameters, pipe_q, stop,))
    stream_process.name = "Camera_stream"
    stream_process.start()

    num_consumers = NUM_CONSUMER_PROCESSES
    consumers = []
    for _ in range(num_consumers):
        consumer_process = Process(target = consume_face_data, args = (parameters, q, search_q, stop,))
        consumer_process.name = "Frame_iterator"
        consumer_process.start()
        consumers.append(consumer_process)

    num_search_process = NUM_SEARCH_PROCESSES
    search_processes = []
    for _ in range(num_search_process):
        search_process = Process(target = search_face_data, args = (parameters, search_q, stop,))
        search_process.name = "Face_search"
        search_process.start()
        search_processes.append(search_process)

    while True:
        ret, frame = cap.read()
        if not ret:
            break

        if camfeed_break_flag is True:
            break

        faces = detect_faces_in_frame(detector, parameters, frame)
        if not faces:
            continue

        # Send faces to pipe_queue for streaming
        send_faces_to_pipe_queue(faces, frame, pipe_q)
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