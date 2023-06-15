import argparse
import configparser
import cv2
import os
import time
import numpy as np
import multiprocessing

from sklearn.metrics.pairwise import cosine_similarity

# Private packages
from face.face import Detection, Recognition, Predictor

from config.params import Parameters
from db.database import *
from db.redis_pubsub import *

CAMERA_FPS = 30

def detect_faces_in_frame(detector, frame):
    faces = detector.detect(frame, 1)
    return faces

def get_face_image_encoding(r, face, frame):
    face_pixels = r.get_face_pixels_for_image(face, frame)
    embedding = r.get_encodings(face, frame)
    return embedding, face_pixels

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

def find_and_publish_records(in_mem_db, faces, frame, r, p, parameters):

    # For each face, first see if it exists in mem
    for face in faces:
        # Constraints start
        #yaw, pitch, roll = r.calculate_yaw_pitch_roll(frame, face, p)
        #if abs(yaw) > float(parameters.yaw_threshold) or abs(pitch) < float(parameters.pitch_threshold):
        #    return
        area = (face.right() - face.left()) * (face.bottom() - face.top())
        if area < float(parameters.area_threshold):
            return
        # Constraints end

        face_encoding, face_pixels = get_face_image_encoding(r, face, frame)
        if face_encoding is None:
            return

        record_from_mem = get_face_record_from_mem(face_encoding, parameters.threshold, in_mem_db)

        if not record_from_mem:
            # Record is not found in memory
            pass
        else:
            cust_id = record_from_mem.get(b'customer_id')
            cust_id_list = in_mem_db.connection.lrange('cust_id_list', 0, -1)
            if cust_id not in cust_id_list:
                in_mem_db.connection.lpush('cust_id_list', cust_id)
                # Publish the customer record to the backend channel
                message = BackendMessage.BillingCustomer.value + ":" + str(cust_id.decode())
                in_mem_db.connection.publish(Channel.Billing.value, message)

def get_billing_frames(cam_time):
    return int(cam_time) * CAMERA_FPS

def capture_and_publish(in_mem_db, cap, detector, r, p, parameters):
    billing_frames = get_billing_frames(parameters.billing_cam_time)
    billing_frames_counter = 0
    frame_count = 0
    while billing_frames_counter <= billing_frames:

        if frame_count % 10 != 0: # Only capture every 10th frame
            billing_frames_counter += 1
            frame_count += 1
            continue

        ret, frame = cap.read()
        if not ret:
            break

        billing_frames_counter += 1
        frame_count += 1

        # Get faces
        faces = detect_faces_in_frame(detector, frame)
        
        if not faces:
            continue
        # Send faces to queue
        
        find_and_publish_records(in_mem_db, faces, frame, r, p, parameters)

    # Clear customer id list
    in_mem_db.connection.ltrim('cust_id_list', 1, 0)
    in_mem_db.publish(Channel.Billing.value, BackendMessage.EndBilling.value)

# Start entry camera
def start_billing_cam(parameters, camera, stop_event):

    # Choose source
    cap = cv2.VideoCapture(camera)
    detector = Detection(parameters)
    recognition = Recognition(parameters)
    prediction = Predictor(parameters)

    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    in_mem_db.connect()
    if not in_mem_db.connection:
        print("Redis db connection failed!: Billing")
        return
        
    p = in_mem_db.connection.pubsub()
    p.subscribe(Channel.Frontend.value)

    while True:
        if stop_event.is_set():
            break
        for message in p.listen():
            if message['type'] == 'message':
                data = message['data']
                if isinstance(data, bytes):
                    data = data.decode('utf-8')
                if data == FrontendMessage.StartBilling.value:
                    capture_and_publish(in_mem_db, cap, detector, recognition, prediction, parameters)

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

def write_billing_pid():
    with open("billing_pid", "w") as f:
        f.write(str(os.getpid()))

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-camera", type=int, help="Camera number for billing", required = True)
    args = parser.parse_args()

    parameters = build_parameters("config.ini")

    write_billing_pid()

    stop_event = multiprocessing.Event()
    start_billing_cam(parameters, args.camera, stop_event)