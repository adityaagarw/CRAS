import os
import platform

def get_model_dir():
    current = os.getcwd()
    if platform.system() == "Windows":
        split_char = '\\'
    else:
        split_char = '/'

    goto_path_dirs = current.split(split_char)
    if platform.system() == "Windows":
        goto_path = '\\'.join(goto_path_dirs[:-1]) + "\\models"
    else:
        goto_path = '/'.join(goto_path_dirs[:-1]) + "/models"

    return goto_path

default_model_dir = get_model_dir()
default_video_path = get_model_dir()
default_username = "cras"
default_password = "cras_admin"
default_db_link = "localhost"
default_db_name = "local_customers"
default_detection_method = "Frontal"
default_sim_method = "Cosine"
default_recognition_library = "VGG"
default_model = 'resnet50'
default_threshold = 0.7
default_yaw_threshold = 10
default_debug_method = 1
default_input_type = "video" # Change to camera

command = " -detection " + default_detection_method + \
          " -library " + default_recognition_library + \
          " -model " + default_model + \
          " -threshold " + str(default_threshold) + \
          " -yaw_threshold " + str(default_yaw_threshold) + \
          " -sim_method " + default_sim_method + \
          " -debug_mode " + str(default_debug_method) + \
          " -username " + default_username + \
          " -password " + default_password + \
          " -db_link " + "\"" + default_db_link + "\"" + \
          " -db_name " + "\"" + default_db_name + "\""  \
          " -input_type " + default_input_type +\
          " -video_path " + "\"" + default_video_path + "\"" + \
          " -model_dir " + "\"" + default_model_dir + "\""

command = "python3 build_config.py" + command
os.system(command)
