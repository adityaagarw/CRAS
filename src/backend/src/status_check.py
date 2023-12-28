import subprocess
import shutil
import os
import time
import fasteners
from db.database import *
from db.redis_pubsub import *
from db.log import *
from utils.utils import Utils

# Define bitmask for each program and the overall status
entry_bitmask = 0b0001  # First LSB
exit_bitmask = 0b0010  # Second LSB
billing_bitmask = 0b0100  # Third LSB
backend_bitmask = 0b1000  # Fourth LSB

def get_python_command():
    python_commands = ['python3', 'python', 'py']

    for command in python_commands:
        if shutil.which(command) is not None:
            return command

def check_status():
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    in_mem_db.connect()
    py_cmd = get_python_command()

    # Wait till all programs are up and set backend status to 1. Loop till then
    while True:
        with fasteners.InterProcessLock(Utils.lock_file):
            status = Utils.read_status()
            status = int(status)
            # Check if starting bit is set
            if status & Utils.starting_bitmask:
                # Check if all programs are up
                if status & Utils.entry_bitmask and status & Utils.exit_bitmask and status & Utils.billing_bitmask and status & Utils.employee_bitmask:
                    # Set backend status to 1
                    print("Backend is up!")
                    Utils.backend_up()
                    Utils.started()
            elif status & Utils.shutdown_system_bitmask:
                # Shutdown case
                #os.remove('status')
                exit(0)
            else:
                # Program is already up
                # Check if any of the entry, exit or billing programs are down
                if not status & Utils.entry_bitmask or not status & Utils.exit_bitmask or not status & Utils.billing_bitmask or not status & Utils.employee_bitmask:
                    # Set backend status to 0
                    Utils.backend_down()
                    # Restart backend
                    os.system(py_cmd + " shutdown_system.py")
                    os.system(py_cmd + " orchestrator.py")
                    exit(0)
                    
        time.sleep(5)

if __name__ == "__main__":
    check_status()
