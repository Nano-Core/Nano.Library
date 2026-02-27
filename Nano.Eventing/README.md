# Nano.Eventing
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Eventing.svg)](https://www.nuget.org/packages/Nano.Eventing/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Eventing.svg)](https://www.nuget.org/packages/Nano.Eventing/)

> _Eventing provider common implementations for Nano applications._

> ⚠️ This NuGet is transitive and included in other Nano Packages, and is not meant to be included directly.

***

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)
* [Configuration](#configuration)
* [Publish and Subscribe](#publish-and-subscribe)
* [Eventing Providers](#eventing-providers)
* [Examples](#examples)

## Summary
Nano supports different ways of publishing and subscribing to events.  
Adding eventing annotations to model implementations, provides a way of synchronizing entities between applications. Additionally, custom event models can be implemented and published, and consumed through subscriptions.  
The ```IEventingProvider``` is registered during startup, and the implementing type defines the eventing provider used in the application. Furthermore, the interface ```IEventing``` is registered as well, and defines the entry to publishing and subscribing to events.  
```Nano.Eventing.Abstractions.IEventing``` and ```Nano.Eventing.Abstractions.IEventingHandler<>```  

## Registration
The eventing provider must be registered as dependencies.
Invoke the method ```AddNanoEventing<TProvider>();```, using the eventing provider implementation as generic type parameters.

```csharp
.ConfigureServices(services =>
{
    services
        .AddNanoEventing<TProvider>();
})
```

Besides registering eventing in your Nano application, you also need to configure your docker-compose setup.

```yaml

```

USE SECRET IN `deployment.yaml`, should we mention it here?


## Configuration
The ```Eventing``` section in the configuration defines the eventing provider and related settings used by the application.

| Setting                         | Type     | Default     | Description                                                                                                                                  |
| ------------------------------- | -------- | ----------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
|  `Host`                         | string   | null        | The hostname or IP address of the event broker or messaging server.                                                                          |             
|  `VHost`                        | string   | /           | The virtual host or namespace on the broker to connect to, if applicable.                                                                    |
|  `Username`                     | string   | null        | Username for authenticating with the broker.                                                                                                 |
|  `Password`                     | string   | null        | Password for authenticating with the broker.                                                                                                 |
|  `Port`                         | ushort   | 5672        | Port to connect to on the broker.                                                                                                            |
|  `Timeout`                      | TimeSpan | 00:00:30    | Connection timeout for the broker, in seconds.                                                                                               |
|  `UseSsl`                       | bool     | false       | Indicates whether to use SSL/TLS when connecting to the broker.                                                                              |
|  `Heartbeat`                    | ushort   | 60          | Heartbeat or keep-alive interval in seconds to maintain the connection. Set to zero to disable heartbeat/keep-alive.                         |
|  `PrefetchCount`                | ushort   | 50          | Prefetch count for consuming messages. Controls how many messages can be fetched at once for processing.                                     |
|  `HealthCheck`                  |          | null        | Eventing health check. _Only relevant for `NanoApiApplication` and `NanoWebApplication`_.                                                    |
|  `HealthCheck.UnhealthyStatus`  | enum     | Unhealthy   | Health status level to report when the eventing provider is unavailable. _Only relevant for `NanoApiApplication` and `NanoWebApplication`_.  |

```json
"Eventing": {
  "Host": null,
  "VHost": null,
  "Username": null,
  "Password": null,
  "Port": 5672,
  "Timeout": 30,
  "UseSsl": false,
  "Heartbeat": 60,
  "PrefetchCount": 50,
  "HealthCheck": {
    "UnhealthyStatus": "Unhealthy"
  }
}
```
 
## Eventing Providers
All eventing providers implement the `IEventingProvider` interface. 
This interface is responsible for handling all configuration and setup required for the eventing provider.  
Additionally it's responsbile for Implementing the `IEventing` interface as well, and register all event handlers??

To implement a new eventing provider:

1. Create a class that implements `IEventingProvider`.
2. Ensure that all required services are registered in `Configure`.
3. Register your provider in the application using `AddNanoEventing<MyProvider>()`.

The following storage providers are currently supported:
* ```RabbitMqProvider```

## Publish and Subscribe
An event can be published from anywhere within the application.  
Nano controllers has the ```IEventing``` dependency injected, and publishing from one of it's actions is recommended. Either override an existing inherited action or implement a new one.  

```csharp
public void Publish()
{
    var @event = new MyEvent();
    this.Eventing.PublishAsync(@event);
}
```

Subscribing to an event requires an event handler implementation.  
Implementing the interface ```IEventingHandler<TEvent>``` ensures that the event handler is registered during startup, and the subscription is configured to listen for incoming events. The method ```CallbackAsync(...)``` is invoked for evert event received.  
It's recommended to share the event model, as both the publisher and subscriber must understand the object model.  

```csharp
public class MyEventHandler : IEventingHandler<MyEvent>
{
    protected virtual IService Service { get; }

    public MyEventHandler(IService service)
    {
        this.Service = service;
    }

    public void CallbackAsync(MyEvent @event, bool isRetrying)
    {
        // Event logic.
    }
}
```
