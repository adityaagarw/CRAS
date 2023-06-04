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
    def get_location():
        response = requests.get('https://api64.ipify.org?format=json').json()
        ip_address = response["ip"]
        response = requests.get(f'https://ipapi.co/{ip_address}/json/').json()
        location = response.get("city") + ', ' + response.get("region") + ', ' + response.get("country_name")
        return location

    def generate_unique_id():
        return uuid.uuid4()
