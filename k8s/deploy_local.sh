#!/bin/bash

# Define variables
RELEASE_NAME=dnw-dapr-pubsub

# Can be: Redis, RabbitMq or Kafka
PUB_SUB_TYPE=RabbitMq

# Preload 3rd party images
docker pull mcr.microsoft.com/dotnet/sdk:6.0-alpine
kind load docker-image mcr.microsoft.com/dotnet/sdk:6.0-alpine

docker pull mcr.microsoft.com/dotnet/aspnet:6.0-alpine
kind load docker-image mcr.microsoft.com/dotnet/aspnet:6.0-alpine

if [ $PUB_SUB_TYPE = "Redis" ]
then
  docker pull arm64v8/redis:latest
  kind load docker-image arm64v8/redis:latest
fi

if [ $PUB_SUB_TYPE = "RabbitMq" ]
then
  docker pull rabbitmq:3.11-management
  kind load docker-image rabbitmq:3.11-management
fi

if [ $PUB_SUB_TYPE = "Kafka" ]
then
  docker pull confluentinc/cp-zookeeper:7.3.0
  kind load docker-image confluentinc/cp-zookeeper:7.3.0
  
  docker pull confluentinc/cp-kafka:7.3.0
  kind load docker-image confluentinc/cp-kafka:7.3.0
fi

# Build images, tag them and push them to the local registry
TAG="localhost:5001/$RELEASE_NAME-checkout:latest"
echo "TAG=$TAG"
docker build -t $TAG -f ../Dnw.Dapr.PubSub.Checkout/Dockerfile ../Dnw.Dapr.PubSub.Checkout
docker push $TAG

TAG="localhost:5001/$RELEASE_NAME-order-processor:latest"
echo "TAG=$TAG"
docker build -t $TAG -f ../Dnw.Dapr.PubSub.OrderProcessor/Dockerfile ../Dnw.Dapr.PubSub.OrderProcessor
docker push $TAG

# Install app into k8s cluster
helm upgrade "$RELEASE_NAME" ./helm \
  --install \
  --namespace "$RELEASE_NAME" \
  --create-namespace \
  --set PubSubType="$PUB_SUB_TYPE"
  
# Restart the deployments
kubectl rollout restart "deployment/checkout-deployment" -n "$RELEASE_NAME"
kubectl rollout restart "deployment/order-processor-deployment" -n "$RELEASE_NAME"