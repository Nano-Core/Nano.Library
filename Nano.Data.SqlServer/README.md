# Nano.Data.SqlServer
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.SqlServer.svg)](https://www.nuget.org/packages/Nano.Data.SqlServer/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.SqlServer.svg)](https://www.nuget.org/packages/Nano.Data.SqlServer/)

> Sql Server data provider for Nano applications._

*** 

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)

## Summary
Data Provider implementation for Sql Server data access.  

> 📖 Learn more about **[Nano Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)**.
> 📖 Learn more about **[Nano Azure Sql Server](https://github.com/Nano-Core/Nano.Kubernetes/tree/master/Nano.Azure.Sql/SqlServer)**.  

Try it out yourself using the **[Api.Data.SqlServer](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.SqlServer)**, or 
**[Console.Data.SqlServer](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Data.SqlServer)** example.  

## Registration
Install the **[Nano.Data.SqlServer](https://www.nuget.org/packages/Nano.Data.SqlServer)** NuGet package.  

```powershell
dotnet add package Nano.Data.SqlServer;
```

Register the `SqlServerProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
    .ConfigureServices(services =>
    {
        services
            .AddNanoData<SqlServerProvider>();
    })
```
