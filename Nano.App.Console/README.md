# Nano.App.Console
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.App.Console.svg)](https://www.nuget.org/packages/Nano.App.Console/)
[![NuGet](https://img.shields.io/nuget/v/Nano.App.Console.svg)](https://www.nuget.org/packages/Nano.App.Console/)

> _Nano Console application._

***

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)
* [Configuration](#configuration)
  * [Localization](#localization)
  * [Exception Handling](#exception-handling)
  * [Api Clients](api-clients)
* [Console Workers](#console-workers) 
* [Startup Tasks](#startup-tasks)

## Summary
The `NanoConsoleApplication` is a ready-to-use application template for building console-based workers with Nano.  
At its core, it is a standard console application that starts and executes one or more workers. When deployed to Kubernetes, it is typically configured 
to run as a CronJob, while in the `Development` environment it simply runs as a normal console application without any scheduling.

The application derives from `BaseNanoApplication` and implements the `IApplication` interface, following established Nano application patterns. 
It provides a concrete and opinionated implementation tailored specifically for console workloads.

`NanoConsoleApplication` exposes convenient static factory methods for creating and configuring the application with sensible defaults, 
while still allowing full customization through configuration and the `ConfigureServices` method. This ensures that all core application behaviors are 
initialized consistently from configuration, reducing boilerplate code and simplifying the creation of new console applications.

> 📖 Learn more about common Nano application features here: **[Nano Application](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App)**.  

## Registration
First install the [Nano.App.Console](https://www.nuget.org/packages/Nano.App.Console) NuGet package.  

```powershell
dotnet add package Nano.App.Console;
```

Then, to create a `NanoConsoleApplication` simply add the following code to `program.cs`.  

```csharp
NanoConsoleApplication
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
The `App` section in the configuration defines behavior related to the application.  

| Setting                    | Type       | Default    | Description                                                                                                                                                          |
| -------------------------- | ---------- | ---------- | ------------------------------------------------------------------------------------------------------ |
|  `Version`                 | string     | 1.0.0.0    | Application version identifier.                                                                        |
|  `Localization`            | object     | null       | Localization configuration options. See **[Localization](#localization)**.                             |
|  `Apis`                    | dictionary | []         | Named Nano API client configurations available to the application. See **[Api-Client](#api-client)**.  |

```json
"App": 
{
  "Version": "1.0.0.0",
  "Localization": null,
  "Apis": null
}
```

## Localization
The Nano configuration supports specifying a default `CultureInfo` for console applications, ensuring that culture-sensitive operations 
such as date, number, and currency formatting—are applied consistently across the entire application lifecycle.  

The `DefaultCultureInfo` will be set to the configured default culture.  

| Setting            | Type   | Default   | Description                                    |
| ------------------ | ------ | --------- | ---------------------------------------------- |
|  `DefaultCulture`  | string | en-US     | The default culture used by the application.   |

```json
"Localization": {
  "DefaultCulture": "en-US"
}
```

## Exception Handling
Exceptions thrown by individual Nano workers are handled internally, ensuring that failures in one worker do not impact the execution of others.  

When a [Logging Provider](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging) is registered, any worker that fails will automatically log the exception.  

No additional configuration or setup is required.  

Try it out yourself using the **[Api.ExceptionHandling](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.ExceptionHandling)** example.  

## Api Clients
Nano API clients provide a consistent and structured way for applications to communicate with other Nano API services.  

In console applications, they allow worker processes to establish connections with one or more 
[Nano API applications](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api), send requests, and retrieve responses in a reliable and predictable manner. 
This enables console workers to leverage the functionality of multiple Nano services while keeping service boundaries clear and maintaining consistent 
error handling, logging, and response propagation across the system.

> 📖 Learn more [Nano Api Clients](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#api-clients)

## Console Workers
You can implement as many workers as you need by creating classes that implement `IWorker`.  
All registered workers will run automatically when the application starts, and the application will shut down once all workers have completed their execution.

For convenience, derive your worker from `BaseWorker`. This allows you to only implement `OnStartAsync()` if you don't need any custom logic in `OnStopAsync()`.

```csharp
public class MyWorker(ILogger logger) 
    : BaseWorker(logger)
{
    public override async Task OnStartAsync(CancellationToken cancellationToken = default)
    {
        // Your startup logic here...
    }

    // Optional
    public override async Task OnStopAsync(CancellationToken cancellationToken = default)
    {
        // Your shutdown/cleanup logic here...
    }
}
```

You can inject any services your worker needs, including scoped services, which will be correctly resolved when the worker is executed.  

Try it out yourself using the **[Api.Workers](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Workers)** example.  

## Startup Tasks
Nano Console applications supports start-up tasks that execute before the application begins processing requests.  
The console worker won't start until all configured start-up tasks have completed successfully.  

While start-up tasks are rarely required for console applications, this feature is available to ensure any necessary initialization can be performed before the worker starts.

> 📖 Learn more [Nano Startup Tasks](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#startup-tasks)

Try it out yourself using the **[Api.StartupTasks](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.StartupTasks)** example.  
