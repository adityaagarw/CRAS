import tkinter as tk
import json

def submit_form():
    store_name = store_name_input.get()
    store_location = store_location_input.get()
    entry_cameras = int(entry_cameras_input.get())
    billing_cameras = int(billing_cameras_input.get())
    exit_cameras = int(exit_cameras_input.get())
    entry_camera_list = entry_camera_list_input.get("1.0", tk.END).split('\n')
    entry_camera_list = [item for item in entry_camera_list if item]
    billing_camera_list = billing_camera_list_input.get("1.0", tk.END).split('\n')
    billing_camera_list = [item for item in billing_camera_list if item]
    exit_camera_list = exit_camera_list_input.get("1.0", tk.END).split('\n')
    exit_camera_list = [item for item in exit_camera_list if item]
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
    root.destroy()  # Exit the program

root = tk.Tk()
root.title("Store Form")
root.geometry("200x600")

store_name_label = tk.Label(root, text="Store Name:")
store_name_input = tk.Entry(root)

store_location_label = tk.Label(root, text="Store Location:")
store_location_input = tk.Entry(root)

entry_cameras_label = tk.Label(root, text="Number of entry cameras:")
entry_cameras_input = tk.Entry(root)

billing_cameras_label = tk.Label(root, text="Number of billing cameras:")
billing_cameras_input = tk.Entry(root)

exit_cameras_label = tk.Label(root, text="Number of exit cameras:")
exit_cameras_input = tk.Entry(root)

entry_camera_list_label = tk.Label(root, text="Entry camera list:")
entry_camera_list_input = tk.Text(root, width=15, height=5)

billing_camera_list_label = tk.Label(root, text="Billing camera list:")
billing_camera_list_input = tk.Text(root, width=15, height=5)

exit_camera_list_label = tk.Label(root, text="Exit camera list:")
exit_camera_list_input = tk.Text(root, width=15, height=5)

submit_button = tk.Button(root, text="Submit", command=submit_form)

store_name_label.pack()
store_name_input.pack()
store_location_label.pack()
store_location_input.pack()
entry_cameras_label.pack()
entry_cameras_input.pack()
billing_cameras_label.pack()
billing_cameras_input.pack()
exit_cameras_label.pack()
exit_cameras_input.pack()
entry_camera_list_label.pack()
entry_camera_list_input.pack()
billing_camera_list_label.pack()
billing_camera_list_input.pack()
exit_camera_list_label.pack()
exit_camera_list_input.pack()
submit_button.pack()

root.mainloop()