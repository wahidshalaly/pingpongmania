# Welcome to Dapr experiment repository
In this experiment, we'll introduce you to basics of Dapr step by step.
This experiment is based on a simple example of 3 services each has a single endpoint, namely Play, Ping, and Pong services.
There's only one simple scenario for interaction between them, Play service calls Ping service, and Ping service calls Pong service.

This experiment is deployed on Azure Kubernetes Service (AKS) and Docker images are stored in Azure Container Registry (ACR).

To speed development on Kubernetes, [Skaffold](https://skaffold.dev) is utilised to make experimenting faster.

## Attach ACR to AKS (If AKS is not already connected to ACR)
```
az aks update -n MyK8sLab -g MyLab --attach-acr wshalalylab
```

## To authenticate to use Azure via Az CLI, AKS via Kubectl, and ACR via Helm CLI(v3)
Sign in to Azure via browser code flow - this is not required frequently
```
az login --use-device-code
```
Sign in to ACR (wshalalylab.azurecr.io) - this will need to be refreshed every few hours.
```
az acr login -n wshalalylab
```
> Note: Docker daemon must be running because it's used to store the access token.

# Skaffold for development mode

## Skaffold initiation
This is required only once before you start using Skaffold.
```
skaffold init --generate-manifests
```
Then select to build images using Dockerfile.
Also, you'll need to do minor modifications to generated manifests that suit your preferences and desired architecture.

---
## Build container images, deploy them, and watch running services
Before building any images, you need to set the default repository for pushing images
```
set SKAFFOLD_DEFAULT_REPO=wshalalylab.azurecr.io/pingpongmania
```
Then you can use Skaffold development mode to watch file changes and automatically build new container images and deploy them to the cluster.
```
skaffold dev
```

If you need to run only without watching changes, you can use 
```
skaffold run --cleanup --tail
```

To clean up after you finish
```
skaffold delete
```

# Secret Store
I'll use Kubernetes secrets as for this experiment.
First, you'll need to create a secret in Kubernetes outside of the Skaffold process.
You can store the value in a text file and use `kubectl` to create the secret.
```
kubectl create secret generic asb-connection-string --from-file=connectionString=.\servicebus-connectionstring.txt -n pingpongmania-dev
```

# Tips
- During local development, you can use `watch` command to keep APIs running and refreshing in the background
```
dotnet watch run --project src\PingService\PingService.csproj
```
But when you do not need this when you're using `skaffold dev`
- You can use [HTTPie](https://httpie.io) for quick API testing
```
http GET http://localhost:5000/api/ping
```

- To fix the NuGet missing package error `NETSDK1064`, make sure to have a `.dockerignore` file.
It must be located at the root of the context used for Docker Build command.
https://docs.microsoft.com/en-us/dotnet/core/tools/sdk-errors/netsdk1064
https://stackoverflow.com/questions/61167032/error-netsdk1064-package-dnsclient-1-2-0-was-not-found
