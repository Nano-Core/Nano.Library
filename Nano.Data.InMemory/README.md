# Nano.Data.InMemory
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.InMemory.svg)](https://www.nuget.org/packages/Nano.Data.InMemory/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.InMemory.svg)](https://www.nuget.org/packages/Nano.Data.InMemory/)

> In-Memory data access for Nano applications._

*** 

## Table of Contents
* [Summary](#summary)
* [Registration](#registration)
* [Dependencies](#dependencies)

## Summary
Data Provider implementation for In-Memory data access.  
Read more about storage here: [Nano.Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)

## Registration
The In-Memory data provider must be registered as dependencies.  
```csharp
    .ConfigureServices(x =>
    {
        x.AddNanoData<InMemoryProvider>();
    })
```

## Dependencies
* https://github.com/Nano-Core/Nano.Azure/tree/master/Nano.Azure.Sql

***