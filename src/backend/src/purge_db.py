import docker
import shutil
import os

# WARNING: THIS SCRIPT PURGES ALL DATABASES
# This script should only be used for development purposes
# It will delete all data in the PostgreSQL and Redis databases
# It will also delete the PostgreSQL and Redis Docker containers
# It will also delete the PostgreSQL data directory
# Run this script only when orchestrator is not running

client = docker.from_env()

def get_python_command():
    python_commands = ['python3', 'python', 'py']

    for command in python_commands:
        if shutil.which(command) is not None:
            return command

    raise EnvironmentError('Python command not found in your system')

def start_docker():
    os.system(get_python_command() + " one_time_setup/install_db.py")
    return 0

def delete_docker_container(container_name):
    try:
        container = client.containers.get(container_name)
        container.stop()
        container.remove()
        print(f"Container '{container_name}' deleted successfully.")
    except docker.errors.NotFound:
        print(f"Container '{container_name}' not found.")
    except docker.errors.ContainerError as e:
        print(f"Error with container '{container_name}': {e}")

def delete_directory(directory_path):
    if os.path.exists(directory_path):
        shutil.rmtree(directory_path)
        print(f"Directory '{directory_path}' deleted successfully.")
    else:
        print(f"Directory '{directory_path}' does not exist.")

if __name__ == "__main__":
    delete_docker_container("postgres-pgvector")
    delete_docker_container("redis-server")
    delete_directory("db/data")
    start_docker()