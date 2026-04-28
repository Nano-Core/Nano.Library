# Nano.Data.InMemory
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.InMemory.svg)](https://www.nuget.org/packages/Nano.Data.InMemory/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.InMemory.svg)](https://www.nuget.org/packages/Nano.Data.InMemory/)

> In-memory data provider for Nano applications._

*** 

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/blob/master/README.md#nanolibrary)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**

## Summary
Data Provider implementation for in-memory data access.  

The in-memory data provider doesn't use migrations and there is no need to implement the `BaseDbContextFactory`. 

> 📖 Learn more about **[Nano Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#nanodata)**.

Try it out yourself using the **[Api.Data.InMemory](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.InMemory)**, or 
**[Console.Data.InMemory](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Data.InMemory)** example.  

## Registration
Install the **[Nano.Data.InMemory](https://www.nuget.org/packages/Nano.Data.InMemory)** NuGet package.  

```powershell
dotnet add package Nano.Data.InMemory;
```

Register the `InMemoryProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
    .ConfigureServices(services =>
    {
        services
            .AddNanoData<InMemoryProvider, TContext>();
    })
```

A `BaseDbContext` must also be implemented and used as `TContext`.  

## Configuration
Configured the application with the necessary data setup.  

```json
"Data": {
  "BatchSize": 25,
  "BulkBatchSize": 500,
  "BulkBatchDelay": 1000,
  "QueryRetryCount": 0,
  "UseLazyLoading": false,
  "StartupAction": "None",
  "UseSoftDeletetion": false,
  "UseSensitiveDataLogging": false,
  "QuerySplittingBehavior": "SingleQuery",
  "DefaultCollation": null,
  "ConnectionString": "nanoDb",
  "Repository": {
    "UseAutoSave": false,
    "QueryIncludeDepth": 4
  },
  "Identity": null,
  "ConnectionPool": null,
  "HealthCheck": null
}
```
