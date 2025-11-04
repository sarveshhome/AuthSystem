# AuthSystem Kubernetes Deployment Guide

## Prerequisites

- Kubernetes cluster
- kubectl CLI tool
- Docker images pushed to registry
- Helm (optional)

## Deployment Steps

### 1. Create Namespace and Switch Context

```bash
# Create namespace
kubectl create namespace authsystem

# Switch to namespace
kubectl config set-context --current --namespace=authsystem


### 2. Create Secrets

# Create secrets
kubectl apply -f k8s/secrets/authsystem-secrets.yaml

# Verify secrets
kubectl get secrets
kubectl describe secret authsystem-secrets

# 3. Create ConfigMaps

# Apply configmap
kubectl apply -f k8s/configmaps/authsystem-config.yaml

# Verify configmap
kubectl get configmaps

# 4. Deploy API

# Deploy API
kubectl apply -f k8s/deployments/api-deployment.yaml
kubectl apply -f k8s/services/api-service.yaml

# Verify API deployment
kubectl get pods -l app=api
kubectl get services api-service