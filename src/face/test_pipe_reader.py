import cv2
import os
import numpy as np

def runloop():
    pipe_name = "cam_feed0"
    pipe_reader = os.open(pipe_name, os.O_RDONLY)

    while True:
        bytearr = os.read(pipe_reader, 262144)
        print("Read from pipe")
        nparr = np.frombuffer(bytearr, np.byte)
        img = cv2.imdecode(nparr, cv2.IMREAD_ANYCOLOR)
        if img is not None:
            cv2.imshow('Video', img)
            if (cv2.waitKey(1) & 0xFF == ord('q')):
                break

    cv2.destroyAllWindows()

if __name__ == "__main__":
    runloop()


