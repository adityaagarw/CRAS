from db.database import *
import tkinter as tk
from tkinter import Scale
import redis

# Connect to Redis
r = redis.Redis(host='127.0.0.1', port=6379)

# Function to update Redis on slider move
def update_redis(key, var):
    value = var.get()
    r.set(key, value)
    print(f"Updated {key} in Redis with value {value}")

# Function to create a slider with a value from Redis
def create_slider(root, key, from_, to_, resolution=0.01, length=300):
    # Fetch the current value from Redis, or use a default if not present
    current_value = r.get(key)
    if current_value is None:
        current_value = (from_ + to_) // 2
    else:
        current_value = float(current_value)

    # Create the slider
    var = tk.DoubleVar(value=current_value)
    slider = Scale(root, from_=from_, to=to_, resolution=resolution, orient='horizontal', label=key, variable=var, length=length)
    slider.pack()
    var.trace_add("write", lambda a, b, c, k=key, v=var: update_redis(k, v))

# Set up the Tkinter window
root = tk.Tk()
root.title("Tune parameters")
root.geometry("400x300")

# Create sliders
create_slider(root, 'param_area_threshold', 0, 50000, 100)
create_slider(root, 'param_threshold', 0, 1, 0.01)
create_slider(root, 'param_yaw_threshold', 0, 180, 1)
create_slider(root, 'param_pitch_threshold', 0, 180, 1)

root.mainloop()