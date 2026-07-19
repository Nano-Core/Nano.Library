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
* **[Kubernetes](#kubernetes)**
* **[GitHub Actions](#github-actions)**

## Summary
Storage Provider implementation for Microsoft Azure File Shares.  

A file share from an Azure Storage Account can be mounted into your container, allowing your Nano application to access it as if it were a local drive. This approach 
enables your application to read from and write to the storage directly, while the underlying Azure file share handles persistence and centralized storage.  

No changes to your application code are required and you can interact with the file share using the `IPathProvider` interface.  

> 📖 Learn more about **[Nano Storage](https://github.com/Nano-Core/Nano.Library/blob/master/Nano.Storage/README.md#nanostorage)**.
> 📖 Learn more about **[Nano Azure File Share](https://github.com/Nano-Core/Nano.Azure/blob/master/Nano.Azure.Storage/README.md#nanoazurestorage)**.  

Try it out yourself using the **[Api.Storage.Azure](https://github.com/Nano-Core/Nano.Lessons/blob/master/Api.Storage.Azure)** or 
**[Console.Storage.Azure](https://github.com/Nano-Core/Nano.Lessons/blob/master/Console.Storage.Azure)** example.  

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

This is the actual kubernetes templates.

```yaml
apiVersion: v1
kind: PersistentVolume
metadata:
  name: %SERVICE_NAME%-azurefile-pv-%VOLUME_NAME_SUFFIX%
spec:
  capacity:
    storage: %STORAGE_SIZE%
  accessModes:
    - ReadWriteMany
  persistentVolumeReclaimPolicy: Retain
  storageClassName: azurefile-static
  mountOptions:
    - dir_mode=0777
    - file_mode=0777
    - uid=0
    - gid=0
  claimRef:
    name: %SERVICE_NAME%-azurefile-pvc-%VOLUME_NAME_SUFFIX%
    namespace: %KUBERNETES_NAMESPACE%
  csi:
    driver: file.csi.azure.com
    volumeHandle: %AZURE_GROUP_STORAGE%#%STORAGE_ACCOUNT_NAME%#%STORAGE_SHARE_NAME%-%VOLUME_NAME_SUFFIX%
    volumeAttributes:
      shareName: %STORAGE_SHARE_NAME%
      storageAccount: %STORAGE_ACCOUNT_NAME%
      resourceGroup: %AZURE_GROUP_STORAGE%
      clientID: %IDENTITY_CLIENT_ID%
      mountWithWorkloadIdentityToken: "true"
```

```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: %SERVICE_NAME%-azurefile-pvc-%VOLUME_NAME_SUFFIX%
  namespace: %KUBERNETES_NAMESPACE%
spec:
  accessModes:
    - ReadWriteMany
  storageClassName: azurefile-static
  resources:
    requests:
      storage: %STORAGE_SIZE%
  volumeName: %SERVICE_NAME%-azurefile-pv-%VOLUME_NAME_SUFFIX%
```

And for the `deployment.yaml` or `cronjob.yaml`

```yaml
spec:
  template:
    metadata:
      labels:
        azure.workload.identity/use: "true"
    spec:
      serviceAccountName: %SERVICE_NAME%-service-account
      containers:
        volumeMounts:
        - name: %SERVICE_NAME%-volume
          mountPath: /mnt/%STORAGE_SHARE_NAME%
        - name: tmp
          mountPath: /tmp
      volumes:
      - name: %SERVICE_NAME%-volume
        persistentVolumeClaim:
          claimName: %SERVICE_NAME%-azurefile-pvc-%VOLUME_NAME_SUFFIX%
      - name: tmp
        emptyDir: {}
```

## GitHub Actions
Last, the `build-and-deploy.yaml` needs a few additional environmental variables related to Azure storage provder.  

```yaml
env: 
  AZURE_GROUP_BACKUP: ${{ vars.AZURE_RESOURCE_GROUP_BACKUP }}
  AZURE_GROUP_STORAGE: ${{ vars.AZURE_RESOURCE_GROUP_STORAGE }}
  STORAGE_SIZE: 1000
  STORAGE_SHARE_NAME: nano-storage-azure
```

Also, the Azure fileshare needs to be created during deployment if it does not already exist. Add the following step to the `build-and-deploy.yaml`.  

```yaml
- name: Create Fileshare
  shell: pwsh
  run: |
    $env:STORAGE_ACCOUNT_NAME = az storage account list -g $env:AZURE_GROUP_STORAGE --query [0].name -o tsv;

    $env:FILE_SHARE_EXISTS = az storage share-rm exists `
        -g $env:AZURE_GROUP_STORAGE `
        -n $env:STORAGE_SHARE_NAME `
        --storage-account $env:STORAGE_ACCOUNT_NAME `
        --query exists;

    if ($env:FILE_SHARE_EXISTS -eq "false")
    { 
        az storage share-rm create `
            -g $env:AZURE_GROUP_STORAGE `
            -n $env:STORAGE_SHARE_NAME `
            --storage-account $env:STORAGE_ACCOUNT_NAME `
            --access-tier TransactionOptimized `
            --quota $env:STORAGE_SIZE;
    }
    else
    {
        az storage share-rm update `
            -g $env:AZURE_GROUP_STORAGE `
            -n $env:STORAGE_SHARE_NAME `
            --storage-account $env:STORAGE_ACCOUNT_NAME `
            --access-tier TransactionOptimized `
            --quota $env:STORAGE_SIZE;
    }

    if ($LastExitCode -ne 0) 
    { 
        throw "error";
    };

    $env:BACKUP_VAULT_NAME = az backup vault list -g $env:AZURE_GROUP_BACKUP --query [0].name -o tsv;

    az backup protection enable-for-azurefileshare `
        -g $env:AZURE_GROUP_BACKUP `
        -v $env:BACKUP_VAULT_NAME `
        -p $env:STORAGE_ACCOUNT_NAME-fileshare-backup-policy `
        --storage-account $env:STORAGE_ACCOUNT_NAME `
        --azure-file-share $env:STORAGE_SHARE_NAME;

    if ($LastExitCode -ne 0) 
    { 
        throw "error";
    };
```

...and includes a step to create a managed identity and federated credentials for authenticating with the storage account.  

```
- name: Managed Identity & Federated Credentials
  shell: pwsh
  run: |
    $env:IDENTITY_NAME = $env:SERVICE_NAME + "-identity";
    $env:IDENTITY_PRINCIPAL_ID = az identity show -g $env:AZURE_GROUP_KUBERNETES -n $env:IDENTITY_NAME --query principalId -o tsv;
    $env:KUBERNETES_ISSUER_URL = az aks list -g $env:AZURE_GROUP_KUBERNETES --query [0].['oidcIssuerProfile.issuerUrl'] -o tsv;
    $env:STORAGE_ACCOUNT_ID = az storage account list -g $env:AZURE_GROUP_STORAGE --query [0].id -o tsv;

    if (-not $env:IDENTITY_PRINCIPAL_ID)
    {
        az identity create `
            -g $env:AZURE_GROUP_KUBERNETES `
            -n $env:IDENTITY_NAME;

        if ($LastExitCode -ne 0)
        {
            throw "error";
        };

        $env:IDENTITY_PRINCIPAL_ID = az identity show -g $env:AZURE_GROUP_KUBERNETES -n $env:IDENTITY_NAME --query principalId -o tsv;
    }

    az role assignment create `
        --assignee-object-id $env:IDENTITY_PRINCIPAL_ID `
        --assignee-principal-type ServicePrincipal `
        --role "Storage File Data SMB MI Admin" `
        --scope $env:STORAGE_ACCOUNT_ID

    if ($LastExitCode -ne 0)
    {
        throw "error";
    };

    az identity federated-credential create `
        --name $env:SERVICE_NAME-credentials `
        --resource-group $env:AZURE_GROUP_KUBERNETES `
        --identity-name $env:IDENTITY_NAME `
        --issuer $env:KUBERNETES_ISSUER_URL `
        --subject "system:serviceaccount:${env:KUBERNETES_NAMESPACE}:${env:SERVICE_NAME}-service-account" `
        --audience api://AzureADTokenExchange;

    if ($LastExitCode -ne 0)
    {
        throw "error";
    };

    echo "IDENTITY_NAME=$env:IDENTITY_NAME" >> $env:GITHUB_ENV;
```

Last, during the Kubernetes deployment step, before any resources are applied, environmental variables required for the new `stoerage-pv.yaml` and `stoerage-pvc.yaml` must be set.

```powershell
$env:IDENTITY_CLIENT_ID = az identity show -g $env:AZURE_GROUP_KUBERNETES -n $env:IDENTITY_NAME --query clientId -o tsv;
$env:VOLUME_NAME_SUFFIX = $env:IDENTITY_CLIENT_ID.Substring(0, 5);
```

The deployment commands have been updated to apply the new Kubernetes storage templates.  
