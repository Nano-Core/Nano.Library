# Nano.Storage
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.svg)](https://www.nuget.org/packages/Nano.Storage/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Storage.svg)](https://www.nuget.org/packages/Nano.Storage/)

***

## Table of Contents
* [Summary](#summary)
* [Configuration](#configuration)
* [Registration](#registration)
* [Storage Providers](#storage-providers)
* [Path Provider](#path-provider)

## Summary
Extend your Nano application with integration to file storage.  
Nano supports cloud storage integration.  

The ```IStorageProvider``` is registered during startup, and the implementing type defines the storage provider used in the application.  

## Configuration
The ```Storage``` section of the configuration defines storage in the application.  

##### Eventing Section
```json
  "Storage": {
    "AccountName": null,
    "AccountKey": null,
    "ShareName": null,
    "EnableHealthCheck": true,
    "UnhealthyStatus": "Unhealthy"
  }
```

Config UseHealthCheck: When enabling health-checks in the storage section of the confiugration, the application will be configured with a health-check for the storage provider. 
When the application starts, a check is made to ensure that the storage provider is up and running, returning a healthy status code when checked.  
The health status of the application, including the storage provider, can be found here:  
* ```http://{host}:{port}/healthz```  

## Registration
Show generic registration example.  
The storage provider must be registered as dependencies.
Invoke the method ```.AddStorage<TProvider>();```, using the storage provider implementation as generic type parameters.

##### Sample Implementation
```csharp
.ConfigureServices(x =>
{
    x.AddStorage<EasyNetQProvider>();
})
```

## Storage Providers
Storage providers integrate shared drives into your container and enables easy access to mapped drives.  
Nano provides several storage providers (and more added on request), so usually there is no need to implement a custom data provider for your application.  

Storage providers implements the interface ```IStorageProvider```. It contains a single method ```Configure(...)```, that is responsible for handling any configuration and setup required for the storage provider.  

The eventing providers currently supported by Nano, can be referenced in the [Appendix - Supported Providers](supported-providers).  

### Supported Providers
The following providers are currently supported.  
* Azure File Share 

### Create Custom Provider
It's also possible to implement your own provider. 
Implement the interface ```IStorageProvider```, and use the implementation when registering Nano storage.

## Path Provider
Nano exposes ```IPathProvider``` that gets registered when a Storage Provider is registered.
This gives easy access to the root path of your shared drives as well as the temp dir.
Works on both Linux, Mac and Windows.

Show Path.Combine example.

## Container setup
Docker / Kubernetes requirements for storage mapping
Show what yaml that needs to be added to docker compose and kubernets and for both tmp drive and shared drive.

## Examples
See examples of Nano applications with storage registered here:
* link 1
* link 2

***