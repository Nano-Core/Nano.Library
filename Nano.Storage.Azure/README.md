# Nano.Storage.Azure
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)

> Azure file share storage for Nano applications._

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
Storage Provider implementation for Microsoft Azure File Shares.  

A file share from an Azure Storage Account can be mounted into your container, allowing your Nano application to access it as if it were a local drive. This approach 
enables your application to read from and write to the storage directly, while the underlying Azure file share handles persistence and centralized storage.  

No changes to your application code are required and you can interact with the file share using the `IPathProvider` interface.  

> 📖 Learn more about **[Nano Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage/README.md/nanostorage)**.
> 📖 Learn more about **[Nano Azure File Share](https://github.com/Nano-Core/Nano.Azure/tree/master/Nano.Azure.Storage)**.  

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
Next, the following to your Kubernetes `deployment.yaml` or `cronjob.yaml` (depending on application type) for the Nano application.
You also need to map the `storage-account-secret` that is created alongside the [Nano Azure Storage Account](https://github.com/Nano-Core/Nano.Azure/tree/master/Nano.Azure.Storage) 
in the `deployment.yaml` for fileshare authentication.

```json
spec:
  template:
    spec:
      containers:
        env:
        - name: Storage__Account__Id
          valueFrom:
            secretKeyRef:
              name: storage-account-secret
              key: azurestorageaccountname
        - name: Storage__Account__Secret
          valueFrom:
            secretKeyRef:
              name: storage-account-secret
              key: azurestorageaccountkey
        volumeMounts:
        - name: tmp
          mountPath: /tmp
        - name: %SERVICE_NAME%-volume
          mountPath: /mnt/%STORAGE_SHARE_NAME%
      volumes:
      - name: tmp
        emptyDir: {}
      - name: %SERVICE_NAME%-volume
        azureFile:
          secretName: storage-account-secret
          shareName: %STORAGE_SHARE_NAME%
          readOnly: false
```

> ⚠️ The `storage-account-secret` secret will be reused for all applications using Azure Fileshare as storage provider.  
That makes it easy to change the secret values later on when needed, and just requires the secret values to be updated and re-deployed.  

## GitHub Actions
Last, the `build-and-deploy.yaml` needs a few additional environmental variables related to Azure storage provder.  

```yaml
env: 
  STORAGE_SHARE_NAME: {share-name}
  STORAGE_CREDENTIALS_ID: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_STORAGE_CREDENTIALS_ID || secrets.STAGING_STORAGE_CREDENTIALS_ID }}
  STORAGE_CREDENTIALS_SECRET: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_STORAGE_CREDENTIALS_SECRET || secrets.STAGING_STORAGE_CREDENTIALS_SECRET }}
  STORAGE_SIZE: 1000
```

Also, the Azure fileshare needs to be created during deployment if it does not already exist. Add the following step to the `build-and-deploy.yaml`.  

```yaml
- name: Create Fileshare
  shell: pwsh
  run: |
    $env:EXISTING_FILE_SHARE = sudo az storage share list --account-name $env:STORAGE_CREDENTIALS_ID --account-key $env:STORAGE_CREDENTIALS_SECRET --query "[?contains(name, '$env:STORAGE_SHARE_NAME')].[name]" -o tsv;
    if ([string]::IsNullOrEmpty($env:EXISTING_FILE_SHARE))
    { 
        sudo az storage share create -n $env:STORAGE_SHARE_NAME --account-name $env:STORAGE_CREDENTIALS_ID --account-key $env:STORAGE_CREDENTIALS_SECRET --quota $env:STORAGE_SIZE;
        if ($LastExitCode -ne 0) 
        { 
            throw "error";
        };  
    }
```
