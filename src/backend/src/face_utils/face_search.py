from db.database import *
from face.face import *
from face_utils.imagetoface import ImageToFace
# Takes in a cv2 image and checks if we have a threshold match or not
# Should return all records above the specified threshold
# If no record is found above the threshold, return 'num' closest matches
class FaceSearch():
        def __init__(self, parameters, num):
            
            self.threshold = parameters.threshold
            self.num_records = num
            self.in_mem_db = InMemoryRedisDB(host="127.0.0.1", port=6379)
            self.local_db = LocalPostgresDB(host='127.0.0.1', port=5432, database='localdb', user='cras_admin', password='admin')
            self.imgToFace = ImageToFace()
            self.detector = Detection(parameters)
            self.r = Recognition(parameters)

            self.local_db.connect()
            if not self.local_db.connection:
                print("Local db connection failed in FaceSearch!")
            else:
                print("Connected to localdb")

            self.in_mem_db.connect()
            if not self.in_mem_db.connection:
                print("Redis db connection failed in FaceSearch!")
            else:
                print("Connected to redis db")

        def fetch_customer_records(self, face_encoding, threshold, local_db):
            face_encoding_str = f"{face_encoding.tolist()}"
            face_record_query = """
                                SELECT *,(1-(encoding <=> %(face_encoding)s)) FROM local_customer_db WHERE (1 - (encoding <=> %(face_encoding)s)) > %(threshold)s; 
                                """
            local_db.cursor.execute(face_record_query, {'face_encoding': face_encoding_str, 'threshold': threshold})
            records = local_db.cursor.fetchall()
            return records

        def fetch_similar_records(self, face_encoding, local_db, num_records):
            face_encoding_str = f"{face_encoding.tolist()}"
            face_record_query = """
                                SELECT *,(1-(encoding <=> %(face_encoding)s)) FROM local_customer_db ORDER BY (encoding <=> %(face_encoding)s) LIMIT %(num_records)s; 
                                """
            local_db.cursor.execute(face_record_query, {'face_encoding': face_encoding_str, 'num_records': num_records})
            records = local_db.cursor.fetchall()
            return records

        # CV2 image
        def search_localdb(self, image):
            face_encoding, face_pixels = self.imgToFace.imageToEncoding(self.detector, self.r, image)
            face_image = self.imgToFace.get_face_image(face_pixels)
            if face_encoding is None:
                print("No face found in given image")
                return None
            
            # First search records above threshold
            records = self.fetch_customer_records(face_encoding, self.threshold, self.local_db)
            if records is None or len(records) == 0:
                print("No records found above threshold")
                records_similar = self.fetch_similar_records(face_encoding, self.local_db, self.num_records)
                if records_similar is None or len(records_similar) == 0:
                    print("No records found similar to given image")
                    return None
                else:
                    return records_similar
            else:
                return records


        def fetch_customer_records_from_mem(self, face_encoding, threshold, in_mem_db):
            # Get all customer records from the in-memory Redis database
            records = in_mem_db.connection.keys('customer_inmem_db:*')
            valid_records = []
            # Initialize variables to track the closest record and similarity
            closest_record = None
            closest_similarity = -1.0

            # Iterate over each record
            for record_key in records:
                # Retrieve the face encoding from the record
                record_data = in_mem_db.connection.hgetall(record_key)
                record_encoding_bytes = record_data.get(b'encoding')

                # Convert the face encodings to numpy arrays
                face_encoding_np = np.frombuffer(face_encoding, dtype=np.float32)
                
                record_encoding_np = np.frombuffer(record_encoding_bytes, dtype=np.float32)

                # Calculate the cosine similarity between the face encodings
                similarity = cosine_similarity(face_encoding_np.reshape(1, -1), record_encoding_np.reshape(1, -1))

                # Check if the similarity exceeds the threshold and is closer than the previous closest
                if similarity > float(threshold):
                    valid_records.append((record_data, similarity))

            return valid_records

        def get_closest_face_records(self, face_encoding, threshold, in_mem_db, num_records):
            # Get all customer records from the in-memory Redis database
            records = in_mem_db.connection.keys('customer_inmem_db:*')

            # Initialize an array to store records along with their similarity
            similar_records = []

            # Iterate over each record
            for record_key in records:
                # Retrieve the face encoding from the record
                record_data = in_mem_db.connection.hgetall(record_key)
                record_encoding_bytes = record_data.get(b'encoding')

                # Convert the face encodings to numpy arrays
                face_encoding_np = np.frombuffer(face_encoding, dtype=np.float32)
                record_encoding_np = np.frombuffer(record_encoding_bytes, dtype=np.float32)

                # Calculate the cosine similarity between the face encodings
                similarity = cosine_similarity(face_encoding_np.reshape(1, -1), record_encoding_np.reshape(1, -1))

                
                # Add the record and its similarity to the list
                similar_records.append((record_data, similarity))

            # Sort the records by similarity in descending order
            similar_records.sort(key=lambda record: record[1], reverse=True)

            # Return the top 'x' records
            return similar_records[:num_records]

        def search_inmemdb(self, image):
            face_encoding, face_pixels = self.imgToFace.imageToEncoding(self.detector, self.r, image)
            face_image = self.imgToFace.get_face_image(face_pixels)
            if face_encoding is None:
                print("No face found in given image")
                return None
            
            # First search records above threshold
            records = self.fetch_customer_records_from_mem(face_encoding, self.threshold, self.in_mem_db)
            if records is None or len(records) == 0:
                print("No records found above threshold")
                records_similar = self.fetch_similar_records_from_mem(face_encoding, self.threshold, self.local_db, self.num_records)
                if records_similar is None or len(records_similar) == 0:
                    print("No records found similar to given image")
                    return None
                else:
                    return records_similar
            else:
                return records