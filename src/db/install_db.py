import os
import docker

def run_postgres_pgvector():
    client = docker.from_env()

    # Pull the PostgreSQL Docker image with pgvector support
    client.images.pull('ankane/pgvector:latest')

    # Create a directory on your local machine to store the PostgreSQL data
    local_data_path = os.getcwd() + '/data'
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

    print('PostgreSQL container is running.')

    # Stop the container (when you're done)
    # container.stop()

def run_redis():
    client = docker.from_env()

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
    run_postgres_pgvector()
    run_redis()
