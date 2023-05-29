import configparser
import time
import os
import cv2
import dlib
from face import Detection, Recognition, Rectangle, Similarity
from params import Parameters


image_path = "/home/aditya/cras/face_recognition_system/src/CRAS/src/models/test.jpg"

def get_face_image_encoding(r, parameters, face, frame):
    rect = Rectangle(face, parameters)
    x, y, width, height = rect.get_coordinates()
    embedding = r.get_encodings(face, frame)
    face_pixels = r.get_face_pixels(face, frame)

    return embedding, face_pixels

def detect_faces_in_frame(detector, parameters, frame):
    faces = detector.detect(frame, 1)

    return faces

def start_test(parameters):
    #frame = cv2.imread(image_path) 
    cap = cv2.VideoCapture(parameters.video_path + "/test.mp4")
    r = Recognition(parameters)
    detector = Detection(parameters)
    det = dlib.get_frontal_face_detector()
    det_g = dlib.get_frontal_face_detector()
    while True:
        ret, frame = cap.read()
        if not ret:
            break

        start_time = time.perf_counter() 
        faces = detect_faces_in_frame(detector, parameters, frame)
        elapsed_time = time.perf_counter() - start_time
        print("Detection time class:", elapsed_time, "seconds")

        for face in faces:
            start_time = time.perf_counter()
            encoding, face_img = get_face_image_encoding(r, parameters, face, frame)    
            elapsed_time = time.perf_counter() - start_time
            print("Encoding time:", elapsed_time, "seconds")

        cv2.imshow('Processed Image', frame)
        if (cv2.waitKey(1) & 0xFF == ord('q')):
            break

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
    parameters = build_parameters("config.ini")
    start_test(parameters)
