# Nano.Data.SqlServer
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.SqlServer.svg)](https://www.nuget.org/packages/Nano.Data.SqlServer/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.SqlServer.svg)](https://www.nuget.org/packages/Nano.Data.SqlServer/)

> Sql Server data access provider for Nano applications._

*** 

## Table of Contents
* [Summary](#summary)
* [Registration](#registration)
* [Dependencies](#dependencies)

## Summary
Data Provider implementation for Sql Server data access.  
Read more about storage here: [Nano.Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)

## Registration
The Sql Server data provider must be registered as dependencies.  
```csharp
    .ConfigureServices(x =>
    {
        x.AddNanoData<SqlServerProvider>();
    })
```

## Dependencies
* https://github.com/Nano-Core/Nano.Azure/tree/master/Nano.Azure.Sql

***