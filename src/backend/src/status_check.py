import subprocess
import shutil
import os
import time

from db.database import *
from db.redis_pubsub import *
from db.log import *

def get_python_command():
    python_commands = ['python3', 'python', 'py']

    for command in python_commands:
        if shutil.which(command) is not None:
            return command

def read_status():
    # Read the status file from the disk
    with(open('status', 'r')) as f:
        status = f.read()

    return status

def write_status(status):
    with(open('status', 'w')) as f:
        f.write(str(status))

def check_status():
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    in_mem_db.connect()
    py_cmd = get_python_command()
    while True:
        status = read_status()
        if status == "1":
            # Failure case
            in_mem_db.connection.publish(Channel.Status.value, Status.BackendDown.value)
            os.system(py_cmd + " shutdown_system.py")
            write_status(0)
            os.system(py_cmd + " orchestrator.py")
            exit(0)

        if status == "2":
            # Shutdown case
            os.remove('status')
            exit(0)

        time.sleep(5)

if __name__ == "__main__":
    check_status()
