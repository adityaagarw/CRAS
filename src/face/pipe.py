import os
import cv2
import platform
if platform.system() == "Windows":
    import win32pipe, win32file, pywintypes
from face.face import Rectangle

class FacePipe:
    def __init__(self, camera):
        self.pipe_name = r'\\.\pipe\webcam_feed' + str(camera)
        #self.pipe_name =r'.\\cam_feed' + str(camera)

    def create_named_pipe(self):
        if platform.system() == "Windows":
            pipe = win32pipe.CreateNamedPipe(
                self.pipe_name,
                win32pipe.PIPE_ACCESS_OUTBOUND,
                win32pipe.PIPE_TYPE_BYTE | win32pipe.PIPE_WAIT,
                1, 262144, 262144,
                0,
                None
            )
            win32pipe.ConnectNamedPipe(pipe, None)
        else:
            if not os.path.exists(self.pipe_name):
                os.mkfifo(self.pipe_name)
            pipe = os.open(self.pipe_name, os.O_WRONLY)
        return pipe

    def send_faces_to_pipe(self, parameters, faces, frame, pipe):
        for face in faces:
            rect = Rectangle(face, parameters)
            x, y, width, height = rect.get_coordinates()        
            cv2.rectangle(frame, (x, y), (x+width, y+height), (0, 255, 0), 2)

        _, img_encoded = cv2.imencode('.jpg', frame)

        if platform.system() == "Windows":
            try:
                # Write the frame to the named pipe
                win32file.WriteFile(pipe, img_encoded.tobytes())
            except pywintypes.error as e:
                self.destroy_pipe(pipe)
                pipe = self.create_named_pipe()
        else:
            os.write(pipe, img_encoded.tobytes())

    def destroy_pipe(self, pipe):
        if platform.system() == "Windows":
            win32file.CloseHandle(pipe)
        else:
            pipe.close()
