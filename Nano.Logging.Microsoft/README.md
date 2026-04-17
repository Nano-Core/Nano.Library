# Nano.Logging.Microsoft
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.Microsoft.svg)](https://www.nuget.org/packages/Nano.Logging.Microsoft/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Logging.Microsoft.svg)](https://www.nuget.org/packages/Nano.Logging.Microsoft/)

> _Microsoft logging for Nano applications._

***

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library#nano-library)**
* **[Summary](#summary)**
* **[Registration](#registration)**

## Summary
This package provides the Microsoft logging provider for Nano.  
The provider is preconfigured to write log output to the console using a concise format:

```
{Timestamp:dd-MM-yyyy HH:mm:ss.ffffff} [{Level:u3}] {Message}{NewLine}{Exception}
```

> 📖 Learn more about **[Nano Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging)**.

Try it out yourself using the **[Api.Logging.Microsoft](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Logging.Microsoft)** or 
**[Console.Logging.Microsoft](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Logging.Microsoft)** example.  

## Registration
Install the **[Nano.Logging.Microsoft](https://www.nuget.org/packages/Nano.Logging.Microsoft)** NuGet package.

```powershell
dotnet add package Nano.Logging.Microsoft;
```

Register the `MicrosoftProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoLogging<MicrosoftProvider>();
})
...
```
