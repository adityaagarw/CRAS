# Utility to search a given face in our DB
import argparse
import cv2
import configparser
import tkinter as tk
import threading

from PIL import Image, ImageTk
from tkinter import filedialog
from io import BytesIO
from face_utils.face_search import FaceSearch
from config.params import Parameters

NUM_RECORDS = 10
CAM_INDEX = 0
customer_rows = []

class CustomerRow:
    def __init__(self, frame, row_data, row_num, row_height=50):
        customer_id = row_data[0]
        name = row_data[1]
        similarity = row_data[22]
        img_byte_data = row_data[4]

        # Convert byte data to image and resize
        image = Image.open(BytesIO(img_byte_data))
        image = image.resize((row_height, row_height), Image.LANCZOS)
        self.photo = ImageTk.PhotoImage(image)

        # Create labels for each column in the row
        tk.Label(frame, text=customer_id).grid(row=row_num, column=3)
        tk.Label(frame, text=name).grid(row=row_num, column=2)
        tk.Label(frame, text=similarity).grid(row=row_num, column=1)
        tk.Label(frame, image=self.photo).grid(row=row_num, column=0)
        frame.grid_columnconfigure(3, minsize=row_height)  # Ensure column width

        # Keep a reference to the image to prevent garbage collection
        frame.image = self.photo

def search_frame(customer_frame, frame, face_search, source):
    global customer_rows
    if source == "localdb":
        records = face_search.search_localdb(frame)
        if records is None or len(records) == 0:
            print("No records found")
        else:
            index = 0
            for record in records:
                row = CustomerRow(customer_frame, record, index, row_height=50)
                customer_rows.append(row)
                index += 1

    elif source == "inmemdb":
        records = face_search.search_inmemdb(frame)
        if records is None or len(records) == 0:
            print("No records found")
        else:
            print(len(records))

    return frame

def clear_customer_data(customer_frame):
    global customer_rows

    # Destroy all widgets in the customer_frame
    for widget in customer_frame.winfo_children():
        widget.destroy()

    # Clear the customer_rows list
    customer_rows.clear()

def open_file():
    # Open a file dialog and get the selected image file path
    file_path = filedialog.askopenfilename(filetypes=[("Image files", "*.jpg *.png")])
    if file_path:
        return file_path
    return None

def capture_frame(button, customer_frame, customer_canvas, face_search, source):
    global cap, processed_canvas, processed_photo, window, text_label
    
    clear_customer_data(customer_frame)
    
    # Capture the frame
    if button == "Capture":
        ret, frame = cap.read()
    elif button == "Browse":
        file_path = open_file()
        if file_path is not None:
            frame = cv2.imread(file_path)
            ret = True
        else:
            ret = False
            print("Invalid/None file")

    if ret:
        # Process the frame
        processed_frame = search_frame(customer_frame, frame, face_search, source)
        customer_frame.update_idletasks()
        customer_canvas.config(scrollregion=customer_canvas.bbox("all"))

        if processed_frame is not None:
            # Convert the processed frame for Tkinter
            image = Image.fromarray(cv2.cvtColor(processed_frame, cv2.COLOR_BGR2RGB))
            processed_photo = ImageTk.PhotoImage(image=image)

            # Update the processed canvas and text
            processed_canvas.create_image(0, 0, image=processed_photo, anchor=tk.NW)
            text_label.config(text="Processed Frame")
        else:
            print("Processed frame is None")

def start_camera():
    global cap, live_canvas, window, running

    # Start the camera
    cap = cv2.VideoCapture(CAM_INDEX)

    while running:
        ret, frame = cap.read()
        if ret:
            # Convert the frame for Tkinter
            cv_img = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            img = Image.fromarray(cv_img)
            imgtk = ImageTk.PhotoImage(image=img)
            live_canvas.create_image(0, 0, image=imgtk, anchor=tk.NW)
        window.update()


def on_close():
    global running, window, cap, thread

    # Stop the camera loop
    running = False

    # Release the camera
    cap.release()

    # Destroy the window
    window.destroy()

def start_camera_process(parameters, source):
    global cap, live_canvas, processed_canvas, window, text_label, running, thread

    fs = FaceSearch(parameters, NUM_RECORDS)

    running = True

    # Create a GUI window
    window = tk.Tk()
    window.title("Camera Feed")

    # Set the callback for window close
    window.protocol("WM_DELETE_WINDOW", on_close)

    # Create a canvas for live image display
    live_canvas = tk.Canvas(window, width=640, height=480)
    live_canvas.grid(row=0, column=0)

    # Create a canvas for processed image display
    processed_canvas = tk.Canvas(window, width=640, height=480)
    processed_canvas.grid(row=0, column=1)

    # Create a scrollable canvas for customer data
    customer_canvas = tk.Canvas(window)
    customer_scrollbar = tk.Scrollbar(window, command=customer_canvas.yview)
    customer_canvas.configure(yscrollcommand=customer_scrollbar.set)
    customer_canvas.grid(row=0, column=2, sticky='nsew')  # Use grid instead of pack
    customer_scrollbar.grid(row=0, column=3, sticky='ns')  # Use grid for scrollbar

    # Create a frame inside the canvas
    customer_frame = tk.Frame(customer_canvas)
    customer_canvas.create_window((0, 0), window=customer_frame, anchor='nw')
    customer_frame.bind("<Configure>", lambda e: customer_canvas.configure(scrollregion=customer_canvas.bbox("all")))

    # Adjust grid column configuration
    window.grid_columnconfigure(2, weight=1)
    window.grid_rowconfigure(0, weight=1)

    # Create a label for text
    text_label = tk.Label(window, text="")
    text_label.grid(row=1, column=0, columnspan=2)

    # Create a capture button
    capture_button = tk.Button(window, text="Capture", command=lambda: capture_frame("Capture", customer_frame, customer_canvas, fs, source))
    capture_button.grid(row=2, column=0, rowspan=2, sticky=tk.NSEW)

    browse_button = tk.Button(window, text="Browse", command=lambda: capture_frame("Browse", customer_frame, customer_canvas, fs, source))
    browse_button.grid(row=2, column=1, rowspan=2, sticky=tk.NSEW)

    # Start the camera in a separate thread
    thread = threading.Thread(target=start_camera, daemon=True)
    thread.start()

    # Start the GUI event loop
    window.mainloop()


def start_image_process(image_path, source):
    # Start image process
    pass

def build_parameters(file):
    config = configparser.ConfigParser()
    config.read(file)
    args = config['general']
    parameters = Parameters(args['detection'], \
                            args['library'], \
                            args['model'], \
                            args['threshold'], \
                            args['yaw_threshold'], \
                            args['pitch_threshold'], \
                            args['area_threshold'], \
                            args['billing_cam_time'], \
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
    # Parse aguments from command line, input should be either image or camera feed.
    # Output option should be either from localdb or inmemdb
    parser = argparse.ArgumentParser(description="CLI for face search")

    # Adding arguments

    parser.add_argument("-from", dest="source", type=str, choices=['localdb', 'inmemdb'], required=True,
                        help="Select the database source: 'localdb' for local database or 'inmemdb' for in-memory database.")

    # Parsing arguments
    args = parser.parse_args()
    parameters = build_parameters("config.ini")

    start_camera_process(parameters, args.source)
