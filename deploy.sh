#!/bin/bash

# Function to check if Docker is running
check_docker() {
    if ! docker info >/dev/null 2>&1; then
        echo "Docker does not seem to be running, please start Docker first"
        exit 1
    fi
}

# Function to check if a container is healthy
check_health() {
    local container_name=$1
    local max_attempts=30
    local attempt=1

    echo "Waiting for $container_name to be healthy..."
    while [ $attempt -le $max_attempts ]; do
        if [ "$(docker inspect -f {{.State.Health.Status}} $container_name)" == "healthy" ]; then
            echo "$container_name is healthy!"
            return 0
        fi
        echo "Attempt $attempt/$max_attempts: $container_name is not healthy yet..."
        sleep 2
        attempt=$((attempt + 1))
    done

    echo "Error: $container_name failed to become healthy within the allocated time"
    return 1
}

# Main deployment script
echo "Checking if Docker is running..."
check_docker

echo "Stopping any existing containers..."
docker compose down

echo "Building new images..."
docker compose build

echo "Starting services..."
docker compose up -d

# Get the container name for the API service
API_CONTAINER=$(docker compose ps -q api)

echo "Waiting for SQL Server to be ready..."
if ! check_health "$(docker compose ps -q sql-server)"; then
    echo "SQL Server failed to start properly"
    exit 1
fi

echo "Running database migrations..."
docker compose exec api dotnet ef database update

echo "Checking API health..."
max_attempts=30
attempt=1
while [ $attempt -le $max_attempts ]; do
    if curl -f http://localhost:5062/health >/dev/null 2>&1; then
        echo "API is healthy!"
        break
    fi
    echo "Attempt $attempt/$max_attempts: API is not ready yet..."
    sleep 2
    attempt=$((attempt + 1))
done

if [ $attempt -gt $max_attempts ]; then
    echo "Error: API failed to become healthy within the allocated time"
    exit 1
fi

echo "Deployment complete! Services are running at:"
echo "API: http://localhost:5062"
echo "SQL Server: localhost:1433"
