import argparse
import configparser
import cv2
import time
import threading
import os
import io
import queue
import numpy as np
import multiprocessing
from datetime import datetime
from PIL import Image
from multiprocessing import Process
from sklearn.metrics.pairwise import cosine_similarity

# Private packages
from face.face import Detection, Recognition, Rectangle, Similarity
from face.pipe import FacePipe
from utils.utils import Utils
from config.params import Parameters
from db.database import *

QUEUE_MAX_SIZE = 100000
message_queue = multiprocessing.Queue(maxsize=QUEUE_MAX_SIZE)
camfeed_break_flag = multiprocessing.Event()

# DB objects
local_db = LocalPostgresDB(host='localhost', port=5432, database='localdb', user='cras_admin', password='admin')
in_mem_db = InMemoryRedisDB(host='localhost', port=6379)

local_db.connect()
if not local_db.connection:
    print("Local db connection failed!")
else:
    print("Connected to localdb")

in_mem_db.connect()
if not in_mem_db.connection:
    print("Redis db connection failed!")
else:
    print("Connected to redis db")

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

def insert_initial_record_inmem(face_encoding, face_pixels):
    new_id = Utils.generate_unique_id()
    face_img = get_face_image(face_pixels)
    current_location = Utils.get_location()

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
        entry_time = str(datetime.now()),
        billed = 0,
        exited = 0,
        visit_time = "",
        exit_time = ""
    )

    in_mem_db.insert_record(new_customer_record)  

    print("Welcome new customer")
    return(new_customer_record)

def insert_existing_record_inmem(new_record, record):
    # Delete exisitng record
    in_mem_db.delete_record(new_record.customer_id)

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
        entry_time=str(new_record.entry_time),
        billed=str(new_record.entry_time),
        exited=0,
        visit_time=None,
        exit_time=None
    )
    in_mem_db.insert_record(existing_customer_record)

def get_face_record_from_mem(face_encoding, threshold):
    # Get all customer records from the in-memory Redis database
    records = in_mem_db.connection.keys('customer_inmem_db:*')

    # Initialize variables to track the closest record and similarity
    closest_record = None
    closest_similarity = -1.0

    # Iterate over each record
    for record_key in records:
        # Retrieve the face encoding from the record
        record_data = in_mem_db.connection.hgetall(record_key)
        #record_encoding = record_data.get('encoding')
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

def get_face_record_from_localdb(face_encoding, threshold):
    # Query to get nearest similarity face record
    face_encoding_str = f"{face_encoding.tolist()}"
    face_record_query = """
                        SELECT * FROM local_customer_db WHERE encoding <=> %(face_encoding)s > %(threshold)s LIMIT 1; 
                        """
    local_db.cursor.execute(face_record_query, {'face_encoding': face_encoding_str, 'threshold': threshold})
    record = local_db.cursor.fetchone()

    return record

def consume_face_data(parameters):
    r = Recognition(parameters)
    while True:
        if camfeed_break_flag is True:
            print("Camera feed stopped ending message queue consumer")
            break
        try:
            obj = message_queue.get()
            faces = obj[0] # Faces
            frame = obj[1] # Frame
        except message_queue.Empty:
            print("Queue is empty!")
            break

        # For each face, first see if it exists in mem otherwise try and fetch it from localdb
        for face in faces:
            start_time = time.perf_counter()
            face_encoding, face_pixels = get_face_image_encoding(r, parameters, face, frame)
            if face_encoding is None:
                continue
            elapsed_time = time.perf_counter() - start_time
            print("Elapsed time:", elapsed_time, "seconds")
            record_from_mem = get_face_record_from_mem(face_encoding, parameters.threshold)

            if not record_from_mem:
                # Create a record < Assign an ID < treat as new
                new_record = insert_initial_record_inmem(face_encoding, face_pixels)
               
                # Check if we have the record in localdb i.e. the customer has visited before 
                record_from_localdb = get_face_record_from_localdb(face_encoding, parameters.threshold)
                if not record_from_localdb:
                    # Do nothing, we have already created a new customer
                    # Record to be added to localdb on exit/billing
                    pass
                else:
                    # Overwrite everything
                    # Delete new record and add existing record
                    insert_existing_record_inmem(new_record, record_from_localdb)
                    pass
            else:
                # Do nothing
                pass

def send_faces_to_queue(faces, frame):
    message_queue.put((faces, frame))
    # Concerning if it keeps rising
    print("Queue size:", message_queue.qsize())

def pipe_stream_process(camera, parameters, cap):
    fp = FacePipe(camera)

    cap = cv2.VideoCapture(parameters.video_path + "/test.mp4")
    pipe = fp.create_named_pipe()
    detector = Detection(parameters)
    while True:
        ret, frame = cap.read()
        if not ret:
            break

        faces = detect_faces_in_frame(detector, parameters, frame)
        fp.send_faces_to_pipe(parameters, faces, frame, pipe)

    fp.destroy_pipe(pipe)


def start_entry_cam(parameters, camera):

    # Choose source
    #cap = cv2.VideoCapture(camera)
    cap = cv2.VideoCapture(parameters.video_path + "/test.mp4")

    detector = Detection(parameters)

    stream_process = Process(target = pipe_stream_process, args = (camera, parameters, cap,))
    stream_process.start()

    num_consumers = 2
    consumers = []
    for _ in range(num_consumers):
        consumer_process = Process(target = consume_face_data, args = (parameters,))
        consumer_process.start()
        consumers.append(consumer_process)

    while True:
        ret, frame = cap.read()
        if not ret:
            break

        # 1. Detect faces in frame
        faces = detect_faces_in_frame(detector, parameters, frame)

        if not faces:
            continue

        # 2. Send faces to message queue for recognition and db operations
        send_faces_to_queue(faces, frame)

    camfeed_break_flag.set()
    stream_process.join()
    for consumer in consumers:
        consumer.join()
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

if __name__ == "__main__":
    parser = argparse.ArgumentParser()

    parser.add_argument("-camera", type=int, help="Camera number for entry", required = True)

    args = parser.parse_args()

    parameters = build_parameters("config.ini")

    start_entry_cam(parameters, args.camera)
