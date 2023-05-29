######################################################################
# <Company-Name>                                                     #
# Copyright 2023                                                     #
#                                                                    #
# This file contains classes for face detection and recognition      #
#                                                                    #
######################################################################

import io
import os
import cv2
import numpy as np
import params
import dlib
from keras_vggface.vggface import VGGFace
from keras_vggface.utils import preprocess_input
from sklearn.metrics.pairwise import cosine_similarity, euclidean_distances, manhattan_distances
from keras_facenet import FaceNet
from PIL import Image

os.environ["CUDA_VISIBLE_DEVICES"] = "-1"

class Rectangle:
    def __init__(self, face, parameters):
        if face is not None:
            if parameters.detection == "Frontal":
                self.x = face.left()
                self.y = face.top()
                self.left = face.left()
                self.right = face.right()
                self.top = face.top()
                self.bottom = face.bottom()
                self.width = face.width()
                self.height = face.height()
            elif parameters.detection == "CNN":
                self.x = face.rect.left()
                self.y = face.rect.top()
                self.left = face.rect.left()
                self.right = face.rect.right()
                self.top = face.rect.top()
                self.bottom = face.rect.bottom()
                self.width = face.rect.width()
                self.height = face.rect.height()
            else:
                self.x = face.rect.left()
                self.y = face.rect.top()
                self.left = face.rect.left()
                self.right = face.rect.right()
                self.top = face.rect.top()
                self.bottom = face.rect.bottom()
                self.width = face.rect.width()
                self.height = face.rect.height()

    def get_coordinates(self):
        return self.x, self.y, self.width, self.height

    def get_rectangle(self):
        return dlib.rectangle(self.left(), self.top(), self.right(), self.bottom())


class Detection:
    def __init__(self, parameters):
        if parameters.detection == "Frontal":
            self.detector = dlib.get_frontal_face_detector()
        elif parameters.detection == "CNN":
            model_file = parameters.model_dir + "/mmod_human_face_detector.dat"
            self.detector = dlib.cnn_face_detection_model_v1(model_file)
        else:
            self.detector = None

    def detect(self, frame, num):
        frame = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        return self.detector(frame, num)

class Predictor:
    def __init__(self, parameters):
        model_file = parameters.model_dir + "/shape_predictor_68_face_landmarks.dat"
        self.shape = dlib.shape_predictor(model_file)

    def get_shape(self, frame, face_rect):
        return self.shape(frame, face_rect)

class Recognition:
    def __init__(self, parameters):
        self.parameters = parameters
        if parameters.library == "DLIB":
            model_file = parameters.model_dir + "/dlib_face_recognition_resnet_model_v1.dat"
            self.encode = dlib.face_recognition_model_v1(model_file)
        elif parameters.library == "VGG":
            self.encode = VGGFace(model=parameters.model, include_top=False, \
                                  input_shape=(224, 224, 3), pooling='avg')
        elif parameters.library == "FaceNet":
            self.encode = FaceNet()
        else:
            self.encode = None

    def get_encodings(self, face, frame):
        rect = Rectangle(face, self.parameters)
        face_pixels = frame[rect.y:rect.y+rect.height, rect.x:rect.x+rect.width]
        if self.parameters.library == "DLIB":
            predictor = Predictor(self.parameters)
            frame_gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
            shape = predictor.get_shape(frame_gray, rect)
            encoding = self.encode.compute_face_descriptor(frame, shape)
        elif self.parameters.library == "VGG":
            if face_pixels.shape[0] != 0 and face_pixels.shape[1] != 0:
                face_pixels = cv2.resize(face_pixels, (224, 224))
                face_pixels = preprocess_input(face_pixels.astype(np.float32))
                encoding = self.encode.predict(np.expand_dims(face_pixels, axis=0))[0]
            else:
                encoding = None
        elif self.parameters.library == "FaceNet":
            if face_pixels.shape[0] != 0 and face_pixels.shape[1] != 0:
                encoding = self.encode.embeddings(np.array([face_pixels]))[0]
            else:
                encoding = None
        else:
            encoding = None

        return encoding

    def get_face_pixels(self, face, frame):
        rect = Rectangle(face, self.parameters)
        return frame[rect.y:rect.y+rect.height, rect.x:rect.x+rect.width]

    def get_face_image(self, face_pixels):
        face_8bit = np.clip(face_pixels, 0, 255).astype(np.uint8)
        face_image = Image.fromarray(face_8bit)
        img_bytes = io.BytesIO()
        face_image.save(img_bytes, format='PNG')
        img_data = img_bytes.getvalue()
        return img_data

    def calculate_yaw_pitch(self, face, frame): 
        frame_gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        predictor = Predictor(self.parameters)
        landmarks = predictor.get_shape(frame_gray, rect)
        landmarks_arr = np.zeros((68, 3), dtype=np.float32)
        num_landmarks = len(landmarks.parts())
        landmarks_arr = []
        for i in range(0, 68):
            x = landmarks.part(i).x
            y = landmarks.part(i).y
            landmarks_arr.append([x, y])

        # Loop through each landmark
        '''for landmark in landmarks_arr:
            # Draw a circle at the landmark position
            center = tuple(landmark)
            radius = 2
            color = (0, 255, 0)
            thickness = -1
            cv2.circle(frame, center, radius, color, thickness)'''

        center = np.mean(landmarks_arr, axis=0)
        # Calculate yaw angle
        left_eye = landmarks_arr[36]
        right_eye = landmarks_arr[45]
        dx = right_eye[0] - left_eye[0]
        dy = right_eye[1] - left_eye[1]
        yaw = math.degrees(math.atan2(dy, dx))

        # Calculate pitch angle
        nose_top = landmarks_arr[27]
        nose_bottom = landmarks_arr[33]
        dx = nose_top[0] - center[0]
        dy = nose_top[1] - center[1]
        dz = nose_top[2] - center[2] if len(center) == 3 else 0
        pitch = math.degrees(math.atan2(dz, math.hypot(dx, dy)))

        return yaw, pitch

class Similarity:
    def __init__(self, parameters):
        self.library = parameters.library
        self.threshold = parameters.threshold
        if parameters.sim_method == "Euclidean":
            self.method = self.compare_faces_euclidean
        elif parameters.sim_method == "Manhattan":
            self.method = self.compare_faces_manhattan
        elif parameters.sim_method == "Cosine":
            self.method = self.compare_faces_cosine
        else:
            self.method = self.compare_faces_cosine

    def print_similarity_matrix(self, unique_face_ids, unique_embeddings):
        num_faces = len(unique_embeddings)
        print("\nSimilarity matrix of unique face embeddings")
        unique_embeddings_array = np.array(unique_embeddings).reshape(-1,1)
        sim_mat =  cosine_similarity(unique_embeddings)
        print('{:>10s}'.format('') + ' '.join(['{:>10s}'.format(face_name[:10]) for face_name in unique_face_ids]))
        for i in range(num_faces):
            print('{:>10s}'.format(unique_face_ids[i][:10]) + ' '.join(['{:10.4f}'.format(sim_mat[i, j]) for j in range(num_faces)]))

    def compare_faces_euclidean(self, embedding1, embedding2):
        # Reshape the embeddings to be compatible with manhattan_distances
        embedding1 = embedding1.reshape(1, -1)
        embedding2 = embedding2.reshape(1, -1)

        # Calculate the Manhattan distance between the embeddings
        distance = euclidean_distances(embedding1, embedding2)

        # Convert the distance to a similarity score
        similarity = 1 / (1 + distance)
        print("Similarity scores: ", similarity_scores[0][0])
        matching_indices = np.where(similarity_scores >= self.threshold)[0]
        if matching_indeces.size == 0:
            return False
        else:
            return True

    def compare_faces_manhattan(self, embedding1, embedding2):
        # Reshape the embeddings to be compatible with manhattan_distances
        embedding1 = embedding1.reshape(1, -1)
        embedding2 = embedding2.reshape(1, -1)

        # Calculate the Manhattan distance between the embeddings
        distance = manhattan_distances(embedding1, embedding2)

        # Convert the distance to a similarity score
        similarity = 1 / (1 + distance)
        print("Similarity scores: ", similarity_scores[0][0])
        matching_indices = np.where(similarity_scores >= self.threshold)[0]
        if matching_indeces.size == 0:
            return False
        else:
            return True

    def compare_faces_cosine(self, embedding1, embedding2):
        if self.library == "DLIB":
            embedding1 = np.array(embedding1)
            embedding2 = np.array(embedding2)
        similarity_scores = cosine_similarity(embedding1.reshape(1, -1), embedding2.reshape(1, -1))
        #print("Similarity scores:", similarity_scores[0][0])
        matching_indices = np.where(similarity_scores >= self.threshold)[0]
        if matching_indices.size == 0:
            return False
        else:
            return True

    def is_same_face(self, embedding1, embedding2):
        return self.method(embedding1, embedding2)

    def is_unique_face(self, embedding, unique_face_embeddings):
        for i, unique_face_embedding in enumerate(unique_face_embeddings):
            if self.method(embedding, unique_face_embedding):
                return False, i
        return True, None
