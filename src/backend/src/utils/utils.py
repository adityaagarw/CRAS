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
    # 00000001: Entry up
    # 00000010: Exit up
    # 00000100: Billing up
    # 00001000: Employee up
    # 00010000: Backend up
    # 00100000: Starting
    # 01000000: Shutdown system
    entry_bitmask = 0b0001  # First LSB
    exit_bitmask = 0b0010  # Second LSB
    billing_bitmask = 0b0100  # Third LSB
    employee_bitmask = 0b1000 # Fourth LSB
    backend_bitmask = 0b10000  # Fifth LSB
    starting_bitmask = 0b100000  # Sixth LSB
    shutdown_system_bitmask = 0b1000000  # Seventh LSB

    lock_file = "status.lock"

    def read_status():
        # Read the status file from the disk
        with open("status", "r") as f:
            status = f.read()
        return status

    def write_status(status):
        with open("status", "w") as f:
            f.write(str(status))

    def entry_up():
        status = Utils.read_status()
        status = int(status)
        status = status | Utils.entry_bitmask
        Utils.write_status(status)

    def exit_up():
        status = Utils.read_status()
        status = int(status)
        status = status | Utils.exit_bitmask
        Utils.write_status(status)

    def billing_up():
        status = Utils.read_status()
        status = int(status)
        status = status | Utils.billing_bitmask
        Utils.write_status(status)

    def backend_up():
        status = Utils.read_status()
        status = int(status)
        status = status | Utils.backend_bitmask
        Utils.write_status(status)

    def entry_down():
        status = Utils.read_status()
        status = int(status)
        status = status & ~Utils.entry_bitmask
        Utils.write_status(status)

    def exit_down():
        status = Utils.read_status()
        status = int(status)
        status = status & ~Utils.exit_bitmask
        Utils.write_status(status)

    def billing_down():
        status = Utils.read_status()
        status = int(status)
        status = status & ~Utils.billing_bitmask
        Utils.write_status(status)

    def backend_down():
        status = Utils.read_status()
        status = int(status)
        status = status & ~Utils.backend_bitmask
        Utils.write_status(status)

    def employee_up():
        status = Utils.read_status()
        status = int(status)
        status = status | Utils.employee_bitmask
        Utils.write_status(status)

    def employee_down():
        status = Utils.read_status()
        status = int(status)
        status = status & ~Utils.employee_bitmask
        Utils.write_status(status)

    def starting():
        status = 0
        status = status | Utils.starting_bitmask
        Utils.write_status(status)

    def started():
        status = Utils.read_status()
        status = int(status)
        status = status & ~Utils.starting_bitmask
        Utils.write_status(status)

    def shutdown_system():
        status = Utils.read_status()
        status = int(status)
        status = status | Utils.shutdown_system_bitmask
        Utils.write_status(status)

