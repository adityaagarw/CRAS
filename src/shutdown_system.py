import os
import psutil

def read_entry_pid():
    # Read the PID from the file
    with open("entry_pid", "r") as f:
        pid = int(f.read())
    return pid

def read_billing_pid():
    # Read the PID from the file
    with open("billing_pid", "r") as f:
        pid = int(f.read())
    return pid

def read_exit_pid():
    # Read the PID from the file
    with open("exit_pid", "r") as f:
        pid = int(f.read())
    return pid

def delete_entry_pid():
    # Delete the PID file
    os.remove("entry_pid")

def delete_billing_pid():
    # Delete the PID file
    os.remove("billing_pid")

def delete_exit_pid():
    # Delete the PID file
    os.remove("exit_pid")

if __name__ == "__main__":
    entry_pid = read_entry_pid()
    billing_pid = read_billing_pid()
    exit_pid = read_exit_pid()

    # Kill the process using psutil
    entry_p = psutil.Process(entry_pid)
    billing_pid = psutil.Process(billing_pid)
    exit_pid = psutil.Process(exit_pid)

    for p in entry_p.children(recursive=True):
        p.terminate()

    entry_p.terminate()

    for p in billing_pid.children(recursive=True):
        p.terminate()

    billing_pid.terminate()
    
    for p in exit_pid.children(recursive=True):
        p.terminate()

    delete_entry_pid()
    delete_billing_pid()
    delete_exit_pid()