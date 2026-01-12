# Nano.Logging.Log4Net
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.Log4Net.svg)](https://www.nuget.org/packages/Nano.Logging.Log4Net/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Logging.Log4Net.svg)](https://www.nuget.org/packages/Nano.Logging.Log4Net/)

> _Log4Net logging for Nano applications._

***

## Table of Contents
* [Summary](#summary)
* [Registration](#registration)

## Summary
Logging Provider implementation for Log4Net.  
Read more about logging here: [Nano.Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging)

## Registration
The Log4Net Logging Provider must be registered as dependencies.  
```csharp
    .ConfigureServices(x =>
    {
        x.AddNanoLogging<Log4NetProvider>();
    })
```

***
