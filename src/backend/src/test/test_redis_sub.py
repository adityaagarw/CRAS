from db.redis_pubsub import *
from db.database import *
from multiprocessing import Process 

def new_customer_handler():
    print("New customer detected!")

def check_for_messages():
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    in_mem_db.connect()
    p = in_mem_db.connection.pubsub()
    p.subscribe(Channel.Backend.value)

    for message in p.listen():
        data = message['data']
        if isinstance(data, bytes):
            print(data.decode())
            if data.decode() == BackendMessage.NewCustomer.value:
                new_customer_handler()

if __name__ == "__main__":
    abcd = Process(target=check_for_messages)
    abcd.start()
    abcd.join()