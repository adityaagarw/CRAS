import redis
from db.database import *

        # customer_id=record[0],
        # name=record[1],
        # phone_number=record[2],
        # encoding=record[3],
        # image=record[4],
        # return_customer=1,
        # last_visit=record[6],
        # average_time_spent=record[7],
        # average_purchase=record[8],
        # maximum_purchase=record[9],
        # remarks=record[10],
        # loyalty_level=record[11],
        # num_visits=record[12],
        # last_location=record[13],
        # location_list=record[14],
        # category=record[15],
        # creation_date=record[16],
        # group_id=record[17],
        # incomplete=1,
        # entry_time=str(new_record.entry_time),
        # billed=0,
        # exited=0,
        # visit_time="",
        # exit_time=""

def debug(in_mem_db, encoding='utf-8'):

    # Retrieve all keys in the database
    records = in_mem_db.connection.keys('customer_inmem_db:*')

    print("\nKeys: ")
    if records:
        print(records)
    print("\n")
    # Iterate over the keys and fetch the corresponding values
    for record_key in records:
        record_data = in_mem_db.connection.hgetall(record_key)
        record_cust_id = record_data.get(b'customer_id').decode()
        print(record_cust_id)
        print(type(record_cust_id))
        print(record_data.get(b'entry_time').decode())
        # Print the record
        # print("Key:", key.decode())
        # print("Record:")
        # for field, value in record.items():
        #     field_str = field.decode()

        #     if field_str == 'encoding':
        #         #value_str = np.frombuffer(value, dtype=np.float32).tolist()
        #         value_str = "ENCODING"
        #     elif field_str == 'image':
        #         #value_str = np.frombuffer(value, dtype=np.uint8).tolist()
        #         value_str = "IMAGE"
        #     else:
        #         value_str = value.decode()
        #     print(f"{field_str}: {value_str}")

        # print()
    print("\n")

if __name__ == "__main__":
    inmem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
    inmem_db.connect()
    debug(inmem_db)