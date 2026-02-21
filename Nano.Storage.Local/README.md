# Nano.Storage.Local
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)

> Local File Share storage for Nano applications._

*** 

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)
* [Dependencies](#dependencies)

## Summary
Storage Provider implementation for Local File Shares.  
Read more about storage here: [Nano.Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage)

This is intended for convinience if you are using a local file system.
Registering with Nano gives you access to use `IPathProvider`.

## Registration
First install the [Nano.Storage.Local](https://www.nuget.org/packages/Nano.Storage.Local) NuGet package.  

```powershell
dotnet add package Nano.Storage.Local;
```

The Azure File Share storage provider must be registered as dependencies.  
```csharp
    .ConfigureServices(x =>
    {
        x.AddNanoStorage<LocalFileShareProvider>();
    })
```

## Dependencies
* https://github.com/Nano-Core/Nano.Azure/tree/master/Nano.Azure.Storage

***