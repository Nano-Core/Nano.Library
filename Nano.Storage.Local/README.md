# Nano.Storage.Local
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.Local.svg)](https://www.nuget.org/packages/Nano.Storage.Local/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Storage.Local.svg)](https://www.nuget.org/packages/Nano.Storage.Local/)

> Local file share storage for Nano applications.

*** 

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/blob/master/README.md#nanolibrary)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
* **[Docker Compose](#docker-compose)**
* **[Kubernetes](#kubernetes)**
* **[GitHub Actions](#github-actions)**

## Summary
Storage provider implementation for local file shares.  

This provider is intended for mapping a Kubernetes persistent volume as a local file system. Registering it with Nano gives you access to the `IPathProvider` interface.

> 📖 Learn more about **[Nano Storage](https://github.com/Nano-Core/Nano.Library/blob/master/Nano.Storage/README.md#nanostorage)**.

Try it out yourself using the **[Api.Storage.Local](https://github.com/Nano-Core/Nano.Lessons/blob/master/Api.Storage.Local)** or 
**[Console.Storage.Local](https://github.com/Nano-Core/Nano.Lessons/blob/master/Console.Storage.Local)** example.  

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
Next, an additional Kubernetes template has been added to create and manage the storage class, `storage-storageclass.yaml`.  

> ⚠️ Single-attach (`ReadWriteOnce`) is fine for a `CronJob`, but a multi-replica API/Web app needs `stateful-set.yaml` (`StatefulSet` + `volumeClaimTemplates`), not `deployment.yaml`, so
each replica gets its own (unshared) disk. For a shared volume, use **[Nano.Storage.Azure](https://github.com/Nano-Core/Nano.Library/blob/master/Nano.Storage.Azure/README.md#nanostorageazure)** instead.

For a `CronJob`, or a single-replica Deployment, mount the volume via a static `storage-pvc.yaml` `PersistentVolumeClaim` as before.

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

For a multi-replica `NanoApiApplication`/`NanoWebApplication`, use `stateful-set.yaml` instead, `volumeClaimTemplates` replaces the static `PersistentVolumeClaim` file entirely, and a 
governing headless service (`service-headless.yaml`, `clusterIP: None`) is required for the `StatefulSet`'s `serviceName` field.

```yaml
spec:
  serviceName: %SERVICE_NAME%-stateful-headless
  template:
    spec:
      containers:
        volumeMounts:
        - name: %SERVICE_NAME%-volume
          mountPath: /mnt/%STORAGE_SHARE_NAME%
        - name: tmp
          mountPath: /tmp
      volumes:
      - name: tmp
        emptyDir: {}
  volumeClaimTemplates:
  - metadata:
      name: %SERVICE_NAME%-volume
    spec:
      accessModes:
        - ReadWriteOnce
      storageClassName: %SERVICE_NAME%-storage-class
      resources:
        requests:
          storage: %STORAGE_SIZE%Gi
```

```yaml
apiVersion: v1
kind: Service
metadata:
  name: %SERVICE_NAME%-stateful-headless
  namespace: %KUBERNETES_NAMESPACE%
spec:
  clusterIP: None
  ports:
  - name: http
    port: 8080
  selector:
    app: %SERVICE_NAME%
```

The application's `autoscaler.yaml` (`HorizontalPodAutoscaler`, always present on `NanoApiApplication`/`NanoWebApplication`) must also have its `scaleTargetRef.kind` changed 
from `Deployment` to `StatefulSet`.

```yaml
spec:
  scaleTargetRef:
    kind: StatefulSet
```

## GitHub Actions
Last, The `build-and-deploy.yaml` needs additional environmental variables related to local storage provder.  

```yaml
env:
  STORAGE_SIZE: {size-in-gb}
  STORAGE_SHARE_NAME: {share-name}
```

And the new Kubernetes templetes must be applied.