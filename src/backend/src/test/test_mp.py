import multiprocessing
import threading

class PicklableTimer(threading.Thread):
    def __init__(self, interval, function, *args, **kwargs):
        super().__init__()
        self.interval = interval
        self.function = function
        self.args = args
        self.kwargs = kwargs
        self.finished = threading.Event()

    def run(self):
        self.finished.wait(self.interval)
        if not self.finished.is_set():
            self.function(*self.args, **self.kwargs)

    def cancel(self):
        self.finished.set()

def commit_record(customer_id):
    print("Committing record for customer: ", customer_id)

def cancel_timer(timer_dict):
    print("Canceling timer")
    id = 1
    timer = timer_dict[id]
    timer.cancel()

def process_call(customer_id):
    manager = multiprocessing.Manager()
    
    timer_dict = {}

    cancel_process = multiprocessing.Process(target=cancel_timer, args=(timer_dict, ))
    
    timer = threading.Timer(10, commit_record, args=(timer_dict, ))
    timer.start()
    print("HELLO WORLD")
    timer_dict[id] = timer
    cancel_process.start()
    print("WORLD HELLO")

if __name__ == "__main__":
    customer_id = 1
    process_call(customer_id)
