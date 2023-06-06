import cv2
from pywinusb import hid

def find_cameras():
    all_devices = hid.find_all_hid_devices()
    for d in all_devices:
        print(d.product_name,"   ", d.vendor_id, "    ", d.product_id)
    # Filter for devices that we know are cameras
    #cameras = [d for d in all_devices if d.product_name == 'MyCameraName']

    #return cameras

def map_cameras_to_indices(cameras):
    index_to_camera = {}

    for index in range(10): # Increase range if you have more cameras
        cap = cv2.VideoCapture(index)
        if cap.isOpened():
            # You can adjust this code to match the property you are distinguishing by
            # In this case, it assumes each camera has a unique product_id
            for camera in cameras:
                if camera.product_id == cap.get(cv2.CAP_PROP_XI_DEVICE_PRODUCT_ID):
                    index_to_camera[index] = camera
                    break

    return index_to_camera

cameras = find_cameras()
#index_to_camera = map_cameras_to_indices(cameras)

# Now, index_to_camera is a dictionary that maps from OpenCV index to the corresponding camera device.
import wmi

# Initializing the wmi constructor
f = wmi.WMI()

# Printing the header for the later columns
print("DeviceID, Description, Manufacturer, Name, PNPDeviceID, Status")

# Iterating through all the USB devices
for device in f.Win32_PnPEntity(ConfigManagerErrorCode=0):
    if 'USB' in device.Name or 'USB' in device.Description: # This condition checks if 'USB' is present in device name or description
        #print(f"{device.DeviceID}, {device.Description}, {device.Manufacturer}, {device.Name}, {device.PNPDeviceID}, {device.Status}")
        print(f"{device.Name}")