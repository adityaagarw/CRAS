# Set python paths

import sys
import time
import os
import json
import subprocess
import shutil
import psutil
import fasteners

from utils.utils import Utils
from db.database import *

def get_python_command():
    python_commands = ['python3', 'python', 'py']

    for command in python_commands:
        if shutil.which(command) is not None:
            return command

    raise EnvironmentError('Python command not found in your system')

def start_docker():
    os.system(get_python_command() + " one_time_setup/install_db.py")
    return 0

def build_config():
    os.system(get_python_command() + " config/default_config.py")
    return 0

def load_store_data(local_db, inmem_db):
    map_store = MapLocaltoInMem(local_db)
    store_record = local_db.fetch_store_record()
    if not store_record:
        print("Store record not found!")
        return 1
    
    inmem_store_record = map_store.map_store_local_to_inmem(store_record)

    inmem_db.insert_record(inmem_store_record, type='store')


def spawn_customer_db_gui(local_db):
    process = subprocess.Popen([get_python_command(), 'one_time_setup/store_db_ui.py'], stdout=subprocess.PIPE)
    output, _ = process.communicate()
    json_data = output.decode().strip()
    form_data = json.loads(json_data)  # Parse the JSON string into a Python dictionary

    # Access and use the form data as needed
    entry_cam_list = []
    billing_cam_list = []
    exit_cam_list = []

    entry_cam_list = [int(cam) for cam in form_data["Entry camera list"]]
    billing_cam_list = [int(cam) for cam in form_data["Billing camera list"]]
    exit_cam_list = [int(cam) for cam in form_data["Exit camera list"]]

    store_record = LocalStore(store_id = Utils.generate_unique_id(),
                location = form_data["Store Location"],
                name = form_data["Store Name"],
                num_entry_cams = form_data["Number of entry cameras"],
                num_billing_cams = form_data["Number of billing cameras"],
                num_exit_cams = form_data["Number of exit cameras"],
                entry_cam = entry_cam_list,
                billing_cam = billing_cam_list,
                exit_cam = exit_cam_list)
    
    local_db.insert_store_record(store_record)

# Initialize and create db if not created
def check_create_db():
    local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
    redis_db = InMemoryRedisDB(host='127.0.0.1', port=6379)

    retry = 0

    while retry < 3:
        local_db.connect()
        if not local_db.connection:
            print("Local db connection failed!, retrying in 10 seconds")
            time.sleep(10)
        else:
            break
            
    if not local_db.connection:
        print("Local db connection failed!")
        return 1
    else:
        print("checkDB: Connected to localdb")

    # Create tables
    local_db.create_table_if_not_exists()

    store_record = local_db.fetch_store_record()
    if not store_record:
        spawn_customer_db_gui(local_db)
    
    # Verify store record
    store_record = local_db.fetch_store_record()
    if not store_record:
        print("Store record not found!")
        return 1

    redis_db.connect()
    if not redis_db.connection:
        print("Redis db connection failed!")
        return 1
    else:
        print("checkDB: Connected to redis db")

    ret = load_store_data(local_db, redis_db)
    if ret:
        print("Failed to load store data")
        return 1

    local_db.disconnect()
    redis_db.disconnect()

    return 0

def start_status_check():
    # Start status check
    subprocess.Popen([get_python_command(), 'status_check.py'])

# Start cameras - Producer 
def start_cameras():
    # Start entry camera
    py_cmd = get_python_command()
    subprocess.Popen([py_cmd, 'cameras/entry.py', '-camera', '1'])
    subprocess.Popen([py_cmd, 'cameras/billing.py', '-camera', '2'])
    subprocess.Popen([py_cmd, 'cameras/exit.py', '-camera', '0'])
    
    subprocess.Popen([py_cmd, 'employee/employee.py'])

 
def get_camera_ids():
    # Get camera ids
    pass

# Start the UI - Consumer
def start_ui():
    pass

if __name__ == "__main__":
    # Check if program is already running
    if os.path.isfile("entry_pid"):
        # Check if process exists and is running
        with open("entry_pid", "r") as f:
            pid = f.read()
            pid = int(pid)
            if psutil.pid_exists(pid):
                print("Entry Program already running")
                exit(1)
            else:
                os.remove("entry_pid")

    if os.path.isfile("billing_pid"):
        # Check if process exists and is running
        with open("billing_pid", "r") as f:
            pid = f.read()
            pid = int(pid)
            if psutil.pid_exists(pid):
                print("Billing Program already running")
                exit(1)
            else:
                os.remove("billing_pid")

    if os.path.isfile("exit_pid"):
        # Check if process exists and is running
        with open("exit_pid", "r") as f:
            pid = f.read()
            pid = int(pid)
            if psutil.pid_exists(pid):
                print("Exit Program already running")
                exit(1)
            else:
                os.remove("exit_pid")

    if os.path.isfile("employee_pid"):
        # Check if process exists and is running
        with open("employee_pid", "r") as f:
            pid = f.read()
            pid = int(pid)
            if psutil.pid_exists(pid):
                print("Employee Program already running")
                exit(1)
            else:
                os.remove("employee_pid")

    # Initialise status file
    Utils.starting()

    start_status_check() #TBD

    get_camera_ids()

    build_config()

    ret = start_docker()
    if ret:
        print("Docker failed to start")
        exit(1)

    ret = check_create_db()
    if ret:
        print("DB failed to start")
        exit(1)

    start_cameras()