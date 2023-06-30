import argparse
import io
import cv2
import numpy as np
from db.database import *
import time
from PIL import Image

def display_inmem_images(inmem_db):
    keys = inmem_db.connection.keys('*')
    for key in keys:
        record = inmem_db.connection.hgetall(key)
        img_bytes = record.get(b'image')
        image = Image.open(io.BytesIO(img_bytes))
        image_arr = np.array(image)
        cv2.imshow("Window", image_arr)
        cv2.waitKey(0)
    cv2.destroyAllWindows()

def display_image(inmem_db, cust_id):
    key = "customer_inmem_db:" + cust_id
    record = inmem_db.connection.hgetall(key)
    img_bytes = record.get(b'image')
    image = Image.open(io.BytesIO(img_bytes))
    image_arr = np.array(image)
    cv2.imshow("Customer", image_arr)
    cv2.waitKey(0)
    cv2.destroyAllWindows()

def delete_inmem_record(inmem_db, record_id):
    inmem_db.delete_record(record_id)

def delete_all_records(inmem_db):
    keys = inmem_db.connection.keys('customer_inmem_db:*')

    count = 0
    for key in keys:
        record = inmem_db.connection.hgetall(key)
        cust_id = record.get(b'customer_id').decode()
        print(cust_id) 
        inmem_db.delete_record(cust_id)
        count += 1

    print("Successfully deleted ", count, " customer records")

    keys = inmem_db.connection.keys('visit_inmem_db:*')

    count = 0
    for key in keys:
        record = inmem_db.connection.hgetall(key)
        cust_id = record.get(b'customer_id').decode()
        print(cust_id) 
        inmem_db.delete_record(cust_id, type='visit')
        count += 1

    print("Successfully deleted ", count, " visit records")

    keys = inmem_db.connection.keys('store_inmem_db:*')

    count = 0
    for key in keys:
        record = inmem_db.connection.hgetall(key)
        cust_id = record.get(b'store_id').decode()
        print(cust_id) 
        inmem_db.delete_record(cust_id, type='store')
        count += 1

    print("Successfully deleted ", count, " store records")

    keys = inmem_db.connection.keys('exited_inmem_db:*')

    count = 0
    for key in keys:
        record = inmem_db.connection.hgetall(key)
        cust_id = record.get(b'customer_id').decode()
        print(cust_id) 
        inmem_db.delete_record(cust_id, type='exited')
        count += 1

    print("Successfully deleted ", count, " exited records")

    keys = inmem_db.connection.keys('incomplete_inmem_db:*')

    count = 0
    for key in keys:
        record = inmem_db.connection.hgetall(key)
        cust_id = record.get(b'customer_id').decode()
        print(cust_id) 
        inmem_db.delete_record(cust_id, type='incomplete')
        count += 1

    print("Successfully deleted ", count, " incomplete records")

def display_inmem_redis_db(inmem_db, encoding='utf-8'):

    # Retrieve all keys in the database
    keys = inmem_db.connection.keys('*')

    for key in keys:
        print(key.decode())
    # Iterate over the keys and fetch the corresponding values
    for key in keys:
        record = inmem_db.connection.hgetall("customer_inmem_db:a0a45f40-c35c-407b-8f69-29790b711ccc")

        # Print the record
        print("Key:", key.decode())
        print("Record:")
        for field, value in record.items():
            field_str = field.decode()

            if field_str == 'encoding':
                #value_str = np.frombuffer(value, dtype=np.float32).tolist()
                value_str = "ENCODING"
            elif field_str == 'image':
                #value_str = np.frombuffer(value, dtype=np.uint8).tolist()
                value_str = "IMAGE"
            else:
                value_str = value.decode()
            print(f"{field_str}: {value_str}")

        print()

    # Disconnect from the in-memory Redis database

if __name__ == "__main__":
    inmem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)  # Update with your Redis connection details
    inmem_db.connect()

    parser = argparse.ArgumentParser()
    parser.add_argument("-delete", type=str, help="Delete a particular record", required = False)
    parser.add_argument("-print", type=str, help="Print the whole db", required=False, default="no")
    parser.add_argument("-delete_all_records", type=str, help="Delete all records", required=False, default="no") 
    parser.add_argument("-display_all_images", type=str, help="Display all images", required=False, default="no")
    parser.add_argument("-display_image", type=str, help="Display image", required=False, default=None)
    args = parser.parse_args()
    if args.print == "yes":
        display_inmem_redis_db(inmem_db)

    if args.delete:
        delete_inmem_record(inmem_db, args.delete)

    if args.delete_all_records == "yes":
        delete_all_records(inmem_db)

    if args.display_all_images == "yes":
        display_inmem_images(inmem_db)

    if args.display_image is not None:
        display_image(inmem_db, args.display_image)

    inmem_db.connection.close()
