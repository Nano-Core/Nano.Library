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
* [Dependencies](#dependencies)

## Summary
Data Provider implementation for Sql Server data access.  
Read more about storage here: [Nano.Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)

## Registration
First install the [Nano.Data.SqlServer](https://www.nuget.org/packages/Nano.Data.SqlServer) NuGet package.  

```powershell
dotnet add package Nano.Data.SqlServer;
```

The Sql Server data provider must be registered as dependencies.  
```csharp
    .ConfigureServices(services =>
    {
        services
            .AddNanoData<SqlServerProvider>();
    })
```

## Dependencies
* https://github.com/Nano-Core/Nano.Azure/tree/master/Nano.Azure.Sql

***