# Nano.Storage
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.svg)](https://www.nuget.org/packages/Nano.Storage/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Storage.svg)](https://www.nuget.org/packages/Nano.Storage/)

> _Storage provider common implementations for Nano applications._

> ⚠️ This NuGet is transitive and included in other Nano Packages, and is not meant to be included directly.

***

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)
* [Configuration](#configuration)
* [Storage Providers](#storage-providers)

## Summary
Add a flexible, provider-agnostic file storage layer to your Nano applications.

To connect a filshare, register an `IStorageProvider`. Each provider handles its own setup, connectivity, and optional health checks, 
while Nano exposes a consistent interface to your application.  

Once a storage provider is registered, Nano automatically registers an `IPathProvider`, which can be injected anywhere in your application. 
This gives easy access to the storage root fileshare directory as well as the temporary (`tmp`) directory.  

## Registration
To use Nano.Storage, register a storage provider as a service dependency in your application.  
Use the `AddNanoStorage<TProvider>()` extension method, where `TProvider` is your chosen `IStorageProvider` implementation:

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoStorage<TProvider>();
})
...
```

In addition to registering storage, map a local folder to a container path in your Docker setup so the container can access the storage directory.

```yaml
services:
  {my.service}:
    volumes:
      - {share-name}:/mnt/{share-name}
```

When deploying Nano applications to Kubernetes, ensure your `deployment.yaml` maps the shared storage directory. The exact configuration depends on 
the storage provider. Check out supported **[Storage Providers](#storage-providers)**.  

> ⚠️ Optionally, map a writable `tmp` directory for temporary files.  
This allows the main container to remain immutable while supporting temporary data.  

```yaml
template
  spec
    volumes:
    - name: tmp
      emptyDir: {}
```

## Configuration
The ```Storage``` section in the configuration defines the storage provider and related settings used by the application.

| Setting                         | Type   | Default     | Description                                                                                                                              |
| ------------------------------- | ------ | ----------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
|  `ShareName`                    | string | null        | The logical container, share, or bucket name used for file storage.                                                                      |
|  `Account`                      | object | null        | The account of the storage provider.                                                                                                     |
|  `Account.Id`                   | string | null        | The account id, username or tenant identifier used to authenticate with the storage provider.                                            |
|  `Account.Secret`               | string | null        | The secret, key, password or credential used to authenticate with the storage provider.                                                  |
|  `HealthCheck`                  | object | null        | Storage health check. _Only relevant for `NanoApiApplication` and `NanoWebApplication`_..                                                |
|  `HealthCheck.UnhealthyStatus`  | enum   | Unhealthy   | The health status reported when the storage provider is unavailable. _Only relevant for `NanoApiApplication` and `NanoWebApplication`_.  |

```json
"Storage": {
  "AccountName": null,
  "AccountKey": null,
  "ShareName": null,
  "HealthCheck": {
    "UnhealthyStatus": "Unhealthy"
  }
}
```

## Storage Providers
Nano provides several storage providers, so usually there is no need to implement a custom provider for your application.  

Storagae providers implements the interface ```IStorageProvider```. It contains a single method ```Configure(...)```, that is responsible for handling 
any configuration and setup required for the storage provider.  

To create a new storage provider, implement the `IStorageProvider` interface. Make sure to register all required services in the `Configure(...)` method, 
and then register your provider with the application using `.AddNanoStorage<TProvider>()`.  

The following storage providers are currently supported in Nano.  

* [Nano.Storage.Azure](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage.Azure)
* [Nano.Storage.Local](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage.Local)
