# Nano.Storage.Azure
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)

> Azure file share storage for Nano applications._

*** 

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/tree/master/README.md#nanolibrary)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
* **[Docker Compose](#docker-compose)**
* **[Kuberentes](#kuberentes)**
* **[GitHub Actions](#github-actions)**

## Summary
Storage Provider implementation for Microsoft Azure File Shares.  

A file share from an Azure Storage Account can be mounted into your container, allowing your Nano application to access it as if it were a local drive. This approach 
enables your application to read from and write to the storage directly, while the underlying Azure file share handles persistence and centralized storage.  

No changes to your application code are required and you can interact with the file share using the `IPathProvider` interface.  

> 📖 Learn more about **[Nano Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage/README.md/nanostorage)**.
> 📖 Learn more about **[Nano Azure File Share](https://github.com/Nano-Core/Nano.Azure/tree/master/Nano.Azure.Storage/README.md#nanoazurestorage)**.  

Try it out yourself using the **[Api.Storage.Azure](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Storage.Azure)** or 
**[Console.Storage.Azure](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Storage.Azure)** example.  

## Registration
Install the **[Nano.Storage.Azure](https://www.nuget.org/packages/Nano.Storage.Azure)** NuGet package.

```powershell
dotnet add package Nano.Storage.Azure;
```

Register the `AzureFileshareProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoStorage<AzureFileshareProvider>();
})
...
```

## Configuration
Add the storage configuration.  

REMOVE CREDENTIALS
```json
"Storage": {
  "ShareName": null,
  "Credentials": {
    "Id": null,
    "Secret": null
  },
  "HealthCheck": {
    "UnhealthyStatus": "Unhealthy"
  }
}
```

## Docker Compose
In addition to registering and configuring storage, map a local folder to a container path in in your `docker-compose.yml` so the container can access the storage directory.

```yaml
services:
  {my.service}:
    volumes:
      - {share-name}:/mnt/{share-name}
```

## Kubernetes
New templates are added for storage, depending on type of storage these differs slightly. Common is that a Persistent Volume and Persistent Volume Claim is needed, and then it must
be mapped into your Kubernetes `deployment.yaml` or `cronjob.yaml` (depending on application type) for the Nano application.

```yaml
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

The `configmap.yaml` file stores the `$env:STORAGE_ACCOUNT_NAME` value, which is used by the TCP-based storage health check to validate connectivity to the Azure File Share endpoint. This 
is required if `HealthCheck` is configured.  

## GitHub Actions
Last, the `build-and-deploy.yaml` needs a few additional environmental variables related to Azure storage provder.  

```yaml
env: 
  AZURE_GROUP_BACKUP: ${{ vars.AZURE_BACKUP_RESOURCE_GROUP }}
  AZURE_GROUP_STORAGE: ${{ vars.AZURE_STORAGE_RESOURCE_GROUP }}
  STORAGE_SIZE: 1000
  STORAGE_SHARE_NAME: {share-name}
```

Also, the Azure fileshare needs to be created during deployment if it does not already exist. Add the following step to the `build-and-deploy.yaml`.  

```yaml
- name: Create Fileshare
  shell: pwsh
  run: |
    $env:STORAGE_ACCOUNT_NAME = sudo az storage account list -g $env:AZURE_GROUP_STORAGE --query [0].name -o tsv;

    $env:FILE_SHARE_EXISTS = sudo az storage share-rm exists `
        -g $env:AZURE_GROUP_STORAGE `
        -n $env:STORAGE_SHARE_NAME `
        --storage-account $env:STORAGE_ACCOUNT_NAME `
        --query exists;

    if ($env:FILE_SHARE_EXISTS -eq "false")
    { 
        sudo az storage share-rm create `
            -g $env:AZURE_GROUP_STORAGE `
            -n $env:STORAGE_SHARE_NAME `
            --storage-account $env:STORAGE_ACCOUNT_NAME `
            --access-tier TransactionOptimized `
            --quota $env:STORAGE_SIZE;
              
        if ($LastExitCode -ne 0) 
        { 
            throw "error";
        };

        $env:BACKUP_VAULT_NAME = sudo az backup vault list -g $env:AZURE_GROUP_BACKUP --query [0].name -o tsv;

        sudo az backup protection enable-for-azurefileshare `
            -g $env:AZURE_GROUP_BACKUP `
            -v $env:BACKUP_VAULT_NAME `
            -p $env:STORAGE_ACCOUNT_NAME-backup-policy `
            --storage-account $env:STORAGE_ACCOUNT_NAME `
            --azure-file-share $env:STORAGE_SHARE_NAME;
              
        if ($LastExitCode -ne 0) 
        { 
            throw "error";
        };
    }
```
