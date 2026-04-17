# Nano.App
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/)
[![NuGet](https://img.shields.io/nuget/v/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/)

> _Common features for all types of Nano application._

> ⚠️ This NuGet is transitive and included in other Nano Packages, and is not meant to be included directly.

***

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library#nano-library)**
* **[Summary](#summary)**
* **[Environment](#environment)**
* **[Configuration](#configuration)**
  * **[Null Logger](#null-logger)**
  * **[Api Clients](#api-clients)**
* **[Start-Up Tasks](#start-up-tasks)**
* **[Custom Services](#custom-services)**
* **[Custom Middleware](#custom-middleware)**
* **[Custom Configuration Section](#custom-configuration-section)**

## Summary
Applications are the core part of Nano.  
In Nano, an application refers to the part of defining, building and running a host process.  

Conrete application implementations derive from `BaseNanoApplication` and implements the `IApplication` interface, following the common Nano application patterns 
and providing a concrete implementation for building applications with Nano. It provides convenient static methods to create and configure the application with 
sensible defaults, while allowing full customization of services through the `ConfigureServices` method. This design ensures that all core applications behaviors 
are initialized consistently using you configuration, reducing boilerplate code and simplifying the setup of new applications.  

Three concrete types application are avaialble in Nano:

| Application   | Documentation                                                                           | Minimal Example                                                                                  |
| ------------- | --------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| Nano API      | [Documentation](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api)     | [Example](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api._Blank)      |
| Nano Console  | [Documentation](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console) | [Example](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console._Blank)  |
| Nano Web      | [Documentation](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Web)     | [Example](https://github.com/Nano-Core/Nano.Lessons/tree/master/Web._Blank)      |

## Environment
By design, Nano is environment-neutral: it does not rely on environment-specific code or behavior.  
Environment-specific behavior is defined solely through configuration files, such as `appsettings.{environment}.json`.

.NET Core reads the `DOTNET_ENVIRONMENT` or `ASPNETCORE_ENVIRONMENT` variable and applies the corresponding configuration. By default, the environment is set to `Development`.  
Generally, it's recommended to keep code environment-neutral and handle differences through configuration and deployment pipelines.

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

Nano handles empty configuration sections, differently than the regular .NET configuration. Empty sections are mapped with all default configuration values, in contrary to 
setting the whole section to null in the configuration. Another improvements is how Nano handles `appsettings.{environment}.json` overrides. In regular .NET configuration, 
setting an entire JSON section to null is ignored. Nano supports this scenario, allowing a section defined in `appsettings.json` to be set to null in an environment-specific 
configuration file, effectively overriding and removing that configuration for the environment. 

## Null Logger
Nano automatically registers a `NullLogger`, ensuring that `ILogger` and related logging services are available even if no logging provider has been configured.  
With the `NullLogger`, all log messages are discarded, so no logs are persisted.  

This is intended as a safety fallback.  

> 📖 Learn more about [Nano Logging Providers](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging).  

## Api Clients
Nano provides a generic api-client implementation, that can be used by other Nano applications to seamlessly connect and communicate with your application. 

First, the application must implement and api-client, and naturally only api-based application types can meaningfully implement it. So this is not relevant for Console 
applications. Derive an implementation from either `BaseApiClient` or `BaseApiClient<TIdentity>` and implement the constructor. Alternatively, if your application is an identity application where 
an identity user model has been implemented, instead derive from `BaseIdentityApiClient<TUser>` or `BaseIdentityApiClient<TUser, TIdentity>`, where the `TUser` generic parameter is 
the user type in your application. The classes contains groups of methods for invoking built-in Nano api methods in the application. The base class provides methods for 
accessing all controllers and actions, using generic methods and conventions.  

A minimal example of an api-client implementation is shown below.  

```csharp 
public class MyApiClient(ApiClient apiClient) : BaseApiClient(apiClient);
``` 

or with Identity.  

```csharp 
public class MyIdentityApiClient(ApiClient apiClient) : BaseIdentityApiClient(apiClient);
``` 

You don't actually need to do more, but read further down in this section and learn how to add custom api-clients methods with custom request and responses for custom endpoints 
in the application.

Second, the application consuming the api-client must add a configuratuon to `appsettings.json`, detailing the connection information and behavior. The configuration is a 
dictionary, and you may add as many api-clients for other applications as needed, just ensure that the key names are unique.

| Setting                        | Type      | Default    | Description                                                                                                                 |
| ------------------------------ | --------- | ---------- | --------------------------------------------------------------------------------------------------------------------------- |
| `Host`                         | string    | localhost  | The API host address.                                                                                                       |
| `Root`                         | string    | api        | The root path for the API endpoints.                                                                                        |
| `Port`                         | int       | 80         | The port to connect to on the host.                                                                                         |
| `UseSsl`                       | bool      | false      | Indicates whether to use SSL (HTTPS) for the connection.                                                                    |
| `Timeout`                      | TimeSpan  | 00:00:30   | The request timeout duration.                                                                                               |
| `LogInRoot`                    | object    | null       | Optional login root configuration for authentication.                                                                       |
| `LogInRoot.Username`           | object    | null       | Optional login root configuration for authentication.                                                                       |
| `LogInRoot.Password`           | object    | null       | Optional login root configuration for authentication.                                                                       |
| `HealthCheck`                  | object    | null       | Optional health check configuration for the API client.                                                                     |
| `HealthCheck.UnhealthyStatus`  | enum      | Unhealthy  | The health status reported when the api is unavailable. _Only relevant for `NanoApiApplication` and `NanoWebApplication`_.  |

```json
"App": {
  "Apis": {
    "MyApiClient": {
      "Host": "localhost",
      "Root": "api",
      "Port": 80,
      "UseSsl": false,
      "Timeout": "00:00:30",
      "LogInRoot": {
        "Username": null,
        "Password": null
      }
      "HealthCheck": {
        "UnhealthyStatus": Unhealthy
      }
    }
  }
}
```

All api-client implementations are automatically registered with the options specified in the configuration during startup, and may be injected as needed.

Now the application has access to three groups of endpoints in forms of properties inherited from the `BaseApiClient`, Auth, Audit and Entity. Implementation deriving 
from `BaseIdentityApiClient` also has access to the Idenity group. Each group has endpoints relative to the name and their purpose. Endpoints not enabled or disabled by 
configuration in the api application of the api-client will return 404 if invoked. For example idf authentication is disabled, and Auth group endpoints is invoked, a 404 response
will be received.

Entity: Methods are available for all nano controller actions, such as finding, querying, adding, deleting and updating. Additionally, the api-client contains a custom action implementation, where both controller and action can be set at execution-time.  
The controller part, relates to the generic entity type, specified as generic type parameter, when invoking one of the api-client methods. The pluralized name of the entity 
is used for the name of the controller. Thus, having an model named ```User```, the corresponding controller should be named ```UsersController```.  
Be aware, that when Nano pluralizes the name of the entity, just and 's' is appended to the name. So in order to use the api-client properly, always name your controllers 
like: ```{EntityTypeName}s```.  

| Setting                         | Parameters    | Description                                                                     |
| ------------------------------- | ------------- | ------------------------------------------------------------------------------- |
| `IndexAsync<TEntity>`           | IndexRequest  | Invokes the 'index' endpoint of the `TEntity` in the api.  |



Auth:
LISTS OF METHODS

Identity: When identity is used in the application, and one or more controllers derived from ```IdentityController<TEntity, TCriteria>```, derived your api-client implementation from ```IdentityApi```, to gain access to all the same methods as it's counter-part controller implementation.
LISTS OF METHODS

It also possible, and in fact very easy, to add custom endpoints to the api-client. First define a custom request, and also optionally a custom response. 
The request must derive from `BaseRequest` and have one of the action atributes annotated. It defines both the http method and the required `action` parameter defines the relative route of the request. Nano takes 
care of building the base part of the route. Naturally both must match your custom endpoint. 

Nano support the following Action attributes.  

| Setting          | Http Method |
| ---------------- | ----------- |
| `OptionsAction`  | OPTIONS     |
| `HeadAction`     | HEAD        |
| `GetAction`      | GET         |
| `QueryAction`    | QUERY       |
| `PostAction`     | POST        |
| `PutAction`      | PUT         |
| `PatchAction`    | PATCH       |
| `DeleteAction`   | DELETE      |
| `ConnectAction`  | CONNECT     |

Next add properties to the request, and annotate them with attributes for `[Route]`, `[Body]`, `[Query]` and `[Header]`.

| Attribute   | Description                                                                                                                                                                                                                                                                                                                                                                                   |
| ----------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `[Header]`  | Defines a header key/value that will be added to the request headers. The attribute contains two optional properties, the `Name` may be used to override the header key, that otherwise uses the property name, and the `ValuePrefix` as the names suggests, prefixes the value set for the header.                                                                                           |
| `[Route]`   | Defines parameters for the route. Multiple properties may have the `[Route]` annotation, and the optional paramteer defines the order of which parameters should be replaced in the route of the `[Action]` annotation on the class.                                                                                                                                                          |
| `[Query]`   | Defines querystring parameters. These should be scalar types. The optional `Name` parameter overrides the querystring parameter name, that is otherwise the name of property.                                                                                                                                                                                                                 |
| `[Body]`    | Defines the body of the request. It should be a complex serializable type. Basically, you create a class contract holding the data of your request body, and Nano will serialize it when invoking the request. `NetTopologySuite` geometry types are supported and works with nano api-clients. Also Nano's built-in types for `Query`, `Pagination` and `Ordering` are supported as well.    |
| `[Form]`    | Defines a form field to be included. Properties must be sclar or for files one of the following types (or IEnumerable of that): `IFormFile`, `FileInfo`, `FileStream`, `Stream` or `NamedStream`. > ⚠️ Complex objects is also supported but that requires the use of [`[FromFormBody]`](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api#request-multipart-json)  on your controllers.                                                                                                            |

Below shows an implemenation derived from `BaseRequest`.

```csharp 
[PostAction("{id}/custom")]
public class MyCustomRequest : BaseRequest
{
    [Route(Order = 0)]
    public string Id { get; set; }

    [Body]
    public MyBody Body { get; set; }

    [Query(Name = "MyQuery")]
    public string Query { get; set; }

    [Header(Name = "CustomHeader", ValuePrefix = "Custom-")]
    public string Header { get; set; }
}
``` 

Next, define and optional response, and for file responses use either `Stream` or `NamedStream` as response, whereas the later will preserve the filename. 
It's also the pluralized type name of the response that defines the controller part of the route. Nano automatically infers this, but for custom request that doesn't have a response
type contract, you must manually set the `request.Controller` property in your request class constructor.

`Accept` Headers and all `X-Forwarded-` are automatically transferrred from the current httpcontext if present, and passed along in the request. The same goes for `Authorization` and 
`X-Api-Key` header, as well as other built-in Nano headers. The list of headers is subject to change, but Nano api-client will always ensure that the request parameters set on outer 
requests, get transffered to internal requests when appropriate.

Authentication happens under-the-hood, when using the api-client implementation. By default, the jwt-token of the authorization request header, is passed along. 
If the service being invoked, shares the same issuer, audience and secret key, the token can be used for authorization. If the endpoint is anonymous, an authentication 
request is first made, using the login defined in the ```ApiOptions```, and that token is used to invoke the request. This is to ensure that if the outer service is anonymous, 
it allows invocations to the nested service.  

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

> ⚠️ Custom middleware is supported only by API and Web applications. In Console applications, the `builder` delegate is ignored.  

## Custom Configuration Section
Extending Nano with custom configuration sections is straightforward and integrates seamlessly with existing Nano configuration.

Use the `IServiceCollection` extension `AddConfigOptions<TOption>(...)` to register your custom configuration section. The generic type TOption defines 
the object model into which the section will be deserialized. You may register as many custom sections as you like, as long as they don't conflict with the 
built-in sections in Nano, `App`, `Logging`, `Data`, `Eventing`, and `Storage`.  

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
