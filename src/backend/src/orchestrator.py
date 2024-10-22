# Set python paths

import sys
import time
import os
import json
import subprocess
import shutil
import psutil
import configparser

from datetime import datetime
from utils.utils import Utils
from db.database import *
from config.params import Parameters

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
def start_cameras(p):
    # Start entry camera
    py_cmd = get_python_command()
    subprocess.Popen([py_cmd, 'cameras/entry.py', '-camera', p.entry_cam, '-cam_type', p.entry_cam_type])
    subprocess.Popen([py_cmd, 'cameras/billing.py', '-camera', p.billing_cam, '-cam_type', p.billing_cam_type])
    subprocess.Popen([py_cmd, 'cameras/exit.py', '-camera', p.exit_cam, '-cam_type', p.exit_cam_type])
    
    subprocess.Popen([py_cmd, 'employee/employee.py'])

 
def get_camera_ids():
    # Get camera ids
    pass

# Start the UI - Consumer
def start_ui():
    pass

def log_session(parameters):
    local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
    local_db.connect()
    if not local_db.connection:
            print("Local db connection failed while trying to log session!")
            return 1

    date_format = "%Y-%m-%d %H:%M:%S"
    time_now = datetime.now().strftime(date_format)
    session = Session(
                start_time = str(time_now), 
                end_time = "", 
                detection = parameters.detection,
                model = parameters.model,
                threshold = parameters.threshold,
                yaw_threshold = parameters.yaw_threshold,
                pitch_threshold = parameters.pitch_threshold,
                area_threshold = parameters.area_threshold,
                billing_cam_time = parameters.billing_cam_time,
                similarity_method = parameters.sim_method,
                total_faces=0,
                same_faces=0,
                misidentified_faces=0,
                unidentified_faces=0
                )
    
    local_db.insert_session_record(session)
    local_db.disconnect()

def add_params_inmem(parameters):
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    in_mem_db.connect()

    #TBD remove hardcoding
    params_record = InMemParams(
                detection = str(parameters.detection),
                model = str(parameters.model),
                threshold = str(parameters.threshold),
                yaw_threshold = str(parameters.yaw_threshold),
                pitch_threshold = str(parameters.pitch_threshold),
                area_threshold = str(parameters.area_threshold),
                billing_cam_time = str(parameters.billing_cam_time),
                similarity_method = str(parameters.sim_method),
                periodic_sleep_time="0.100",
                num_threads_per_process="1",
                frames_per_second="30",
                exit_threshold=str(parameters.exit_threshold),
                entry_roi_x1="320",
                entry_roi_y1="200",
                entry_roi_x2="900",
                entry_roi_y2="600",
                exit_roi_x1="320",
                exit_roi_y1="200",
                exit_roi_x2="900",
                exit_roi_y2="600",
                resize_factor="1.0"
                )
    in_mem_db.insert_params(params_record)

    in_mem_db.set_detection(str(parameters.detection))
    in_mem_db.set_model(str(parameters.model))
    in_mem_db.set_threshold(str(parameters.threshold))
    in_mem_db.set_yaw_threshold(str(parameters.yaw_threshold))
    in_mem_db.set_pitch_threshold(str(parameters.pitch_threshold))
    in_mem_db.set_area_threshold(str(parameters.area_threshold))
    in_mem_db.set_billing_cam_time(str(parameters.billing_cam_time))
    in_mem_db.set_similarity_method(str(parameters.sim_method))
    in_mem_db.set_periodic_sleep_time("0.100")
    in_mem_db.set_num_threads_per_process("1")
    in_mem_db.set_frames_per_second("30")
    in_mem_db.set_exit_threshold(str(parameters.exit_threshold))
    in_mem_db.set_entry_roi(320,200,900,600)
    in_mem_db.set_exit_roi(320,200,900,600)
    in_mem_db.set_resize_factor("1.0")
    in_mem_db.disconnect()

def set_boot_time_start():
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    in_mem_db.connect()
    time_now = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    in_mem_db.set_boot_time_start(str(time_now))
    in_mem_db.disconnect()

def purge_redis(setting):
    # Depending on setting delete all redis data on startup
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    in_mem_db.connect()
    if setting == "every_session":
        in_mem_db.connection.flushall()
    elif setting == "every_day":
        # Check if it is first session of the day:
        local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
        local_db.connect()

        date_format = "%Y-%m-%d %H:%M:%S"
        time_now = datetime.now().strftime(date_format)

        latest_session_date = local_db.get_latest_session_date()
        if not latest_session_date or latest_session_date != time_now.split(" ")[0]:
            in_mem_db.connection.flushall()

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
    parameters = Parameters.build_parameters('config.ini')

    ret = start_docker()
    if ret:
        print("Docker failed to start")
        exit(1)

    ret = check_create_db()
    if ret:
        print("DB failed to start")
        exit(1)

    # Delete in mem data on startup
    #purge_setting = "every_session"
    #purge_redis(purge_setting)

    # Set boot time start
    set_boot_time_start()

    # Add parameters inmemory
    add_params_inmem(parameters)

    # Log session details
    if log_session(parameters) == 1:
        print("Failed to log session details")
        exit(1)

    start_cameras(parameters)