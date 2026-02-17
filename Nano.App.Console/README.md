# Nano.App.Console
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.App.Console.svg)](https://www.nuget.org/packages/Nano.App.Console/)
[![NuGet](https://img.shields.io/nuget/v/Nano.App.Console.svg)](https://www.nuget.org/packages/Nano.App.Console/)

> _Nano Console application._

***

## Table of Contents
* [Summary](#summary)
* [Registration](#registration)
* [Configuration](#configuration)
  * [Localization](#localization)
  * [Api Clients](api-clients)
* [Console Worker](#console-worker) 
* [Start-Up Tasks](#start-up-tasks)

## Summary
The `NanoConsoleApplication` is a ready-to-use application template for building console workers with Nano.  

It derives from `BaseNanoApplication` and implements the `IApplication` interface, following the common Nano application patterns and providing a concrete implementation 
for building console applications with Nano. It provides convenient static methods to create and configure the application with sensible defaults, while allowing full customization 
of services through the `ConfigureServices` method. This design ensures that all core applications behaviors are initialized consistently using you configuration, reducing boilerplate code 
and simplifying the setup of new applications.  

> 📖 Learn more about common Nano application features here: **[Nano Application](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App)**.  

## Registration
program.cs
```csharp
public class Program
{
    public static void Main()
    {
        NanoConsoleApplication
            .ConfigureApp()
            .ConfigureServices(x =>
            {
                // Your services...
            })
            .Build()
            .Run();
    }
}
```

## Configuration
The `App` section in the configuration defines behavior related to the application.  

| Setting                    | Type       | Default    | Description                                                                                                                                                          |
| -------------------------- | ---------- | ---------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|  `Version`                 | string     | 1.0.0.0    | Application version identifier.                                                                                                                                      |
|  `Localization`            | object     | null       | Localization configuration options. See **[Localization](#localization)**.                                                                                           |
|  `Apis`                    | dictionary | []         | Named Nano API client configurations available to the application. See **[Api-Client](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#api-client)**.  |

```json
"Console": 
{
    "Localization": null
}
```

## Localization
The confiuration in Nano supports specifying the default `CultureInfo` for a console application.  

| Setting            | Type   | Default   | Description                                    |
| ------------------ | ------ | --------- | ---------------------------------------------- |
|  `DefaultCulture`  | string | en-US     | The default culture used by the application.   |

```json
"Localization": {
  "DefaultCulture": "en-US"
}
```

## Api Clients
See [Nano Api Clients](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#api-clients)

## Console Worker
When the application is started, a console-worker is also started and executed until finished, and the application shuts down.  
The abstract ```BaseWorker``` implements ```IHostedService```, and contains default method implementations of ```StartAsync``` and ```StopAsync```. 

Derive your own worker implementation from ```BaseWorker```, as shown below.  

MAKE CODE, AND ALSO MAYBE MAKE SIMPLER CONSTRUCTOR (protected BaseWorker(ILogger logger, IHostApplicationLifetime applicationLifetime, StartupTaskContext startupTaskContext))
```csharp
```

## Start-Up Tasks
Nano supports startup-tasks, that executes before the application starts. 
For console applications the worker won't start before all startup tasks has completed.
It's rarely needed for console applications, but supported just in case.

Read more [Nano Start-Up Tasks](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#start-up-tasks)
