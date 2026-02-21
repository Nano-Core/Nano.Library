# Nano.Storage.Azure
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)

> Azure File Share storage for Nano applications._

*** 

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)
* [Dependencies](#dependencies)

## Summary
Storage Provider implementation for Microsoft Azure File Shares.  
Read more about storage here: [Nano.Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage)

## Registration
First install the [Nano.Storage.Azure](https://www.nuget.org/packages/Nano.Storage.Azure) NuGet package.  

```powershell
dotnet add package Nano.Storage.Azure;
```

The Azure File Share storage provider must be registered as dependencies.  
```csharp
    .ConfigureServices(x =>
    {
        x.AddNanoStorage<AzureFileshareProvider>();
    })
```

Also, add the following to your Kubernets ```deployment.yaml``` for the Nano application.
```yaml
template:
  spec:
    volumes:
    - name: {service-name}-volume
      azureFile:
        secretName: storage-account-secret
        shareName: {share-name}
        readOnly: false
```

## Dependencies
* https://github.com/Nano-Core/Nano.Azure/tree/master/Nano.Azure.Storage

***