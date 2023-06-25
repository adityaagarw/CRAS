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
    record = inmem_db.connection.hgetall('customer_inmem_db:92909f09-7e45-4a09-893d-56c3630d9616')
    timestamp1 = datetime.strptime("2023-06-10 22:49:41.793134", "%Y-%m-%d %H:%M:%S.%f")
    timestamp2 = datetime.strptime("2023-06-10 23:51:43.790000", "%Y-%m-%d %H:%M:%S.%f")

    visit_time_local = timestamp2 - timestamp1
    
    # new_record = InMemCustomer(
    #     customer_id = "62b34c47-6a78-43ad-98b9-7deb6bccdc31",
    #     name = "Aditya A",
    #     phone_number = "8105002564",
    #     encoding = record.get(b'encoding'),
    #     image = record.get(b'image'),
    #     return_customer = "1",
    #     last_visit = "2023-06-10 22:49:41.793134",
    #     average_time_spent = "01:30:00.000000",
    #     average_purchase = "",
    #     maximum_purchase = "",
    #     remarks = "Return Customer",
    #     loyalty_level = "Gold",
    #     num_visits = "2",
    #     last_location = "Gurgaon",
    #     location_list = "{Gurgaon, Gurgaon}",
    #     category = "Customer",
    #     creation_date = "2023-06-10 22:49:41.793134",
    #     group_id = "1234",
    #     incomplete = "0",
    #     entry_time = "2023-06-10 22:49:41.793134",
    #     billed = "1",
    #     exited = "1",
    #     visit_time = str(visit_time_local),
    #     exit_time = "2023-06-10 23:51:43.790000"
    # )
    new_record = InMemCustomer(
        customer_id = "62b34c47-6a78-43ad-98b9-7deb6bccdc31",
        name = "",
        phone_number = "",
        encoding = record.get(b'encoding'),
        image = record.get(b'image'),
        return_customer = "0",
        last_visit = "2023-06-10 22:49:41.793134",
        average_time_spent = "01:30:00.000000",
        average_purchase = "",
        maximum_purchase = "",
        remarks = "Return Customer",
        loyalty_level = "Gold",
        num_visits = "2",
        last_location = "Gurgaon",
        location_list = "{Gurgaon, Gurgaon}",
        category = "Customer",
        creation_date = "2023-06-10 22:49:41.793134",
        group_id = "1234",
        incomplete = "0",
        entry_time = "2023-06-10 22:49:41.793134",
        billed = "1",
        exited = "1",
        visit_time = str(visit_time_local),
        exit_time = "2023-06-10 23:51:43.790000"
    )
    new_visit_record = InMemVisit(
        customer_id = "62b34c47-6a78-43ad-98b9-7deb6bccdc31",
        visit_id = "cb2522ef-be59-4d34-962b-2d8ef8c48845",
        store_id = "75f9c9c1-c313-440c-9eec-96710a6ec4e1",
        entry_time = "2023-06-10 22:49:41.793134",
        exit_time = "2023-06-10 23:51:43.790000",
        billed = "1",
        bill_amount = "10000",
        time_spent = str(visit_time_local),
        visit_remark = "New customer",
        customer_rating = "5",
        customer_feedback = "5",
        incomplete = "0"
    )

    inmem_db.insert_record(new_record)
    inmem_db.insert_record(new_visit_record, type="visit")

def insert_record_to_local_db_from_mem(inmem_db, local_db):
    customer_record = inmem_db.connection.hgetall('customer_inmem_db:62b34c47-6a78-43ad-98b9-7deb6bccdc31')
    visit_record = inmem_db.connection.hgetall('visit_inmem_db:62b34c47-6a78-43ad-98b9-7deb6bccdc31')

    face_encoding = np.frombuffer(customer_record.get(b'encoding'), dtype=np.float32).tolist()
    # ins_customer_record = LocalCustomer(
    #     customer_id = customer_record.get(b'customer_id').decode(),
    #     name = customer_record.get(b'name').decode(),
    #     phone_number = customer_record.get(b'phone_number').decode(),
    #     encoding = face_encoding,
    #     image = customer_record.get(b'image'),
    #     return_customer = int(customer_record.get(b'return_customer').decode()),
    #     last_visit = customer_record.get(b'last_visit').decode(),
    #     average_time_spent = customer_record.get(b'average_time_spent').decode(),
    #     average_purchase = customer_record.get(b'average_purchase').decode(),
    #     maximum_purchase = customer_record.get(b'maximum_purchase').decode(),
    #     remarks = customer_record.get(b'remarks').decode(),
    #     loyalty_level = customer_record.get(b'loyalty_level').decode(),
    #     num_visits = int(customer_record.get(b'num_visits').decode()),
    #     last_location = customer_record.get(b'last_location').decode(),
    #     location_list = customer_record.get(b'location_list').decode(),
    #     category = customer_record.get(b'category').decode(),
    #     creation_date = customer_record.get(b'creation_date').decode(),
    #     group_id = customer_record.get(b'group_id').decode(),
    # )

    ins_customer_record = LocalCustomer(
        customer_id = customer_record.get(b'customer_id').decode(),
        name = "",
        phone_number = "",
        encoding = face_encoding,
        image = customer_record.get(b'image'),
        return_customer = "0",
        last_visit = "",
        average_time_spent = "",
        average_purchase = "",
        maximum_purchase = "",
        remarks = "New Customer",
        loyalty_level = "",
        num_visits = "1",
        last_location = "\{ABCD\}",
        location_list = "",
        category = "",
        creation_date = "2023-06-10 22:49:41.793134",
        group_id = "",
    )

    if (ins_customer_record.average_purchase == ""):
        ins_customer_record.average_purchase = "0"

    if (ins_customer_record.maximum_purchase == ""):
        ins_customer_record.maximum_purchase = "0"
        
    # ins_visit_record = LocalVisit(
    #     customer_id = visit_record.get(b'customer_id').decode(),
    #     visit_id = visit_record.get(b'visit_id').decode(),
    #     store_id = visit_record.get(b'store_id').decode(),
    #     entry_time = visit_record.get(b'entry_time').decode(),
    #     exit_time = visit_record.get(b'exit_time').decode(),
    #     billed = int(visit_record.get(b'billed').decode()),
    #     bill_amount = visit_record.get(b'bill_amount').decode(),
    #     time_spent = visit_record.get(b'time_spent').decode(),
    #     visit_remark = visit_record.get(b'visit_remark').decode(),
    #     customer_rating = visit_record.get(b'customer_rating').decode(),
    #     customer_feedback = visit_record.get(b'customer_feedback').decode(),
    #     incomplete = int(visit_record.get(b'incomplete').decode())
    # )
    ins_visit_record = LocalVisit(
        customer_id = visit_record.get(b'customer_id').decode(),
        visit_id = visit_record.get(b'visit_id').decode(),
        store_id = visit_record.get(b'store_id').decode(),
        entry_time = "",
        exit_time = visit_record.get(b'exit_time').decode(),
        billed = int(visit_record.get(b'billed').decode()),
        bill_amount = visit_record.get(b'bill_amount').decode(),
        time_spent = "",
        visit_remark = visit_record.get(b'visit_remark').decode(),
        customer_rating = visit_record.get(b'customer_rating').decode(),
        customer_feedback = visit_record.get(b'customer_feedback').decode(),
        incomplete = int(visit_record.get(b'incomplete').decode())
    )
    local_db.insert_customer_record(ins_customer_record)
    local_db.insert_visit_record(ins_visit_record)

def insert_record_to_inmem_from_localdb(inmem_db, local_db):
    customer_record = inmem_db.connection.hgetall('customer_inmem_db:25ec8280-a827-4383-95e5-b33dbfffdb81')

    face_encoding = np.frombuffer(customer_record.get(b'encoding'), dtype=np.float32)

    face_encoding_str = f"{face_encoding.tolist()}"
    face_record_query = """
                        SELECT *, (encoding <=> %(face_encoding)s) AS distance FROM local_customer_db WHERE 1 - (encoding <=> %(face_encoding)s) > 0.65 LIMIT 1;
                        """

    local_db.cursor.execute(face_record_query, {'face_encoding': face_encoding_str})
    record = local_db.cursor.fetchone()

    customer_id = record[0]
    name = record[1]
    phone_number = record[2]
    encoding = record[3]
    image = record[4]
    return_customer = record[5]
    last_visit = record[6]
    average_time_spent = record[7]
    average_purchase = record[8]
    maximum_purchase = record[9]
    remarks = record[10]
    loyalty_level = record[11]
    num_visits = record[12]
    last_location = record[13]
    location_list = record[14]
    category = record[15]
    creation_date = record[16]
    group_id = record[17]
    cos_distance = record[18]

    print("Customer id: ", customer_id, " Type: ", type(customer_id))
    print("Name: ", name, " Type: ", type(name))
    print("Phone number: ", phone_number, " Type: ", type(phone_number))
    print("Encoding: ", "encoding", " Type: ", type(encoding))
    print("Image: ", image.tobytes(), " Type: ", type(image))
    print("Return customer: ", return_customer, " Type: ", type(return_customer))
    print("Last visit: ", last_visit, " Type: ", type(last_visit))
    print("Average time spent: ", average_time_spent, " Type: ", type(average_time_spent))
    print("Average purchase: ", average_purchase, " Type: ", type(average_purchase))
    print("Maximum purchase: ", maximum_purchase, " Type: ", type(maximum_purchase))
    print("Remarks: ", remarks, " Type: ", type(remarks))
    print("Loyalty level: ", loyalty_level, " Type: ", type(loyalty_level))
    print("Number of visits: ", num_visits, " Type: ", type(num_visits))
    print("Last location: ", last_location, " Type: ", type(last_location))
    print("Location list: ", location_list, " Type: ", type(location_list))
    print("Category: ", category, " Type: ", type(category))
    print("Creation date: ", creation_date, " Type: ", type(creation_date))
    print("Group id: ", group_id, " Type: ", type(group_id))
    print("Cosine distance: ", cos_distance, " Type: ", type(cos_distance))
    
    entry_time = datetime.now()
    ins_record = InMemCustomer(
        customer_id = customer_id,
        name = name,
        phone_number = str(phone_number),
        encoding = encoding,
        image = image.tobytes(),
        return_customer = str(return_customer),
        last_visit = str(last_visit),
        average_time_spent = str(average_time_spent),
        average_purchase = str(average_purchase),
        maximum_purchase = str(maximum_purchase),
        remarks = remarks,
        loyalty_level = str(loyalty_level),
        num_visits = str(num_visits),
        last_location = last_location,
        location_list = str(location_list),
        category = str(category),
        creation_date = str(creation_date),
        group_id = str(group_id),
        incomplete="1",
        entry_time=str(entry_time),
        billed="0",
        exited="0",
        visit_time="",
        exit_time=""
    )

    ins_visit_record = InMemVisit(
        customer_id=record[0],
        visit_id=str(Utils.generate_unique_id()),
        store_id=inmem_db.fetch_store_id(),
        entry_time=str(entry_time),
        exit_time="",
        billed="0",
        bill_amount="0",
        time_spent="",
        visit_remark="",
        customer_rating="",
        customer_feedback="",
        incomplete="1"
    )

    inmem_db.insert_record(ins_record)
    inmem_db.insert_record(ins_visit_record, type="visit")

    #print(record)

if __name__ == "__main__":
    inmem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    inmem_db.connect()
    local_db = local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
    local_db.connect()
    #insert_record_to_inmem_from_localdb(inmem_db, local_db)
    #copy_and_insert_record_inmem(inmem_db)
    insert_record_to_local_db_from_mem(inmem_db, local_db)