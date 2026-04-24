# Nano.App.Web
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.App.Web.svg)](https://www.nuget.org/packages/Nano.App.Web/)
[![NuGet](https://img.shields.io/nuget/v/Nano.App.Web.svg)](https://www.nuget.org/packages/Nano.App.ConsWebole/)

> _Nano Web application._

> ⚠️ Experimental — proceed with caution.

***

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library#nano-library)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
  * **[Razor](#razor)**
  * **[Blazor](#blazor)**

## Summary
Extends the API application with built-in support for Razor Pages and Blazor through registered services and middleware.  

> ⚠️ Before proceeding, it is highly recommended to familiarize yourself generally with **[Nano Applications](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App)**.  

## Registration
First install the [Nano.App.Web](https://www.nuget.org/packages/Nano.App.Web) NuGet package.  

```powershell
dotnet add package Nano.App.Web;
```

Then, to create a `NanoWebApplication` simply add the following code to `program.cs`.  

```csharp
NanoWebApplication<TRoot>
    .ConfigureApp()
    .ConfigureServices(x =>
    {
        // Your services...
    })
    .Build()
    .Run();
```

Register your custom services in the `ConfigureServices(x => { })` method to extend Nano with additional functionality or integrations.  

## Configuration
The `App` section in the configuration controls application-level behavior, similar to the API application.  
Currently, the web application does not add any additional configuration options beyond those available in the API application.  

> 📖 Learn more about **[Nano API Configuration](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api#configuration)**.

## Razor
Coming...

## Blazor
Coming...
