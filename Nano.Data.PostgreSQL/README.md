# Nano.Data.PostgreSQL
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.PostgreSQL.svg)](https://www.nuget.org/packages/Nano.Data.PostgreSQL/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.PostgreSQL.svg)](https://www.nuget.org/packages/Nano.Data.PostgreSQL/)

> PostgreSQL data provider for Nano applications._

*** 

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)

## Summary
Data Provider implementation for PostgreSQL data access.  

> 📖 Learn more about **[Nano Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)**.
> 📖 Learn more about **[Nano Azure PostgreSQL](https://github.com/Nano-Core/Nano.Kubernetes/tree/master/Nano.Azure.Sql/PostgreSql)**.  

Try it out yourself using the **[Api.Data.PostgreSQL](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.PostgreSQL)**, or 
**[Console.Data.PostgreSQL](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Data.PostgreSQL)** example.  

## Registration
Install the **[Nano.Data.PostgreSQL](https://www.nuget.org/packages/Nano.Data.PostgreSQL)** NuGet package.  

```powershell
dotnet add package Nano.Data.PostgreSQL;
```

Register the `PostgreSqlProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
    .ConfigureServices(services =>
    {
        services
            .AddNanoData<PostgreSqlProvider>();
    })
```
