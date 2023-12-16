import os
import docker
import time
import subprocess

def start_docker():
    # Check if Docker is running
    try:
        docker_version = subprocess.check_output(['docker', '--version']).decode('utf-8').strip()
        print('Docker is already running:', docker_version)
    except (FileNotFoundError, subprocess.CalledProcessError):
        # Start Docker if it's not running
        subprocess.call('start docker', shell=True)
        print('Starting Docker...')

def run_postgres_pgvector():
    client = docker.from_env()
    if not client:
        client.start()
    # Check if the PostgreSQL container exists
    existing_containers = client.containers.list(all=True, filters={'name': 'postgres-pgvector'})
    if existing_containers:
        container = existing_containers[0]
        if container.status != 'running':
            # Start the container if it's not running
            print("Starting existing postgres container")
            container.start()
            time.sleep(60)
        else:
            print('PostgreSQL container is already running.')
            return
    else:
        # Pull the PostgreSQL Docker image with pgvector support
        client.images.pull('ankane/pgvector:latest')

        # Create a directory on your local machine to store the PostgreSQL data
        local_data_path = os.getcwd() + '/db/data'
        volumes = {local_data_path: {'bind': '/var/lib/postgresql/data', 'mode': 'rw'}}

        # Start the PostgreSQL container
        container = client.containers.run(
            'ankane/pgvector:latest',
            detach=True,
            ports={'5432': '5432'},
            environment={
                'POSTGRES_USER': 'cras_admin',
                'POSTGRES_PASSWORD': 'admin',
                'POSTGRES_DB': 'localdb'
            },
            volumes=volumes,
            name='postgres-pgvector'
        )

        time.sleep(20)

    print('PostgreSQL container is running.')

    # Stop the container (when you're done)
    # container.stop()

def run_redis():
    client = docker.from_env()

    # Check if the Redis container exists
    existing_containers = client.containers.list(all=True, filters={'name': 'redis-server'})
    if existing_containers:
        container = existing_containers[0]
        if container.status != 'running':
            # Start the container if it's not running
            print("Starting existing redis container")
            container.start()
        else:
            print('Redis container is already running.')
            return
    else:
        # Pull the Redis Docker image
        client.images.pull('redis:latest')

        # Start the Redis container
        container = client.containers.run(
            'redis:latest',
            detach=True,
            ports={'6379': '6379'},
            name='redis-server'
        )

    print('Redis container is running.')

    # Stop the container (when you're done)
    # container.stop()

if __name__ == "__main__":
    start_docker()
    run_postgres_pgvector()
    run_redis()