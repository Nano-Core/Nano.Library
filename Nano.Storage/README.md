# Nano.Storage
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.svg)](https://www.nuget.org/packages/Nano.Storage/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Storage.svg)](https://www.nuget.org/packages/Nano.Storage/)

> _Pluggable, provider-agnostic file storage for Nano applications._

***

## Table of Contents
* [Summary](#summary)
* [Registration](#registration)
* [Configuration](#configuration)
* [Storage Providers](#storage-providers)
* [Examples](#examples)

## Summary
**Nano.Storage** adds a flexible, provider-agnostic file storage layer to your Nano applications.  

Connect a storage system by registering an `IStorageProvider`. 
Each provider handles setup, connectivity, and optional health checks, etc. while Nano exposes a consistent interface.  

When a storage provider is registered, Nano automatically provides an `IPathProvider` that can be injected, giving easy access to 
the root directory and the temporary (`tmp`) directory.

## Registration
To use Nano.Storage, you must register a storage provider as a dependency in your application.  
This is done using the `AddNanoStorage<TProvider>()` extension method, where `TProvider` is your chosen `IStorageProvider` implementation.

```csharp
.ConfigureServices(services =>
{
    services
        .AddNanoStorage<TProvider>();
});
```

#### docker-compose:
Besides registering storage in your Nano application, you also need to configure your docker-compose setup 
so that your container can access the storage directory both locally. 

This is done by mapping a local folder to a path inside the container:
```yaml
services:
  {my.service}:
    volumes:
      - {share-name}:/mnt/{share-name}
```

#### Kubernetes:
When deploying Nano applications to Kubernetes, your `deployment.yaml` must also include a mapping to the shared storage directory.  

How this mapping is configured depends on the storage provider you are using.  
Please refer to the documentation of the specific `IStorageProvider` implementation for detailed instructions.  

This ensures that your application code can access the same storage paths inside Kubernetes in cloud environments.

> 💡 Optionally (recommended), map a writable `tmp` directory for temporary files. This allows the main container to remain immutable while supporting transient data.
```yaml
template
  spec
    volumes:
    - name: tmp
      emptyDir: {}
```

## Configuration
The ```Storage``` section in the configuration defines the storage provider and related settings used by the application.

| Setting                         | Type   | Default     | Description                                                                                                           |
| ------------------------------- | ------ | ----------- | --------------------------------------------------------------------------------------------------------------------- |
|  `AccountName`                  | string | null        | The account or tenant identifier used to authenticate with the storage provider.                                      |
|  `AccountKey`                   | string | null        | The secret, key, or credential used to authenticate with the storage provider.                                        |
|  `ShareName`                    | string | null        | The logical container, share, or bucket name used for file storage.                                                   |
|  `HealthCheck`                  |        | null        | Storage health check. _Only relevant for_ ```NanoWebApplication```.                                                   |
|  `HealthCheck.UnhealthyStatus`  | enum   | Unhealthy   | The health status reported when the storage provider is unavailable. _Only relevant for_ ```NanoWebApplication```.    |

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
Storage providers integrate shared storage into your application and provide easy access to mapped directories.  

All storage providers implement the `IStorageProvider` interface. 
This interface is responsible for handling all configuration and setup required for the storage provider.  

To implement a new storage provider:

1. Create a class that implements `IStorageProvider`.
2. Ensure that all required services are registered in `Configure`.
3. Register your provider in the application using `AddNanoStorage<MyProvider>()`.

The following storage providers are currently supported:
* [Azure.Storage.Azure](https://github.com/Nano-Core/Nano.Azure/tree/master/Nano.Azure.Storage.Azure)

## Examples
See examples of Nano applications with storage registered here:
* [Nano.Templates.Web.Storage](https://github.com/Nano-Core/Nano.Templates/tree/master/Web.Storage)
* [Nano.Templates.Console.Storage](https://github.com/Nano-Core/Nano.Templates/tree/master/Console.Storage)

***