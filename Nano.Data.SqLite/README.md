# Nano.Data.SqLite
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.SqLite.svg)](https://www.nuget.org/packages/Nano.Data.SqLite/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.SqLite.svg)](https://www.nuget.org/packages/Nano.Data.SqLite/)

> SqLite data provider for Nano applications._

*** 

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/blob/master/README.md#nanolibrary)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
* **[Docker-compose](#docker-compose)**
* **[Kubernetes](#kubernetes)**
* **[GitHub Actions](#github-actions)**

## Summary
Data Provider implementation for SqLite data access.  

> ⚠️ SqLite does not natively support spatial types, and `mod_spatialite` is not reliable.

> 📖 Learn more about **[Nano Data](https://github.com/Nano-Core/Nano.Library/blob/master/Nano.Data/README.md#nanodata)**.

Try it out yourself using the **[Api.Data.SqLite](https://github.com/Nano-Core/Nano.Lessons/blob/master/Api.Data.SqLite)**, or 
**[Console.Data.SqLite](https://github.com/Nano-Core/Nano.Lessons/blob/master/Console.Data.SqLite)** example.  

## Registration
Install the **[Nano.Data.SqLite](https://www.nuget.org/packages/Nano.Data.SqLite)** NuGet package.  

```powershell
dotnet add package Nano.Data.SqLite;
```

Register the `SqLiteProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
    .ConfigureServices(services =>
    {
        services
            .AddNanoData<SqLiteProvider, TContext>();
    })
```

A `BaseDbContext` and `BaseDbContextFactory` must also be implemented and used as `TContext`, and also an initial migration added.

```powershell
dotnet ef migrations add Initial --project {project-name}
```

## Configuration
Add the data configuration to `appsettings.json`.  

```json
"Data": {
  "BatchSize": 25,
  "BulkBatchSize": 500,
  "BulkBatchDelay": 1000,
  "QueryRetryCount": 0,
  "UseLazyLoading": false,
  "StartupAction": "None",
  "UseSensitiveDataLogging": false,
  "QuerySplittingBehavior": "SingleQuery",
  "DefaultCollation": null,
  "ConnectionString": "Data Source=/mnt/data/nanoDb.sqlite",
  "Repository": {
    "UseAutoSave": false,
    "QueryIncludeDepth": 4
  },
  "Identity": null,
  "ConnectionPool": null,
  "HealthCheck": {
    "UnhealthyStatus": "Unhealthy"
  }
}
```

...and `appsettings.Development.json`

```json
"Data": {
  "StartupAction": "Migrate"
}
```

## Docker Compose
Add SqLite as a service dependency in `docker-compose.yml`.  

```yaml
services:
  {service-name}:
    volumes:
      - ./bin/data:/mnt/data
```

## Kubernetes
Add an additional Kubernetes template, `data-storageclass.yaml`, for dynamically provisioning the disk backing the SqLite database file.

> ⚠️ Single-attach (`ReadWriteOnce`) — fine for a `CronJob`, but a multi-replica API/Web app needs `stateful-set.yaml` (`StatefulSet` + `volumeClaimTemplates`), not `deployment.yaml`, so 
each replica gets its own (unshared) database file. For one shared database, use a network provider such as **[Nano.Data.MySql](https://github.com/Nano-Core/Nano.Library/blob/master/Nano.Data.MySql/README.md#nanodatamysql)**.

For a `CronJob`, or a single-replica Deployment, mount the disk via a static `data-pvc.yaml` `PersistentVolumeClaim` as before.

```json
spec:
  template:
    spec:
      containers:
        volumeMounts:
        - name: %SERVICE_NAME%-volume
          mountPath: /mnt/data
      volumes:
      - name: %SERVICE_NAME%-volume
        persistentVolumeClaim:
          claimName: %SERVICE_NAME%-data-pvc
```

For a multi-replica application, use `stateful-set.yaml` instead, `volumeClaimTemplates` replaces the static `PersistentVolumeClaim` file entirely, and a governing headless service 
(`service-headless.yaml`, `clusterIP: None`) is required for the `StatefulSet`'s `serviceName` field.

```yaml
spec:
  serviceName: %SERVICE_NAME%-stateful-headless
  template:
    spec:
      containers:
        volumeMounts:
        - name: %SERVICE_NAME%-volume
          mountPath: /mnt/data
  volumeClaimTemplates:
  - metadata:
      name: %SERVICE_NAME%-volume
    spec:
      accessModes:
        - ReadWriteOnce
      storageClassName: %SERVICE_NAME%-data-storage-class
      resources:
        requests:
          storage: %SQL_SIZE%Gi
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

The application's `autoscaler.yaml` (`HorizontalPodAutoscaler`, always present on `NanoApiApplication`/`NanoWebApplication`) must also have its `scaleTargetRef.kind` changed from 
`Deployment` to `StatefulSet`.

```yaml
spec:
  scaleTargetRef:
    kind: StatefulSet
```

## GitHub Actions
Add the following environment variables to the `build-and-deploy.yml`.  

```yaml
env:
  SQL_SIZE: 10
```

Deployment commands must also be updated to apply each of the new Kubernetes templates.  
