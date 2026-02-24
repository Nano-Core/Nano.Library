# Nano.Logging.Log4Net
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.Log4Net.svg)](https://www.nuget.org/packages/Nano.Logging.Log4Net/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Logging.Log4Net.svg)](https://www.nuget.org/packages/Nano.Logging.Log4Net/)

> _Log4Net logging for Nano applications._

***

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)

## Summary
This package provides the Log4Net logging provider for Nano.  
The provider is preconfigured to write log output to the console using a concise format:

```
%utcdate{dd-MM-yyyy HH:mm:ss.ffffff} [%level{3}] %message%newline%exception
```

> 📖 Learn more about **[Nano Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging)**.

Try it out yourself using the **[Api.Logging.Log4Net](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Logging.Log4Net)** or 
**[Console.Logging.Log4Net](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Logging.Log4Net)** example.  

## Registration
Install the **[Nano.Logging.Log4Net](https://www.nuget.org/packages/Nano.Logging.Log4Net)** NuGet package.

```powershell
dotnet add package Nano.Logging.Log4Net;
```

Register the `Log4NetProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoLogging<Log4NetProvider>();
})
...
```
