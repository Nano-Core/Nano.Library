# Nano.App
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/)
[![NuGet](https://img.shields.io/nuget/v/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/)


### Table of Contents

* [Null Logger]()















### Table of Contents
* [Summary](#summary)
* [Configuration](#configuration)
* [Start-Up](#start-up)
* [Build and Run](#build-and-run)
* [Background Jobs](#background-jobs)
* [Injected Services](#injected-services)

*** 

### Summary
In Nano, an application refers to the part of defining, building and running a host process.  

The implementation isn't much different from the regular approach, when building .Net Core applications. The ```WebHost``` is being configured, then built and finally executed. During startup dependencies are registered, and components are configured and initialized. The supplied configuration (```appsettings.json```), allows for a fine-grained control over the behavior of these dependencies and components.  

A few abstractions and interfaces are used to control an application. This resembles the ```startup.cs``` class you would normally implement in .Net core, but includes much more. The interface ```IApplication``` defines an application. When building the ```WebHost``` of the application, the interface is injected as dependency, resolving to the type implemented in your application. The abstract class ```BaseApplication```, implements ```IApplication```, and most importantly, it contains a static method ```ConfigureApp<TApplication>(...)```, that is responsible for initializing and configuring the application.  

Nano comes with a concrete ```DefaultApplication``` implementation, deriving directly from ```BaseApplication```. That is in most cases sufficient, avoiding having to implement a custom implementation.    

*** 

### Configuration
The ```appsettings.json``` contains the configurational information used by Nano, when initializing and building the application.  

The ```App``` section of the configuration defines behavior related to the application. The section is deserialized into an instance of ```AppOptions```, and injected as dependency during startup, thus available for injection throughout the application.  

See [Appendix - App Settings](app-settings) for details about the app section and the meaning of the variables.

##### App Section
```json
"App": {
  "Name": "Application",
  "EntryPoint": "Application.dll",
  "Description": null,
  "Version": "1.0.0.0",
  "TermsOfService": null,
  "DefaultTimeZone": "UTC",
  "Cultures": {
    "Default": "en-US",
    "Supported": [
      "en-US"
    ]
  }
}
```

*** 

### Start-Up
The application implementation in nano, is essentially the ```startup.cs``` class of .Net Core.  

The ```DefaultApplication``` implementation is the default startup class, and normally deriving your own implementation isn't needed. In fact, it's recommended not to, and instead configure any custom initialization within the ```.ConfigureServices(...)``` inline expression, as explained later. Is a more advanced setup required, and implementing a custom startup class can't be avoided, derive the implementation from one of the provided application classes, that in turn derives from ```DefaultApplication```, and override any methods as needed. For example, in order to inject custom middleware.   
Remember to invoke base method implementations!  

##### Sample Implementation
```csharp
public class MyApplication : DefaultApplication 
{ 
    public MyApplication(IConfiguration configuration)
        : base(configuration)
    {

    }
}
```

*** 

### Build and Run
This covers the entry point of the application, and how the startup implementation is applied and executed.  

The ```DefaultApplication``` doesn't provide any initialization alone, and using it directly would manually require to declare and instantiate one of the ```IHostBuilder``` implementations of .Net Core. Nano provides application implementations for a ```WebApplication``` and a ```ConsoleApplication```.  

The application implementation should include a static method to retrieve an implementation of ```IHostBuilder```. The included application implementations already includes such a method, ```.ConfigureApp(...)```. Invoke the method from the ```Main(...)``` method of ```Program``` class, and configure any additional dependencies inline in the ```ConfigureServices(x => ...)``` method. 

When having a custom application implementation, either implement your own static method returning the appropriate ```IHostBuilder``` implementation, or use the method of one of the included application implementations. The later, can be accomplished by passing the custom application type as generic parameter, to the ```.ConfigureApp<TApplication>(...)``` method overload of one of the included application implementations, as shown below.

##### Sample implementation
```csharp
// With default IApplication implementation
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

// With custom IApplication implementation
public class Program
{
    public static void Main()
    {
        WebApplication
            .ConfigureApp<MyApplication>()
            .ConfigureServices(x =>
            {
                // Additional dependencies...
            })
            .Build()
            .Run();
    }
}
```

***

### Background Jobs
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

***






### Table of Contents
* [Summary](#summary)
* [Environment](#environment)
* [Override Variables](#override-variables)
* [Custom Sections](#custom-sections)
* [Injected Services](#injected-services)

***
 
### Summary
The configuration follows the regular .Net Core pattern, populating key/values from a configuration provider. Nano defaults to use ```appsettings.json``` as provider, but all or individual settings may be overridden from either environmental variables or command-line parameters, as detailed later.  

The configuration is divided into several sections, each controlling the behavior of different parts of Nano. The structure and properties of each section corresponds to an ```{section}Option``` implementation, and is initialized and populated during application start-up, thus available for later dependency injection, if needed. Most settings comes with a default value, and may be omitted. For improved clarity, it's recommended to include them anyway.  

Each section and their variables are described in detail, in [Appendix - App Settings](app-settings).  

***

### Environment
By design Nano is environment neutral, and does neither operate with any environment specific configuration, nor has any environment specific behavior. Everything is defined by having different configurations for different environments, like 
```appsettings.{environment}.json```. 

.Net Core reads the environment variable ```ASPNETCORE_ENVIRONMENT```, and applies the corresponding configuration. The environment defaults to ```Development```. It's recommended to keep the code environment neutral, and handle differences in configuration and in the deployment pipeline.  

*** 

### Override Variables
Configuration variables from ```appsettings.json``` may be overridden.  

This can be accomplished by parsing command-line arguments or setting environmental variables, having the same path|name as the corresponding variable in the file.  

The order of which the different ways of specifying configuration variables is traversed. The later overrides the former.
1. App Settings
2. Command-Line Arguments
3. Environmental Variables
4. User Secrets (Development environment only)

***

### Custom Sections
Extending the configuration and adding custom sections, obtaining the same registration and behavior as the existing Nano sections, is straight forward.  

Nano provides the ```IServiceCollection``` extension method ```.AddOptions<TOption>(...)```, registering the dependency of a custom configuration section. The ```TOption``` generic type paramter, defines the object model, the section will be deserialized into.  

##### Sample Implementation
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























### Table of Contents
* [Summary](#summary)
* [Configuration](#configuration)

*** 

### Summary
Nano provides a generic api-client implementation, that can be used by other services to seamlessly connect and communicate with your micro-service. The implementation consists of an abstract ```BaseApi``` class implementation, from which the concrete and specific implementation of the micro-service can be derived. The base class provides methods for accessing all controllers and actions, using generic methods and conventions.  

The constructor of the ```BaseApi``` implementation, requires an instance of ```ApiOptions```. When deriving from a concrete implementation from ```BaseApi```, it's automatically registered during startup as dependency and may be injected throughout the application. The ```ApiOptions```, get resolved automatically as well, reading the values from ```appsettings.json```. For each api create a sections, named identical to the concrete class implementation of ```BaseApi```.

##### Sample Implementation
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

#### Authentication
Authentication happens under-the-hood, when using the api-client implementation. By default, the jwt-token of the authorization request header, is passed along. If the service being invoked, shares the same issuer, audience and secret key, the token can be used for authorization. If the endpoint is anonymous, an authentication request is first made, using the login defined in the ```ApiOptions```, and that token is used to invoke the request. This is to ensure that if the outer service is anonymous, it allows invocations to the nested service.  

#### Url Conventions
When Nano is building the url to invoke, the base part is made up from the properties on ```ApiOptions```, like this.
* http({UseSsl})://{Host}:{Port}/{Root}/  

The controller part, relates to the generic entity type, specified as generic type parameter, when invoking one of the api-client methods. The pluralized name of the entity is used for the name of the controller. Thus, having an model named ```User```, the corresponding controller should be named ```UsersController```.  

Be aware, that when Nano pluralizes the name of the entity, just and 's' is appended to the name. So in order to use the api-client properly, always name your controllers like: ```{EntityTypeName}s```.  

#### Responses
Define a json response, and for files use either ```Stream``` or ```NamedStream``` as response.  

#### Identity
When identity is used in the application, and one or more controllers derived from ```IdentityController<TEntity, TCriteria>```, derived your api-client implementation from ```IdentityApi```, to gain access to all the same methods as it's counter-part controller implementation.
##### Sample Implementation
```csharp 
public class MyApi : IdentityApi
{
    public MyApi(ApiOptions options)
        : base(options)
    {

    }
}
``` 

***