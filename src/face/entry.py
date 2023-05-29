import argparse
import configparser
import cv2
import time
import threading
import os
import queue
from face import Detection, Recognition, Rectangle, Similarity
from utils import get_location, generate_unique_id
from params import Parameters
from pipe import FacePipe
import multiprocessing
from multiprocessing import Process

QUEUE_MAX_SIZE = 100
message_queue = multiprocessing.Queue(maxsize=QUEUE_MAX_SIZE)
camfeed_break_flag = multiprocessing.Event()

def detect_faces_in_frame(detector, parameters, frame):
    faces = detector.detect(frame, 1)

    return faces

def get_face_image_encoding(r, parameters, face, frame):
    rect = Rectangle(face, parameters)
    x, y, width, height = rect.get_coordinates()
    embedding = r.get_encodings(face, frame)
    face_pixels = r.get_face_pixels(face, frame)

    return embedding, face_pixels

def get_face_record_from_mem(face_encoding):
    pass

def get_face_record_from_localdb(face_encoding):
    pass

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
            elapsed_time = time.perf_counter() - start_time
            print("Elapsed time:", elapsed_time, "seconds")
            record_from_mem = get_face_record_from_mem(face_encoding)

            if not record_from_mem:
                # Create a record < Assign an ID < treat as new
                
                record_from_localdb = get_face_record_from_localdb(face_encoding)
                if not record_from_localdb:
                    # Do nothing
                    pass
                else:
                    # Overwrite everything
                    pass
            else:
                pass

def send_faces_to_queue(faces, frame):
    message_queue.put((faces, frame))
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

    num_consumers = 1
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
