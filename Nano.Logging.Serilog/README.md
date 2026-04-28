# Nano.Logging.Serilog
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.Serilog.svg)](https://www.nuget.org/packages/Nano.Logging.Serilog/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Logging.Serilog.svg)](https://www.nuget.org/packages/Nano.Logging.Serilog/)

> _Serilog logging for Nano applications._

*** 

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/tree/master/README.md#nanolibrary)**
* **[Summary](#summary)**
* **[Registration](#registration)**

## Summary
This package provides the Serilog logging provider for Nano.  
The provider is preconfigured to write log output to the console using a concise format:

```
{Timestamp:dd-MM-yyyy HH:mm:ss.ffffff} [{Level:u3}] {Message}{NewLine}{Exception}
```

> 📖 Learn more about **[Nano Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging/README.md#nanologging)**.

Try it out yourself using the **[Api.Logging.Serilog](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Logging.Serilog)** or 
**[Console.Logging.Serilog](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Logging.Serilog)** example.  

## Registration
Install the **[Nano.Logging.Serilog](https://www.nuget.org/packages/Nano.Logging.Serilog)** NuGet package.

```powershell
dotnet add package Nano.Logging.Serilog;
```

Register the `SerilogProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoLogging<SerilogProvider>();
})
...
```
