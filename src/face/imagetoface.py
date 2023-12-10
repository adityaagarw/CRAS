import cv2
import io

from PIL import Image

# Responsible for converting image to face encoding by detecting, extracting and encoding face
class ImageToFace:
    def __init__(self):
        pass
    def detect_face_in_frame(self, detector, frame):
        faces = detector.detect(frame, 1)
        return faces[0] if len(faces) > 0 else None

    def get_face_image_encoding(self, r, face, frame):
        face_pixels = r.get_face_pixels_for_image(face, frame)
        embedding = r.get_encodings(face, frame)
        return embedding, face_pixels
    
    def get_face_image(self, face_pixels, target_size=(160, 160)):
        try:
            face_pixels_rgb = cv2.cvtColor(face_pixels, cv2.COLOR_BGR2RGB)
        except:
            face_pixels_rgb = face_pixels

        try:
            face_image = Image.fromarray(face_pixels_rgb)
        except:
            return None
        
        face_image = face_image.resize(target_size)
        img_bytes = io.BytesIO()
        face_image.save(img_bytes, format='PNG')
        img_data = img_bytes.getvalue()
        return img_data
    
    def imageToEncoding(self, detector, r, picture):
        
        face = self.detect_face_in_frame(detector, picture)
        if face is None:
            return None, None
        
        encoding, face_pixels = self.get_face_image_encoding(r, face, picture)
        return encoding, face_pixels
    