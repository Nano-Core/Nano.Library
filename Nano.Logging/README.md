# Nano.Logging
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.svg)](https://www.nuget.org/packages/Nano.Logging/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Logging.svg)](https://www.nuget.org/packages/Nano.Logging/)

> _Logging provider common implementations for Nano applications._

> ⚠️ This NuGet is transitive and included in other Nano Packages, and is not meant to be included directly.

***

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library#nano-library)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
* **[Logging Providers](#logging-providers)**

## Summary
Enabling logging in a Nano application is done by adding a logging provider.  
When a provider is registered, Nano automatically registers the following interfaces during application startup:

* `ILoggerFactory`
* `ILogger`
* `ILogger<T>`

All logging providers write to the console by default.  
In `Staging` and `Production` environments, Kubernetes agents intercept `STDOUT` and `STDERR` and stream logs into centralized logging. It is recommended 
to let the log collector handle log storage and routing, rather than configuring individual applications.  

If no logging provider is registered, a `NullLogger` is used. This means `ILogger` and `ILogger<T>` will resolve correctly, 
but all logs are discarded. Similarly, `ILoggerFactory` is available but produces no output.  

## Registration
To enable logging, the logging provider must be registered.    
Use the `AddNanoLogging<TProvider>()` method, specifying your logging provider implementation as the generic type parameter.

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoLogging<TProvider>();
})
...
```

## Configuration
The ```Logging``` section in the configuration defines the logging provider and related settings used by the application.

| Setting                         | Type   | Default      | Description                                                                                                                     |
| ------------------------------- | ------ | ------------ | ------------------------------------------------------------------------------------------------------------------------------- |
|  `LogLevel`                     | enum   | Information  | The default minimum LogLevel used by the logging provider. Values: Debug, Information, Warning, Error, Fatal.                   |
|  `LogLevelOverrides`            | array  | []           | Optional overrides for specific namespaces, allowing different log levels for different parts of the application.               |
|  `LogLevelOverrides.Namespace`  | string | null         | The log level to apply for a specific namespace. You may prepend an asterisk (`*`) as a wildcard to match multiple namespaces.  |
|  `LogLevelOverrides.LogLevel`   | enum   | Warning      | The namespace for which this log level override applies. Values: Debug, Information, Warning, Error, Fatal.                     |

```json
"Logging": {
  "LogLevel": "Information",
  "LogLevelOverrides": 
  [
    {
      "Namespace": "Microsoft",
      "LogLevel": "Warning"
    }
  ]
}
```

> 📖 Learn more about **[Application Configuration](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#configuration)** here.  

## Logging Providers
Nano provides several logging providers, so usually there is no need to implement a custom provider for your application.  

Logging providers implements the interface ```ILoggingProvider```. It contains a single method ```Configure(...)```, that is responsible for handling 
any configuration and setup required for the logging provider.  

To create a new logging provider, implement the `ILoggingProvider` interface. Make sure to register all required services in the `Configure` method, 
and then register your provider with the application using `.AddNanoLogging<TProvider>()`.  

The following logging providers are currently supported in Nano.  

* [Log4Net](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging.Log4Net)
* [Microsoft](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging.Microsoft)
* [NLog](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging.NLog)
* [Serilog](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging.Serilog)
