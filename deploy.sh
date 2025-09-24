#!/bin/bash
# echo "Stopping existing containers..."
# docker-compose down

echo "Building new images..."
docker-compose build

echo "Starting services..."
docker-compose up -d

echo "Running database migrations..."
docker-compose exec api dotnet ef database update

echo "Deployment complete!"
