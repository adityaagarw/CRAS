import numpy as np
from db.database import *

inmem_db = InMemoryRedisDB(host='localhost', port=6379)
inmem_db.connect()

def display_inmem_redis_db(encoding='utf-8'):
    # Connect to the in-memory Redis database
    inmem_db = InMemoryRedisDB(host='localhost', port=6379)  # Update with your Redis connection details
    inmem_db.connect()

    # Retrieve all keys in the database
    keys = inmem_db.connection.keys('*')

    # Iterate over the keys and fetch the corresponding values
    for key in keys:
        record = inmem_db.connection.hgetall(key)

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
    inmem_db.connection.close()

# Example usage: Display the records in the in-memory Redis database
display_inmem_redis_db()
