# Nano.Storage.Local
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)

> Local file share storage for Nano applications._

*** 

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)

## Summary
Storage provider implementation for local file shares.  

This provider is intended for convenience when using a local file system. Registering it with Nano gives you access to the `IPathProvider` interface.

> ⚠️ In cloud environments, the file share path is expected to already exist. No external drives are mapped to the container or machine.

> 📖 Learn more about **[Nano Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage)**.

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

In addition to registering storage, map a local folder to a container path in your Docker setup to give the container access to the storage directory:

```yaml
services:
  {my.service}:
    volumes:
      - {share-name}:/mnt/{share-name}
```

As the container in Kubernetes is read-only, the following must also be added to your Kubernetes `deployment.yaml` to ensure the file share is writable.  

```yaml
template
  spec
    volumes:
    - name: {share-name}
      emptyDir: {}
```