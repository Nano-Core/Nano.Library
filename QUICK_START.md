# Quick Start Guide

> Quick Start guide for Nano applications._

***

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/tree/master/README.md#nanolibrary)**
* **[1. Choosing Application Type](#choosing-application-type)**
* **[2. Application Configuration](#application-configuration)**
* **[3. Adding Logging Provider](#adding-logging-provider)**
* **[4. Adding Data Provider](#adding-data-provider)**
* **[5. Adding Eventing Provider](#adding-eventing-provider)**
* **[6. Adding Storage Provider](#adding-storage-provider)**
* **[7. Implementing Your Application Domain](#implementing-your-application-domain)**
* **[8. Congratz, Launch](#congratz-launch)**

## Choosing Application Type
The first step is to choose the application type that best fits your use case. Nano currently supports three application types: **[Api](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api/README.md#nanoapi)**, 
**[Web](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Web/README.md#nanoweb)**, and **[Console](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console/README.md#nanoconsole)**. Each application 
type is provided as a _blank_ solution template. These templates include the essential project structure, configuration, and dependencies required to get started, without 
adding unnecessary complexity.  

To begin, copy or clone the solution that matches your chosen application type.  

- **[Api._Blank](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api._Blank)**
- **[Console._Blank](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console._Blank)**
- **[Web._Blank](https://github.com/Nano-Core/Nano.Lessons/tree/master/Web._Blank)**

These templates provide a minimal starting point for each application type.

Then rename the solution and projects to fit your application, updating namespaces and identifiers as needed throughout the files.  

At this point, you have a fully functional Nano baseline solution, capable of running locally and deploying to Kubernetes via GitHub Actions. For a detailed overview of the 
included projects, files, and overall structure, see **[Solution Composition](https://github.com/Nano-Core/Nano.Library/tree/master/README.md#solution-composition)**.  

The `program.cs` looks like this.  

```csharp
NanoApiApplication      // NanoWebpplication, NanoConsoleApplication 
    .ConfigureApp()
    .ConfigureServices(services =>
    {
        // Your services.
    })
    .Build()
    .Run();
```

## Configuring Your Application
The next step is configuring your application. This varies depending on the application type. API and Web applications typically include a significantly larger configuration 
surface than Console applications. API and Web applications use the configuration structure shown below. The example reflects the default setup, where many configuration 
sections are set to `null`. Features are enabled on an opt-in basis by configuring the relevant sections as needed.

```json
  "App": {
    "Version": "1.0.0.0",
    "ShutdownTimeout": 10,
    "Hosting": {
      "Root": "api",
      "Http": {
        "Ports": [
          8080
        ]
      }
    },
    "HttpPolicyHeaders": {
      "ContentType": null,
      "ReferrerPolicy": null,
      "FrameOptions": null,
      "XssProtection": null,
      "Csp": null,
      "Cors": null,
      "Hsts": null,
      "Robots": null,
      "ForwardedHeaders": null
    },
    "ResponseCache": null,
    "ResponseCompression": null,
    "Session": null,
    "TimeZone": null,
    "Localization": null,
    "HealthCheck": null,
    "Documentation": null,
    "VirusScan": null,
    "ErrorHandling": {
      "ExposeErrors": false
    },
    "Authentication": {
      "Jwt": null
    },
    "Apis": {
    }
  }
````

> 📖 Learn more about **[Nano Api Configuration](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api/README.md/README.md#configuration)**.

For Console applications, the configuration is minimal and straightforward.  

```json
"App": {
  "Version": "1.0.0.0",
  "LocalizationOptions": null,
  "Apis": { }
}

```

> 📖 Learn more about **[Nano Console Configuration](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console/README.md#configuration)**.

## Adding Logging Provider
Now that the application has been created and configured, the next step is to add providers that extend the commonly used capabilities of your Nano application.

First, most applications, if not all, require a **[Logging Provider](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging/README.md#nanologging)**. Register it in 
the `ConfigureServices(...)` method in `Program.cs`.

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoLogging<TProvider>();
})
...
```

The `TProvider` defines the logging implementation used by your application. Nano supports multiple providers, all producing console-based output with a consistent logging 
format. The choice of provider is primarily a matter of preference and existing familiarity.

| Provider                                                                                                                       | Type                  | Description                                                                                                                     |
| ------------------------------------------------------------------------------------------------------------------------------ | --------------------- | --------------------------------------------------------------------------------------- |
| **[Log4Net](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging.Log4Net/README.md#nanologginglog4net)**         | `Log4NetProvider`     | Console logging using a log4net-based implementation.                                   |
| **[Microsoft](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging.Microsoft/README.md#nanologgingmicrosoft)**   | `MicrosoftProvider`   | Console logging using the built-in Microsoft.Extensions.Logging abstractions.           |
| **[NLog](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging.NLog/README.md#nanologgingnlog)**                  | `NLog`                | Console logging using an NLog-based implementation.                                     |
| **[Serilog](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging.Serilog/README.md#nanologgingserilog)**         | `SerilogProvider`     | Console logging using a Serilog-based implementation with structured logging support.   |

The logging configuration is straightforward, with sensible default values provided, as shown below.
 
```json
"Logging": {
  "LogLevel": "Information",
  "LogLevelOverrides": [
  ]
}
```

> 📖 Learn more about **[Logging Configuration](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging/README.md#configuration)**.

## Adding Data Provider
Moving on to the **[Data Provider](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md)**. This provider is more optional than logging, but still required by 
most applications.

Register the Nano Data Provider in the `ConfigureServices(...)` method in `Program.cs`.

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddDataContext<TProvider, TContext>();
})
...
```

The `TProvider` defines the data implementation used by your application. Nano supports multiple providers, each integrating with different relational database systems 
while exposing a consistent abstraction layer.

| Provider                                                                                                                   | Type                  | Description                                                                                                                     |
| -------------------------------------------------------------------------------------------------------------------------- | --------------------- | ------------------------------------------------------------------------ |
| **[InMemory](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.InMemory/README.md#nanodatainmemory)**        | `InMemoryProvider`    | In-memory data provider intended for testing and lightweight scenarios.  |
| **[MySql](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.MySql/README.md#nanodatamysql)**                 | `MySqlProvider`       | MySQL data provider using Pomelo.EntityFrameworkCore.MySql.              |
| **[PostgreSQL](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.PostgreSQL/README.md#nanodatapostgresql)**  | `PostgreSqlProvider`  | PostgreSQL data provider using Npgsql.                                   |
| **[SqLite](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.SqLite/README.md#nanodatasqlite)**              | `SqLiteProvider`      | SQLite data provider.                                                    |
| **[SqlServer](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.SqlServer/README.md#nanodatasqlserver)**     | `SqlServerProvider`   | SQL Server data provider.                                                |

The `TContext` defines the Nano `BaseDbContext` implementation. You must create an Entity Framework `DbContext` that derives from `BaseDbContext`.

```csharp
public class MyDbContext(DbContextOptions contextOptions, IOptionsMonitor<DataOptions> dataOptions)
    : BaseDbContext(contextOptions, dataOptions);
```

For most providers, database migrations are also required. To support this, you must add a factory implementation derived from `BaseDbContextFactory<TProvider, TContext>`. The 
example below shows a factory using the `MySqlProvider`.

```csharp
public class MyDbContextFactory : BaseDbContextFactory<MySqlProvider, MySqlDbContext>;
```

The data configuration is straightforward and allows features to be easily enabled or disabled. Below is the default Nano data configuration.

```json
"Data": {
  "BatchSize": 25,
  "BulkBatchSize": 500,
  "BulkBatchDelay": 1000,
  "QueryRetryCount": 0,
  "StartupAction": "None",
  "UseLazyLoading": false,
  "UseSensitiveDataLogging": false,
  "QuerySplittingBehavior": "SingleQuery",
  "DefaultCollation": null,
  "ConnectionString": null,
  "Repository": {
    "UseAutoSave": true,
    "QueryIncludeDepth": 4
  },
  "Identity": null,
  "ConnectionPool": null,
  "HealthCheck": null
}
```

Most importantly, the required `ConnectionString` must be configured.  
Also, configuring **[Data Health Check](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#health-check)** is recommended to ensure system observability.  

> 📖 Learn more about **[Data Configuration](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#configuration)**.

Once the data provider has been registered and the configuration is in place, you can start defining Nano entity models and data mappings. Create an entity model by deriving 
from `BaseEntity` and adding properties. Then implement the mapping by creating a class that derives from `BaseEntityMapping<TEntity>`, where `TEntity` is your model type. In 
this mapping class, configure how each property is mapped to Entity Framework.

```csharp
public class MyEntity : BaseEntity
{
    public string Name { get; set; } = null!;
}
```

...and the mapping.  

```csharp
public class MyEntityMapping : BaseEntityMapping<MyEntity>
{
    public override void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Configure(builder);

        builder
            .Property(x => x.Name);
    }
}
```
> ⚠️ Always ensure you call `base.Configure(builder)`, otherwise inherited Nano properties may not function correctly.

More advanced uses of Nano entity models are also available, including support for user entities used with 
**[Data Identity](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#didentity)** features, as well as mapping database views as read-only entities.  

> 📖 Learn more about **[Nano Data Models](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#data-models)** and 
**[Nano Data Mappings](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data#configuration/README.md#data-mappings)**.

For entity models where you want **[Soft Delete](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#soft-delete)** instead of the default hard delete behavior, 
implement the `IEntitySoftDelete` interface on your entity model.

Nano also supports **[Entity Events](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#entity-events)**, enabling asynchronous synchronization of entity models 
between applications. To publish changes for an entity, add the `[Publish]` attribute to the entity model class definition. In the consuming application, create a corresponding 
entity model with the same name and annotate it with the `[Subscribe]` attribute. It will then receive events whenever the entity is created, updated, or deleted. Additional 
properties can be included in published events, making it easy to synchronize commonly shared data across different parts of the system.

Entity events require an Eventing Provider to be registered (see further sections).

Another noteworthy feature of the data provider is the **[`[Include] Attribute`](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#include-annotation)**. Navigation 
properties on entity models can be annotated with this attribute to create a graph-lite structure. When an entity is retrieved through the 
**[Data Repositories](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#repositories)**, all included navigation properties are automatically loaded as part 
of the query.

> 📖 Learn more about the other **[Nano Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md)** features.

## Adding Eventing Provider
Adding an **[Eventing Provider](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing/README.md)** (message queueing) follows the same pattern as the other Nano providers.  

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoEventing<TProvider>();
})
...
```

The `TProvider` defines the eventing implementation used by your application. The supported providers are listed below.  

| Provider                                                                                                                       | Type                | Description                                                                                             |
| ------------------------------------------------------------------------------------------------------------------------------ | ------------------- | ------------------------------------------------------------------------------------------------------- |
| **[RabbitMq](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing.RabbitMq/README.md#nanoeventingrabbitmq)**    | `RabbitMqProvider`  | Message broker-based eventing using RabbitMQ for reliable asynchronous communication between services.  |

And again, the configuration is straightforward.
 
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

Most importantly, the required `Host` and `Credentials` must be configured.  
Also, configuring **[Data Health Check](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing/README.md#health-check)** is recommended to ensure system observability.  

> 📖 Learn more about **[Eventing Configuration](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing/README.md#configuration)**.

When an eventing provider has been registered, it exposes the `IEventing` interface, which can be injected to publish custom event contracts from anywhere in the application.  

Start by defining an event contract.  

```csharp
public class MyEvent
{
    public string Text { get; set; }
}
```

...and then publish it.  

```csharp
await this.Eventing
    .PublishAsync(new MyEvent
    {
        Text = "Message"
    });
```

In another application, create a handler by deriving from `BaseEventHandler<TEvent>` and implement the `CallbackAsync(...)` method. This method is invoked whenever an event of 
type `TEvent` is received.  

```csharp
public class MyEventingHandler() : BaseEventHandler<MyEvent>(routingKey: null, overridePrefetchCount: null)
{
    public override Task CallbackAsync(MyEvent @event, bool isRedelivered, CancellationToken cancellationToken = default)
    {
        ...
    }
}
```

> 📖 Learn more about the other **[Nano Eventing](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing/README.md#nanoeventing)** features.

## Adding Storage Provider
The **[Storage Provider](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage/README.md/README.md#nanostorage)** can be added by registering 
it in the `ConfigureServices(...)` method in `Program.cs`.  

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoStorage<TProvider>();
})
...
```

The `TProvider` defines the storage implementation used by your application. The supported providers are listed below.  

| Provider                                                                                                            | Type                      | Description                                                  |
| ------------------------------------------------------------------------------------------------------------------- | ------------------------- | ------------------------------------------------------------ |
| **[Azure](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage.Azure/README.md#nanostorageazure)**    | `AzureFileshareProvider`  | Nano Storage provider implementation for Azure File Shares.  |
| **[Local](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage.Local/README.md#nanostoragelocal)**    | `LocalFileShareProvider`  | Storage provider for local file system–backed storage.       |

And configuration.
 
```json
"Storage": {
  "ShareName": null,
  "Credentials": {
    "Id": null,
    "Secret": null
  },
  "HealthCheck": null
}
```

Most importantly, the required `ShareName` and `Credentials` must be configured.  
Also, configuring **[Storage Health Check](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage/README.md#health-check)** is recommended to ensure system observability.  

> 📖 Learn more about **[Storage Configuration](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage/README.md#configuration)**.

Once a storage provider has been registered, it exposes the `IPathProvider` interface. This can be injected anywhere in the application to provide easy access to the configured 
root path of the file storage location.  

> 📖 Learn more about the other **[Nano Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage/README.md)** features.

## 8. Implementing Your Application Domain
Now it is time to implement the actual domain of your application. At this stage, you start defining the application-specific behavior and structure. Depending on the application 
type, different building blocks are available. For web-based applications, you typically define **[Controllers](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api/README.md#controllers)**,  
while for console-based applications, you implement **[Console Workers](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console/README.md#console-workers)**.

#### API-based applications
First, here is an example of an entity controller that works with your data model and exposes standard Create, Read, Update, and Delete (CRUD) endpoints.

```csharp
public class MyEntitysController(ILogger<MyEntitysController> logger,IRepository repository, IEventing? eventing) 
    : BaseEntityController<MyEntity, MyEntityQueryCriteria>(logger, repository, eventing)
{
    // Custom Actions
}
```

The `MyEntityQueryCriteria` defines the criteria used for the query action contract of the entity controller. It specifies how entities can be filtered, sorted, and retrieved, 
and is defined as follows.

```csharp 
public class MyEntityQueryCriteria : BaseQueryCriteria
{
    public virtual string? Name { get; set; }

    public override IList<CriteriaExpression> GetExpressions()
    {
        var expressions = base.GetExpressions();

        var expression = expressions.FirstOrDefault() ?? new CriteriaExpression();

        if (!string.IsNullOrEmpty(this.Name))
        {
            expression
                .StartsWith("Name", this.Name);
        }

        expressions
            .Add(expression);

        return expressions;
    }
}
```

Controllers can later be consumed by other applications through the **[Nano Api Client](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App/README.md#api-clients)**. To do this, 
create an implementation that derives from `BaseApiClient` and include it—along with the relevant entity models—in the consuming application. This provides a simple and consistent 
way to connect to the service and use its entity functionality.

#### Console-based applications
Create a worker by deriving from `BaseWorker`, as shown below.  

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
All workers are automatically executed when the application starts and reports ready.

### Launch
🚀 Congratulations your Nano application is ready!  