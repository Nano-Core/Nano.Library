# Nano.App
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/)
[![NuGet](https://img.shields.io/nuget/v/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/)

> _Common features for all types of Nano application._

> ⚠️ This NuGet is included in other Nano Packages, and is not meant to be included directly.

***

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Environment](#environment)
* [Configuration](#configuration)
  * [Null Logger](#null-logger)
  * [Api Clients](#api-clients)
* [Start-Up Tasks](#start-up-tasks)
* [Custom Services](#custom-services)
* [Custom Middleware](#custom-middleware)
* [Custom Configuration Sections](#custom-configuration-sections)

## Summary
Applications are the core part of Nano.  
In Nano, an application refers to the part of defining, building and running a host process.  

Conrete application implementations derive from `BaseNanoApplication` and implements the `IApplication` interface, following the common Nano application patterns 
and providing a concrete implementation for building applications with Nano. It provides convenient static methods to create and configure the application with 
sensible defaults, while allowing full customization of services through the `ConfigureServices` method. This design ensures that all core applications behaviors 
are initialized consistently using you configuration, reducing boilerplate code and simplifying the setup of new applications.  

Three concrete application implementations are avaialble in Nano:

| Application                                                                              | Description                                                          |
| ---------------------------------------------------------------------------------------- | -------------------------------------------------------------------- |
| [Nano API](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api)           | API application exposing endpoints and optional static pages.        |
| [Nano Console](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console)   | Console application that runs a job and exits.                       |
| [Nano Web](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Web)           | Web application exposing endpoints and dynamic Razor/Blazor pages.   |

## Environment
By design, Nano is environment-neutral: it does not rely on environment-specific code or behavior.  
Environment-specific behavior is defined solely through configuration files, such as `appsettings.{environment}.json`.

.NET Core reads the `DOTNET_ENVIRONMENT` or `ASPNETCORE_ENVIRONMENT` variable and applies the corresponding configuration. By default, the environment is set to `Development`.  
It is recommended to keep code environment-neutral and handle differences through configuration and deployment pipelines.

Nano supports three standard environments:

| Environment     | Type   | Description                  |
| --------------- | ------ | ---------------------------- |
| `Development`   | Local  | Local development machine.   |
| `Staging`       | Cloud  | Cloud Kubernetes deployment. |
| `Production`    | Cloud  | Cloud Kubernetes deployment. |

> ⚠️ Additional environments can easily be added by updating deployment settings and corresponding configuration files.

## Configuration
Nano follows the standard .NET Core configuration pattern, loading key/value pairs from configuration providers.  
By default, Nano uses `appsettings.json`, but individual settings can be overridden using environment variables, command-line arguments, or user secrets.

The order of precedence for configuration sources is as follows (later items override earlier ones):

1. App Settings  
   a. `appsettings.json`  
   b. `appsettings.{environment}.json`  
2. Command-Line Arguments (`args`) 
3. Environment Variables  
4. User Secrets (`Development` environment only)

## Null Logger
Nano automatically registers a `NullLogger`, ensuring that `ILogger` and related logging services are available even if no logging provider has been configured.  
With the `NullLogger`, all log messages are discarded, so no logs are persisted.  

This is intended as a safety fallback.  

> 📖 Learn more about [Nano Logging Providers](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging).  

## Api Clients
Nano provides a generic api-client implementation, that can be used by other services to seamlessly connect and communicate with your micro-service. The implementation consists of an 
abstract ```BaseApi``` class implementation, from which the concrete and specific implementation of the micro-service can be derived. The base class provides methods for accessing all 
controllers and actions, using generic methods and conventions.  
The constructor of the ```BaseApi``` implementation, requires an instance of ```ApiOptions```. When deriving from a concrete implementation from ```BaseApi```, it's automatically 
registered during startup as dependency and may be injected throughout the application. The ```ApiOptions```, get resolved automatically as well, reading the values 
from ```appsettings.json```. For each api create a sections, named identical to the concrete class implementation of ```BaseApi```.

```csharp 
public class MyApi : DefaultApi
{
    public MyApi(ApiOptions options)
        : base(options)
    {

    }
}
``` 

It's possible to derive form the ```BaseApi``` abstract implementation, though this will leave the api-client empty, expect for any custom method implementations. 
Typically, this is the case when the default controller isn't used in the application, and all controller implementations are derived from ```BaseController```.  

SHOW CONFIG JSON AND TABLE

```json
"MyApi": {
    "Host": "localhost",
    "Root": "api",
    "Port": 80,
    "UseSsl": false,
    "TimeoutInSeconds": 30,
    "UseHealthCheck": true,
    "UnhealthyStatus": "Degraded",
    "LogIn": {
      "Username": null,
      "Password": null
    }
}
```
  
Methods are available for all nano controller actions, such as finding, querying, adding, deleting and updating. Additionally, the api-client contains a custom action implementation, 
where both controller and action can be set at execution-time.  

Authentication:
Authentication happens under-the-hood, when using the api-client implementation. By default, the jwt-token of the authorization request header, is passed along. 
If the service being invoked, shares the same issuer, audience and secret key, the token can be used for authorization. If the endpoint is anonymous, an authentication 
request is first made, using the login defined in the ```ApiOptions```, and that token is used to invoke the request. This is to ensure that if the outer service is anonymous, 
it allows invocations to the nested service.  

Url Conventions:
When Nano is building the url to invoke, the base part is made up from the properties on ```ApiOptions```, like this.
* http({UseSsl})://{Host}:{Port}/{Root}/  

The controller part, relates to the generic entity type, specified as generic type parameter, when invoking one of the api-client methods. The pluralized name of the entity 
is used for the name of the controller. Thus, having an model named ```User```, the corresponding controller should be named ```UsersController```.  
Be aware, that when Nano pluralizes the name of the entity, just and 's' is appended to the name. So in order to use the api-client properly, always name your controllers 
like: ```{EntityTypeName}s```.  

Responses:
Define a json response, and for files use either ```Stream``` or ```NamedStream``` as response.  

Auth:
Auth api inheriting

```csharp 
public class MyApi : AuthApi
{
    public MyApi(ApiOptions options)
        : base(options)
    {

    }
}
```

Identity:
When identity is used in the application, and one or more controllers derived from ```IdentityController<TEntity, TCriteria>```, derived your api-client implementation from ```IdentityApi```, to gain access to all the same methods as it's counter-part controller implementation.

```csharp 
public class MyApi : IdentityApi
{
    public MyApi(ApiOptions options)
        : base(options)
    {

    }
}
```

## Start-Up Tasks
Nano supports running background jobs during application start-up.  
A failing startup task that throws an unhandled exception will cause the application to shut down. Startup tasks must complete successfully for the application to run.

Implement startup tasks by deriving from the abstract `BaseStartupTask` or implementing the `IStartupTask` interface. Dependencies are automatically registered and 
executed during application start-up. For convenience, deriving from `BaseStartupTask` allows you to implement only `OnStartAsync()` if no custom logic 
is needed in `OnStopAsync()`.  

```csharp
public class MyStartUpTask(ILogger logger) 
    : BaseStartupTask(logger)
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

You can inject any registered service your startup task needs, including scoped services, which will be correctly resolved when the startup task is executed.  

## Custom Services
All Nano application types support registering custom services.  
Simply add your services during the `ConfigureServices(...)` step when building the application in `Program.cs`.  

Each application type’s documentation provides detailed guidance on how to configure and build the application, customize services, and where to place them.

```csharp
...
    .ConfigureServices(services =>
    {
        // Your services...
    })
...
```

## Custom Middleware
Custom middleware works similarly to custom services.  
During the `Build(...)` step in `Program.cs`, add your middleware to the `IApplicationBuilder` delegate parameter.  

> ⚠️ All custom middleware is appended to the end of the middleware pipeline registered by Nano.

```csharp
...
    .Build(builder =>
    {
        // Your middleware..
    })
...
```

## Custom Configuration Sections
Extending Nano with custom configuration sections is straightforward and integrates seamlessly with existing Nano configuration.

Use the `IServiceCollection` extension `AddConfigOptions<TOption>(...)` to register your custom configuration section. The generic type TOption defines 
the object model into which the section will be deserialized.

To add a custom configuration section, first define an options model that represents the structure of your configuration section.

```csharp
public class MySectionModel
{
    // Properties...
}
```

Next, Add a matching section to `appsetings.json`.

```json
{
    "MySection": {
    }
}
```

Last, register the section as options.

```csharp
.ConfigureServices(services =>
{
    services
        .AddNanoConfigSection<MySectionModel>("MySection", out var options);
})
```

The options are returned for use in further service registration, and it is now also available for dependency injection through the `IOptions<T>` and related interfaces.  
