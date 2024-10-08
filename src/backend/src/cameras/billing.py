import argparse
import configparser
from datetime import datetime
import cv2
import os
import time
import numpy as np
import fasteners
import multiprocessing

from sklearn.metrics.pairwise import cosine_similarity

# Private packages
from face.face import Detection, Recognition, Predictor

from config.params import Parameters
from db.database import *
from db.redis_pubsub import *
from db.log import *

from utils.utils import Utils

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

def find_and_publish_records(in_mem_db, faces, frame, r, p, parameters, type):

    # For each face, first see if it exists in mem
    for face in faces:
        # Constraints start
        # current_yaw = in_mem_db.get_yaw_threshold()
        # current_pitch = in_mem_db.get_pitch_threshold()
        #yaw, pitch, roll = r.calculate_yaw_pitch_roll(frame, face, p)
        #if abs(yaw) > float(current_yaw) or abs(pitch) < float(current_pitch):
        #    return
        current_area = in_mem_db.get_area_threshold()
        current_threshold = in_mem_db.get_threshold()
        area = (face.right() - face.left()) * (face.bottom() - face.top())
        if area < float(current_area):
            return
        # Constraints end

        face_encoding, face_pixels = get_face_image_encoding(r, face, frame)
        if face_encoding is None:
            return

        record_from_mem = get_face_record_from_mem(face_encoding, current_threshold, in_mem_db)

        if not record_from_mem:
            # Record is not found in memory
            pass
        else:
            cust_id = record_from_mem.get(b'customer_id')
            cust_id_list = in_mem_db.connection.lrange('cust_id_list', 0, -1)
            if cust_id not in cust_id_list:
                in_mem_db.connection.lpush('cust_id_list', cust_id)
                # Publish the customer record to the backend channel
                if type == "Billing":
                    print_log(in_mem_db, "Backend", datetime.now(), "billing", "BillingCustomer", cust_id.decode(), "Billing customer found", line_number(), "DEBUG")
                    message = BackendMessage.BillingCustomer.value + ":" + str(cust_id.decode())
                    in_mem_db.connection.publish(Channel.Billing.value, message)
                if type == "Rescan":
                    print_log(in_mem_db, "Backend", datetime.now(), "billing", "RescanCustomer", cust_id.decode(), "Rescan customer found", line_number(), "DEBUG")
                    message = BackendMessage.RescanCustomer.value + ":" + str(cust_id.decode())
                    in_mem_db.connection.publish(Channel.Billing.value, message)

def get_billing_frames(cam_time):
    return int(cam_time) * CAMERA_FPS

def capture_and_publish(in_mem_db, cap, detector, r, p, parameters, type):
    current_billing_cam_time = in_mem_db.get_billing_cam_time()
    billing_frames = get_billing_frames(current_billing_cam_time)
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
        
        find_and_publish_records(in_mem_db, faces, frame, r, p, parameters, type)

    # Clear customer id list
    in_mem_db.connection.ltrim('cust_id_list', 1, 0)

    if type == "Billing":
        in_mem_db.connection.publish(Channel.Billing.value, BackendMessage.EndBilling.value)
    if type == "Rescan":
        in_mem_db.connection.publish(Channel.Billing.value, BackendMessage.EndRescan.value)


# Start entry camera
def start_billing_cam(parameters, camera, cam_type, stop_event):

    # Choose source
    if cam_type == "Index":
        camera = int(camera)
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

    in_mem_db.connection.publish(Channel.Status.value, Status.BillingCamUp.value)
    with fasteners.InterProcessLock(Utils.lock_file):
        Utils.billing_up()

    while True:
        if stop_event.is_set():
            break
        for message in p.listen():
            if message['type'] == 'message':
                data = message['data']
                if isinstance(data, bytes):
                    data = data.decode('utf-8')
                if data == FrontendMessage.StartBilling.value:
                    capture_and_publish(in_mem_db, cap, detector, recognition, prediction, parameters, "Billing")
                if data == FrontendMessage.StartRescan.value:
                    capture_and_publish(in_mem_db, cap, detector, recognition, prediction, parameters, "Rescan")

    cap.release()
    cv2.destroyAllWindows()

def write_billing_pid():
    with open("billing_pid", "w") as f:
        f.write(str(os.getpid()))

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-camera", type=str, help="Camera number for billing", required = True)
    parser.add_argument("-cam_type", type=str, help="Camera Type: Stream/Index", required = True)

    args = parser.parse_args()

    parameters = Parameters.build_parameters("config.ini")

    write_billing_pid()

    stop_event = multiprocessing.Event()
    start_billing_cam(parameters, args.camera, args.cam_type, stop_event)