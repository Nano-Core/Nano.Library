# Nano.App
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/)
[![NuGet](https://img.shields.io/nuget/v/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/)

> _Shared components for all types of Nano application._

## Table of Contents
* [Summary](#summary)
* [Environment](#environment)
* [Configuration](#configuration)
* [Custom Configuration](#custom-configuration)
* [Null Logger](#null-logger)
* [Start-Up Tasks](#start-up-tasks)
* [Api Client](#api-client)
* [Custom Application](#custom-applications)
* [Examples](#examples)

## Summary
In Nano, an application refers to the part of defining, building and running a host process.  
The implementation isn't much different from the regular approach, when building .Net Core applications. The ```WebHost``` is being configured, then built and finally executed. During startup dependencies are registered, and components are configured and initialized. The supplied configuration (```appsettings.json```), allows for a fine-grained control over the behavior of these dependencies and components.  
A few abstractions and interfaces are used to control an application. This resembles the ```startup.cs``` class you would normally implement in .Net core, but includes much more. The interface ```IApplication``` defines an application. When building the ```WebHost``` of the application, the interface is injected as dependency, resolving to the type implemented in your application. The abstract class ```BaseApplication```, implements ```IApplication```, and most importantly, it contains a static method ```ConfigureApp<TApplication>(...)```, that is responsible for initializing and configuring the application.  
Nano comes with a concrete ```DefaultApplication``` implementation, deriving directly from ```BaseApplication```. That is in most cases sufficient, avoiding having to implement a custom implementation.    

## Environment
By design Nano is environment neutral, and does neither operate with any environment specific configuration, nor has any environment specific behavior. Everything is defined by having different configurations for different environments, like 
`appsettings.{environment}.json`. 

.Net Core reads the environment variable `ASPNETCORE_ENVIRONMENT`, and applies the corresponding configuration. The environment defaults to `Development`. It's recommended to keep the code environment neutral, and handle differences in configuration and in the deployment pipeline.  

## Configuration
The configuration that is the same for all application types.

The configuration follows the regular .Net Core pattern, populating key/values from a configuration provider. 
Nano defaults to use ```appsettings.json``` as provider, but all or individual settings may be overridden from either environmental variables, 
command-line parameters, or secrets.

The order of which the different ways of specifying configuration variables is traversed. The later overrides the former.
1. App Settings
2. Command-Line Arguments
3. Environmental Variables
4. User Secrets (Development environment only)


## Custom Configuration
Extending the configuration and adding custom sections, obtaining the same registration and behavior as the existing Nano sections, is straight forward.  
Nano provides the ```IServiceCollection``` extension method ```.AddOptions<TOption>(...)```, registering the dependency of a custom configuration section. The ```TOption``` generic type paramter, defines the object model, the section will be deserialized into.  

First, implement an options model.
```csharp
public class MyOptions
{
    // Properties...
}
```
Second, add a section to ```appsetings.json```, matching the structure of the options model above.
```json
{
    "MySection": {
    }
}
```
Last, register the dependency in ```Program.Main()```, passing section name.
```csharp
.ConfigureServices(x =>
{
    x.AddConfigOptions<MyOptions>("mySection");
})
```

## Null Logger
Nano automatically registers a `NullLogger`. That ensures that `ILogger` and realted logging services are available even when no Logging Provider 
has been included in the solution. Logs will be sent to the void and lost so it's not recommended.

## Start-Up Tasks
Nano supports running background jobs upon start-up.  
Derive an implementation from the abstract ```BaseStartupTask```, and the task dependency will automatically be registered and started during application start-up.  

The ```BaseStartupTask``` implements ```IHostedService``` abstractly, and contains a mechanism for controlling the number of background tasks running, ensuring proper completion when application shuts down.  

```csharp
public class MyStartUpTask : BaseStartupTask
{
    public MyStartUpTask(StartupTaskContext startupTaskContext) 
        : base(startupTaskContext)
    {
    }

    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
```

## Api-Client
Nano provides a generic api-client implementation, that can be used by other services to seamlessly connect and communicate with your micro-service. The implementation consists of an abstract ```BaseApi``` class implementation, from which the concrete and specific implementation of the micro-service can be derived. The base class provides methods for accessing all controllers and actions, using generic methods and conventions.  
The constructor of the ```BaseApi``` implementation, requires an instance of ```ApiOptions```. When deriving from a concrete implementation from ```BaseApi```, it's automatically registered during startup as dependency and may be injected throughout the application. The ```ApiOptions```, get resolved automatically as well, reading the values from ```appsettings.json```. For each api create a sections, named identical to the concrete class implementation of ```BaseApi```.

```csharp 
public class MyApi : DefaultApi
{
    public MyApi(ApiOptions options)
        : base(options)
    {

    }
}
``` 

It's possible to derive form the ```BaseApi``` abstract implementation, though this will leave the api-client empty, expect for any custom method implementations. Typically, this is the case when the default controller isn't used in the application, and all controller implementations are derived from ```BaseController```.  

### Configuration
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
  
Methods are available for all nano controller actions, such as finding, querying, adding, deleting and updating. Additionally, the api-client contains a custom action implementation, where both controller and action can be set at execution-time.  

Authentication:
Authentication happens under-the-hood, when using the api-client implementation. By default, the jwt-token of the authorization request header, is passed along. If the service being invoked, shares the same issuer, audience and secret key, the token can be used for authorization. If the endpoint is anonymous, an authentication request is first made, using the login defined in the ```ApiOptions```, and that token is used to invoke the request. This is to ensure that if the outer service is anonymous, it allows invocations to the nested service.  

Url Conventions:
When Nano is building the url to invoke, the base part is made up from the properties on ```ApiOptions```, like this.
* http({UseSsl})://{Host}:{Port}/{Root}/  

The controller part, relates to the generic entity type, specified as generic type parameter, when invoking one of the api-client methods. The pluralized name of the entity is used for the name of the controller. Thus, having an model named ```User```, the corresponding controller should be named ```UsersController```.  
Be aware, that when Nano pluralizes the name of the entity, just and 's' is appended to the name. So in order to use the api-client properly, always name your controllers like: ```{EntityTypeName}s```.  

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

## Custom Applications
For the most part it should not be needed to implement custom application types in Nano.
Situations where you need a way different middleware setup, or other special requirements it is possible through a custom application implemenation 
deriving from `IApplication` and `BaseApplication`.

```csharp
public class MyApplication : DefaultApplication 
{ 
    public MyApplication(IConfiguration configuration)
        : base(configuration)
    {

    }
}

public class Program
{
    public static void Main()
    {
        WebApplication
            .ConfigureApp()
            .ConfigureServices(x =>
            {
                // Additional dependencies...
            })
            .Build()
            .Run();
    }
}
```

## Examples