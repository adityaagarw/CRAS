class Parameters:
    def __init__(self, detection, library, model, threshold, yaw_threshold, sim_method, debug_mode, username, password, db_link, db_name, input_type, video_path, model_dir):
        self.detection = detection
        self.library = library
        self.model = model
        self.threshold = threshold
        self.yaw_threshold = yaw_threshold
        self.sim_method = sim_method
        self.debug_mode = debug_mode
        self.username = username
        self.password = password
        self.db_link = db_link
        self.db_name = db_name
        self.input_type = input_type
        self.video_path = video_path
        self.model_dir = model_dir
