# Tools
- Use `watch` command to keep APIs running and refreshing in the background
> TODO: Need an example of use

- Use [HTTPie](https://httpie.io) for quick API testing
> TODO: Need an example of use

- Fix missing NuGet package error after restore by using a .dockerignore file
https://docs.microsoft.com/en-us/dotnet/core/tools/sdk-errors/netsdk1064
https://stackoverflow.com/questions/61167032/error-netsdk1064-package-dnsclient-1-2-0-was-not-found

------

# Attach ACR to AKS (If AKS is not already connected to ACR)
```
az aks update -n MyK8sLab -g MyLab --attach-acr wshalalylab
```

# Steps to package and deploy solution

---
## Create container image using ACR CLI
```
az acr build -t pingpongmania/ping-service:0.1.0-{{.Run.ID}} -r wshalalylab .
az acr build -t pingpongmania/pong-service:0.1.0-{{.Run.ID}} -r wshalalylab .
az acr build -t pingpongmania/play-service:0.1.0-{{.Run.ID}} -r wshalalylab .
```

## Package helm chart (from ./deploy/helm folder)
```
helm package play-service
helm package ping-service
helm package pong-service
```

## To authenticate to use Azure via Az CLI, AKS via Kubectl, and ACR via Helm CLI(v3)
```
az login --use-device-code

#ACR registry name: wshalalylab
az acr login -n wshalalylab
```
> Note: Docker daemon must be running because it's used to store the access token.

## Push helm chart to remote repository
```
helm push play-service-0.4.0.tgz oci://wshalalylab.azurecr.io/helm
helm push ping-service-0.4.0.tgz oci://wshalalylab.azurecr.io/helm
helm push pong-service-0.4.0.tgz oci://wshalalylab.azurecr.io/helm
```

## Install, upgrade, and uninstall Helm charts

### Install charts
```
helm install pong-service oci://wshalalylab.azurecr.io/helm/pong-service --version 0.4.0
helm install ping-service oci://wshalalylab.azurecr.io/helm/ping-service --version 0.4.0
helm install play-service oci://wshalalylab.azurecr.io/helm/play-service --version 0.4.0
```

### Upgrade installed charts
```
helm upgrade pong-service oci://wshalalylab.azurecr.io/helm/pong-service --version 0.4.0
helm upgrade ping-service oci://wshalalylab.azurecr.io/helm/ping-service --version 0.4.0
helm upgrade play-service oci://wshalalylab.azurecr.io/helm/play-service --version 0.4.0
```

### Uninstall charts
```
helm uninstall play-service
helm uninstall ping-service
helm uninstall pong-service
```


Note: Next deployment can be done using `helm upgrade` instead of `helm install`
