import os
import configparser

from config.params import Parameters
from datetime import datetime, timedelta
from db.database import *
from db.redis_pubsub import *
from face.face import Detection, Recognition, Rectangle, Predictor
from face.imagetoface import ImageToFace
from utils.utils import Utils

def create_new_employee(data, in_mem_db, r):
    local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
    local_db.connect()

    date_format = "%Y-%m-%d %H:%M:%S"
    # Parse new employee data
    # TBD
    new_id = Utils.generate_unique_id()
    whole_image = data[1]
    name = data[2]
    phone_number = data[3]
    face_encoding, pixels = ImageToFace.imageToEncoding(r.detector, r, whole_image)
    face_image = ImageToFace.get_face_image(pixels)

    time_now = datetime.now().strftime(date_format)

    # Add new employee to local and in mem structures
    new_employee = LocalEmployee(
        employee_id=new_id,
        name=name,
        phone_number=phone_number,
        face_image=face_image,
        face_encoding=face_encoding
    )

    new_employee_inmem = InMemEmployee(
        employee_id=new_id,
        name=name,
        phone_number=phone_number,
        face_image=face_image,
        face_encoding=face_encoding,
        entry_time=time_now,
        exit_time="",
        num_exits="0",
        in_store="0"
    )

    in_mem_db.insert_record(new_employee_inmem, type="employee")
    local_db.insert_employee_record(new_employee)
    local_db.disconnect()

    in_mem_db.connection.publish(Channel.Employee.value, BackendMessage.NewEmployeeAck.value)

def change_customer_to_employee(data, in_mem_db):
    local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
    local_db.connect()

    date_format = "%Y-%m-%d %H:%M:%S"

    # Fetch customer details using id
    # TBD
    cust_id = data[1]
    name = data[2]
    phone_number = data[3]

    record_key = 'customer_inmem_db:' + cust_id
    record = in_mem_db.connection.hgetall(record_key)

    if len(record) != 0:
        face_encoding = record.get(b'encoding')
        face_image = record.get(b'image')
    else:
        face_image = None
        face_encoding = None
    
    time_now = datetime.now().strftime(date_format)

    # Add new employee to local and in mem structures
    new_employee = LocalEmployee(
        employee_id=cust_id,
        name=name,
        phone_number=phone_number,
        face_image=face_image,
        face_encoding=face_encoding
    )

    new_employee_inmem = InMemEmployee(
        employee_id=cust_id,
        name=name,
        phone_number=phone_number,
        face_image=face_image,
        face_encoding=face_encoding,
        entry_time=time_now,
        exit_time="",
        num_exits="0",
        in_store="1"
    )

    in_mem_db.insert_record(new_employee_inmem, type="employee")
    local_db.insert_employee_record(new_employee)
    local_db.disconnect()

    in_mem_db.connection.publish(Channel.Employee.value, BackendMessage.MarkAsEmployeeAck.value)

def start_employee_process(parameters):
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    in_mem_db.connect()
    if not in_mem_db.connection:
        print("Redis db connection failed!: Employee")
        return
        
    p = in_mem_db.connection.pubsub()
    p.subscribe(Channel.Employee.value)

    r = Recognition(parameters)

    while True:
        for message in p.listen():
            if message['type'] == 'message':
                data = message['data']
                split_data = data.split(":")
                if isinstance(data, bytes):
                    data = data.decode('utf-8')
                if split_data[0] == FrontendMessage.NewEmployee.value:
                    create_new_employee(split_data, in_mem_db, r)
                if split_data[0] == FrontendMessage.MarkAsEmployee.value:
                    change_customer_to_employee(split_data, in_mem_db)

def write_employee_pid():
    with open("employee_pid", "w") as f:
        f.write(str(os.getpid()))

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
    write_employee_pid()
    parameters = build_parameters("config.ini")
    start_employee_process(parameters)