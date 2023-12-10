import os
import psutil

def kill_all_python_processes():
    current_pid = os.getpid()
    for process in psutil.process_iter(attrs=['pid', 'name']):
        try:
            process_info = process.info
            pid = process_info['pid']
            name = process_info['name']
            
            if name and 'python' in name.lower() and pid != current_pid:
                print(f"Killing Python process with PID {pid}")
                psutil.Process(pid).terminate()
        except (psutil.NoSuchProcess, psutil.AccessDenied, psutil.ZombieProcess):
            print("Some exception occured while killing python processes")
            pass

if __name__ == "__main__":
    kill_all_python_processes()