import sys
import json
from PySide2.QtWidgets import QApplication, QMainWindow, QWidget, QLabel, QLineEdit, QTextEdit, QPushButton, QVBoxLayout

class MainForm(QWidget):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Store Form")
        self.resize(400, 300)

        self.store_name_label = QLabel("Store Name:")
        self.store_name_input = QLineEdit()

        self.store_location_label = QLabel("Store Location:")
        self.store_location_input = QLineEdit()

        self.entry_cameras_label = QLabel("Number of entry cameras:")
        self.entry_cameras_input = QLineEdit()

        self.billing_cameras_label = QLabel("Number of billing cameras:")
        self.billing_cameras_input = QLineEdit()

        self.exit_cameras_label = QLabel("Number of exit cameras:")
        self.exit_cameras_input = QLineEdit()

        self.entry_camera_list_label = QLabel("Entry camera list:")
        self.entry_camera_list_input = QTextEdit()

        self.billing_camera_list_label = QLabel("Billing camera list:")
        self.billing_camera_list_input = QTextEdit()

        self.exit_camera_list_label = QLabel("Exit camera list:")
        self.exit_camera_list_input = QTextEdit()

        self.submit_button = QPushButton("Submit")
        self.submit_button.clicked.connect(self.submit_form)

        layout = QVBoxLayout()
        layout.addWidget(self.store_name_label)
        layout.addWidget(self.store_name_input)
        layout.addWidget(self.store_location_label)
        layout.addWidget(self.store_location_input)
        layout.addWidget(self.entry_cameras_label)
        layout.addWidget(self.entry_cameras_input)
        layout.addWidget(self.billing_cameras_label)
        layout.addWidget(self.billing_cameras_input)
        layout.addWidget(self.exit_cameras_label)
        layout.addWidget(self.exit_cameras_input)
        layout.addWidget(self.entry_camera_list_label)
        layout.addWidget(self.entry_camera_list_input)
        layout.addWidget(self.billing_camera_list_label)
        layout.addWidget(self.billing_camera_list_input)
        layout.addWidget(self.exit_camera_list_label)
        layout.addWidget(self.exit_camera_list_input)
        layout.addWidget(self.submit_button)

        self.setLayout(layout)

    def submit_form(self):
        store_name = self.store_name_input.text()
        store_location = self.store_location_input.text()
        entry_cameras = int(self.entry_cameras_input.text())
        billing_cameras = int(self.billing_cameras_input.text())
        exit_cameras = int(self.exit_cameras_input.text())
        entry_camera_list = self.entry_camera_list_input.toPlainText().split('\n')
        billing_camera_list = self.billing_camera_list_input.toPlainText().split('\n')
        exit_camera_list = self.exit_camera_list_input.toPlainText().split('\n')

        form_data = {
            "Store Name": store_name,
            "Store Location": store_location,
            "Number of entry cameras": entry_cameras,
            "Number of billing cameras": billing_cameras,
            "Number of exit cameras": exit_cameras,
            "Entry camera list": entry_camera_list,
            "Billing camera list": billing_camera_list,
            "Exit camera list": exit_camera_list
        }

        json_data = json.dumps(form_data)
        print(json_data)  # Print the form data structure to the standard output
        sys.exit()  # Exit the program

def apply_dark_theme():
    dark_theme = """
        QWidget {
            background-color: #333333;
            color: #ffffff;
        }

        QLabel {
            color: #ffffff;
        }

        QLineEdit, QTextEdit {
            background-color: #282828;
            color: #ffffff;
        }

        QPushButton {
            background-color: #505050;
            color: #ffffff;
        }

        /* Add more style rules as needed */
    """
    app.setStyleSheet(dark_theme)

if __name__ == "__main__":
    app = QApplication(sys.argv)
    apply_dark_theme()
    main_window = QMainWindow()
    form = MainForm()
    main_window.setCentralWidget(form)
    main_window.show()
    sys.exit(app.exec_())
