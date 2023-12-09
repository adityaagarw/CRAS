def detect_faces_in_frame(detector, frame):
    faces = detector.detect(frame, 1)
    return faces