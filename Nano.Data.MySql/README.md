# Nano.Data.MySql
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.MySql.svg)](https://www.nuget.org/packages/Nano.Data.MySql/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.MySql.svg)](https://www.nuget.org/packages/Nano.Data.MySql/)

> MySQL data access provider for Nano applications._

*** 

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)
* [Dependencies](#dependencies)

## Summary
Data Provider implementation for MySQL data access.  
Read more about storage here: [Nano.Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)

## Registration
First install the [Nano.Data.MySql](https://www.nuget.org/packages/Nano.Data.MySql) NuGet package.  

```powershell
dotnet add package Nano.Data.MySql;
```

The MySQL data provider must be registered as dependencies.  
```csharp
    .ConfigureServices(x =>
    {
        x.AddNanoData<MySqlProvider>();
    })
```

## Dependencies
* https://github.com/Nano-Core/Nano.Azure/tree/master/Nano.Azure.Sql

***