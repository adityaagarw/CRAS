import os
import psutil

def read_entry_pid():
    try:
        # Read the PID from the file
        with open("entry_pid", "r") as f:
            pid = int(f.read())
    except:
        print("Entry program not running!")
        exit(1)
    return pid

def read_billing_pid():
    try:
        # Read the PID from the file
        with open("billing_pid", "r") as f:
            pid = int(f.read())
    except:
        print("Billing program not running!")
        exit(1)
    return pid

def read_exit_pid():
    # Read the PID from the file
    try:
        with open("exit_pid", "r") as f:
            pid = int(f.read())
    except:
        print("Exit program not running!")
        exit(1)
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

    with open("status", "w") as f:
        f.write("2")

    entry_pid = read_entry_pid()
    billing_pid = read_billing_pid()
    exit_pid = read_exit_pid()

    # Kill the process using psutil
    try:
        entry_p = psutil.Process(entry_pid)
    except:
        entry_p = None
        
    try:
        billing_pid = psutil.Process(billing_pid)
    except:
        billing_pid = None

    try:
        exit_pid = psutil.Process(exit_pid)
    except:
        exit_pid = None

    # Check if entry_p is running
    if entry_p and entry_p.is_running():
        print("Shutting down entry program")
        for p in entry_p.children(recursive=True):
            p.terminate()
        entry_p.terminate()
        delete_entry_pid()
    else:
        print("Entry program already shut down")
        os.remove("entry_pid")
    
    # Check if billing_pid is running
    if billing_pid and billing_pid.is_running():
        print("Shutting down billing program")
        for p in billing_pid.children(recursive=True):
            p.terminate()
        billing_pid.terminate()
        delete_billing_pid()
    else:
        print("Billing program already shut down")
        os.remove("billing_pid")
    
    # Check if exit_pid is running
    if exit_pid and exit_pid.is_running():
        print("Shutting down exit program")
        for p in exit_pid.children(recursive=True):
            p.terminate()
        exit_pid.terminate()
        delete_exit_pid()
    else:
        print("Exit program already shut down")
        os.remove("exit_pid")
