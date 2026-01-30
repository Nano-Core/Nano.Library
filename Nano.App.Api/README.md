# Nano.App.Api
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.App.Api.svg)](https://www.nuget.org/packages/Nano.App.Api/)
[![NuGet](https://img.shields.io/nuget/v/Nano.App.Api.svg)](https://www.nuget.org/packages/Nano.App.Api/)


## Table of Contents
* [Summary](#summary)
* [Configuration](#configuration)
* [Start-Up Tasks](#start-up-tasks)



## Start-Up Tasks
Nano supports startup-tasks, that executes before the application starts. 
A 'self' startup-healtcheck will report ready when all startup tasks have completed. Only relevant for api applications.

Read more [Nano.App](nano-app#start-up-tasks)







FromFormBody!!! Try the new FromForm.

## Configuration
Nano Api config
Add custom sections


## Registration


## Naming Conventions
IMPORTANT: Controllers must be named the same as their entity pluralized, e.g. MyEntity and MyEntitysController.

































### Table of Contents
* [Summary](#summary)
* [Configuration](#configuration)
* [Injected Services](#injected-services)

*** 

### Summary
The ```WebApplication``` derives from ```DefaultApplication```.  
As the names states, the implementation is for web based applications.  

##### Sample implementation
```csharp
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

*** 

### Configuration
The ```Web``` section of the configuration defines behavior related to the application. The section is deserialized into an instance of ```Apiptions```, and injected as dependency during startup, thus available for injection throughout the application.  

See [Appendix - App Settings](app-settings) for details about the web section and the meaning of the variables.

##### Web Section
```json
  "Web": {
    "Hosting": {
        "Root": "api",
        "Ports": [
        ],
        "PortsHttps": [
        ],
        "ShutdownTimeout": 10,
        "UseHttpsRequired": false,
        "UseForwardedHeaders": true, 
        "UseResponseCompression": true, 
        "UseContentTypeOptions": false, 
        "ExposeErrors": false,
        "ExposeAuditController": true, 
        "ExposeAuthController": true, 
        "Hsts": {
            "IsEnabled": false, 
            "MaxAge": null,
            "UsePreload": false,
            "IncludeSubdomains": false 
        },
        "Csp": {
          "BlockAllMixedContent": false,
          "UpgradeInsecureRequests": false,
          "Defaults": {
            "IsNone": false,
            "IsSelf": false,
            "Sources": [
            ]
          },
          "Styles": {
            "IsNone": false,
            "IsSelf": false,
            "IsUnsafeInline": false,
            "Sources": [
            ]
          },
          "Scripts": {
            "IsNone": false,
            "IsSelf": false,
            "IsUnsafeEval": false,
            "IsUnsafeInline": false,
            "StrictDynamic": false,
            "Sources": [
            ]
          },
          "Objects": {
            "IsNone": false,
            "IsSelf": false,
            "Sources": [
            ]
          },
          "Images": {
            "IsNone": false,
            "IsSelf": false,
            "Sources": [
            ]
          },
          "Media": {
            "IsNone": false,
            "IsSelf": false,
            "Sources": [
            ]
          },
          "Frames": {
            "IsNone": false,
            "IsSelf": false,
            "Sources": [
            ]
          },
          "FrameAncestors": {
            "IsNone": false,
            "IsSelf": false,
            "Sources": [
            ]
          },
          "Fonts": {
            "IsNone": false,
            "IsSelf": false,
            "Sources": [
            ]
          },
          "Connections": {
            "IsNone": false,
            "IsSelf": false,
            "Sources": [
            ]
          },
          "BaseUris": {
            "IsNone": false,
            "IsSelf": false,
            "Sources": [
            ]
          },
          "Children": {
            "IsNone": false,
            "IsSelf": false,
            "Sources": [
            ]
          },
          "Forms": {
            "IsNone": false,
            "IsSelf": false,
            "Sources": [
            ]
          },
          "Manifests": {
            "IsNone": false,
            "IsSelf": false,
            "Sources": [
            ]
          },
          "Workers": {
            "IsNone": false,
            "IsSelf": false,
            "Sources": [
            ]
          },
          "Sandbox": {
            "AllowForms": false,
            "AllowModals": false,
            "AllowOrientationLock": false,
            "AllowPointerLock": false,
            "AllowPopups": false,
            "AllowPopupsToEscapeSandbox": false,
            "AllowPresentation": false,
            "AllowSameOrigin": false,
            "AllowScripts": false,
            "AllowTopNavigation": false
          },
          "PermissionsPolicy": {
            "Accelerometer": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "AmbientLightSensor": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "AutoPlay": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "Battery": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "Camera": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "DisplayCapture": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "DocumentDomain": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "EncryptedMedia": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "ExecutionWhileNotRendered": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "ExecutionWhileOutOfViewport": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "FullScreen": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "Gamepad": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "Geolocation": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "Gyroscope": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "LayoutAnimations": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "LegacyImageFormats": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "Magnetometer": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "Microphone": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "Midi": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "NavigationOverride": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "OversizedImages": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "Payment": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "PictureInPicture": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "PublicKeyCredentialsGet": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "SpeakerSelection": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "SyncXhr": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "UnoptimizedImages": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "UnsizedMedia": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "Usb": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "ScreenWakeLock": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "WebShare": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            },
            "XrSpatialTracking": {
              "IsNone": false,
              "IsSelf": false,
              "Sources": [
              ]
            }
          },
          "ReportUris": [
          ],
          "PluginTypes": [
          ]
        },
        "Cors": {
          "AllowedOrigins": [
          ],
          "AllowedHeaders": [
          ],
          "AllowedMethods": [
          ],
          "AllowCredentials": true,
          "Origin": {
            "EmbedderPolicy": null,
            "OpenerPolicy": null,
            "ResourcePolicy": null
          }
        },
        "Cache": {
            "IsEnabled": true, 
            "MaxSize": 1024,
            "MaxBodySize": 102400,
            "MaxAge": "00:20:00"
        },
        "Robots": {
            "IsEnabled": false, 
            "UseNoIndex": false,
            "UseNoFollow": false,
            "UseNoSnippet": false,
            "UseNoArchive": false,
            "UseNoOdp": false,
            "UseNoTranslate": false,
            "UseNoImageIndex": false
        },
        "Session": {
            "IsEnabled": true, 
            "Timeout": "00:20:00"
        },
        "HealthCheck": {
          "UseHealthCheck": true,
          "UseHealthCheckUI": true,
          "EvaluationInterval": 10,
          "FailureNotificationInterval": 60,
          "MaximumHistoryEntriesPerEndpoint": 50,
          "WebHooks": [
            {
              "Name": null,
              "Url": null,
              "Payload": null
            }
          ] 
        },
        "Certificate": {
            "Path": null,
            "Password": null
        },
        "VirusScan": {
            "IsEnabled": false,
            "Host": "clamav",
            "Port": "3310",
            "UseHealthCheck": true
        },
        "ReferrerPolicyHeader": "Disabled",
        "FrameOptionsPolicyHeader": "Disabled",
        "XssProtectionPolicyHeader": "Disabled"
    },
    "Documentation": {
        "IsEnabled": true,
        "CspNonce": null,
        "UseDefaultVersion": true,
        "Contact": {
            "Name": null,
            "Email": null,
            "Url": null
        },
        "License": {
            "Name": null,
            "Url": null
        }
    }
}
```

***

































### Query Criteria
Nano uses the [DynamicExpression](https://github.com/vivet/DynamicExpression) library to map properties of the query and criteria to a Linq Expression of the entity.  

Query criteria defines a contract for a model. It's used when invoking the ```Query(...)``` method of the controller of the model. Adding properties to the contract and overridding the method ```GetExpression<TEntity>()```, mapping the contract properties to the actual entity, will at runtime dynamically build the ```Expression<T>``` used in Linq queries by Entity Framework (or for that matter any Linq provider).  

Obviously, since query criteria is only used by controller actions, there is no need for them when building console applications.  

#### Query
The base query class contains pagination (number, count) and ordering (by, direction) properties. Naturally, these are used by controller actions and data queries to control the number of returned results as well as the order of which they are sorted.  

#### Criteria
Nano contains a ```DefaultQueryCritiera``` class, implementing the ```IQueryCriteria``` interface, of the  [DynamicExpression](https://github.com/vivet/DynamicExpression) library. It combines a ```Expression<TEntity>``` for the auditable properties ```IsActive```, ```CreateAt``` and ```UpdatedAt```, and expexts a ```DefaultEntity```. If models doesn't derive from ```DefaultEntity```, either derive the query criteria from ```BaseQueryCriteria``` or implement the interface ```IQueryCriteria``` directly.  

So simply, create a class deriving from ```DefaultQueryCritiera```, add criteria properties, and last override the ```GetExpression<TEntity>()``` method mapping the criteria properties to the entity properties, as shown below.  

```csharp 
public class MyQueryCriteria : DefaultQueryCriteria
{
    public virtual string PropertyOne { get; set; }

    public override CriteriaExpression GetExpression<TEntity>()
    {
        var filter = base.GetExpression<TEntity>();

        if (this.PropertyOne != null)
            filter.StartsWith("PropertyOne", this.PropertyOne);

        return filter;
    }
}
```





### Configuration
The ```Security``` section of the configuration defines behavior related to authentication and authorization in the application. The section is deserialized into an instance of ```SecurityOptions```, and injected as dependency during startup, thus available for injection throughout the application.  
```json
"Security": {
    "Jwt": {
        "IsEnabled": true,
        "Issuer": "issuer",
        "Audience": "audience",
        "PublicKey": null,
        "PrivateKey": null,
        "ExpirationInMinutes": 60,
        "RefreshExpirationInHours": 72
    },
    "ExternalLogins": {
        "Google": {
            "ClientId": null,
            "ClientSecret": "N/A",
            "Scopes": [
            ]
        },
        "Facebook": {
            "AppId": null,
            "AppSecret": "N/A",
            "Scopes": [
            ]
        },
        "Microsoft": {
            "TenantId": null,
            "ClientId": null,
            "ClientSecret": "N/A",
            "Scopes": [
            ]
        }
    }
}
```

### Roles & Claims
#### Roles
Name | Type | Description |
---- | ---- | ---- |
Guest | built-in | Currently, not authorized to do anything.
Reader | built-in | Authorized to read.
Writer | built-in | Authorized to read and write
Service | built-in | Authorized to all services.
Administrator | built-in | Full access to everything.
MyRole | custom | Custom role specified during signup or login.

#### Claims
Name | Type | Description |
---- | ---- | ---- |
AppId | built-in | The id of the application. Set during logon, and used for supporting multiple refresh tokens. Default value: "Default"
Id | built-in | The user id.
Email | built-in | The user's email address.
Name | built-in | The username
MyClaim | custom | Custom claim specified during signup or login.

***

### Authentication
Nano supports authenticating with user credentials (username and password), and also using one of the supported external providers.  

A successful authentication returns a Nano ```AccessToken```. 
```json
{
    "AppId": null,
    "UserId": null,
    "Token": null,
    "ExpireAt": null,
    "IsExpired": false,
    "RefreshToken": {
        "Token": null,
        "ExpireAt": null,
        "IsExpired": false,
    },
}
```

The ```jwt-token``` contains the following claims and values.  
```json
{
  "appId": "Default",
  "jti": "74ec40fe-bb18-4bd6-8ec5-51b37f9c8a5c",
  "sub": "08d9da95-9a3b-4ec6-83b0-fa11da6bae7e",
  "name": "admin@domain.com",
  "email": "admin@domain.com",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": [
    "service",
    "administrator"
  ],
  "nbf": 1111111111,
  "exp": 1111111111,
  "iss": "development.nano",
  "aud": "development.nano"
}
```

See [Controller Authentication](controllers#authentication) for details about how to authenticate with your web application.   

#### Api Key
It's also possible to authenticate using an api-key, by setting the header ```x-api-key``` with a valid api-key.  

Api-keys can be managed through the ``IdentityManager```.  

#### External Providers
Nano supports the following external providers.

Name | Type | Description
-------------|-------------|-------------
Google | Implicit | Google authentication using implicit flow. No token refresh possible.
Facebook | Implicit | Facebook authentication using implicit flow. No token refresh possible.
Microsoft | Auth-Code | Microsoft authentication using auth code flow.

When logging in with an external provider through a single-page-application, Nano finalizes the flow by validating the external response. On successful validation a Nano  jwt-token is created, wherein the retrieved external access-token is embedded, should it later be needed to authorize against the external provider.  

Scopes for ```id```, ```email``` and ```username``` should be enabled for external providers.  

***

### Root Log-In
Nano comes with a built-in administrator, defined in the [Security Section](#configuration), as shown below. The administrator user will be created during start-up, if it doesn't already exist.

The administrator has unrestricted permissions, and may access any part of the application. 
```json
"Security": {
  "User": {
    "AdminUsername": "admin",
    "AdminPassword": "<<secret>>",
  }
}
```

***










































### Table of Contents
* [Summary](#summary)
* [Object Model](#object-model)
* [Inherited Actions](#inherited-actions)
* [Injected Dependencies](#injected-dependencies)
* [Authentication](#authentication)
* [Authorization](#authorization)
* [Model Validation](#model-validation)
* [Error Handling](#error-handling)
* [Security](#security)
* [TimeZone](#timezone)
* [Virus Scan](#virus-scan)
* [Localization](#localization)
* [Documentation](#documentation)
* [Versioning](#versioning)
* [Custom Headers](#custom-headers)
* [Content Type Negotiation](#content-type-negotiation)
* [Serialization / Deserialization](#serialization--deserialization)
* [Health Checks](#health-checks)
*** 

### Summary
Nano includes a few base controller classes, providing derived functionality for model model validation, content-type negotiation, exception handling and more. Furthermore, deriving controllers inherit a rich set of action methods for adding, updating, deleting and querying the model implemented by the controller.  

Additionally, Nano includes the following concrete controllers.
* **```AuditController```** Read-only actions for exposing audit logging entries, if audit is enabled.    
* **```AuthController```** Login and logoff actions.  
* **```IdentityController```** Identity management actions.  

***

### Object Model
The ```BaseController<TRepository, TEntity, TIdentity, TCriteria>``` abstract class implementation, is as shown defined by four generic type parameters. 
* ```TRepository```, defines the underlying implementation of the ```IRepository``` interface. 
* ```TEntity```, defines the entity of the controller.
* ```TIdentity```, defines the identity type used by the entity related to finding entity by a unique identifier.
* ```TCriteria```, defines the query criteria implementation related to querying the entity.  

Normally, there is no reason to derive implementations directly from the ```BaseController<TRepository, TEntity, TIdentity, TCriteria>```. Deriving from the ```DefaultController<MyEntity, MyQueryCriteria>```is equivalent to ```BaseController<DefaultRepository, TEntity, Guid, TCriteria>```, where as shown, the ```TRepository``` parameter is defined as ```DefaultRepository```, and ```TIdentity```is of type ```System.Guid```. 

In situations where only read-only actions is permitted on the model associated with a controller, Nano has the ```DefaultControllerReadOnly``` to support that. When deriving from that, only query actions are inherited. Additionally, Nano also provides default controller implementations for ```Creatable-```, ```Updatable-``` and ```DeletableController```, if needed.  

##### Object Model Diagram
![Controller Classes](https://raw.githubusercontent.com/wiki/Nano-Core/Nano.Library/Images/Controller-Classes.png)

Additionally, some other controllers exists in Nano as well. These controllers differ in the generic parameter types, to allow for a more flexible use. For example, if no data repository is required by the application, a controller exists where the generic parameter ```TRepository``` is omitted.  


##### Sample Implementation
```csharp 
public class MyController : DefaultController<MyEntity, MyQueryCriteria>
{
    public MyController(ILogger logger, IRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    { }
}
```

#### Identity Controller
The ```IdentityController<TEntity, TCriteria>``` contains methods for creating and managing identities used to authenticate with the application. Derive a controller implementation from ```IdentityController<TEntity, TCriteria>```, and inherit all the identity actions.  

The ```IdentityController<TEntity, TCriteria>``` derives from ```DefaultControllerUpdatable<TEntity, TCriteria>```,  allowing identities to only be updated (and fetched, naturally), but may only be created through the inherited identity action ```signup```.  

##### Sample Implementation
```csharp 
public class MyController : IdentityController<MyEntity, MyQueryCriteria>
{
    public MyController(ILogger logger, IRepository repository, IEventing eventing, IdentityManager identityManager)
        : base(logger, repository, eventing, identityManager)
    { }
}
```

*** 

### Inherited Actions
When deriving a controller implementation from ```DefaultController<MyEntity, MyQueryCriteria>```, a rich set of action methods are inherited through the ```BaseControllerReadOnly<MyEntity, MyQueryCriteria>``` and the ```BaseControllerWriteable<MyEntity, MyQueryCriteria>``` abstract controller implementations. 

##### Routing
Routing requests to controller actions, is done using the default convention, as shown below 
* ```http(s)://{host}:{port}/{root}/{controller}/{action}/{Id?}```

##### Read-Only Actions
* ```Index (GET / POST)```, get a list of models (pagination, ordering).
* ```Details (GET)```, get details about a model.
* ```Details Many (POST)```, get details about many models.
* ```Query (GET / POST)```, query for many models (criteria, pagination, ordering).
* ```Query First (GET / POST)```, query for first model (criteria, pagination, ordering).
* ```Query Count (GET / POST)```, query for number of models (criteria).

##### Writable Actions
* ```Create (POST)```, create a model.
* ```Create And Get (POST)```, create a model and reloads the model.
* ```Create Many (POST)```, create many models.
* ```Create Many Bulk (POST)```, create bulk many models.
* ```Edit (POST / PUT)```, update a model.
* ```Edit And Get (POST / PUT)```, update a model and reloads the model.
* ```Edit Many (POST / PUT)```, update many models.
* ```Edit Many Query (POST / PUT)```, update many models based on a query.
* ```Edit Many Bulk (POST / PUT)```, update bulk many models.
* ```Delete (POST / DELETE)```, gets the html 'delete' view.
* ```Delete (POST / DELETE)```, delete a model.
* ```Delete Many (POST / DELETE)```, delete many models.
* ```Delete Many Query (POST / DELETE)```, delete many models based on a query.
* ```Delete Many Bulk (POST / DELETE)```, delete bulk many models.

##### Auth Actions
* ```login``` authenticating a user and returns an access token (jwt).  
* ```login/refresh``` refreshes the login and creates a new access token (jwt).  
* ```login/external/direct``` sign-in a user, from data received from a separate external authentication.  
* ```login/external/direct/transient``` sign-in a user transient, from data received from a separate external authentication.  
* ```login/external/google``` sign-in a user, using Google external provider.  
* ```login/external/google/transient``` sign-in a user, using Google external provider.  
* ```login/external/facebook``` sign-in a user, using Facebook external provider.  
* ```login/external/facebook/transient``` sign-in a user, using Facebook external provider.  
* ```login/external/microsoft``` sign-in a user, using Microsoft external provider.  
* ```login/external/microsoft/transient``` sign-in a user, using Microsoft external provider.  
* ```external/google/data``` get external data and access token from Google. Use with ```login/external/direct```.  
* ```external/facebook/data``` get external data and access token from Facebook. Use with ```login/external/direct```.  
* ```external/microsoft/data``` get external data and access token from Microsoft. Use with ```login/external/direct```.  
* ```external/schemes``` returns all configured external login providers.

##### Identity Actions
* ```signup``` Registers a new user.
* ```signup/external/direct``` registers a new user using an external login provider data retrieved separately. Use with ```external/{provider}/data```
* ```signup/external/google``` sign-up a user, using Google external provider.
* ```signup/external/facebook``` sign-up a user, using Facebook external provider.
* ```signup/external/microsoft``` sign-up a user, using Microsoft external provider.
* ```username/set``` Sets a username for a user.
* ```password/set``` Sets a password for a user.
* ```password/reset``` Resets the password of a user.
* ```password/reset/token``` Generates an reset password token for a user. 
* ```password/change``` Changes the password of a user.
* ```password/options``` Get the password options.
* ```email/is-taken``` checks if an email address is already taken.
* ```email/change``` Changes the email address of a user
* ```email/change/token``` Generates an change email token for a user.
* ```email/confirm``` Confirms the email of a user.
* ```email/confirm/token``` Generates an email confirmation token for a user.
* ```phone/is-taken``` checks if an phone number is already taken.
* ```phone/change``` Changes the phone number of a user
* ```phone/change/token``` Generates an change phone token for a user.
* ```phone/confirm``` Confirms the phone of a user.
* ```phone/confirm/token``` Generates an phone confirmation token for a user.
* ```external-logins/{userId}``` Gets the external logins of a user.
* ```external-logins/add/google``` Add Google external logins to a user.
* ```external-logins/add/facebook``` Add Facebook external logins to a user.
* ```external-logins/add/microsoft``` Add Microsoft external logins to a user.
* ```external-logins/remove``` Removes the external login of a user.
* ```roles/{id}``` gets roles of a user.
* ```roles/assign``` assign a role to a user.
* ```roles/remove``` remove a role from a user.
* ```roles/claims``` get claims of a role.
* ```roles/claims/assign``` assign a claim to a role. 
* ```roles/claims/remove``` assign a claim from a role.
* ```roles/claims/replace``` replace a claim from a role.
* ```claims``` get claims of a user.
* ```claims/assign``` assign a claim to a user. 
* ```claims/remove``` assign a claim from a user.
* ```claims/replace``` replace a claim from a user.
* ```activate``` activates a user.
* ```deactivate``` deactivates a user.
* ```delete``` delete a user.
* ```delete/many``` delete many users.

***

### Injected Dependencies
Nano controllers has three dependencies injected into the constructor, all of which is registered when building the application. Derived controller implementations can add further injections as needed.  
* ```ILogger```, is the interface for logging in the controller. 
* ```IRepository```, is the interface for get, add, update, delete and query data in the controller.
* ```IEventing```, is the interface for publishing events in the controller.

*** 

### Authentication
When authentication has been enabled and configured, and the application is running, users authenticate in order to gain access to the controllers and their actions. Nano has a ```AuthController``` implementation, responsible for this.  

Nano supports various methods of authentication as described in [Security - Authentication](security#authentication) section.  

***

### Authorization
When a user successfully authenticates, a jwt-token is returned to the client. Subsequent requests must include the ```Authorization``` header, containing the value ```Baerer {token}```.  See [JWT Explained](https://jwt.io/introduction/) for further details.  

In Nano the ```HttpContext``` is extended with methods, that decrypts the authorization token, and extracts specific claim values.  
* ```GetIsAnonymous()``` Returns whether the user is anonymous.  
* ```GetJwtAppId()``` Returns app id.  
* ```GetJwtToken()``` Returns the complete token.  
* ```GetJwtUserId()``` Extracts the user-id of the logged in user.  
* ```GetJwtUserName()``` Extracts the user username of the logged in user.  
* ```GetJwtUserEmail()``` Extracts the user email address of the logged in user.  
* ```GetJwtExternalToken()``` Extracts the external provider access token, if logged in with external provider.  

All methods returns null, when not authenticated and no authorization token is passed along with the request.  

#### Roles
By default, authorization to controller actions is handling by the built-in roles and policies defined by Nano. Controllers may be decorated with the ```AuthorizeAttribute```, and allow to override the default authorization and use custom defined roles and policies.  

See [Security - Authorization](Security#authorization) for further details about authorization.  

***

### Model Validation
When deriving a controller implementation from ```BaseController```, model validation is automatically enabled. Based on the annotations (attributes) which model properties is decorated with. When a model fails validation, a bad request with the validation errors is returned.  

Other than that, then validation isn't any different from normal.  

See the official Microsoft documentation here: [Model Validation Documentation](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-2.1)  

*** 

### Error Handling
When an exception or other errors occurs for a request, and ```500 Internal Server Error``` is returned to the client, contain ```Error``` response, as shown below.  

```csharp
public class Error
{
    public virtual string Summary { get; set; } // Summary of the error.
    public virtual string[] Exceptions { get; set; } // Array of exceptions.
    public virtual int StatusCode { get; set; } // The http status code.
    public virtual bool IsTranslated { get; set; } // Indicates if the exception messages is translated.
    public virtual bool IsCoded { get; set; } // Indicates if the exception messages is coded.
}
``` 

If ```Error.IsTranslated``` is true, then the consumer can expect the Exceptions to be translated to the language matching the Current CultureInfo, unless translations in that language is not available, and the default translation is used.  

If ```Error.IsCoded``` is true, then the consumer can expect the Exceptions to be a code, that can be used to map an error message for the user.  

***

### Security
Several security configuration options allows for detailed control over the security setup of the application.  

#### Secure Transport Layer (SSL)
First, by specifying a https port, will enable secure socket layer communication. Also a certificate path and password must be specified in the configuration, in order to have a valid security protocol enabled. If the application should only support secure connections, configuring the https redirect option, will redirect traditional insecure http requests to the https protocol.  

#### Header Protection
Additionally, the configuration supports various options, for controlling transport-security, XXS-protection, cache control, as well as content-type and download restrictions. Check out the [Web Section](App-Settings#web-section) of the configuration for detailed on how to enable the supported security protocols.  

***

### TimeZone
Nano supports the built in methods for specifying the timezone when invoking requests.  

See the official documentation about timezone here: [TimeZone Documentation](https://github.com/vivet/Vivet.AspNetCore/tree/master/Vivet.AspNetCore.RequestTimeZone#vivetaspnetcorerequesttimezone)  

***

### Virus Scan
Nano supports virus scan by specifying a network connection to a clamav instance.  
All uploaded files will be scanned, and a ```VirusScanException``` will be thrown if one or more files contains any virus or malware.  

See the official documentation about virus scan here: [Virus Scan Documentation](https://github.com/vivet/Vivet.AspNetCore/tree/master/Vivet.AspNetCore.RequestVirusScan#vivetaspnetcorerequestvirusscan)  

***

### Localization
Nano supports the built in methods for specifying the language when invoking requests.  

See the official Microsoft documentation about localization here: [Localization Documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.1)  

***

### Documentation
When documentation is enabled in the configuration file, a web-interface documenting the service, it's endpoints and it's models - is created and deployed.  

The documentation is based on [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).  

***

### Versioning
During application startup, Nano registers dependencies for enabling api versioning. Obviously, since most controller actions will be inherited from one or more of the base controller implementations, versioning has to be annotated in derived controller classes. Additionally, versioned action methods must be overridden in the derived controller, as the versioning would otherwise apply to all derived controller implementations.  

Clients can specify the version in the following ways:
* Route segment (```vx```)
* Query parameter (```api-version```)
* Http header (```X-Api-Version```)

Besides that, versioning follows the standard .Net Core approach.  

See the official Microsoft documentation here: [Versioning Documentation](https://docs.microsoft.com/en-us/dotnet/core/versions/)  

***

### Custom Headers
During application startup, Nano initializes middleware that adds custom headers to all action responses.  

The custom headers is listed below.
* ```RequestId``` (a unique trace identifier of the request, added to all logging context)

Additionally, headers related to authentication is appended as well, but may differ depending on the chosen security scheme.  

*** 

### Content-Type Negotiation 
Nano supports several different formats (listed below) for the requests and responses of the controller actions.  

The format, also known as content-type may be specified, either through the ```Content-Type``` or   ```Accept``` header, or by appending the following querystring parameter: ```?format={format}```. The later is default by ([Microsoft Formatting](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/formatting)), by any of the three methods will work with Nano.

##### Supported formats
* Json

***

### Serialization / Deserialization
Nano has a custom contract serializer implementation.  

The serializer derives from the regular implementation, but removes empty lists and system properties, related to soft-deletion and lazy-loading for instance. The serializer applies to deserializing incoming requests, though this process does nothing out of the ordinary. It also applies when serializing models into response content.  

##### JsonSerializerSettings
```csharp
var settings = new JsonSerializerSettings
{
    MaxDepth = 128,
    Culture = CultureInfo.CurrentCulture,
    NullValueHandling = NullValueHandling.Ignore,
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    PreserveReferencesHandling = PreserveReferencesHandling.None,
    ContractResolver = new EntityContractResolver()
};
```

When serializing responses, if lazy-loading entities is enabled in data configuration, it's disabled. This ensures that data will not be lazy fetched when the serializer navigates relational properties, but only data existing in the change-tracker will be serialized.  

The serializer will not serialize navigations that is of type ```IEntity```, except when they are annotated with ```IncludeAttribute```. This is to avoid returning unwanted navigation references, that is automatically added if dependent navigations are loaded separately.  

***

### Health Checks
When enabling health-checks in the web section of the confiugration, the application will be configured with a health-check. The health status of the application, can be found here:  
* ```http://{host}:{port}/healthz```  

If the health check UI is also enabled in the confiugration, an interface for monitoring the health of the application, as well as any enabled health checks for dependent providers, can be found here:  
* ```http://{host}:{port}/healthz-ui```  

***