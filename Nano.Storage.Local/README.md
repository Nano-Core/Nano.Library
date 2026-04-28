# Nano.Storage.Local
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)

> Local file share storage for Nano applications._

*** 

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/blob/master/README.md#nanolibrary)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
* **[Docker Compose](#docker-compose)**
* **[Kuberentes](#kuberentes)**
* **[GitHub Actions](#github-actions)**

## Summary
Storage provider implementation for local file shares.  

This provider is intended for mapping a Kubernetes persistent volume as a local file system. Registering it with Nano gives you access to the `IPathProvider` interface.

> 📖 Learn more about **[Nano Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage/README.md/nanostorage)**.

Try it out yourself using the **[Api.Storage.Local](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Storage.Local)** or 
**[Console.Storage.Local](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Storage.Local)** example.  

## Registration
Install the **[Nano.Storage.Local](https://www.nuget.org/packages/Nano.Storage.Local)** NuGet package.

```powershell
dotnet add package Nano.Storage.Local;
```

Register the `LocalFileShareProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoStorage<LocalFileShareProvider>();
})
...
```

## Configuration
Add the storage configuration.  

```json
"Storage": {
  "ShareName": null,
  "HealthCheck": {
    "UnhealthyStatus": "Unhealthy"
  }
}
```

## Docker Compose
In addition to registering and configuring storage, map a local folder to a container path in your `docker-compose.yml` to give the container access to the storage directory:

```yaml
services:
  {my.service}:
    volumes:
      - {share-name}:/mnt/{share-name}
```

## Kubernetes
Next, two additional Kubernetes templates have been added to create and manage the storage, `storage-storageclass.yaml` and `storage-pvc.yaml`.  

Also, as the container in Kubernetes is read-only, the following must also be added to your Kubernetes `deployment.yaml` or `cronjob.yaml` (depending on application type) 
to ensure the file share is writable.  

```json
spec:
  template:
    spec:
      containers:
        volumeMounts:
        - name: %SERVICE_NAME%-volume
          mountPath: /mnt/%STORAGE_SHARE_NAME%
        - name: tmp
          mountPath: /tmp
      volumes:
      - name: %SERVICE_NAME%-volume
        persistentVolumeClaim:
          claimName: %SERVICE_NAME%-pvc
      - name: tmp
        emptyDir: {}
```

## GitHub Actions
Last, The `build-and-deploy.yaml` needs additional environmental variables related to local storage provder.  

```yaml
env:
  STORAGE_SHARE_NAME: {share-name}
  STORAGE_SIZE: {size}
```
