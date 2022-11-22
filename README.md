# Introduction

PubSub example using the DAPR pubsub building block. The example can be configured to use Redis, RabbitMq or Kafka for the pubsub functionality.    

# Running in a local KinD cluster

The default pubsub implementation uses 'RabbitMq'. To use something else up change the PUB_SUB_TYPE variable in the ./k8s/deploy_local.sh script:

```shell
# Can be: Redis, RabbitMq or Kafka
PUB_SUB_TYPE=Redis
```

Make sure you have created a local (KinD) k8s cluster with a private docker registry available at http://localhost:5001/. Then run the following script to deploy the app to the cluster: 

```shell
./k8s/deploy_local.sh
```

The app is deployed to a namespace with the name 'dnw-dapr-pubsub'. To get all the pods in the namespace use:

```shell
kubectl get po -n dnw-dapr-pubsub
```

Set up port-forwarding for the publisher (Dnw.Dapr.PubSub.Checkout):

```shell
kubctl port-forward -n dnw-dapr-pubsub checkout-deployment-xxx 5050:5050
```

Use curl to publishing a message:

```shell
curl http://localhost:5050/publish-message
```

Verify the order-processor (Dnw.Dapr.PubSub.OrderProcessor) received the message:

```shell
kubectl logs -n dnw-dapr-pubsub order-processor-deployment-xxx 
```

You should see 'Subscriber received : Order { OrderId = 20 }' in the console output:

```shell
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://[::]:5051
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /app/
Subscriber received : Order { OrderId = 20 }
```