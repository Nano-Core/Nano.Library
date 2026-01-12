# Nano.Logging.Serilog
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.Serilog.svg)](https://www.nuget.org/packages/Nano.Logging.Serilog/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Logging.Serilog.svg)](https://www.nuget.org/packages/Nano.Logging.Serilog/)

> _Serilog logging for Nano applications._

*** 

## Table of Contents
* [Summary](#summary)
* [Registration](#registration)

## Summary
Logging Provider implementation for Serilog.  
Read more about logging here: [Nano.Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging)

## Registration
The Serilog Logging Provider must be registered as dependencies.  
```csharp
    .ConfigureServices(x =>
    {
        x.AddNanoLogging<SerilogProvider>();
    })
```

***