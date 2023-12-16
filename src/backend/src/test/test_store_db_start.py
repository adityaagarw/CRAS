import subprocess
import json

def receive_form_data():
    process = subprocess.Popen(['python3', 'test_store_db_ui.py'], stdout=subprocess.PIPE)
    output, _ = process.communicate()
    json_data = output.decode().strip()
    form_data = json.loads(json_data)  # Parse the JSON string into a Python dictionary

    # Access and use the form data as needed
    store_name = form_data["Store Name"]
    store_location = form_data["Store Location"]
    entry_cameras = form_data["Number of entry cameras"]
    billing_cameras = form_data["Number of billing cameras"]
    exit_cameras = form_data["Number of exit cameras"]
    entry_camera_list = form_data["Entry camera list"]
    billing_camera_list = form_data["Billing camera list"]
    exit_camera_list = form_data["Exit camera list"]

    # Print or process the form data
    print("Store Name:", store_name)
    print("Store Location:", store_location)
    print("Number of entry cameras:", entry_cameras)
    print("Number of billing cameras:", billing_cameras)
    print("Number of exit cameras:", exit_cameras)
    print("Entry camera list:", entry_camera_list)
    print("Billing camera list:", billing_camera_list)
    print("Exit camera list:", exit_camera_list)

if __name__ == "__main__":
    receive_form_data()