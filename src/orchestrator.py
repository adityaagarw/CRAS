# Set python paths

import sys
import os
import subprocess
import shutil
from db.database import LocalPostgresDB, InMemoryRedisDB, InMemoryRedisEmployeeDB

def get_python_command():
    python_commands = ['python3', 'python', 'py']

    for command in python_commands:
        if shutil.which(command) is not None:
            return command

    raise EnvironmentError('Python command not found in your system')

def start_docker():
    os.system("python3 db/install_db.py")
    return 0

def build_config():
    os.system("python3 config/default_config.py")
    return 0

# Initialize and create db if not created
def check_create_db():
    local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
    redis_db = InMemoryRedisDB(host='127.0.0.1', port=6379)

    local_db.connect()
    if not local_db.connection:
        print("Local db connection failed!")
        return 1
    else:
        print("checkDB: Connected to localdb")

    # Create tables
    local_db.create_table_if_not_exists()

    redis_db.connect()
    if not redis_db.connection:
        print("Redis db connection failed!")
        return 1
    else:
        print("checkDB: Connected to redis db")

    local_db.disconnect()
    redis_db.disconnect()

    return 0

# Start cameras - Producer 
def start_cameras():
    # Start entry camera
    py_cmd = get_python_command()
    subprocess.Popen([py_cmd, 'cameras/entry.py', '-camera', '0'])
 
def get_camera_ids():
    # Get camera ids
    pass

# Start the UI - Consumer
def start_ui():
    pass

if __name__ == "__main__":
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
    start_ui()
