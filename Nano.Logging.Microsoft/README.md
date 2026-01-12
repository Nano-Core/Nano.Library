# Nano.Logging.Microsoft
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.Microsoft.svg)](https://www.nuget.org/packages/Nano.Logging.Microsoft/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Logging.Microsoft.svg)](https://www.nuget.org/packages/Nano.Logging.Microsoft/)

> _Microsoft logging for Nano applications._

***

## Table of Contents
* [Summary](#summary)
* [Registration](#registration)

## Summary
Logging Provider implementation for Microsoft.  
Read more about logging here: [Nano.Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging)

## Registration
The Microsoft Logging Provider must be registered as dependencies.  
```csharp
    .ConfigureServices(x =>
    {
        x.AddNanoLogging<MicrosoftProvider>();
    })
```

***
