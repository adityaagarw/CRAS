import os
import shutil
import platform
import xml.etree.ElementTree as ET

def get_python_command():
    python_commands = ['python3', 'python', 'py']

    for command in python_commands:
        if shutil.which(command) is not None:
            return command

    raise EnvironmentError('Python command not found in your system')

def get_model_dir():
    current = os.getcwd()
    if platform.system() == "Windows":
        split_char = '\\'
    else:
        split_char = '/'

    goto_path_dirs = current.split(split_char)
    if platform.system() == "Windows":
        goto_path = '\\'.join(goto_path_dirs[:-1]) + "\\src\\models"
    else:
        goto_path = '/'.join(goto_path_dirs[:-1]) + "/src/models"

    return goto_path

# Function to obtain these parameters from Cras.config xml file where node is Settings
def get_default_params(file_path):
    # Open and read Cras.config xml file using an xml parser
    tree = ET.parse(file_path)
    root = tree.getroot()
    # Check if the root has a child named 'Settings'
    settings_node = root.find('Settings')
    if settings_node is None:
        raise ValueError("No 'Settings' node found in the XML")

    # Dictionary to hold settings
    settings = {}

    # Iterate through each child element in 'Settings'
    for element in settings_node:
        if element.tag == 'add':
            key = element.get('key')
            value = element.get('value')
            settings[key] = value

    return settings


# default_model_dir = get_model_dir()
# default_video_path = get_model_dir()
# default_username = "cras_admin"
# default_password = "admin"
# default_db_link = "127.0.0.1"
# default_db_name = "localdb"
# default_detection_method = "Frontal"
# default_sim_method = "Cosine"
# default_recognition_library = "VGG"
# default_model = 'resnet50'
# default_threshold = 0.65
# default_yaw_threshold = 30
# default_pitch_threshold = 170
# default_area_threshold = 10000
# default_billing_cam_time = 5
# default_debug_method = 1
# default_input_type = "video" # Change to camera
# default_num_entry_cams = 1
# default_num_billing_cams = 1
# default_num_exit_cams = 1
# default_entry_cam = 0
# default_billing_cam = 1
# default_exit_cam = 2

settings = get_default_params('..\\..\\frontend\\src\\CRAS\\Cras.config')

default_model_dir = get_model_dir()
default_video_path = get_model_dir()
default_username = settings['username']
default_password = settings['password']
default_db_link = settings['db_link']
default_db_name = settings['db_name']
default_detection_method = settings['detection']
default_sim_method = settings['sim_method']
default_recognition_library = settings['library']
default_model = settings['model']
default_threshold = settings['threshold']
default_yaw_threshold = settings['yaw_threshold']
default_pitch_threshold = settings['pitch_threshold']
default_area_threshold = settings['area_threshold']
default_billing_cam_time = settings['billing_cam_time']
default_debug_method = settings['debug_mode']
default_input_type = settings['input_type']
default_num_entry_cams = settings['num_entry_cams']
default_num_billing_cams = settings['num_billing_cams']
default_num_exit_cams = settings['num_exit_cams']
default_entry_cam = settings['entry_cam']
default_enty_cam_type = settings['entry_cam_type']
default_billing_cam = settings['billing_cam']
default_billing_cam_type = settings['billing_cam_type']
default_exit_cam = settings['exit_cam']
default_exit_cam_type = settings['exit_cam_type']

command = " -detection " + default_detection_method + \
          " -library " + default_recognition_library + \
          " -model " + default_model + \
          " -threshold " + str(default_threshold) + \
          " -yaw_threshold " + str(default_yaw_threshold) + \
          " -pitch_threshold " + str(default_pitch_threshold) + \
          " -area_threshold " + str(default_area_threshold) + \
          " -billing_cam_time " + str(default_billing_cam_time) + \
          " -sim_method " + default_sim_method + \
          " -debug_mode " + str(default_debug_method) + \
          " -username " + default_username + \
          " -password " + default_password + \
          " -db_link " + "\"" + default_db_link + "\"" + \
          " -db_name " + "\"" + default_db_name + "\""  \
          " -input_type " + default_input_type +\
          " -video_path " + "\"" + default_video_path + "\"" + \
          " -model_dir " + "\"" + default_model_dir + "\"" + \
          " -num_entry_cams " + str(default_num_entry_cams) + \
          " -num_billing_cams " + str(default_num_billing_cams) + \
          " -num_exit_cams " + str(default_num_exit_cams) + \
          " -entry_cam " + str(default_entry_cam) + \
          " -entry_cam_type " + str(default_enty_cam_type) + \
          " -billing_cam " + str(default_billing_cam) + \
          " -billing_cam_type " + str(default_billing_cam_type) + \
          " -exit_cam " + str(default_exit_cam) + \
          " -exit_cam_type " + str(default_exit_cam_type)

command = get_python_command() + " config/build_config.py" + command
os.system(command)
