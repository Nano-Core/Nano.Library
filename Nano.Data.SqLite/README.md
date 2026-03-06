# Nano.Data.SqLite
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.SqLite.svg)](https://www.nuget.org/packages/Nano.Data.SqLite/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.SqLite.svg)](https://www.nuget.org/packages/Nano.Data.SqLite/)

> SqLite data provider for Nano applications._

*** 

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)

## Summary
Data Provider implementation for SqLite data access.  

> 📖 Learn more about **[Nano Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)**.
> 📖 Learn more about **[Nano Azure SqLite](https://github.com/Nano-Core/Nano.Kubernetes/tree/master/Nano.Azure.Sql/SqLite)**.  

Try it out yourself using the **[Api.Data.SqLite](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.SqLite)**, or 
**[Console.Data.SqLite](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Data.SqLite)** example.  

## Registration
Install the **[Nano.Data.SqLite](https://www.nuget.org/packages/Nano.Data.SqLite)** NuGet package.  

```powershell
dotnet add package Nano.Data.SqLite;
```

Register the `SqLiteProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
    .ConfigureServices(services =>
    {
        services
            .AddNanoData<SqLiteProvider>();
    })
```
