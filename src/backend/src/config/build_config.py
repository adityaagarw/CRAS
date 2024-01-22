import argparse
import configparser
from config.params import Parameters

def build_configuration_file(p):
    config = configparser.ConfigParser()
    config['general'] = {}
    default = config['general']
    config.set('general', 'detection', p.detection)
    config.set('general', 'library', p.library)
    config.set('general', 'model', p.model)
    config.set('general', 'threshold', str(p.threshold))
    config.set('general', 'yaw_threshold', str(p.yaw_threshold))
    config.set('general', 'pitch_threshold', str(p.pitch_threshold))
    config.set('general', 'area_threshold', str(p.area_threshold))
    config.set('general', 'billing_cam_time', str(p.billing_cam_time))
    config.set('general', 'sim_method', p.sim_method)
    config.set('general', 'debug_mode',str(p.debug_mode))
    config.set('general', 'username', p.username)
    config.set('general', 'password', p.password)
    config.set('general', 'db_link', p.db_link)
    config.set('general', 'db_name', p.db_name)
    config.set('general', 'input_type', p.input_type)
    config.set('general', 'video_path', p.video_path)
    config.set('general', 'model_dir', p.model_dir)
    config.set('general', 'num_entry_cams', str(p.num_entry_cams))
    config.set('general', 'num_billing_cams', str(p.num_billing_cams))
    config.set('general', 'num_exit_cams', str(p.num_exit_cams))
    config.set('general', 'entry_cam', str(p.entry_cam))
    config.set('general', 'entry_cam_type', str(p.entry_cam_type))
    config.set('general', 'billing_cam', str(p.billing_cam))
    config.set('general', 'billing_cam_type', str(p.billing_cam_type))
    config.set('general', 'exit_cam', str(p.exit_cam))
    config.set('general', 'exit_cam_type', str(p.exit_cam_type))
    config.set('general', 'exit_threshold', str(p.exit_threshold))


    with open('config.ini', 'w') as configfile:
        config.write(configfile)

if __name__ == "__main__":
    parser = argparse.ArgumentParser()

    parser.add_argument("-detection", type=str, help="Specify the detection method. Options: Frontal, CNN", required = True)
    parser.add_argument("-library", type=str, help="Specify the library. Options: DLIB, VGG, FaceNet", required = True)
    parser.add_argument("-model", type=str, help="Specify the model used by vgg lib. Options: vgg16, resnet50, senet50", required = True)
    parser.add_argument("-threshold", type=float, help="Specify the threshold for similarity", required = True)
    parser.add_argument("-yaw_threshold", type=int, help="Specify the yaw threshold for face", required = True)
    parser.add_argument("-pitch_threshold", type=int, help="Specify the pitch threshold for face", required = True)
    parser.add_argument("-area_threshold", type=int, help="Specify the area threshold for face", required = True)
    parser.add_argument("-billing_cam_time", type=int, help="Specify the time for billing camera", required = True)
    parser.add_argument("-sim_method", type=str, help="Specify the similarity comparision method", required = True)
    parser.add_argument("-debug_mode", type=int, help="Specify whether to run the app in debug mode", required = True)
    parser.add_argument("-username", type=str, help="Specify db username", required = True)
    parser.add_argument("-password", type=str, help="Specify db password", required = True)
    parser.add_argument("-db_link", type=str, help="Specify db connection link", required = True)
    parser.add_argument("-db_name", type=str, help="Specify db name to connect to", required = True)
    parser.add_argument("-input_type", type=str, help="Specify whether using camera or video stream", required = True)
    parser.add_argument("-video_path", type=str, help="Specify the path where video is located", required = True)
    parser.add_argument("-model_dir", type=str, help="Specify where trained models are stored", required = True)
    parser.add_argument("-num_entry_cams", type=str, help="Specify number of entry_cams", required = True)
    parser.add_argument("-num_billing_cams", type=str, help="Specify number of billing_cams", required = True)
    parser.add_argument("-num_exit_cams", type=str, help="Specify number of exit_cams", required = True)
    parser.add_argument("-entry_cam", type=str, help="Specify entry_cam", required = True)
    parser.add_argument("-entry_cam_type", type=str, help="Specify entry_cam_type", required = True)
    parser.add_argument("-billing_cam", type=str, help="Specify billing_cam", required = True)
    parser.add_argument("-billing_cam_type", type=str, help="Specify billing_cam_type", required = True)
    parser.add_argument("-exit_cam", type=str, help="Specify exit_cam", required = True)
    parser.add_argument("-exit_cam_type", type=str, help="Specify exit_cam_type", required = True)
    parser.add_argument("-exit_threshold", type=float, help="Specify exit threshold", required = True)

    args = parser.parse_args()
    parameters = Parameters(args.detection, \
                            args.library, \
                            args.model, \
                            args.threshold, \
                            args.yaw_threshold, \
                            args.pitch_threshold, \
                            args.area_threshold, \
                            args.billing_cam_time, \
                            args.sim_method, \
                            args.debug_mode, \
                            args.username, \
                            args.password, \
                            args.db_link, \
                            args.db_name, \
                            args.input_type, \
                            args.video_path, \
                            args.model_dir, \
                            args.num_entry_cams, \
                            args.num_billing_cams, \
                            args.num_exit_cams, \
                            args.entry_cam, \
                            args.entry_cam_type, \
                            args.billing_cam, \
                            args.billing_cam_type, \
                            args.exit_cam, \
                            args.exit_cam_type, \
                            args.exit_threshold)

    build_configuration_file(parameters)
