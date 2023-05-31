# Set python paths

import sys
import os
from db.database import LocalPostgresDB, InMemoryRedisDB, InMemoryRedisEmployeeDB

def set_library_path():
    current_dir = os.path.dirname(os.path.abspath(__file__))
    parent_dir = os.path.dirname(current_dir)

    if parent_dir not in sys.path:
        sys.path.append(parent_dir)

# Initialize and create db if not created
def check_create_db():
    local_db = LocalPostgresDB(host='localhost', port=5432, database='localdb', user='cras_admin', password='admin')
    redis_db = InMemoryRedisDB(host='localhost', port=6379)

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
    entry = "python3 cameras/entry.py -camera 0"
    os.system(entry)

# Start the UI - Consumer
def start_ui():
    pass


if __name__ == "__main__":
    set_library_path()
    ret = check_create_db()
    if not ret:  
        start_cameras()
        start_ui()
