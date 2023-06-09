import subprocess
import os
import redis

def check_status():
    # Read entry pid
    # Read billing pid
    # Read exit pid

    # Check if entry pid is running
    if os.path.isfile("entry_pid"):
        with open("entry_pid", "r") as f:
            pid = f.read()


    # Check if billing pid is running

    # Check if exit pid is running




if __name__ == "__main__":
    check_status()
