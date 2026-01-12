# Nano.Logging
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.svg)](https://www.nuget.org/packages/Nano.Logging/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Logging.svg)](https://www.nuget.org/packages/Nano.Logging/)

> _Pluggable, provider-agnostic logging for Nano applications._

***

## Table of Contents
* [Summary](#summary)
* [Registration](#registration)
* [Configuration](#configuration)
* [Logging Providers](#logging-providers)
* [Examples](#examples)

## Summary
Nano registers the interfaces ```ILoggerFactory```, ```ILogger``` and ```ILogger<T>``` during application startup. 

All Providers are logging to console. In live environment orchestrations grap those STD OUT and STD ERR and stream them to your desired logs collector.
Argue that to avoid uncessary connection from logging to cloud environment, like it would be required for Application Insights. 
Instead it's better to add the application insight collector on the virtual machines in the cloud.

If no logging is registered for a Nano application, the `NullLogger` will be registered, and ```ILogger``` and ```ILogger<T>``` will resolve, but all logging
will be flushed into the void.

## Registration
The logging provider must be registered as dependencies.
Invoke the method ```AddNanoLogging<TProvider>()```, using the logging provider implementation as generic type parameters.

```csharp
.ConfigureServices(services =>
{
    services
        .AddNanoLogging<TProvider>();
})
```

## Configuration
The ```Logging``` section in the configuration defines the logging provider and related settings used by the application.

| Setting                         | Type   | Default      | Description                                                                                                       |
| ------------------------------- | ------ | ------------ | ----------------------------------------------------------------------------------------------------------------- |
|  `LogLevel`                     | enum   | Information  | The default minimum LogLevel used by the logging provider. Values: Debug, Information, Warning, Error, Fatal.     |
|  `LogLevelOverrides`            | Array  | []           | Optional overrides for specific namespaces, allowing different log levels for different parts of the application. |
|  `LogLevelOverrides.Namespace`  | string | null         | The LogLevel to use for the specified namespace.                                                                  |
|  `LogLevelOverrides.LogLevel`   | enum   | Warning      | The namespace for which this log level override applies. Values: Debug, Information, Warning, Error, Fatal.       |

```json
"Logging": {
  "LogLevel": "Information",
  "LogLevelOverrides": 
  [
    {
      "Namespace": "System",
      "LogLevel": "Warning"
    }
  ]
}
```

## Logging Providers
Nano provides several logging providers (and more added on request), so usually there is no need to implement a custom data provider for your application.  

Logging providers implements the interface ```ILoggingProvider```. It contains a single method ```Configure(...)```, that is responsible for handling any configuration and setup required for the logging provider.  

To implement a new storage provider:

1. Create a class that implements `ILoggingProvider`.
2. Ensure that all required services are registered in `Configure`.
3. Register your provider in the application using `AddNanoLogging<MyProvider>()`.

The following logging providers are currently supported:
* ```Log4NetProvider```
* ```MicrosoftProvider```
* ```NLogProvider```
* ```SerilogProvider```

***

## Examples
See examples of Nano applications with storage registered here:
* [Nano.Templates.Web.Logging](https://github.com/Nano-Core/Nano.Templates/tree/master/Web.Logging)
* [Nano.Templates.Console.Logging](https://github.com/Nano-Core/Nano.Templates/tree/master/Console.Logging)

