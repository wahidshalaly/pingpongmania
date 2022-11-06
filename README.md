# Welcome to Dapr experiment
In this experiment, I'll introduce you to basics of Dapr step by step, so it's adviced to review code commit-by-commit.
This experiment is based on a simple example of 3 services each has a single endpoint, namely Play, Ping, and Pong services.
There's only one simple scenario for interaction between them, Play calls Ping, and Ping calls Pong, either via HTTP or Pub/Sub.

To speed development on Kubernetes, [Skaffold](https://skaffold.dev) is utilised to make experimenting faster.

# Setting up a local Kubernetes cluster and deploy the sample
You may setup a local Kubernetes cluster on Minikube, or use Azure Kubernetes Service instead to free your laptop resources
- Install Minikube from [minikube](https://minikube.sigs.k8s.io/docs/start/)
- Enable Minikube addons: ingress
- Install Dapr to local cluster using `dapr init -k`
- Finally, use Skaffold to deploy to cluster `skaffold dev`

# Notes on Skaffold for development mode

## Skaffold initiation
This is required only once before you start using Skaffold.
```
skaffold init --generate-manifests
```
Then select to build images using Dockerfile.
Also, you'll need to do minor modifications to generated manifests that suit your preferences and desired architecture.

---
## Build container images, deploy them, and watch running services
If you're not using a local cluster you'll need to use a container registry, like Azure Container Registry.

Before building any images, you need to set the default repository if you're not using local cluster
```pwsh
$env:SKAFFOLD_DEFAULT_REPO='wshalalylab.azurecr.io/pingpongmania'
gci $env:SKAFFOLD_DEFAULT_REPO
```

Then you can use Skaffold development mode to watch file changes and automatically build new container images and deploy them to the cluster.
```
skaffold dev
```

If you need to run only without watching changes, you can use
```
skaffold run --cleanup --tail
```

To clean up after you finish playing with the sample
```
skaffold delete
```

# Secret Store
Kubernetes secrets store is used for this experiment.
First, you'll need to create a secret in Kubernetes outside of the Skaffold process.
You can store the value in a text file and use `kubectl` to create the secret.
If you decided to test the Pub/Sub service invocation, you'll need to provide
two connection string, one for Azure Service Bus and another for Azure Table Storage.
Write secrets to text files with names indicated below then import then into Kubernetes cluster.

```
kubectl create secret generic storage-account-key --from-file=storageAccountName=..\azurestorage-accountkey.txt -n pingpongmania-dev
kubectl create secret generic asb-connection-string --from-file=connectionString=..\servicebus-connectionstring.txt -n pingpongmania-dev
```

# Tips
- You can use [HTTPie](https://httpie.io) for quick API testing
```
http GET http://localhost:5000/api/ping
```
- To fix the NuGet missing package error `NETSDK1064`, make sure to have a `.dockerignore` file.
It must be located at the root of the context used for Docker Build command.
https://docs.microsoft.com/en-us/dotnet/core/tools/sdk-errors/netsdk1064
https://stackoverflow.com/questions/61167032/error-netsdk1064-package-dnsclient-1-2-0-was-not-found
