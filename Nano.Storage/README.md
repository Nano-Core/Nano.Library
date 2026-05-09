# Nano.Storage
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.svg)](https://www.nuget.org/packages/Nano.Storage/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Storage.svg)](https://www.nuget.org/packages/Nano.Storage/)

> _Storage provider common implementations for Nano applications._

> ⚠️ This NuGet is transitive and included in other Nano Packages, and is not meant to be included directly.

***

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/tree/master/README.md#nanolibrary)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
  * **[Health Checks](#health-checks)**
* **[Storage Providers](#storage-providers)**

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

In addition to registering storage, map a local folder to a container path in in your `docker-compose.yml` so the container can access the storage directory.

```yaml
services:
  {service-name}:
    volumes:
      - ./bin/{share-name}:/mnt/{share-name}
```

When deploying Nano applications to Kubernetes, ensure your `deployment.yaml` or `cronjob.yaml`, depending on the application type, maps the shared storage directory and 
a secret is created for the storage account. The exact configuration depends on the storage provider. See supported **[Storage Providers](#storage-providers)** for details.  

> ⚠️ Optionally, map a writable `tmp` directory for temporary files.  
This allows the main container to remain immutable while supporting temporary data.  

```yaml
spec:
  template:
    spec:
      containers:
        volumeMounts:
        - name: tmp
          mountPath: /tmp
      volumes:
      - name: tmp
        emptyDir: {}
```

Last, the `build-and-deploy` needs to have additional environmental variables defined used by the storage provider. Again exactly what variables depends on the specific 
storage provider. See supported **[Storage Providers](#storage-providers)** for details.  

## Configuration
The ```Storage``` section in the configuration defines the storage provider and related settings used by the application.

| Setting            | Type   | Default     | Description                                                                                 |
| ------------------ | ------ | ----------- | ------------------------------------------------------------------------------------------- |
|  `ShareName`       | string | null        | The logical container, share, or bucket name used for file storage.                         |
|  `Credentials`     | object | null        | Optional. The credential or account of the storage provider.                                |
|  `HealthCheck`     | object | null        | Storage health check. _Only relevant for `NanoApiApplication` and `NanoWebApplication`_..   |

```json
"Storage": {
  "ShareName": null,
  "HealthCheck": null
}
```

> 📖 Learn more about **[Application Configuration](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App/README.md#configuration)** here.  

## Health Checks
When health checks are enabled in the storage configuration, Nano automatically registers a health check for the configured storage provider.  

This allows the application to verify that the underlying storage fileshare connection is available and operational. The health check integrates with ASP.NET Core's 
health check system and can be used by monitoring tools, load balancers, or container orchestrators to determine the health status of the application.  

| Setting             | Type   | Default     | Description                                                                                                                              |
| ------------------- | ------ | ----------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
|  `AccountName`      | string | null        | The storage account name used by health checks for some storage providers.                                                               |
|  `UnhealthyStatus`  | enum   | Unhealthy   | The health status reported when the storage provider is unavailable. _Only relevant for `NanoApiApplication` and `NanoWebApplication`_.  |

```json
"Storage": {
  "HealthCheck": {
    "AccountName": null,
    "UnhealthyStatus": "Unhealthy"
  }
}
```

> ⚠️ In order for storage healthcheck to take effect, healthchecks must be enabled for the application. 
Read more about **[Nano Health Check](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api/README.md#health-checks)**.

## Storage Providers
Nano provides several storage providers, so usually there is no need to implement a custom provider for your application.  

Storagae providers implements the interface ```IStorageProvider```. It contains a single method ```Configure(...)```, that is responsible for handling 
any configuration and setup required for the storage provider.  

To create a new storage provider, implement the `IStorageProvider` interface. Make sure to register all required services in the `Configure(...)` method, 
and then register your provider with the application using `.AddNanoStorage<TProvider>()`.  

The following storage providers are currently supported in Nano.  

* **[Nano.Storage.Azure](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage.Azure/README.md/nanostorageazure)**
* **[Nano.Storage.Local](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage.Local/README.md/nanostoragelocal)**
