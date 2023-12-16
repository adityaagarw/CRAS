import subprocess
import shutil
import os
import redis
import time

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
    py_cmd = get_python_command()
    while True:
        status = read_status()
        if status == "1":
            # Failure case
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
