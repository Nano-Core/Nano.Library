# Nano.Logging.NLog
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.NLog.svg)](https://www.nuget.org/packages/Nano.Logging.NLog/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Logging.NLog.svg)](https://www.nuget.org/packages/Nano.Logging.NLog/)

> NLog logging for Nano applications._

***

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/tree/master/README.md#nanolibrary)**
* **[Summary](#summary)**
* **[Registration](#registration)**

## Summary
This package provides the NLog logging provider for Nano.  
The provider is preconfigured to write log output to the console using a concise format:

```
${date:format=dd-MM-yyyy HH\\:mm\\:ss.ffffff} [${level:uppercase=true:truncate=3}] ${message}${onexception:${newline}${exception:format=toString}}
```

> 📖 Learn more about **[Nano Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging/README.md#nanologging)**.

Try it out yourself using the **[Api.Logging.NLog](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Logging.NLog)** or 
**[Console.Logging.NLog](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Logging.NLog)** example.  

## Registration
Install the **[Nano.Logging.NLog](https://www.nuget.org/packages/Nano.Logging.NLog)** NuGet package.

```powershell
dotnet add package Nano.Logging.NLog;
```

Register the `NLogProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoLogging<NLogProvider>();
})
...
```
