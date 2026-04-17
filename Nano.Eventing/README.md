# Nano.Eventing
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Eventing.svg)](https://www.nuget.org/packages/Nano.Eventing/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Eventing.svg)](https://www.nuget.org/packages/Nano.Eventing/)

> _Eventing provider common implementations for Nano applications._

> ⚠️ This NuGet is transitive and included in other Nano Packages, and is not meant to be included directly.

***

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library#nano-library)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
  * **[Health Checks](#health-checks)**
* **[Serialization](#serialization)**
* **[Eventing Providers](#eventing-providers)**
* **[Publish and Subscribe](#publish-and-subscribe)**

## Summary
Nano provides a robust eventing framework that enables applications to publish events and have them consumed by other applications. 
This facilitates decoupled, asynchronous communication, making it easier to build scalable and distributed systems.  

When the `IEventingProvider` is registered during application startup, the `IEventing` service becomes available for publishing custom events to the configured provider. 
Any part of the application can then emit events without needing to know the details of the underlying messaging infrastructure.  

To publish an event, simply call: ```await IEventing.PublishAsync<TEvent>(new());```.  

To handle incoming events, implement `IEventingHandler<TEvent>` for your specific event types. These handlers are automatically registered and will consume messages 
from the broker as they are published.  

## Registration
To use Nano eventing, the eventing provider must be registered as a dependency during application startup. This is done by invoking 
the `AddNanoEventing<TProvider>()` method, specifying your chosen eventing provider implementation as the generic type parameter.

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoEventing<TProvider>();
})
...
```

Both `docker-compose.yml` for Docker setups and `deployment.yaml` or `cronjob.yaml` for Kubernetes deployments must be updated to support Nano eventing. This includes 
configuring service dependencies for the eventing broker. The exact configuration will depend on the chosen eventing provider. For detailed instructions, 
see supported **[Eventing Providers](#eventing-providers)**.

## Configuration
The ```Eventing``` section in the configuration defines the eventing provider and related settings used by the application.

| Setting                         | Type     | Default     | Description                                                                                                                                  |
| ------------------------------- | -------- | ----------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
|  `Host`                         | string   | null        | Required. The hostname or IP address of the event broker or messaging server.                                                                          |             
|  `VHost`                        | string   | /           | The virtual host or namespace on the broker to connect to, if applicable.                                                                    |
|  `Port`                         | ushort   | 5672        | Port to connect to on the broker.                                                                                                            |
|  `Timeout`                      | TimeSpan | 00:00:30    | Connection timeout for the broker, in seconds.                                                                                               |
|  `UseSsl`                       | bool     | false       | Indicates whether to use SSL/TLS when connecting to the broker.                                                                              |
|  `Heartbeat`                    | ushort   | 60          | Heartbeat or keep-alive interval in seconds to maintain the connection. Set to zero to disable heartbeat/keep-alive.                         |
|  `PrefetchCount`                | ushort   | 50          | Prefetch count for consuming messages. Controls how many messages can be fetched at once for processing.                                     |
|  `Credentials`                  | object   | null        | Optional. Account / credentials information for eventing.                                                                                              |
|  `Credentials.Id`               | string   | null        | Required. Username for authenticating.                                                                                                                 |
|  `Credentials.Secret`           | string   | null        | Required. Password for authenticating.                                                                                                                 |
|  `HealthCheck`                  | object   | null        | Eventing health check. _Only relevant for `NanoApiApplication` and `NanoWebApplication`_.                                                    |

```json
"Eventing": {
  "Host": null,
  "VHost": null,
  "Port": 5672,
  "Timeout": 30,
  "UseSsl": false,
  "Heartbeat": 60,
  "PrefetchCount": 50,
  "Credentials": {
    "Id": null,
    "Secret": null
  }
  "HealthCheck": null
}
```

> 📖 Learn more about **[Application Configuration](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#configuration)** here.  

## Health Checks
When health checks are enabled in the eventing configuration, Nano automatically registers a health check for the configured eventing provider.  

This allows the application to verify that the underlying broker connection is available and operational. The health check integrates with ASP.NET Core's health check system 
and can be used by monitoring tools, load balancers, or container orchestrators to determine the health status of the application.  

| Setting                         | Type     | Default     | Description                                                                                                                                  |
| ------------------------------- | -------- | ----------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
|  `HealthCheck.UnhealthyStatus`  | enum     | Unhealthy   | Health status level to report when the eventing provider is unavailable. _Only relevant for `NanoApiApplication` and `NanoWebApplication`_.  |

```json
"Eventing": {
  "HealthCheck": {
    "UnhealthyStatus": "Unhealthy"
  }
}
```

## Serialization
Nano eventing uses `Newtonsoft.Json` for serialization and deserialization. It supports all built-in Nano types, types derived from Nano base types, 
and all `Geometry` types from `NetTopologySuite`.  

The serializer is configured to handle various edge cases for robustness. However, event contracts should remain simple. Eventing is not intended for 
transferring large payloads. It works best with small, well-defined message contracts that represent identifiable business events.  

The serializer is case-insensitive.  

## Eventing Providers
All eventing providers in Nano implement the `IEventingProvider` interface. This interface is responsible for configuring and setting up the underlying 
eventing infrastructure, as well as providing an implementation of the `IEventing` interface for publishing events. It also ensures that all 
relevant `IEventingHandler<TEvent>` implementations are registered so that events can be consumed as they are published.  

To implement a new eventing provider:
1. Create a class that implements `IEventingProvider`.
2. Register all required services and dependencies in the `Configure` method.
3. Add your provider to the application using:

```csharp
services.AddNanoEventing<MyProvider>();
```

The following eventing providers are currently supported in Nano:
* [Nano.Eventing.RabbitMq](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing.RabbitMq)

Additional providers can be implemented by following the same pattern, allowing you to extend Nano’s eventing system to any messaging broker of your choice.

## Publish and Subscribe
Events can be published from anywhere within your application.  

The first step is to define an event model, which acts as the contract for the messages.  

```csharp
public class MyEvent
{
    public string Text { get; set; }
}
```

The event model may by any type derived from Nano types, as well as all `Geometry` types from `NetTopologySuite`. Keep it simple though. See [Serialization](#Serialization)
for details.    

> ⚠️ Share the event model as a NuGet package to ensure a consistent contract between publishers and subscribers. Exchange and queue names are derived automatically 
from the event type.

Next, to publish an event from one application:

```csharp
await this.Eventing
.PublishAsync(new MyEvent
{
    Text = "Message from another service"
});
```

⚠️ IEventing also provides a `SubscribeAsync(...)` method, but manual invocation is not required. All `IEventingHandler<T>` implementations are automatically 
registered during application startup.

To consume events in another application, implement an eventing handler for the specific event type by deriving an implementation from `BaseEventHandler<TEvent>`.  

```csharp
public class MyEventingHandler : BaseEventHandler<MyEvent>
{
    public override async Task CallbackAsync(MyEvent myEvent, bool isRedelivered)
    {
        await Task.CompletedTask;

        Console.WriteLine(myEvent.Text);
    }
}
```

> ⚠️ Be aware that all `BaseEventHandler<TEvent>` implementations must be non-generic, or they will be ignored during startup registration.  

While it is possible to implement `IEventingHandler<MyEvent>` directly, it is generally not recommended. Doing so requires manually implementing two properties that 
are handled automatically in the base class. By deriving from `BaseEventHandler<TEvent>` instead, these properties are managed for you and can optionally 
be set via the constructor.  

```csharp
public class MyEventingHandler() : BaseEventHandler<MyEvent>(routingKey: null, overridePrefetchCount: null)
{
    ...
}
```

In most cases, specifying the `routingKey` is unnecessary, but it allows the same event model to be consumed by different receivers if needed. 
This feature should be used sparingly, as it supports advanced or conditional event consumption.  

The `overridePrefetchCount` allows an event handler to override the globally configured Eventing.PrefetchCount for a specific handler. 
This is useful when an event requires more processing or resources, as a lower prefetch count can help prevent the consuming application from being overloaded.
