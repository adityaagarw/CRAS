# Test pgsql back by creating a test container and restoring the backup
import docker
import os
import time

def run_postgres_container(container_name, db_data_path, db_port):
    client = docker.from_env()
    if not client:
        client.start()

    existing_containers = client.containers.list(all=True, filters={'name': container_name})
    if existing_containers:
        container = existing_containers[0]
        if container.status != 'running':
            print(f"Starting existing {container_name}")
            container.start()
            time.sleep(60)
        else:
            print(f'{container_name} is already running.')
            return
    else:
        client.images.pull('ankane/pgvector:latest')

        volumes = {db_data_path: {'bind': '/var/lib/postgresql/data', 'mode': 'rw'}}

        container = client.containers.run(
            'ankane/pgvector:latest',
            detach=True,
            ports={'5435': db_port},
            environment={
                'POSTGRES_USER': 'cras_admin',
                'POSTGRES_PASSWORD': 'admin',
                'POSTGRES_DB': 'localdb'
            },
            volumes=volumes,
            name=container_name
        )

        time.sleep(20)

    print(f'{container_name} is running.')

def run_test_postgres_container():
    test_data_path = os.getcwd() + '/test_data'
    test_container_name = 'postgres-pgvector-test'
    test_db_port = '5435'  # Different port for the test container
    run_postgres_container(test_container_name, test_data_path, test_db_port)

# Run the test PostgreSQL container
run_test_postgres_container()

# Here you can add code to restore the backup to the test container and verify it
# Example: docker exec postgres-pgvector-test pg_restore ...
