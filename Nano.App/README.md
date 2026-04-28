# Nano.App
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/)
[![NuGet](https://img.shields.io/nuget/v/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/)

> _Common features for all types of Nano application._

> ⚠️ This NuGet is transitive and included in other Nano Packages, and is not meant to be included directly.

***

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/blob/master/README.md#nanolibrary)**
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

Three concrete types application are avaialble in Nano.  

| Application   | Documentation                                                                                                  | Minimal Example                                                                                  |
| ------------- | -------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| Nano API      | [Documentation](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api/README#nanoappapi)          | [Example](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api._Blank)      |
| Nano Console  | [Documentation](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console/README#nanoappconsole)  | [Example](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console._Blank)  |
| Nano Web      | [Documentation](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Web/README#nanoappweb)          | [Example](https://github.com/Nano-Core/Nano.Lessons/tree/master/Web._Blank)      |

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

> 📖 Learn more about **[Nano Logging Providers](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging/README.md#nanologging)**.  

## Api Clients
Nano provides a generic API client implementation that allows other Nano applications to seamlessly connect and communicate with your application over HTTP.  

This feature is intended for API-based application types. Console applications do not implement API clients, since they do not expose HTTP endpoints, but they can still consume 
API clients from other applications. To create an API client, derive from either `BaseApiClient` or `BaseApiClient<TIdentity>` and implement the required constructor. If your 
application uses Nano **[Identity](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#identity)**, you should instead derive from `BaseIdentityApiClient<TUser>` or 
`BaseIdentityApiClient<TUser, TIdentity>`, where `TUser` represents the `IEntityUser` model defined in your application and `TIdentity` represents the identity type.  

The base classes provide a structured set of methods for invoking built-in Nano API endpoints. They expose controllers and actions through a generic, convention-based approach, 
allowing you to call endpoints without manually constructing HTTP requests. This ensures strongly typed communication and consistent integration patterns across Nano applications.  

A minimal example of an API client implementation is shown below.  

```csharp 
public class MyApiClient(ApiClient apiClient) : BaseApiClient(apiClient)
{
}
``` 

or with Identity.  

```csharp 
public class MyIdentityApiClient<TUser>(ApiClient apiClient) : BaseIdentityApiClient(apiClient)
{
}
``` 

You do not need to implement anything beyond this to get a fully working API client. However, you can continue reading to learn how to extend the client with custom methods, 
including custom request and response types for your own application-specific endpoints. You can also explore the built-in API client methods for Nano entity models, audit, 
authentication, and identity.

The application consuming the API client must add a configuration entry in `appsettings.json` that defines the connection information and behavior. The configuration is structured 
as a dictionary, allowing multiple API clients for different applications to be registered. Each entry must match the type name of the api-client implementation and must be unique 
to avoid conflicts.

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

All API client implementations are automatically registered during startup using the options defined in the configuration. They can then be injected and used wherever needed through 
dependency injection.  

The application has access to several groups of endpoints exposed as properties on the `BaseApiClient`: Entity, Auth, and Audit. Implementations deriving from `BaseIdentityApiClient` also 
have access to the Identity group. Each group contains endpoints organized by their respective domain and purpose. Endpoints that are not enabled in the API application via configuration 
will return a 404 response if invoked. For example, if authentication is disabled and an endpoint from the Auth group is called, the request will result in a 404 response.  

The Entity method group in the API client provides generic methods for working with your **[entity data models](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#data-models)** 
through the **[Entity Controllers](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api/README.md#controllers)** in your application. It exposes methods for all standard Nano controller 
actions, including get, query, add, edit, and delete. The `TEntity` generic parameter defines the entity type used when invoking a method and also determines the corresponding controller 
route segment. For example, using `MyEntity` will target the `/MyEntitys/` route, following Nano’s controller naming conventions.  

The following entity methods are available. Additional overloads are also provided, but have been omitted here for simplicity.  

| Setting                 | Parameters               | Description                                                                        |
| ----------------------- | ------------------------ | ---------------------------------------------------------------------------------- |
| `IndexAsync`            | IndexRequest             | Executes `index` for an entity type to retrieve a collection of entities.          |
| `DetailsAsync`          | DetailsRequest           | Executes `details` for an entity type to retrieve a single entity by request.      |
| `DetailsManyAsync`      | DetailsManyRequest       | Executes `details/many` to retrieve multiple entities by request.                  |
| `QueryAsync`            | QueryRequest             | Executes `query` to retrieve matching entities by request.                         |
| `QueryFirstAsync`       | QueryFirstRequest        | Executes `query/first` to retrieve the first matching entity by request.           |
| `QueryCountAsync`       | QueryCountRequest        | Executes `query/count` to count matching entities by request.                      |
| `CreateAsync`           | CreateRequest            | Invokes the `create` endpoint and returns the created entity.                      |
| `CreateOrEditAsync`     | CreateRequest            | Invokes the `create/edit` endpoint and returns the created or edited entity.       |
| `CreateOrGetAsync`      | CreateOrGetRequest       | Invokes the `create/get` endpoint and returns the created or existing entity.      |
| `CreateAndGetAsync`     | CreateAndGetRequest      | Invokes the `create/reload` endpoint and returns the created entity.               |
| `CreateManyAsync`       | CreateManyRequest        | Invokes the `create/many` endpoint for multiple entities.                          |
| `CreateManyBulkAsync`   | CreateManyBulkRequest    | Invokes the bulk `'create/many/bulk` endpoint.                                     |
| `EditAsync`             | EditRequest              | Updates an entity via the `edit` endpoint and returns the updated entity.          |
| `EditAndGetAsync`       | EditAndGetRequest        | Updates an entity via the `edit/get` endpoint and returns the updated entity.      |
| `EditManyAsync`         | EditManyRequest          | Updates multiple entities via the `edit/many` endpoint.                            |
| `EditManyBulkAsync`     | EditManyBulkRequest      | Bulk updates entities via the `edit/many/bulk` endpoint.                           |
| `EditQueryAsync`        | EditQueryRequest         | Updates entities matching a query via the `edit/query` endpoint.                   |
| `EditQueryBulkAsync`    | EditQueryBulkRequest     | Bulk (batch) updates entities matching a query via the `edit/query/bulk` endpoint. |
| `DeleteAsync`           | DeleteRequest            | Deletes a single entity using the `delete` endpoint.                               |
| `DeleteManyAsync`       | DeleteManyRequest        | Deletes multiple entities using the `delete/many` endpoint.                        |
| `DeleteManyBulkAsync`   | DeleteManyBulkRequest    | Bulk deletes multiple entities using the `delete/many/bulk` endpoint.              |
| `DeleteQueryAsync`      | DeleteQueryRequest       | Deletes entities matching a query using the `delete/query` endpoint.               |
| `DeleteQueryBulkAsync`  | DeleteQueryBulkRequest   | Bulk (batch) deletes entities using a query via the `delete/query/bulk` endpoint.  |

A dedicated method group is provided for Audit, exposing read-only operations for `AuditEntry<TIdentity>`.  

| Setting                                   | Parameters           | Description                                                                    |
| ----------------------------------------- | -------------------- | ------------------------------------------------------------------------------ |
| `IndexAsync<TIdentity>`                   | IndexRequest         | Executes `audit/index` to retrieve a filtered or paged set of audit entries.   |
| `DetailsAsync<TIdentity>`                 | DetailsRequest       | Executes `audit/details` to retrieve a single audit entry.                     |
| `DetailsManyAsync<TIdentity>`             | DetailsManyRequest   | Executes `audit/details/many` to retrieve multiple audit entries.              |
| `QueryAsync<TCriteria>`                   | QueryRequest         | Executes `audit/query` to retrieve audit entries matching criteria.            |
| `QueryFirstAsync<TCriteria>`              | QueryFirstRequest    | Executes `audit/query/first` to retrieve the first matching audit entry.       |
| `QueryCountAsync<TCriteria>`              | QueryCountRequest    | Executes `audit/query/count` to count matching audit entries.                  |

The following methods are available for Auth operations.  

| Setting                          | Parameters                    | Description                                                                        |
| -------------------------------- | ----------------------------- | ---------------------------------------------------------------------------------- |
| `GetExternalSchemesAsync`        | GetExternalSchemesRequest     | Executes `auth/external-schemes` to retrieve available external login providers.   |
| `LogInAsync`                     | LogInRequest                  | Executes `auth/login` to authenticate a user and obtain an access token.           |
| `LogInRootAsync`                 | LogInRootRequest              | Executes `auth/login/root` to authenticate using root credentials.                 |
| `LogInApiKeyAsync`               | LogInApiKeyRequest            | Executes `auth/login/apikey` to authenticate using an API key.                     |
| `LogInExternalAsync`             | BaseLogInExternalRequest      | Executes `auth/login/external` to authenticate via an external provider.           |
| `LogInExternalTransientAsync`    | BaseLogInExternalRequest      | Executes `auth/login/external/transient` using a transient external flow.          |
| `LogInRefreshAsync`              | LogInRefreshRequest           | Executes `auth/login/refresh` to refresh an access token.                          |
| `LogOutAsync`                    | -                             | Executes `auth/logout` to invalidate the current session or token.                 |

Finally, for API client implementations that support Identity, the following methods are available through the Identity method group.  

| Setting                                   | Parameters                         | Description                                                                                            |
| ----------------------------------------- | ---------------------------------- | ------------------------------------------------------------------------------------------------------ |
| `DetailsDeactivatedAsync`                 | DetailsDeactivatedRequest          | Executes `identity/details/deactivated` to retrieve a deactivated entity by identifier.                |
| `GetPasswordOptionsAsync`                 | -                                  | Executes `identity/password/options` to retrieve password policy configuration.                        |
| `IsEmailAddressTakenAsync`                | IsEmailAddressTakenRequest         | Executes `identity/email/taken` to determine whether an email address is already registered.           |
| `IsPhoneNumberTakenAsync`                 | IsPhoneNumberTakenRequest          | Executes `identity/phone/taken` to determine whether a phone number is already registered.             |
| `SignUpAsync`                             | SignUpRequest                      | Executes `identity/signup` to register a new user.                                                     |
| `SignUpExternalAsync`                     | BaseSignUpExternalRequest          | Executes `identity/signup/external` to register a new user via an external provider.                   |
| `SetUsernameAsync`                        | SetUsernameRequest                 | Executes `identity/username` to set or update the username for a user.                                 |
| `SetPasswordAsync`                        | SetPasswordRequest                 | Executes `identity/password/set` to set an initial password for a user.                                |
| `ChangePasswordAsync`                     | ChangePasswordRequest              | Executes `identity/password/change` to change an existing user password.                               |
| `GetResetPasswordTokenAsync`              | GenerateResetPasswordTokenRequest  | Executes `identity/password/reset/token` to generate a password reset token.                           |
| `ResetPasswordAsync`                      | ResetPasswordRequest               | Executes `identity/password/reset` to reset a user password using a token.                             |
| `GetChangeEmailTokenAsync`                | GenerateChangeEmailTokenRequest    | Executes `identity/email/change/token` to generate a change email token.                               |
| `ChangeEmailAsync`                        | ChangeEmailRequest                 | Executes `identity/email/change` to update the user's email address.                                   |
| `GetConfirmEmailTokenAsync`               | GenerateConfirmEmailTokenRequest   | Executes `identity/email/confirm/token` to generate an email confirmation token.                       |
| `ConfirmEmailAsync`                       | ConfirmEmailRequest                | Executes `identity/email/confirm` to confirm a user's email address.                                   |
| `GetChangePhoneTokenAsync`                | GenerateChangePhoneTokenRequest    | Executes `identity/phone/change/token` to generate a phone change token.                               |
| `ChangePhoneAsync`                        | ChangePhoneRequest                 | Executes `identity/phone/change` to update the user's phone number.                                    |
| `GetConfirmPhoneTokenAsync`               | GenerateConfirmPhoneTokenRequest   | Executes `identity/phone/confirm/token` to generate a phone confirmation token.                        |
| `ConfirmPhoneAsync`                       | ConfirmPhoneRequest                | Executes `identity/phone/confirm` to confirm a user's phone number.                                    |
| `GetCustomPurposeTokenAsync`              | GenerateCustomPurposeTokenRequest  | Executes `identity/custom/token` to generate a custom purpose token.                                   |
| `ConfirmCustomPurposeTokenAsync`          | ConfirmCustomPurposeRequest        | Executes `identity/custom/confirm` to validate a custom purpose token.                                 |
| `ActivateUserAsync`                       | ActivateUserRequest                | Executes `identity/user/activate` to activate a user account.                                          |
| `DeactivateUserAsync`                     | DeactivateUserRequest              | Executes `identity/user/deactivate` to deactivate a user account.                                      |
| `GetUserRolesAsync`                       | GetUserRolesRequest                | Executes `identity/user/roles` to retrieve all roles assigned to a user.                               |
| `AssignUserRoleAsync`                     | AssignUserRoleRequest              | Executes `identity/user/roles/assign` to assign a role to a user.                                      |
| `RemoveUserRoleAsync`                     | RemoveUserRoleRequest              | Executes `identity/user/roles/remove` to remove a role from a user.                                    |
| `GetUserClaimsAsync`                      | GetUserClaimsRequest               | Executes `identity/user/claims` to retrieve all claims assigned to a user.                             |
| `AssignUserClaimAsync`                    | AssignUserClaimRequest             | Executes `identity/user/claims/assign` to assign a claim to a user.                                    |
| `ReplaceUserClaimAsync`                   | ReplaceUserClaimRequest            | Executes `identity/user/claims/replace` to replace an existing user claim.                             |
| `AssignOrReplaceUserClaimAsync`           | AssignOrReplaceUserClaimRequest    | Executes `identity/user/claims/assign-or-replace` to upsert a user claim.                              |
| `RemoveUserClaimAsync`                    | RemoveUserClaimRequest             | Executes `identity/user/claims/remove` to remove a claim from a user.                                  |
| `GetExternalLoginsAsync`                  | GetExternalLoginsRequest           | Executes `identity/external-logins` to retrieve linked external login providers for a user.            |
| `AddExternalLoginAsync`                   | BaseAddExternalLoginRequest        | Executes `identity/external-logins/add` to link an external login provider to a user account.          |
| `RemoveExternalLoginAsync`                | BaseRemoveExternalLoginRequest     | Executes `identity/external-logins/remove` to unlink an external login provider from a user account.   |
| `GetRefreshTokensAsync`                   | GetRefreshTokensRequest            | Executes `identity/refresh-tokens` to retrieve all refresh tokens for a user.                          |
| `GetActiveRefreshTokensAsync`             | GetActiveRefreshTokensRequest      | Executes `identity/refresh-tokens/active` to retrieve active refresh tokens for a user.                |
| `DeleteRefreshTokenAsync`                 | DeleteRefreshTokenRequest          | Executes `identity/refresh-tokens/delete` to revoke a specific refresh token.                          |
| `GetApiKeysAsync`                         | GetApiKeysRequest                  | Executes `identity/api-keys` to retrieve all API keys for a user.                                      |
| `CreateApiKeysAsync`                      | CreateApiKeyRequest                | Executes `identity/api-keys/create` to create a new API key.                                           |
| `EditApiKeysAsync`                        | EditApiKeyRequest                  | Executes `identity/api-keys/edit` to update an existing API key.                                       |
| `RevokeApiKeysAsync`                      | RevokeApiKeyRequest                | Executes `identity/api-keys/revoke` to revoke an API key.                                              |
| `GetApiKeyRolesAsync`                     | GetApiKeyRolesRequest              | Executes `identity/api-keys/roles` to retrieve roles assigned to an API key.                           |
| `AssignApiKeyRoleAsync`                   | AssignApiKeyRoleRequest            | Executes `identity/api-keys/roles/assign` to assign a role to an API key.                              |
| `RemoveApiKeyRoleAsync`                   | RemoveApiKeyRoleRequest            | Executes `identity/api-keys/roles/remove` to remove a role from an API key.                            |
| `GetApiKeyClaimsAsync`                    | GetApiKeyClaimsRequest             | Executes `identity/api-keys/claims` to retrieve claims assigned to an API key.                         |
| `AssignApiKeyClaimAsync`                  | AssignApiKeyClaimRequest           | Executes `identity/api-keys/claims/assign` to assign a claim to an API key.                            |
| `ReplaceApiKeyClaimAsync`                 | ReplaceApiKeyClaimRequest          | Executes `identity/api-keys/claims/replace` to replace an existing claim on an API key.                |
| `AssignOrReplaceApiKeyClaimAsync`         | AssignOrReplaceApiKeyClaimRequest  | Executes `identity/api-keys/claims/assign-or-replace` to upsert a claim on an API key.                 |
| `RemoveApiKeyClaimAsync`                  | RemoveApiKeyClaimRequest           | Executes `identity/api-keys/claims/remove` to remove a claim from an API key.                          |
| `GetRolesAsync`                           | GetRolesRequest                    | Executes `identity/roles` to retrieve all roles.                                                       |
| `CreateRoleAsync`                         | CreateRoleRequest                  | Executes `identity/roles/create` to create a new role.                                                 |
| `DeleteRoleAsync`                         | DeleteRoleRequest                  | Executes `identity/roles/delete` to delete an existing role.                                           |
| `GetRoleClaimsAsync`                      | GetRoleClaimsRequest               | Executes `identity/roles/claims` to retrieve claims assigned to a role.                                |
| `AssignRoleClaimAsync`                    | AssignRoleClaimRequest             | Executes `identity/roles/claims/assign` to assign a claim to a role.                                   |                          
| `ReplaceRoleClaimAsync`                   | ReplaceRoleClaimRequest            | Executes `identity/roles/claims/replace` to replace an existing role claim.                            |
| `AssignOrReplaceRoleClaimAsync`           | AssignOrReplaceRoleClaimRequest    | Executes `identity/roles/claims/assign-or-replace` to upsert a role claim.                             |
| `RemoveRoleClaimAsync`                    | RemoveRoleClaimRequest             | Executes `identity/roles/claims/remove` to remove a claim from a role.                                 |

In addition to the built-in Nano methods, custom API client methods are fully supported. Adding custom methods with dedicated request and response contracts is straightforward and is the 
recommended way to extend the API client with application-specific functionality.

Start by creating a request type that derives from `BaseRequest`. Then annotate the class with one of the available `ActionAttribute` variants shown below. This attribute defines both the 
HTTP method and the relative route of the endpoint. The configuration must match the corresponding endpoint defined in your application controller.

| Action Attribute  | Http Method |
| ----------------- | ----------- |
| `OptionsAction`   | OPTIONS     |
| `HeadAction`      | HEAD        |
| `GetAction`       | GET         |
| `QueryAction`     | QUERY       |
| `PostAction`      | POST        |
| `PutAction`       | PUT         |
| `PatchAction`     | PATCH       |
| `DeleteAction`    | DELETE      |
| `ConnectAction`   | CONNECT     |

> ⚠️ Ensure that the action route is relative and does not start with `/`.

A simple POST request implementation could you look like this.

```csharp 
[PostAction("custom")]
public class MyCustomRequest : BaseRequest
```

Next, add properties to the request and annotate them with request data attributes. The table below lists the available Nano attributes, along with their purpose and behavior.

| Attribute   | Description                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| ----------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `[Header]`  | Defines a header key/value pair that will be added to the request headers. It provides two optional properties: `Name`, which overrides the header key (otherwise the property name is used), and `ValuePrefix`, which prefixes the header value.                                                                                                                                                                                                   |
| `[Route]`   | Defines route parameters. Multiple properties can use `[Route]`. The optional parameter determines the order in which values are substituted into the route defined by the `[Action]` attribute on the request class.                                                                                                                                                                                                                               |
| `[Query]`   | Defines query string parameters. These should be scalar types. The optional `Name` parameter overrides the query parameter name; otherwise, the property name is used.                                                                                                                                                                                                                                                                              |
| `[Body]`    | Defines the request body. It must be a serializable complex type. Typically, this is a dedicated contract class representing the request payload. Nano automatically serializes it when invoking the request. `NetTopologySuite` geometry types are supported, as well as Nano’s built-in `Query`, `Pagination`, and `Ordering` types.                                                                                                              |
| `[Form]`    | Defines a form field included in a multipart request. Properties must be scalar types or one of the thgese file types: `IFormFile`, `FileInfo`, `FileStream`, `Stream`, or `NamedStream` (including `IEnumerable` variants). ⚠️ Complex objects are also supported, but require the use of **[`[FromFormBody]`](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api/README.md#request-multipart-json)**  on the controller action.   |

The updated request example below demonstrates how each attribute can be used in practice.

```csharp 
[PostAction("{id}/custom")]
public class MyCustomRequest : BaseRequest
{
    [Header(Name = "CustomHeader", ValuePrefix = "Custom-")]
    public string Header { get; set; }

    [Route(Order = 0)]
    public string Id { get; set; }

    [Query(Name = "MyQuery")]
    public string Query { get; set; }

    [Body]
    public MyBody Body { get; set; }

    [Form]
    public NamedStream File { get; set; }
}
``` 

Next, an optional response contract can be defined if the endpoint returns data. The response type can be any concrete class and does not need to inherit from any base type.  

```csharp 
public class MyResponse
{
    // Properties
}
```

For file responses, use either `Stream` or `NamedStream` if the filename should be preserved.  

Similar to the entity API client methods, the pluralized `TResponse` type name represents the controller segment of the route. This is the standard convention in Nano and works 
well in most cases, since custom endpoints typically extend existing entity controllers and return the corresponding entity type, ensuring route consistency. However, when an 
endpoint does not return a response, or when the route does not align with the `TResponse` naming convention, the controller route must be explicitly specified in the request, 
as shown below.

```csharp 
public class MyRequest : BaseRequest
{
    public MyRequest()
    {
        this.Controller = "my-controller-route";
    }
}
```

Finally, add a method to your API client class that invokes the request using either `InvokeAsync<TRequest>` for endpoints that do not return a response, 
or `InvokeAsync<TRequest, TResponse>` for endpoints that return a typed response, both provided by `BaseApiClient`. These methods act as the execution layer for your custom 
API client methods and handle the full request lifecycle, including serialization, routing, and HTTP communication. This keeps your API client methods clean and consistent, 
while ensuring that all execution logic is centralized in the shared base implementation.

```csharp 
public class MyApiClient(ApiClient apiClient) : BaseApiClient(apiClient)
{
    public Task GetMyResponse(MyRequest request)
    {
        return this.InvokeAsync<MyRequest>(request);
    }

    public Task<MyResponse?> GetMyResponse(MyRequest request)
    {
        return this.InvokeAsync<MyRequest, MyResponse>(request);
    }
}
``` 

In addition to request execution, the API client also handles propagation of HTTP context and authentication details to ensure consistent behavior across service boundaries. `Accept` 
headers and all `X-Forwarded-*` headers are automatically transferred from the current `HttpContext`, if available, and forwarded with the outgoing request. The same applies to 
the `Authorization` and `X-Api-Key` headers, as well as other built-in Nano headers. The set of automatically forwarded headers may evolve over time, but the Nano API client ensures 
that relevant request metadata from the outer request is propagated to internal requests whenever appropriate.

Authentication is handled internally by the API client implementation to ensure consistent and secure communication between services. By default, the JWT token from the incoming 
`Authorization` header is reused and forwarded with outgoing requests. If the target service shares the same issuer, audience, and signing key, this token can be used directly for 
authorization. If no JWT token is present, Nano will, if configured, automatically authenticate using the root login credentials defined in `ApiOptions`. The resulting JWT token is 
stored statically and reused for subsequent requests to avoid unnecessary re-authentication.

It is also possible to explicitly override the JWT token by setting `BaseRequest.JwtTokenOverride`, allowing full control over the authentication context for individual requests.

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
