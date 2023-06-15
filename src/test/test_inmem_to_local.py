from db.database import *
from utils.utils import Utils
from datetime import datetime
import numpy as np
import psycopg2
import redis
import time


#Key: customer_inmem_db:74a79c47-6a78-43ad-98b9-7deb6bccdc30
#2023-06-10 22:49:41.793134

def print_sample_encoding(inmem_db):
    record = inmem_db.connection.hgetall('customer_inmem_db:74a79c47-6a78-43ad-98b9-7deb6bccdc30')
    print(record.get(b'encoding'))
    encoding = np.frombuffer(record.get(b'encoding'), dtype=np.float32).tolist()
    print(str(encoding))
    print(record.get(b'image'))
    image = np.frombuffer(record.get(b'image'), dtype=np.uint8).tolist()
    print(str(image))


def copy_and_insert_record_inmem(inmem_db):
    record = inmem_db.connection.hgetall('customer_inmem_db:74a79c47-6a78-43ad-98b9-7deb6bccdc30')
    timestamp1 = datetime.strptime("2023-06-10 22:49:41.793134", "%Y-%m-%d %H:%M:%S.%f")
    timestamp2 = datetime.strptime("2023-06-10 23:51:43.790000", "%Y-%m-%d %H:%M:%S.%f")

    visit_time_local = timestamp2 - timestamp1
    
    new_record = InMemCustomer(
        customer_id = "62b34c47-6a78-43ad-98b9-7deb6bccdc31",
        name = "Aditya A",
        phone_number = "8105002564",
        encoding = record.get(b'encoding'),
        image = record.get(b'image'),
        return_customer = 1,
        last_visit = "2023-06-10 22:49:41.793134",
        average_time_spent = "01:30:00",
        average_purchase = "10000",
        maximum_purchase = "20000",
        remarks = "Return Customer",
        loyalty_level = "Gold",
        num_visits = 2,
        last_location = "Gurgaon",
        location_list = "{Gurgaon, Gurgaon}",
        category = "Customer",
        creation_date = "2023-06-10 22:49:41.793134",
        group_id = "1234",
        incomplete = 0,
        entry_time = "2023-06-10 22:49:41.793134",
        billed = 1,
        exited = 1,
        visit_time = str(visit_time_local),
        exit_time = "2023-06-10 23:51:43.790000"
    )

    new_visit_record = InMemVisit(
        customer_id = "62b34c47-6a78-43ad-98b9-7deb6bccdc31",
        visit_id = "cb2522ef-be59-4d34-962b-2d8ef8c48845",
        store_id = "75f9c9c1-c313-440c-9eec-96710a6ec4e1",
        entry_time = "2023-06-10 22:49:41.793134",
        exit_time = "2023-06-10 23:51:43.790000",
        billed = 1,
        bill_amount = "10000",
        time_spent = str(visit_time_local),
        visit_remark = "New customer",
        customer_rating = "5",
        customer_feedback = "5",
        incomplete = 0
    )

    #inmem_db.insert_record(new_record)
    inmem_db.insert_record(new_visit_record, type="visit")

def insert_record_to_local_db_from_mem(inmem_db, local_db):
    customer_record = inmem_db.connection.hgetall('customer_inmem_db:62b34c47-6a78-43ad-98b9-7deb6bccdc31')
    visit_record = inmem_db.connection.hgetall('visit_inmem_db:62b34c47-6a78-43ad-98b9-7deb6bccdc31')

    ins_customer_record = LocalCustomer(
        customer_id = customer_record.get(b'customer_id').decode(),
        name = customer_record.get(b'name').decode(),
        phone_number = customer_record.get(b'phone_number').decode(),
        encoding = customer_record.get(b'encoding'),
        image = customer_record.get(b'image'),
        return_customer = int(customer_record.get(b'return_customer').decode()),
        last_visit = customer_record.get(b'last_visit').decode(),
        average_time_spent = customer_record.get(b'average_time_spent').decode(),
        average_purchase = customer_record.get(b'average_purchase').decode(),
        maximum_purchase = customer_record.get(b'maximum_purchase').decode(),
        remarks = customer_record.get(b'remarks').decode(),
        loyalty_level = customer_record.get(b'loyalty_level').decode(),
        num_visits = int(customer_record.get(b'num_visits').decode()),
        last_location = customer_record.get(b'last_location').decode(),
        location_list = customer_record.get(b'location_list').decode(),
        category = customer_record.get(b'category').decode(),
        creation_date = customer_record.get(b'creation_date').decode(),
        group_id = customer_record.get(b'group_id').decode(),
    )

    ins_visit_record = LocalVisit(
        customer_id = visit_record.get(b'customer_id').decode(),
        visit_id = visit_record.get(b'visit_id').decode(),
        store_id = visit_record.get(b'store_id').decode(),
        entry_time = visit_record.get(b'entry_time').decode(),
        exit_time = visit_record.get(b'exit_time').decode(),
        billed = int(visit_record.get(b'billed').decode()),
        bill_amount = visit_record.get(b'bill_amount').decode(),
        time_spent = visit_record.get(b'time_spent').decode(),
        visit_remark = visit_record.get(b'visit_remark').decode(),
        customer_rating = visit_record.get(b'customer_rating').decode(),
        customer_feedback = visit_record.get(b'customer_feedback').decode(),
        incomplete = int(visit_record.get(b'incomplete').decode())
    )


if __name__ == "__main__":
    inmem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    inmem_db.connect()
    local_db = local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
    local_db.connect()
    insert_record_to_local_db_from_mem(inmem_db, local_db)







