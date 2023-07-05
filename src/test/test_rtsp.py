import cv2

def open_cam():
    capture = cv2.VideoCapture('rtsp://crasadmin:lol12345@192.168.2.108:554/cam/realmonitor?channel=4&subtype=0')
    if capture:
        print("Capture success")
        print(capture)
    else:
        print("Capture unsuccessful")

    while True:
        ret, frame = capture.read()
        if ret:
            cv2.imshow("Frame", frame)
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break


if __name__ == "__main__":
    open_cam()