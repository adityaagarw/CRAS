import configparser

class Parameters:
    def __init__(self, detection, library, model, threshold, yaw_threshold, pitch_threshold, area_threshold, billing_cam_time, sim_method, debug_mode, 
                 username, password, db_link, db_name, input_type, video_path, model_dir, 
                 num_entry_cams, num_billing_cams, num_exit_cams, entry_cam, entry_cam_type, billing_cam, billing_cam_type, exit_cam, exit_cam_type):
        self.detection = detection
        self.library = library
        self.model = model
        self.threshold = threshold
        self.yaw_threshold = yaw_threshold
        self.pitch_threshold = pitch_threshold
        self.area_threshold = area_threshold
        self.billing_cam_time = billing_cam_time
        self.sim_method = sim_method
        self.debug_mode = debug_mode
        self.username = username
        self.password = password
        self.db_link = db_link
        self.db_name = db_name
        self.input_type = input_type
        self.video_path = video_path
        self.model_dir = model_dir
        self.num_entry_cams = num_entry_cams
        self.num_billing_cams = num_billing_cams
        self.num_exit_cams = num_exit_cams
        self.entry_cam = entry_cam
        self.entry_cam_type = entry_cam_type
        self.billing_cam = billing_cam
        self.billing_cam_type = billing_cam_type
        self.exit_cam = exit_cam
        self.exit_cam_type = exit_cam_type

    @classmethod
    def build_parameters(cls, file):
        config = configparser.ConfigParser()
        config.read(file)
        args = config['general']
        parameters = cls(args['detection'], \
                                args['library'], \
                                args['model'], \
                                args['threshold'], \
                                args['yaw_threshold'], \
                                args['pitch_threshold'], \
                                args['area_threshold'], \
                                args['billing_cam_time'], \
                                args['sim_method'], \
                                args['debug_mode'], \
                                args['username'], \
                                args['password'], \
                                args['db_link'], \
                                args['db_name'], \
                                args['input_type'], \
                                args['video_path'], \
                                args['model_dir'], \
                                args['num_entry_cams'], \
                                args['num_billing_cams'], \
                                args['num_exit_cams'], \
                                args['entry_cam'], \
                                args['entry_cam_type'], \
                                args['billing_cam'], \
                                args['billing_cam_type'], \
                                args['exit_cam'], \
                                args['exit_cam_type'])
        return parameters