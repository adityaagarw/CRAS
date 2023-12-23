######################################################################
# <Company-Name>                                                     #
# Copyright 2023                                                     #
#                                                                    #
# Generic utilities                                                  #
#                                                                    #
######################################################################

import requests
import uuid

class Utils:
    def generate_face_encoding_from_image():
        pass
    def get_location():
        response = requests.get('https://api64.ipify.org?format=json').json()
        ip_address = response["ip"]
        response = requests.get(f'https://ipapi.co/{ip_address}/json/').json()
        location = response.get("city") + ', ' + response.get("region") + ', ' + response.get("country_name")
        return location

    def generate_unique_id():
        return uuid.uuid4()
    
    # Write to status file
    # 0: Success
    # 1: Failure
    # 2: Shutdown
    def write_status(status):
        with open("status", "w") as f:
            f.write(status)
