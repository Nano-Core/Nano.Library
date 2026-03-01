# Nano.Data.PostgreSQL
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.PostgreSQL.svg)](https://www.nuget.org/packages/Nano.Data.PostgreSQL/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.PostgreSQL.svg)](https://www.nuget.org/packages/Nano.Data.PostgreSQL/)

> PostgreSQL data access provider for Nano applications._

*** 

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)
* [Dependencies](#dependencies)

## Summary
Data Provider implementation for PostgreSQL data access.  
Read more about storage here: [Nano.Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)

## Registration
First install the [Nano.Data.PostgreSQL](https://www.nuget.org/packages/Nano.Data.PostgreSQL) NuGet package.  

```powershell
dotnet add package Nano.Data.PostgreSQL;
```

The PostgreSQL data provider must be registered as dependencies.  
```csharp
    .ConfigureServices(services =>
    {
        services
            .AddNanoData<PostgreSqlProvider>();
    })
```

## Dependencies
* https://github.com/Nano-Core/Nano.Azure/tree/master/Nano.Azure.Sql
