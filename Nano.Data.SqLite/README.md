# Nano.Data.SqLite
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.SqLite.svg)](https://www.nuget.org/packages/Nano.Data.SqLite/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.SqLite.svg)](https://www.nuget.org/packages/Nano.Data.SqLite/)

> SqLite data provider for Nano applications._

*** 

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/tree/master/README.md#nanolibrary)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
* **[Docker-compose](#docker-compose)**
* **[Kubernetes](#kubernetes)**
* **[GitHub Actions](#github-actions)**

## Summary
Data Provider implementation for SqLite data access.  

> ⚠️ SqLite does not natively support spatial types, and `mod_spatialite` is not reliable.

> 📖 Learn more about **[Nano Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#nanodata)**.

Try it out yourself using the **[Api.Data.SqLite](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.SqLite)**, or 
**[Console.Data.SqLite](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Data.SqLite)** example.  

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
  "ConnectionString": "Data Source=/data/nanoDb.sqlite",
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
  "UseMigrateDatabase": true
}
```

## Docker Compose
Add SqLite as a service dependency in `docker-compose.yml`.  

```yaml
services:
  {service-name}:
    volumes:
      - ./bin/data:/data
```

## Kubernetes
Add two additional kubernetes templates, `storageclass.yaml` and `pvc.yaml`, for dynamically manage and creating the disk for the SqLite database.

Also, update `deployment.yaml` adding the volumes and volume mounts.  

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
          claimName: %SERVICE_NAME%-pvc
```

## GitHub Actions
Add the following environment variables to the `buid-and-deply.yml`.  

```yaml
env:
  DATA_NAME: nanoDb
  DATA_SIZE: 10Gi
  DATA_CONNECTIONSTRING: "Data Source=/data/{{ env.nanoDb }}.sqlite"
```

Additionally, this step has been added to ensure database migrations are applied.  

```yaml
- name: Database Migration
  shell: pwsh
  run: |
    dotnet ef database update `
      --no-build `
      --startup-project $env:APP_NAME `
      --connection "$env:DATA_MIGRATION_CONNECTIONSTRING" `;

    if ($LastExitCode -ne 0)
    { 
        throw "error";
    };
```

Deployment commands must also be updated to apply each of the new Kubernetes templates.  

```powershell
Get-Content .kubernetes/{resource-name}.yaml `
    | foreach { [Environment]::ExpandEnvironmentVariables($_) } `
    | Set-Content .kubernetes/{resource-name}.tmp.yaml;

sudo kubectl apply -f .kubernetes/{resource-name}.tmp.yaml;
```
