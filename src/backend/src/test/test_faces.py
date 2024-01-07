import cv2
import argparse
import configparser
import numpy as np
import math
from sklearn.metrics.pairwise import cosine_similarity
from face.face import Detection, Recognition, Predictor, Rectangle, Similarity
from config.params import Parameters

def get_face_image_encoding(r, parameters, face, frame):
    rect = Rectangle(face, parameters)
    x, y, width, height = rect.get_coordinates()
    embedding = r.get_encodings(face, frame)
    face_pixels = r.get_face_pixels(face, frame)

    return embedding, face_pixels

def calculate_yaw_pitch_roll(frame, rect, p):
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    landmarks = p.get_shape(gray, rect)
    #2D image points. If you change the image, you need to change vector
    image_points = np.array([
                                (landmarks.part(30).x, landmarks.part(30).y),     # Nose tip
                                (landmarks.part(8).x, landmarks.part(8).y),     # Chin
                                (landmarks.part(36).x, landmarks.part(36).y),     # Left eye left corner
                                (landmarks.part(45).x, landmarks.part(45).y),     # Right eye right corner
                                (landmarks.part(48).x, landmarks.part(48).y),     # Left Mouth corner
                                (landmarks.part(54).x, landmarks.part(54).y)      # Right mouth corner
                            ], dtype="double")

    landmarks_arr = []
    for i in range(0, 68):
        x = landmarks.part(i).x
        y = landmarks.part(i).y
        landmarks_arr.append([x, y])

    # Loop through each landmark
    for landmark in landmarks_arr:
        # Draw a circle at the landmark position
        center = tuple(landmark)
        radius = 1
        color = (0, 255, 0)
        thickness = -1
        cv2.circle(frame, center, radius, color, thickness)

    # 3D model points.
    model_points = np.array([
                                (0.0, 0.0, 0.0),             # Nose tip
                                (0.0, -330.0, -65.0),        # Chin
                                (-225.0, 170.0, -135.0),     # Left eye left corner
                                (225.0, 170.0, -135.0),      # Right eye right corner
                                (-150.0, -150.0, -125.0),    # Left Mouth corner
                                (150.0, -150.0, -125.0)      # Right mouth corner
                        
                            ])

    # Camera internals
    size = frame.shape
    focal_length = size[1]
    center = (size[1]/2, size[0]/2)
    camera_matrix = np.array(
                             [[focal_length, 0, center[0]],
                             [0, focal_length, center[1]],
                             [0, 0, 1]], dtype = "double"
                             )
 
    dist_coeffs = np.zeros((4,1)) # Assuming no lens distortion
    (success, rotation_vector, translation_vector) = cv2.solvePnP(model_points, image_points, camera_matrix, dist_coeffs, flags=cv2.SOLVEPNP_ITERATIVE)
 
    # Project 3D points for axis lines
    axis_points = np.array([
        (500.0, 0.0, 0.0),     # X-axis points
        (0.0, 500.0, 0.0),     # Y-axis points
        (0.0, 0.0, 500.0)      # Z-axis points
    ], dtype="double")

    imgpts, _ = cv2.projectPoints(axis_points, rotation_vector, translation_vector, camera_matrix, dist_coeffs)

    # Draw the axis lines
    cv2.line(frame, (int(image_points[0][0]), int(image_points[0][1])), (int(imgpts[0][0][0]), int(imgpts[0][0][1])), (255, 0, 0), 2)  # X-axis line (blue)
    cv2.line(frame, (int(image_points[0][0]), int(image_points[0][1])), (int(imgpts[1][0][0]), int(imgpts[1][0][1])), (0, 255, 0), 2)  # Y-axis line (green)
    cv2.line(frame, (int(image_points[0][0]), int(image_points[0][1])), (int(imgpts[2][0][0]), int(imgpts[2][0][1])), (0, 0, 255), 2)  # Z-axis line (red)

    # Calculate Euler angles
    rmat, jac = cv2.Rodrigues(rotation_vector)
    angles, mtxR, mtxQ, Qx, Qy, Qz = cv2.RQDecomp3x3(rmat)

    return angles[1], angles[0], angles[2]

def start_cam_test(parameters):
    default_face_encoding = load_default_image_and_store_encoding()
    cap = cv2.VideoCapture(0)
    detector = Detection(parameters)
    r = Recognition(parameters)
    sim = Similarity(parameters)
    p = Predictor(parameters)
    while True:
        ret, frame = cap.read()
        if not ret:
            break
        faces = detector.detect(frame, 1)
        if faces:
            for face in faces:
                rect = Rectangle(face, parameters)
                x, y, width, height = rect.get_coordinates() 
                embedding, _ = get_face_image_encoding(r, parameters, face, frame)
                sim_score = sim.get_cosine_similarity(default_face_encoding, embedding)
                yaw, pitch, roll = calculate_yaw_pitch_roll(frame, rect.get_rectangle(), p)
                area = (face.right() - face.left()) * (face.bottom() - face.top())
                cv2.rectangle(frame, (x, y), (x+width, y+height), (0, 255, 0), 2)
                cv2.putText(frame, str(sim_score), (x, y-5), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 1)
                cv2.putText(frame, "Roll: " + str(roll), (x, y-20), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255, 0, 0), 2)
                cv2.putText(frame, "Yaw: " + str(yaw), (x, y-35), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255, 0, 0), 2)
                cv2.putText(frame, "Pitch: " + str(pitch), (x, y-50), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255, 0, 0), 2)
                cv2.putText(frame, "Area: " + str(area), (x, y-65), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255, 0, 0), 2)

        cv2.imshow('test_recognition', frame)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

def load_default_image_and_store_encoding():
    detector = Detection(parameters)
    r = Recognition(parameters)
    c = cv2.imread("test.png")
    
    faces = detector.detect(c, 1)
    if not faces:
        return
    for face in faces:
        default_face_encoding, _ = get_face_image_encoding(r, parameters, face, c)
    
    return default_face_encoding

if __name__ == "__main__":
    parameters = Parameters.build_parameters("config.ini")
    start_cam_test(parameters)