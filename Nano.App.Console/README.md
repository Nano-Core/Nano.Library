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
* [Console Worker](#console-worker) 
* [Start-Up Tasks](#start-up-tasks) 

## Summary
The ```NanoConsoleApplication``` derives from ```BaseNanoApplication```. It initializes the required services and into a service collection, and is able to handle and resolve dependencies, similar to a web application. This is explained in details further down.    
Use the ```NanoConsoleApplication``` when you require to construct a background-worker, cloud function or similar single executing application.  

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
The ```Console``` section of the configuration defines behavior related to the application. The section is deserialized into an instance of ```ConsoleOptions```, and injected as dependency during startup, thus available for injection throughout the application.  
```json
"Console": 
{
    "Localization": null
}
```

## Localization
Nano supports the built in methods for specifying the language when invoking requests.  

| Setting            | Type   | Default   | Description                                    |
| ------------------ | ------ | --------- | ---------------------------------------------- |
|  `DefaultCulture`  | string | en-US     | The default culture used by the application.   |

```json
"App": {
  "DefaultCulture": "en-US"
}
```

See the official Microsoft documentation about localization here: [Localization Documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.1)  


## Console Worker
When the console application is run, a background-worker is started and executed until finished, and the application shuts down.  
The abstract ```BaseWorker``` implements ```IHostedService```, and is the top-most worker implementation. The concrete ```DefaultWorker``` implementation derives from ```BaseWorker```, and contains default method implementations of ```StartAsync``` and ```StopAsync```. Derive your own worker implementation from ```DefaultWorker```, as shown below.  

```csharp
public class MyWorker : DefaultWorker
{
    public MyWorker(ILogger logger, IRepository repository, IEventing eventing, IApplicationLifetime applicationLifetime)
        : base(logger, repository, eventing, applicationLifetime)
    {

    }

    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await base.StartAsync(cancellationToken);

        // Custom implementation
    }

    public override async Task StopAsync(CancellationToken cancellationToken = default)
    {
        // Custom implementation

        await base.StopAsync(cancellationToken);  // Stops application
    }
}
```

## Start-Up Tasks
Nano supports startup-tasks, that executes before the application starts. 
For console applications the worker won't start before all startup tasks has completed.
It's rarely needed for console applications, but supported just in case.

Read more [Nano.App](nano-app#start-up-tasks)

*** 
