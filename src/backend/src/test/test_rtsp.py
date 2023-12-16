import cv2

def open_cam(q):
    capture = cv2.VideoCapture('rtsp://crasadmin:lol12345@192.168.2.108:554/cam/realmonitor?channel=4&subtype=0&proto=tcp')
    capture.set(cv2.CAP_PROP_FOURCC, cv2.VideoWriter_fourcc('H', '2', '6', '4'))

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