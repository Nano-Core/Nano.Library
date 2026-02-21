# Nano.Logging.NLog
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.NLog.svg)](https://www.nuget.org/packages/Nano.Logging.NLog/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Logging.NLog.svg)](https://www.nuget.org/packages/Nano.Logging.NLog/)

> NLog logging for Nano applications._

***

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)

## Summary
Logging Provider implementation for NLog.  
Read more about logging here: [Nano.Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging)

## Registration
First install the [Nano.Logging.NLog](https://www.nuget.org/packages/Nano.Logging.NLog) NuGet package.  

```powershell
dotnet add package Nano.Logging.NLog;
```

The NLog Logging Provider must be registered as dependencies.  
```csharp
    .ConfigureServices(x =>
    {
        x.AddNanoLogging<NLogProvider>();
    })
```

***
