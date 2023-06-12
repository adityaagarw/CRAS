from db.redis_pubsub import *
from db.database import *
from multiprocessing import Process 

def publish_message():
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    in_mem_db.connect()
    in_mem_db.connection.publish(Channel.Frontend.value, FrontendMessage.StartBilling.value)

def check_for_messages():
    in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    in_mem_db.connect()
    p = in_mem_db.connection.pubsub()
    p.subscribe(Channel.Billing.value)

    for message in p.listen():
        data = message['data']
        if isinstance(data, bytes):
            print(data.decode())

if __name__ == "__main__":
    publish_message()
    abcd = Process(target=check_for_messages)
    abcd.start()
    abcd.join()