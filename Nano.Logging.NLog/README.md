# Nano.Logging.NLog
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.NLog.svg)](https://www.nuget.org/packages/Nano.Logging.NLog/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Logging.NLog.svg)](https://www.nuget.org/packages/Nano.Logging.NLog/)

> NLog logging for Nano applications._

***

## Table of Contents
* [Summary](#summary)
* [Registration](#registration)

## Summary
Logging Provider implementation for NLog.  
Read more about logging here: [Nano.Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging)

## Registration
The NLog Logging Provider must be registered as dependencies.  
```csharp
    .ConfigureServices(x =>
    {
        x.AddNanoLogging<NLogProvider>();
    })
```

***
