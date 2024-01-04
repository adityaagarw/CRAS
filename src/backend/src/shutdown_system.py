import os
import psutil
import fasteners
from utils.utils import Utils
from datetime import datetime
from db.database import *

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

def read_employee_pid():
    # Read the PID from the file
    try:
        with open("employee_pid", "r") as f:
            pid = int(f.read())
    except:
        print("Employee program not running!")
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

def delete_employee_pid():
    # Delete the PID file
    os.remove("employee_pid")

def log_session():
    date_format = "%Y-%m-%d %H:%M:%S"
    time_now = datetime.now().strftime(date_format)
    local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
    local_db.connect()
    if not local_db.connection:
            print("Local db connection failed while trying to log session!")
            return 1
    
    local_db.update_shutdown_time(str(time_now))

if __name__ == "__main__":

    entry_pid = read_entry_pid()
    billing_pid = read_billing_pid()
    exit_pid = read_exit_pid()
    employee_pid = read_employee_pid()

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

    try:
        employee_pid = psutil.Process(employee_pid)
    except:
        employee_pid = None

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

    # Check if employee_pid is running
    if employee_pid and employee_pid.is_running():
        print("Shutting down employee program")
        for p in employee_pid.children(recursive=True):
            p.terminate()
        employee_pid.terminate()
        delete_employee_pid()
    else:
        print("Employee program already shut down")
        os.remove("employee_pid")

    with fasteners.InterProcessLock(Utils.lock_file):
        Utils.shutdown_system()

    print("System shutdown complete")
    log_session()
