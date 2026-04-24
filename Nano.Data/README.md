# Nano.Data
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.svg)](https://www.nuget.org/packages/Nano.Data/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.svg)](https://www.nuget.org/packages/Nano.Data/)

> _Data provider common implementations for Nano applications._

> ⚠️ This NuGet is transitive and included in other Nano Packages, and is not meant to be included directly.

***

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library#nano-library)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
  * **[Connection Pool](#connection-pool)**
  * **[Identity](#identity)**
  * **[Health Checks](#health-checks)**
* **[Data Providers](#data-providers)**
* **[Data Context](#data-context)**
* **[Data Models](#data-models)**
* **[Data Mappings](#data-mappings)**
* **[Migrations](#migrations)**
* **[Repositories](#repositories)**
  * **[Autosave](#autosave)**
  * **[Cache](#cache)**
  * **[Include Annotation](#include-annotation)**
* **[Audit](#audit)**
* **[Soft Delete](#soft-delete)**
* **[Lazy Loading](#lazy-loading)**
* **[Triggers](#triggers)**
* **[Entity Events](#entity-events)**

## Summary
Nano provides a data access implementation built on top of **[Entity Framework](https://learn.microsoft.com/en-us/ef/)**, enabling applications to integrate with SQL-based 
databases in a consistent and structured way.

To enable data support in a Nano application, a few core components must be implemented and registered. First, a **[Data Providers](#data-providers)** must be selected. Next, 
the application must define a database context by deriving implementations from `BaseDbContext` and `BaseDbContextFactory`.  

Once the context has been implemented, the data provider and the data context must be registered during application startup by invoking `AddDataContext<TProvider, TContext>()` 
in `program.cs`.  

After registration, the application can define entity models derived from `BaseEntity`, along with their corresponding mappings derived from `BaseEntityMapping<TEntity>`.  

When Nano data is registered, the `IRepository` interface becomes available for interacting with the database. This abstraction uses the unit-of-work pattern, providing 
a consistent and safe approach for working with persistent data.

The following sections describe how to configure and register Nano data in your application in more detail.## Summary

Also, explore it yourself by trying the various **[Nano Lessons](https://github.com/Nano-Core/Nano.Lessons)** for different data providers.  

## Registration
To enable Nano data support, register the data provider and data context using `AddDataContext<TProvider, TContext>()`. The generic parameters represent the data provider 
implementation and the application's data context.  

When registering the data provider, a **[Data Context](#data-context)** must always be specified as part of the generic type signature.  

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddDataContext<TProvider, TContext>();
})
...
```

By default, Nano uses `Guid` as the primary key type for all entities. This is the recommended approach. However, Nano also supports `string`, `int`, and `long` as identity types.
If you want to use a custom identity type, it must be specified during registration by providing the TIdentity generic parameter.  

```
...
.ConfigureServices(services =>
{
    services
        .AddDataContext<TProvider, TContext, TIdentity>();
})
...
```

> ⚠️ When using a non-default identity type, `TIdentity` must also be specified on **[Data Models](#data-models)**, **[Data Mappings](#data-mappings)**, and other related 
Nano abstractions.  

## Configuration
The `Data` section in the configuration defines the data provider and related settings used by the application.

| Setting                        | Type   | Default     | Description                                                                                                                                                             |
| ------------------------------ | ------ | ----------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `BatchSize`                    | int    | 25          | The maximum batch size for queries.                                                                                                                                     |
| `BulkBatchSize`                | int    | 500         | The maximum batch size for bulk operations.                                                                                                                             |
| `BulkBatchDelay`               | int    | 1000        | The delay (in milliseconds) between bulk batches.                                                                                                                       |
| `QueryRetryCount`              | int    | 0           | The number of times a query will retry on failure.                                                                                                                      |
| `UseLazyLoading`               | bool   | false       | A value indicating whether lazy loading is enabled. See **[Lazy Loading](#lazy-loading)**. ⚠️ Not recommended, use **[Nano Include Annotation](#include-annotation)**.  |
| `StartupAction`                | enum   | None        | The startup action for the database. Allowed Values: `None`, `Create` or `Migrate`. Defaults to `None`.                                                                 |
| `UseSensitiveDataLogging`      | bool   | false       | A value indicating whether sensitive data logging is enabled.                                                                                                           |
| `QuerySplittingBehavior`       | enum   | SingleQuery | The default query splitting behavior for EF Core queries.                                                                                                               |              
| `DefaultCollation`             | string | null        | The default collation for the database. ⚠️ Note: Changing this setting affects only new migrations and will not modify existing tables or columns.                      |
| `ConnectionString`             | string | null        | Required. The connection string for the database.                                                                                                                       |
| `Repository`                   | object | default     | The cache configuration options. See **[Repositories](#repositories)**.                                                                                                 |
| `Repository.UseAutoSave`       | bool   | true        | A value indicating whether automatic saving of changes in repositories is enabled. See **[Autosave](#autosave)**.                                                       |
| `Repository.QueryIncludeDepth` | int    | 4           | The maximum depth for query includes. See **[Include Annotation](#include-annotation)**.                                                                                |
| `ConnectionPool`               | object | null        | The connection pool configuration options. See **[Connection Pool](#connection-pool)**.                                                                                 |
| `Identity`                     | object | null        | The identity configuration options. See **[Identity](#identity)**.                                                                                                      |
| `HealthCheck`                  | object | null        | The options for configuring health checks. See **[Health Check](#health-check)**. _Only relevant for `NanoApiApplication` and `NanoWebApplication`_.                    |

```json
"Data": {
  "BatchSize": 25,
  "BulkBatchSize": 500,
  "BulkBatchDelay": 1000,
  "QueryRetryCount": 0,
  "UseLazyLoading": false,
  "StartupAction": None,
  "UseSensitiveDataLogging": false,
  "QuerySplittingBehavior": "SingleQuery",
  "DefaultCollation": null,
  "ConnectionString": null,
  "Repository": { 
    "UseAutoSave": false,
    "QueryIncludeDepth": 4
  },
  "ConnectionPool": null,
  "Identity": null,
  "HealthCheck": null
}
```

> 📖 Learn more about **[Application Configuration](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#configuration)** here.  

## Connection Pool
Nano supports optional connection pooling for the underlying Entity Framework data provider. When enabled, database contexts are reused from a pool, which can improve performance 
and reduce allocation overhead.  

| Setting       | Type | Default | Description                              |
| ------------- | ---- | ------- | ---------------------------------------- |
|  `PoolSize`   | int  | 1024    | The pool size of the connection pool.    |

## Identity
Identity configures the data store used for authentication and authorization. It manages users, roles, and related security data required for signing in and enforcing 
access control.  

| Setting                               | Type     | Default         | Description                                                                                                              |
| ------------------------------------- | -------- | --------------- | ------------------------------------------------------------------------------------------------------------------------ |
| `TokensExpirationInHours`             | TimeSpan | 24:00:00        | The expiration time for tokens in hours.                                                                                 |
| `UseAudit`                            | enum     | None            | Defines which intitity models to to audit. Allows multiple values. See possible values below.                            |
| `User`                                | object   | default         | Options for user-specific settings.                                                                                      |
| `User.IsUniqueEmailAddressRequired`   | bool     | true            | A value indicating whether each user must have a unique email address.                                                   |
| `User.IsUniquePhoneNumberRequired`    | bool     | false           | A value indicating whether each user must have a unique phone number.                                                    |
| `User.AllowedUserNameCharacters`      | string   | abcde...        | The allowed characters for usernames.                                                                                    |
| `User.DefaultRoles`                   | array    | [administrator] | The default roles assigned to a new user. If `null`, new users will automatically be assigned the _Administrator_ role.  |
| `SignIn`                              | object   | default         | Options for sign-in requirements.                                                                                        |
| `SignIn.RequireConfirmedEmail`        | bool     | False           | A value indicating whether users must have a confirmed email to sign in.                                                 |
| `SignIn.RequireConfirmedPhoneNumber`  | bool     | false           | A value indicating whether users must have a confirmed phone number to sign in.                                          |
| `Lockout`                             | object   | default         | Options for account lockout policies.                                                                                    |
| `Lockout.AllowedForNewUsers`          | bool     | true            | A value indicating whether lockout is allowed for new users.                                                             |
| `Lockout.MaxFailedAccessAttempts`     | int      | 3               | The maximum number of failed access attempts before a user is locked out.                                                |
| `Lockout.DefaultLockoutTimeSpan`      | TImeSpan | 00:30:00        | The default lockout duration for a user.                                                                                 |
| `Password`                            | object   | default         | Options for password complexity requirements.                                                                            |
| `Password.RequireDigit`               | bool     | true            | A value indicating whether the password must contain at least one digit.                                                 |
| `Password.RequireNonAlphanumeric`     | bool     | true            | A value indicating whether the password must contain at least one non-alphanumeric character.                            |
| `Password.RequireLowercase`           | bool     | true            | A value indicating whether the password must contain at least one lowercase letter.                                      |
| `Password.RequirUppercase`            | bool     | true            | A value indicating whether the password must contain at least one uppercase letter.                                      |
| `Password.RequiredLength`             | int      | 12              | The minimum required length of the password.                                                                             |
| `Password.RequiredUniqueCharacters`   | int      | 3               | The number of unique characters required in the password.                                                                |
| `ApiKey`                              | object   | default         | Optional. Options for API keys.                                                                                          |
| `ApiKey.Secret`                       | string   | null            | Required. The secret key used to create and validate API keys.                                                           |

```json
"Data": {
  "Identity": { 
    "TokensExpiration": "24:00:00",
    "UseAudit": false,
    "User": {
      "IsUniqueEmailAddressRequired": true,
      "IsUniquePhoneNumberRequired": false,
      "AllowedUserNameCharacters": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+",
      "DefaultRoles": [
        "administrator"
      ]
    },
    "SignIn": {
      "RequireConfirmedEmail": false,
      "RequireConfirmedPhoneNumber": false 
    },
    "Lockout": {
      "AllowedForNewUsers": true,
      "MaxFailedAccessAttempts": 3,
      "DefaultLockoutTimeSpan": "00:30:00"
    },
    "Password": {
      "RequireDigit": false,
      "RequireNonAlphanumeric": false,
      "RequireLowercase": false,
      "RequirUppercase": false,
      "RequiredLength": 5,
      "RequiredUniqueCharacters": 5
    },
    "ApiKey": {
      "Secret": null
    }
  }
}
```

The following values can be used for the `UseAudit` configuration setting. Multiple values can be specified as a comma-separated list in `appsettings.json`.

| Value         | Description                                                                 |
| ------------- | --------------------------------------------------------------------------- |
| `None`        | No identity model audited.                                                  | 
| `Standard`    | Standard idenity models are audited (User, UserRole, ApiKey, ApiKeyRole).   | 
| `All`         | All identity models are audited.                                            | 
| `User`        | The identity user model is audited.                                         | 
| `UserRole`    | The identity user role model is audited.                                    | 
| `UserClaim`   | The identity user claim model is audited.                                   | 
| `UserLogin`   | The identity user login model is audited.                                   | 
| `Role`        | The identity role model is audited.                                         | 
| `RoleClaim`   | The identity role claim model is audited.                                   | 
| `ApiKey`      | The identity apikey model is audited.                                       | 
| `ApiKeyClaim` | The identity apikey claim model is audited.                                 | 
| `ApiKeyRole`  | The identity apikey role model is audited.                                  | 

> ⚠️ All sensitive properties, as well as properties used internally for non-business, technical, or derived purposes, are automatically excluded.

When identity has been configured the following roles are automatically added.  

| Role          | Description                          |
| ------------- | ------------------------------------ |
| reader        | Authorized to read.                  | 
| writer        | Authorized to read and write.        | 
| creator       | Authorized to create.                | 
| editor        | Authorized to update.                | 
| deleter       | Authorized to create.                | 
| identity      | Authorized to use identity actions.  | 
| Administrator | Full access to everything.           | 

Identity in Nano involves more than just configuration. An identity data model and corresponding mappings must also be implemented to define how identity data is stored and accessed. 
Read more about **[Identity Data Models](#data-models)** and **[Identity Data Mappings](#data-mappings)**. Nano also provides a repository for working with identity-related 
functionality. See **[Repositories](#repositories)** for more details.

Nano also provides a public `SecurePasswordGenerator` class for creating strong, secure passwords.  

Try it out yourself using the **[Api.Data.Identity](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.Identity)**.  

## Health Checks
When health checks are enabled in the data configuration, Nano automatically registers a health check for the configured data provider.  

This allows the application to verify that the underlying database connection is available and operational. The health check integrates with ASP.NET Core's health check system 
and can be used by monitoring tools, load balancers, or container orchestrators to determine the health status of the application.  

| Setting                        | Type   | Default     | Description                                                                                                                           |
| ------------------------------ | ------ | ----------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| `HealthCheck.UnhealthyStatus`  | enum   | Unhealthy   | The health status reported when the data provider is unavailable. _Only relevant for `NanoApiApplication` and `NanoWebApplication`_.  |

```json
"Data": {
  "HealthCheck": {
    "UnhealthyStatus": Unhealthy
  }
}
```

## Data Providers
Data providers integrate a SQL database into a Nano application through Entity Framework.  

All Nano data providers implement the `IDataProvider` interface. This interface is responsible for configuring and initializing the underlying data context and 
any required services.  

To create a custom data provider, implement the `IDataProvider` interface and register all required services and dependencies inside the provider’s `Configure` method. 
Once implemented, the provider can be added to the application during startup configuration.  

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoData<MyProvider, MyDbContext>();
})
...
```

The following data providers are currently supported in Nano.  

* **[Nano.Data.InMemory](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.InMemory)**
* **[Nano.Data.MySql](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.MySql)**
* **[Nano.Data.PostgreSQL](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.PostgreSQL)**
* **[Nano.Data.SqLite](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.SqLite)**
* **[Nano.Data.SqlServer](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.SqlServer)**

## Data Context
Nano provides built-in management for the `DbContext` while still letting you use it as you normally would.  

To integrate your database, simply derive a class from `BaseDbContext` or `BaseDbContext<TIdentity>` if you want to use a primary key type other than `Guid`.

```csharp
public class MyDbContext(DbContextOptions contextOptions, IOptionsMonitor<DataOptions> dataOptions)
    : BaseDbContext(contextOptions, dataOptions);
```

You don’t need to implement any additional logic in `MyDbContext`. Entity Framework just requires that the derived `DbContext` exists in the entry assembly, where `program.cs` is 
located. Nano will automatically detect and configure all **[Data Mappings](#data-mappings)**, so overriding the `OnModelCreating(...)` method is unnecessary.

> ⚠️ If you override any methods from `BaseDbContext`, always call the `base` method to ensure Nano functions correctly.

It is recommended to use the **[Repositories](#repositories)** to interact with the entity model in your data context. However, if the repository functionality does not 
cover your needs, you can always inject the data context directly.  

In addition, you must derive a concrete implementation from `BaseDbContextFactory`.  

```csharp
public class MySqlDbContextFactory : BaseDbContextFactory<MySqlProvider, MySqlDb**Context>;
```

No further implementation is required. The factory class simply needs to exist in your application project alongside `program.cs`, similar to the `BaseDbContext` implementation.

## Data Models
Models in Nano, also referred to as entities, represent the tables in your database.

To create a entity model, derive a non-generic class from `BaseEntity` or `BaseEntity<TIdentity>` if you want to use a custom primary key type. By inheriting from `BaseEntity`, 
your model automatically includes several built-in properties.  

| Property      | Type            | Description                                                                                     |
| ------------- | --------------- | ----------------------------------------------------------------------------------------------- |
| `Id`          | TIdentity       | The primary key of the model. _Automatically assigned for new instances._                       |
| `CreatedAt`   | DateTimeOffset  | The timestamp when the model was created. _Automatically set to `UtcNow` for new instances._    |
| `IsDeleted`   | int             | Used only when **[Soft Delete](#soft-delete)** is enabled. _Defaults to 0._                     |

```csharp
public class MyEntity : BaseEntity
{
    // Properties
}
```

If you specify a `TIdentity` type during data **[Registration](#registration)** and in your **[Data Context](#data-context)**, you must use the same type when deriving your 
concrete entities.

Alternatively, you can derive your entity model from one of the specialized CRUD base classes: `BaseEntityReadOnly`, `BaseEntityCreatable`, `BaseEntityCreatableAndUpdatable`, 
`BaseEntityUpdatable`, or `BaseEntityDeletable`, to restrict the allowed `IRepository` operations for that entity.  

> ⚠️ `BaseEntityReadOnly` is immutable and is not intended to be used directly.

For more advanced scenarios, if you do not want the built-in properties provided by Nano, you can derive your entity model from `BaseEntityIdentity` or 
`BaseEntityIdentity<TIdentity>`. This gives your entity only the `Id` property, but limits most built-in `IRepository` operations. To restore specific operations, your entity 
must implement the corresponding interfaces: `IEntityReadOnly`, `IEntityWritable`, `IEntityCreatable`, `IEntityCreatableAndUpdatable`, `IEntityUpdatable`, or `IEntityDeletable` 
(`IEntityDeletableSoft`). These interfaces mirror the functionality of the CRUD base entity classes.  

> 💡 For simplicity and maintainability, it is recommended to derive entity models from `BaseEntity` or one of the specific base classes rather than 
implementing the interfaces directly.  

Try it out yourself using the **[Api.Data.Mysql](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.Mysql)** or  
**[Console.Data.Mysql](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Data.Mysql)** examples. Similar examples are available for other data providers as well.  

Nano also contains another important base entity model. The `BaseEntityUser` and `BaseEntityUser<TIdentity>`. When **[Identity](#identity)** has been configured for an application, 
the entity user base classes defines the user model of the application and are having `IdentityUser` associated. The base class itself derives from `BaseEntity`, and behaves in the 
same way as regular entities. It contains a single navigation property, `IdentityUser` that is included using an **[Include Annotation](#include-annotation)** when retreiving the 
user entity. The `IdentityUser` navigation property on your derived user entity is mapped to the `Id` property, ensuring that both entities share the same identifier. This allows you to 
easily access identity data without having to deal directly with the underlying identity data when working with the model.  

```csharp
public class MyEntityUser : BaseEntityUser
{
    public virtual IdentityUserEx<TIdentity> IdentityUser { get; set; } = null!;
    
    // Custom Properties
}
```

> ⚠️ It is recommended to derive only a single implementation from `BaseEntityUser`. However, Nano supports multiple identity implementations if required.

The following entity identity models are available in Nano.  

| Entity Model                           | Table                           | Description                                                                    |
| -------------------------------------- | ------------------------------- | ------------------------------------------------------------------------------ |
| `IdentityUserEx<TIdentity>`            | `__EFIdentityUser`              | The identity user.                                                             |
| `IdentityUserRole<TIdentity>`          | `__EFIdentityUserRole`          | Roles associated with identity users.                                          |
| `IdentityUserClaim<TIdentity>`         | `__EFIdentityUserClaim`         | Claims associated with identity users                                          |
| `IdentityUserLogin<TIdentity>`         | `__EFIdentityUserLogin`         | External user logins for identity users.                                       |
| `IdentityUserToken<TIdentity>`         | `__EFIdentityUserToken`         | User tokens created for change email, confirm email, etc. for identity users.  |
| `IdentityUserChangeData<TIdentity>`    | `__EFIdentityUserChangeData`    | Email and phone number temporary change data for identity users.               |
| `IdentityUserRefreshToken<TIdentity>`  | `__EFIdentityUserRefreshToken`  | Refresh tokens for identity                                                    |
| `IdentityRole<TIdentity>`              | `__EFIdentityRole`              | The identity Roles.                                                            |
| `IdentityRoleClaim<TIdentity>`         | `__EFIdentityRoleClaim`         | Claims associated with identity roles.                                         |
| `IdentityApiKey<TIdentity>`            | `__EFIdentityApiKey`            | Api keys for identity users.                                                   |
| `IdentityApiKeyClaim<TIdentity>`       | `__EFIdentityApiKeyClaim`       | Claims for Api keys.                                                           |
| `IdentityApiKeyRole<TIdentity>`        | `__EFIdentityApiKeyRole`        | Roles for Api keys.                                                            |

> ⚠️ Even when Identity is not configured, the tables are still created to preserve the data model in case Identity is enabled later.

Generally, you do not need to interact with the identity models directly. Use the **[Identity Repository](#Repositories)** to work with identity instead. If direct access is required, 
the identity models are available through the **[Data Context](#data-context)** like any other entity. Bypassing the `IIdentityRepository` is not recommended, as it may 
unintentionally bypass critical identity logic.

Try it out yourself using the **[Api.Data.Identity](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.Identity)** example.  

Nano also supports defining views in the entity model. The `BaseEntityView` entity class can be used to define a model for a SQL view.  

```csharp
public class MyEntityView : BaseEntityView
{
    // Properties
}
```

Try it out yourself using the **[Api.Data.MySql.Views](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.MySql.Views)** example.  

Spatial `Geometry` types from `NetToplogySuite` is also supported. Try it out yourself using the 
**[Api.Data.MySql.Spatial](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.MySql.Spatial)** example.  

## Data Mappings
Each data model in your application should have a corresponding non-generic data mapping with a parameterless constructor. Data mappings define how your entities are configured 
in the database and allow you to customize Entity Framework behavior.  

```csharp
public class MyEntityMapping : BaseEntityMapping<MyEntity>
{
    public override void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        base.Configure(builder);

        // Add Entity Framework mappings here.
    }
}
```

> ⚠️ Always call `base.Configure(builder);` in your data mappings to ensure Nano functions correctly.  

Nano automatically applies all data mappings, so there is no need to manually register them. Only non-abstract, non-generic mapping classes are automatically 
detected and applied.  

> 💡 It is strongly recommended to map all properties of your entity models within your data mappings to ensure consistency and correctness.

When mapping a `BaseEntityUser`, derive the mapping implementation from `BaseEntityUserMapping<TEntity>`. Apart from this base class, the mapping is implemented in the same way 
as for regular entity models.  

```csharp
public class MyEntityUserMapping : BaseEntityUserMapping<MyEntityUser>
{
    public override void Configure(EntityTypeBuilder<MyEntityUser> builder)
    {
        base.Configure(builder);

        // Add Entity Framework mappings here.
    }
}
```

Nano also supports mapping read-only database views. To do this, derive your mapping from `BaseEntityViewMapping<TEntity>`.

```csharp
public class MyEntityViewMapping : BaseEntityViewMapping<MyEntity>
{
    public override void Map(EntityTypeBuilder<MyEntity> builder)
    {
        base.Map(builder);

        // Add Entity Framework mappings here.
    }
}
```

Try it out views yourself using the **[Api.Data.MySql.Views](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.MySql.Views)** example.  

Nano automatically updates all unique index mappings to include the `IsDeleted` property. This ensures that soft-deleted entities can coexist without violating 
uniqueness constraints.  

Mapping of spatial types varies between data providers. Refer to your provider's spatial documentation for details.  

Explore spatial and other advanced mappings using the **[Api.Data.MySql.Mappings](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.MySql.Mappings)** example.  

## Migrations
Migrations in Nano work the same way as standard Entity Framework migrations.  

Before creating migrations, ensure you have implemented a `BaseDbContextFactory<MySqlProvider, MySqlDbContext>`. Then, in PowerShell, add a new migration.  

```powershell
dotnet ef migrations add Initial --project {project}
```

Migrations are not applied automatically unless the `StartupAction` option is set to `Migrate` in the configuration.

> ⚠️ It is recommended to enable migrations only in `Development` environments.

In `Staging` and `Production`, migrations are applied during deployment, via GitHub Actions.

```powershell
dotnet ef database update `
  --no-build `
  --startup-project $env:APP_NAME `
  --connection "$env:MYSQL_MIGRATION_CONNECTIONSTRING";
```

> ⚠️ Entity Framework does not handle views, stored procedures, or functions in migrations, they must be added or modified manually to migrations. 

To manage views or stored procedures in migrations take a look at the **[Api.Data.MySql.Views](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.MySql.Views)** or 
**[Api.Data.MySql.StoredProcedures](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.MySql.StoredProcedures)** example for best practices.  

> 📖 Learn more about **[EF Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)**.

## Repositories
Repositories in Nano provide a layer on top of Entity Framework and the `BaseDbContext`, encapsulating common functionality and features for working with entity models. 
The `IRepository` interface defines generic methods for retrieving, querying, adding, updating, and deleting entities, where the generic parameter specifies the type of 
entity the operation works on. For example, adding a new instance of `MyEntity` would look like this.

```csharp
this.Repository
    .AddAsync<MyEntity>(new MyEntity());
```

Different methods in `IRepository` have specific generic constraints based on the type of the entity model and the operation being performed. For instance, `AddAsync<TEntity>` 
is constrained to models implementing `IEntityCreatable`.  

The following table lists the methods available in `IRepository` along with their parameters, generic constraints, and a brief description.  

| Method                                      | Parameters                                                | TEntity Constraint                    | Description                                                                               |
| ------------------------------------------- | --------------------------------------------------------- | ------------------------------------- |------------------------------------------------------------------------------------------ |
| `GetAsync<TEntity, TKey>`                   | key, includeDepth                                         | `IEntityIdentity`                     | Gets an entity by its unique key including related entities to a specified depth.         |
| `GetFirstAsync<TEntity, TCriteria>`         | criteria, includeDepth                                    | `IEntity`                             | Gets the first entity matching the specified criteria.                                    |
| `GetFirstAsync<TEntity>`                    | where, ordering, includeDepth                             | `IEntity`                             | Gets the first entity matching the predicate.                                             |
| `GetManyAsync<TEntity, TKey>`               | keys, includeDepth                                        | `IEntityIdentity`                     | Gets entities matching the specified keys.                                                |
| `GetManyAsync<TEntity>`                     | query, includeDepth                                       | `IEntity`                             | Gets entities matching the specified query.                                               |
| `GetManyAsync<TEntity, TCriteria>`          | criteria, includeDepth                                    | `IEntity`                             | Gets entities matching the specified criteria.                                            |
| `GetManyAsync<TEntity>`                     | where, pagination, ordering, includeDepth                 | `IEntity`                             | Gets entities matching a predicate with pagination and ordering.                          |
| `GetManyAsync<TEntity, TKey>`               | where, pagination, includeDepth, orderBy, orderDirection  | `IEntityIdentity`                     | Gets entities matching a predicate ordered by a key selector with pagination.             |
| `AddAsync<TEntity>`                         | entity                                                    | `IEntityCreatable`                    | Adds a single entity.                                                                     |
| `AddOrGetAsync<TEntity, TKey>`              | entity                                                    | `IEntityCreatable`, `IEntityIdentity` | Adds an entity and reloads it including related entities. ⚠️ _Always uses autosave._      |
| `AddAndGetAsync<TEntity, TKey>`             | entity                                                    | `IEntityCreatable`, `IEntityIdentity` | Adds or retreives an entity including related entities. ⚠️ _Always uses autosave._        |
| `AddManyAsync<TEntity>`                     | entities                                                  | `IEntityCreatable`                    | Adds multiple entities.                                                                   |
| `AddManyBulkAsync<TEntity>`                 | entities                                                  | `IEntityCreatable`                    | Bulk adds multiple entities using EF Plus Enterprise.                                     |
| `UpdateAsync<TEntity>`                      | entity                                                    | `IEntityUpdatable`                    | Updates a single entity.                                                                  |
| `UpdateAndGetAsync<TEntity, TKey>`          | entity                                                    | `IEntityUpdatable`, `IEntityIdentity` | Updates an entity and reloads it including related entities. ⚠️ _Always uses autosave._   |
| `UpdateManyAsync<TEntity>`                  | entities                                                  | `IEntityUpdatable`                    | Updates multiple entities.                                                                |
| `UpdateManyAsync<TEntity>`                  | where, propertyUpdates                                    | `IEntityUpdatable`                    | Updates entities matching a predicate.                                                    |
| `UpdateManyAsync<TEntity, TCriteria>`       | criteria, propertyUpdates                                 | `IEntityUpdatable`                    | Updates entities based on specified criteria.                                             |
| `UpdateManyBulkAsync<TEntity>`              | entities                                                  | `IEntityUpdatable`                    | Bulk updates multiple entities using EF Plus Enterprise.                                  |
| `UpdateManyBulkAsync<TEntity>`              | where, propertyUpdates                                    | `IEntityUpdatable`                    | Bulk (batch) updates entities matching a predicate.                                       |
| `UpdateManyBulkAsync<TEntity, TCriteria>`   | criteria, propertyUpdates                                 | `IEntityUpdatable`                    | Bulk (batch) updates entities based on specified criteria.                                |
| `AddOrUpdateAsync<TEntity>`                 | entity                                                    | `IEntityCreatableAndUpdatable`        | Adds or updates a single entity.                                                          |
| `AddOrUpdateManyAsync<TEntity>`             | entities                                                  | `IEntityCreatableAndUpdatable`        | Adds or updates multiple entities.                                                        |
| `DeleteAsync<TEntity, TKey>`                | id                                                        | `IEntityDeletable`, `IEntityIdentity` | Deletes an entity by its key.                                                             |
| `DeleteAsync<TEntity>`                      | entity                                                    | `IEntityDeletable`                    | Deletes a specific entity instance.                                                       |
| `DeleteManyAsync<TEntity, TKey>`            | ids                                                       | `IEntityDeletable`, `IEntityIdentity` | Deletes multiple entities by their keys.                                                  |
| `DeleteManyAsync<TEntity>`                  | entities                                                  | `IEntityDeletable`                    | Deletes multiple entities.                                                                |
| `DeleteManyAsync<TEntity, TCriteria>`       | criteria                                                  | `IEntityDeletable`                    | Deletes entities matching specified criteria.                                             |
| `DeleteManyAsync<TEntity>`                  | expression                                                | `IEntityDeletable`                    | Deletes entities matching a filter expression.                                            |
| `DeleteManyBulkAsync<TEntity, TKey>`        | ids                                                       | `IEntityDeletable`, `IEntityIdentity` | Bulk deletes entities with specified keys.                                                |
| `DeleteManyBulkAsync<TEntity>`              | entities                                                  | `IEntityDeletable`                    | Bulk deletes specified entities.                                                          |
| `DeleteManyBulkAsync<TEntity, TCriteria>`   | criteria                                                  | `IEntityDeletable`                    | Bulk (batch) deletes entities matching specified criteria.                                |
| `DeleteManyBulkAsync<TEntity>`              | expression                                                | `IEntityDeletable`                    | Bulk (batch) deletes entities matching a filter expression.                               |
| `CountAsync<TEntity, TCriteria>`            | criteria                                                  | `IEntity`                             | Returns the number of entities matching the criteria.                                     |
| `CountAsync<TEntity>`                       | expression                                                | `IEntity`                             | Returns the number of entities matching a filter expression.                              |
| `SumAsync<TEntity>`                         | whereExpr, sumExpr                                        | `IEntity`                             | Calculates the sum of a numeric expression for matching entities.                         |
| `AverageAsync<TEntity>`                     | whereExpr, avgExpr                                        | `IEntity`                             | Calculates the average of a numeric expression for matching entities.                     |
| `ExecuteProcedureAsync<TResult>`            | procedureName, parameters                                 | -                                     | Execute a stored procedure and return a single result.                                    |
| `ExecuteProcedureScalarAsync<TResult>`      | procedureName, parameters                                 | -                                     | Execute a stored procedure and return a list results.                                     |
| `ExecuteProcedureAsync<TResult>`            | procedureName, parameters                                 | -                                     | Execute a stored procedure and return a scalar value.                                     |
| `SaveChangesAsync`                          | -                                                         | -                                     | Persists all pending changes to the data store.                                           | 

Several methods include overloads, which have been merged here for simplicity.  

One of the most useful parameters is `includeDepth`, which overrides the globally configured include depth and determines how many levels of [Include Annotations](#include-annotation) 
are applied in a query. This allows you to map complex entity models with related entities, while also controlling how much of the entity graph is loaded. Sometimes you may want 
only the plain entity, while other times you may need the full inclusion tree.  

Try it out yourself using the **[Api.Data.Mysql](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.Mysql)** or  
**[Console.Data.Mysql](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Data.Mysql)** examples. Similar examples are available for other data providers as well.  

Nano also provides a dedicated repository for managing entity user models, the `IIdentityRepository` and `IIdentityRepository<TIdentity>`. These repositories expose common identity 
operations such as login, signup, email changes, management of roles and claims, etc. Internally, the repository encapsulates functionality from `UserManager<T>` and 
`RoleManager<T>`, providing a unified interface for working with identity features. This abstraction simplifies identity management and allows applications to interact with 
identity logic through a single, consistent repository.  

| Method                                  | Type                 | Parameters                                    | Description                                                                                                          |
| --------------------------------------- | -------------------- | --------------------------------------------- | -------------------------------------------------------------------------------------------------------------------- |
| `SignInAsync`                           | Login                | signIn                                        | Attempts to sign in a user using the specified credentials. Returns the authenticated user if successful.            |
| `SignInExternalAsync`                   | Login                | signInExternal                                | Attempts to sign in a user using an external authentication provider. Returns the authenticated user if successful.  |
| `SignInApiKeyAsync`                     | Login                | signInApiKey                                  | Attempts to sign in a user using an api key.                                                                         |
| `SignOutAsync`                          | Login                | userId, appId                                 | Signs out the currently authenticated user and removes any associated refresh tokens.                                |
| `IsEmailAddressTakenAsync`              | Sign Up              | emailAddress                                  | Checks whether the specified email address is already registered. Returns true if taken.                             |
| `IsPhoneNumberTakenAsync`               | Sign Up              | phoneNumber                                   | Checks whether the specified phone number is already registered. Returns true if taken.                              |
| `GetPasswordOptionsAsync`               | Sign Up              | —                                             | Retrieves the password configuration options for the identity system, if available.                                  |
| `SignUpAsync<TUser>`                    | Sign Up              | signUp                                        | Registers a new user with the specified sign-up information. Returns the created user entity.                        |
| `SignUpExternalAsync<TUser>`            | Sign Up              | signUpExternal                                | Registers a new user using external login provider information. Returns the created user entity.                     |
| `GetIdentityUserAsync`                  | User                 | id                                            | Retrieves the identity user by its identifier. Throws if the user is not found.                                      |
| `GetIdentityUserOrDefaultAsync`         | User                 | id                                            | Retrieves the identity user by its identifier, or returns null if the user does not exist.                           |
| `GetDeactivatedUserAsync`               | User                 | id                                            | Retrieves the deactivated identity user by its identifier. Throws if the user is not found.                          |
| `SetUsernameAsync`                      | User                 | id, setUsername                               | Updates the username of the specified user.                                                                          |
| `SetPasswordAsync`                      | User                 | id, setPassword                               | Sets a password for the specified user.                                                                              |
| `ChangePasswordAsync`                   | User                 | id, changePassword                            | Changes the password of a user given the old and new passwords.                                                      |
| `GenerateResetPasswordTokenAsync`       | User                 | generateResetPasswordToken                    | Generates a password reset token for the specified user.                                                             |
| `ResetPasswordAsync`                    | User                 | id, resetPassword                             | Resets the password of a user using a valid reset token.                                                             |
| `GenerateChangeEmailTokenAsync`         | User                 | id, generateChangeEmailToken                  | Generates a change email token for the specified user.                                                               |
| `ChangeEmailAsync   `                   | User                 | id, changeEmail, setUsername (bool)           | Changes the email address of a user using a valid token.                                                             |
| `GenerateConfirmEmailTokenAsync`        | User                 | id, generateConfirmEmailToken                 | Generates an email confirmation token for the specified user.                                                        |
| `ConfirmEmailAsync`                     | User                 | id, confirmEmail                              | Confirms the email of a user using a valid confirmation token.                                                       |
| `GenerateChangePhoneNumberTokenAsync`   | User                 | id, generateChangePhoneToken                  | Generates a change phone number token for a user.                                                                    |
| `ChangePhoneNumberAsync`                | User                 | id, changePhoneNumber                         | Changes the phone number of a user using a valid token.                                                              |
| `GenerateConfirmPhoneNumberTokenAsync`  | User                 | id, generateConfirmPhoneToken                 | Generates a confirm phone number token for a user.                                                                   |
| `ConfirmPhoneNumberAsync`               | User                 | id, confirmPhoneNumber                        | Confirms the phone number of a user using a valid token.                                                             |
| `GenerateCustomPurposeTokenAsync`       | User                 | id, generateCustomPurposeToken                | Generates a custom purpose token for a user for a specific purpose.                                                  |
| `ConfirmCustomPurposeTokenAsync`        | User                 | id, confirmCustomPurpose                      | Confirms a custom purpose token of a user.                                                                           |
| `ActivateIdentityUser`                  | User                 | id                                            | Activates a user by setting IsActive to true.                                                                        |
| `DeactivateIdentityUser`                | User                 | id                                            | Deactivates a user by setting IsActive to false and removing refresh tokens.                                         |
| `DeleteUserAsync`                       | User                 | id                                            | Deletes a user and all related identity data.                                                                        |
| `GetUserRolesAsync`                     | User Roles           | id                                            | Retrieves the role names assigned to a specific user by ID.                                                          |
| `GetUserRolesAsync`                     | User Roles           | identityUser                                  | Retrieves the role names assigned to a specific user instance.                                                       |
| `AssignUserRoleAsync`                   | User Roles           | id, assignUserRole                            | Assigns a role to a user.                                                                                            |
| `RemoveUserRoleAsync`                   | User Roles           | id, removeUserRole                            | Removes a role from a user.                                                                                          |
| `GetAllUserClaims`                      | User Claims          | identityUser, transientRoles, transientClaims | Retrieves all claims of a user, including role-based and optional transient claims.                                  |
| `GetUserClaimAsync`                     | User Claims          | id, getUserClaim                              | Retrieves a specific claim of a user by claim type. Returns null if not found.                                       |
| `GetUserClaimsAsync`                    | User Claims          | id                                            | Retrieves all claims of a user by user ID. Throws if the user does not exist.                                        |
| `GetUserClaimsAsync`                    | User Claims          | identityUser                                  | Retrieves all claims of a user. Throws if identityUser is null.                                                      |
| `AssignUserClaimAsync`                  | User Claims          | id, assignClaim                               | Assigns a claim to a user. Returns the created user claim.                                                           |
| `ReplaceUserClaimAsync`                 | User Claims          | id, replaceClaim                              | Replaces an existing claim of a user with a new value. Returns the updated claim.                                    |
| `AssignOrReplaceUserClaimAsync`         | User Claims          | id, assignOrReplaceClaim                      | Assigns a claim to a user, or replaces it if it already exists. Returns the resulting claim.                         |
| `RemoveUserClaimAsync`                  | User Claims          | id, removeClaim                               | Removes a claim from a user.                                                                                         |
| `GetUserExternalLoginAsync`             | User External Logins | id, providerName                              | Retrieves a specific external login provider associated with a user by user id.                                      |
| `GetUserExternalLoginAsync`             | User External Logins | identityUser, providerName                    | Retrieves a specific external login provider associated with a user by identity user instance.                       |
| `GetUserExternalLoginsAsync`            | User External Logins | id                                            | Retrieves all external login providers associated with a user by user id.                                            |
| `GetUserExternalLoginsAsync`            | User External Logins | identityUser                                  | Retrieves all external login providers associated with a user by identity user instance.                             |
| `AddExternalLoginAsync`                 | User External Logins | id, externalProvider                          | Adds an external login provider to a user. Returns the added UserLoginInfo.                                          |
| `RemoveExternalLoginAsync`              | User External Logins | id, providerName                              | Removes an external login provider from a user.                                                                      |
| `GetRefreshToken`                       | Refresh Tokens       | id, appId                                     | Retrieves a refresh token for a specific user and application. Returns null if not found.                            |
| `GetRefreshTokens`                      | Refresh Tokens       | id                                            | Retrieves all refresh tokens for a specific user.                                                                    |
| `GetActiveRefreshTokens`                | Refresh Tokens       | id                                            | Retrieves all active (non-expired) refresh tokens for a specific user.                                               |
| `CreateRefreshToken`                    | Refresh Tokens       | id, refreshToken, appId                       | Creates a new refresh token for a user and application, replacing any existing token.                                |
| `DeleteRefreshTokenAsync`               | Refresh Tokens       | id                                            | Deletes a refresh token by its identifier. Does nothing if the token does not exist.                                 |
| `GetApiKeyAsync`                        | Api Keys             | apiKeyId                                      | Retrieves an API key by its identifier. Returns null if not found.                                                   |
| `GetApiKeysAsync`                       | Api Keys             | id                                            | Retrieves all API keys associated with a specific user.                                                              |
| `CreateApiKeyAsync`                     | Api Keys             | id, createApiKey                              | Creates a new API key for a user. Returns the created API key and outputs the plaintext key.                         |
| `ValidateApiKeyAsync`                   | Api Keys             | validateApiKey                                | Validates a provided API key and returns its associated record if valid; otherwise null.                             |
| `EditApiKeyAsync`                       | Api Keys             | apiKeyId, editApiKey                          | Updates the name or metadata of an existing API key. Returns the updated API key or null.                            |
| `RevokeApiKeyAsync`                     | Api Keys             | apiKeyId, revokeApiKey                        | Revokes an API key, marking it as inactive. Returns the updated API key or null.                                     |
| `GetAllApiKeyClaims`                    | Api Key Claims       | identityUser, transientRoles, transientClaims | Retrieves all claims of an api key, including role-based and optional transient claims.                                  |
| `GetRoleClaimAsync`                     | Api Key Claims       | roleId, getClaim                          | Retrieves a specific claim of an api key by claim type.                                                                  |
| `GetRoleClaimsAsync`                    | Api Key Claims       | roleId                                        | Retrieves all claims of an api key by apikeyId.                                                                           |
| `GetRoleClaimsAsync`                    | Api Key Claims       | identityRole                                  | Retrieves all claims of a api key instance.                                                                             |
| `AssignRoleClaimAsync`                  | Api Key Claims       | roleId, assignClaim                           | Assigns a claim to an api key.                                                                                           |
| `ReplaceRoleClaimAsync`                 | Api Key Claims       | roleId, replaceClaim                          | Replaces an existing claim of an api key with a new value.                                                               |
| `AssignOrReplaceRoleClaimAsync`         | Api Key Claims       | roleId, assignOrReplaceClaim                  | Assigns a claim to an api key or replaces it if it already exists.                                                       |
| `RemoveRoleClaimAsync`                  | Api Key Claims       | roleId, removeClaim                           | Removes a claim from an api key.                                                                                         |
| `GetRolesAsync`                         | Roles                | —                                             | Retrieves all roles in the system.                                                                                   |
| `CreateRoleAsync`                       | Roles                | roleName                                      | Creates a new role. Returns the created role.                                                                        |
| `DeleteRoleAsync`                       | Roles                | roleName                                      | Deletes an existing role.                                                                                            |
| `GetRoleClaimAsync`                     | Role Claims          | roleId, getClaim                              | Retrieves a specific claim of a role by claim type.                                                                  |
| `GetRoleClaimsAsync`                    | Role Claims          | roleId                                        | Retrieves all claims of a role by role ID.                                                                           |
| `GetRoleClaimsAsync`                    | Role Claims          | identityRole                                  | Retrieves all claims of a role instance.                                                                             |
| `AssignRoleClaimAsync`                  | Role Claims          | roleId, assignClaim                           | Assigns a claim to a role.                                                                                           |
| `ReplaceRoleClaimAsync`                 | Role Claims          | roleId, replaceClaim                          | Replaces an existing claim of a role with a new value.                                                               |
| `AssignOrReplaceRoleClaimAsync`         | Role Claims          | roleId, assignOrReplaceClaim                  | Assigns a claim to a role or replaces it if it already exists.                                                       |
| `RemoveRoleClaimAsync`                  | Role Claims          | roleId, removeClaim                           | Removes a claim from a role.                                                                                         |

Try it out yourself using the **[Api.Data.Identity](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.Mysql)** example.    

## Autosave
The repository can be configured for autosave. When enabled, all methods that modify data will automatically persist changes to the database. No need to call 
`dbContext.SaveChanges(...)` manually. Autosave is convenient for smaller queries or when adding/updating entire entity model trees in a single operation, rather than performing 
multiple individual add or update calls.  

If you need more fine-grained control over when changes are committed, you can disable `UseAutoSave` in the repository configuration.  

Try it out yourself using the **[Api.Data.Repository.Autosave](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.Repository.Autosave)**.  

## Cache
Currently, Nano does not support data caching.  

Future releases plan to add both in-memory caching and distributed caching with Redis. These can be used together to provide a short-lived in-memory cache 
for fast access and a longer-lived distributed cache for shared data across instances.  

## Include Annotation
Nano provides the `IncludeAttribute`, which can be placed on entity model navigation properties. It supports both reference and collection navigations and is ignored on all other 
property types. When retrieving entities through `IRepository` methods, any navigation property decorated with `IncludeAttribute` will be automatically included in the query, 
similar to using `IQueryable.Include(...)`. Inclusion works recursively in all directions up to the configured include depth.  

Be cautious when including collection navigations, as this can result in retrieving a large number of records. It is best used when the collection size is relatively small.  

For owned entity models, if a navigation property inside the owned type has the `IncludeAttribute`, the parent property referencing the owned entity must also be annotated 
with `IncludeAttribute`. Otherwise, Nano will not traverse into the owned type during query inclusion.  

The `IncludeAttribute` also has an optional parameter, `QuerySplitBehavior`, which determines how the query is executed: `Single` (joins) or `SplitQuery` (separate queries). 
For large collections, `SplitQuery` is recommended. If no `QuerySplitBehavior` is specified, the globally configured default is used.  

This feature is particularly useful because it allows you to build full entity graphs rather than retrieving single entities. When you need only the base entity, the graph inclusion 
can easily be overridden through the various `IRepository` methods.

Try it out yourself using the **[Api.Data.Repository.Includes](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.Repository.Includes)**.  

## Audit
Audit logging is enabled by default in Nano. To enable auditing for an entity, the entity model must implement the `IEntityAuditable` interface. When implemented, the entity and all 
of its properties will be tracked for audit logging. Individual properties can be excluded from auditing using the `[AuditExclude]` annotation.  

When an auditable entity is updated, Nano records only the properties that have changed in the audit log. If an entity is updated while detached from the `DbContext`, Nano 
automatically retrieves the current database values to ensure that both the original and updated values are correctly captured.  

Even if auditing is not enabled for any entities, Nano still maps the audit entity models and their corresponding tables. This ensures the database schema remains consistent and 
allows auditing to be enabled later without requiring schema changes.

Nano provides two built-in audit entities: `AuditEntry` (or `AuditEntry<TIdentity>`) and `AuditEntryProperty` (or `AuditEntryProperty<TIdentity>`). These are mapped to the 
tables `__EFAudit` and `__EFAuditProperties`. An `AuditEntry` has a one-to-many relationship with `AuditEntryProperty`, which stores the individual property changes. The 
`AuditEntryProperty` collection is automatically included when retrieving `AuditEntry` entities through the use of the **[Include Annotation](#include-annotation)**, ensuring property 
changes are always available with the audit entry. Each audit entry stores the a `RequestId` if available, and the `CreatedBy` user. When a request is executed within an HTTP context, the authenticated user is recorded automatically. If 
no HTTP context is available, the `CreatedBy` value defaults to `"Anonymous"`.  

Audit integrates seamlessly with both **[Soft Delete](#soft-delete)** and entity **[Triggers](#triggers)**. Any changes made to an entity during before-save triggers are captured in the audit log, and for 
soft-deleted entities, the audit reflects the soft-delete state rather than a regular deletion state.  

The audit implementation is based on the **[EntityFramework Plus](https://github.com/zzzprojects/EntityFramework-Plus)** project.  

Try it out yourself using the **[Api.Data.Audit](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.Audit)**.  

## Soft Delete
If you want a model to use soft-delete, simply implement the `IEntitySoftDeletable` interface.  

When an entity implements `IEntitySoftDeletable` and is removed from the data context, it will be soft deleted instead of physically removed. Soft-deleted entities have 
their `IsDeleted` property set to the current Unix epoch time and are automatically filtered out in future queries. The `IsDeleted` column is created for all tables, regardless 
of whether it is soft-deletable or not.  

If you entity models derive from one of the `BaseEntity` classes, your model will already have the `IsDeleted` property implemented.  

Unlike regular deletes, soft-deleting entities does not support cascading deletes.  

When dealing with soft-deleted entities together with unique indexes, conflicts can arise if one or more deleted entities have duplicate unique values. Nano automatically 
adjusts unique indexes by appending the `IsDeleted` property, with the exception of the primary key.

Try it out yourself using the **[Api.Data.SoftDelete](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.SoftDelete)**.  

## Lazy Loading
Load related data from the database only when it is first accessed, not when the parent entity is retrieved. This can reduce unnecessary queries but may cause extra database 
calls if the data is accessed repeatedly. 

> ⚠️ Lazy-loading may trigger unexpected queries (N+1 problem), leading to serious performance issues. Use with caution.

When lazy loading is enabled in the configuration, navigation properties will be loaded automatically during serialization if they are marked with the 
[Include Annotation](#include-annotation). Normally, these properties should already be loaded, but if you retrieve entity models without including them, the serializer 
will trigger lazy-loading to fetch the missing data.  

Try it out yourself using the **[Api.Data.LazyLoading](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.LazyLoading)**.  

## Triggers
Triggers in Nano are logic-based events that occur before or after an add, update, or delete operation, and are not classic SQL triggers. These triggers are executed at the 
code level as part of saving changes to the Entity Framework DbContext and should not be confused with database triggers.  

Nano supports the following trigger events.  

| Trigger           | Lifecycle | Description                                             |
| ----------------- | --------- | ------------------------------------------------------- |
| `OnInserting`     | Before    | Triggered before the entity is inserted.                |
| `OnInserted`      | After     | Triggered after the entity is inserted.                 |
| `OnInsertFailed`  | -         | Triggered if an error occurs when inserting the entity. |
| `OnUpdating`      | Before    | Triggered before the entity is updated.                 |
| `OnUpdated`       | After     | Triggered after the entity is updated.                  |
| `OnUpdateFailed`  | -         | Triggered if an error occurs when updating the entity.  |
| `OnDeleting`      | Before    | Triggered before the entity is deleted.                 |
| `OnDeleted`       | After     | Triggered after the entity is deleted.                  |
| `OnDeleteFailed`  | -         | Triggered if an error occurs when deleting the entity.  |

Each entity model can have one or more triggers associated with it, and the same trigger type may also be configured for the same entity model. To add a trigger, define 
the action delegate in the [Data Mapping](#data-mapping), as shown below for `OnInserting`.  

```csharp
builder
    .OnInserting(entry => 
    {
        // logic
    });
```

The `entry` parameter provides access to the entity model associated with the trigger, as well as the current `DbContext` and the application's `IServiceProvider`. This setup 
allows you to access or resolve any services required to handle the trigger logic, such as logging, notifications, or other supporting operations. By using the `entry` parameter, 
you can implement triggers that interact seamlessly with the rest of your application's infrastructure while keeping the trigger code clean and focused.

Delete triggers in Nano also work with soft-delete, so they will correctly invoke `OnDeleting`, `OnDeleted`, and other related events. It is recommended not to modify the 
entity that caused the trigger in any actions that occur after the entity has been saved to the database, as this can lead to duplicate update invocations. Use post-save events 
for tasks other than altering the entity itself.

> ⚠️ You do not need to call `SaveChanges` on objects modified within triggers. Nano handles this automatically, and manual calls are discouraged.

Triggers are best used for small, self-contained tasks, such as updating an `UpdatedAt` property in an `OnUpdating` trigger or calculating an aggregate value based on 
entity properties. More complex logic should be handled in dedicated services within your application.

Try it out yourself using the **[Api.Data.Triggers](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.Triggers)**.  

## Entity Events
In Nano, entity events provide a straightforward way to synchronize data changes across applications. They enable automatic propagation of entity state changes based on navigation properties and 
reverse dependency tracking, ensuring that related models remain consistent across service boundaries. In a microservice architecture, managing relationships and consistency between distributed data models is 
inherently complex. Entity events address this challenge by introducing a lightweight, attribute-driven eventing model that abstracts cross-service communication. Instead of manually implementing integration 
logic, developers can declaratively define event behavior using attributes. 

The `PublishAttribute` is used to emit an `EntityEvent` whenever an entity is created, modified, or deleted. These events represent the intent and outcome of a state change in a structured and consistent format.
On the receiving side, models annotated with the `SubscribeAttribute` can react to these events. The built-in `EntityEventHandler` processes incoming events and automatically applies the corresponding 
create, update, or delete operations to the local model.  

This approach provides a simple and consistent mechanism for maintaining data synchronization across services, reducing integration complexity while preserving clear ownership boundaries between applications.  

> ⚠️ Entity Events require **[Eventing](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing)** to be configured in the application.

Only entities implementing `IEntityIdentity<TIdentity>` are eligible for participation in entity eventing. The attribute allows a configurable list of publish properties, which determines the data included 
in the generated event payload. For entities deriving from `BaseEntity`, the `CreatedAt` property is automatically included to ensure consistent timestamp synchronization between publishers and subscribers.

The attribute supports inheritance, allowing publish behavior to be defined across base and derived types in a flexible way. The most specific type marked with `PublishAttribute` determines the published 
event type. If only a base class has the attribute, any derived entity will be published as that base type, which is useful for exposing a simplified contract model to subscribers while preserving internal 
inheritance structures. If both the base and derived types are annotated, each is published according to its own definition. When the base class is abstract, it does not emit events directly but instead 
contributes publish property definitions that are inherited by derived types.  

Publish properties are string names defined on the `PropertyNames` property of the `PublishAttribute`. Publish properties can traverse navigation paths, like `Customer.Address.StreetName`. Only valid 
Entity Framework scalar types are allowed as the final segment of a navigation path. To ensure deterministic event structure, the final property segment must be unique across all defined navigation paths. 
For instance, it is not allowed to publish both `BillingAddress.StreetName` and `DeliveryAddress.StreetName`, as they would conflict on the same terminal property name. In addition, only reference or 
owned navigations are supported. Collection navigations are explicitly excluded, since collections should be modeled and published as separate entities rather than embedded within a single entity 
event definition.

When publish properties include navigation paths, Nano ensures that all required data is available before an event is created. During direct operations (create or update), Nano traverses the defined 
navigation paths and automatically hydrates missing data from the database where necessary. This guarantees that the emitted entity event is fully populated, even if related entities were not explicitly 
loaded. For reverse change scenarios, where a dependent entity is modified or deleted, Nano traverses the relationship graph in the opposite direction. It identifies all affected root entities and emits 
corresponding entity events for each impacted aggregate. To optimize performance, hydration is skipped entirely when a modification does not affect any defined publish properties. In such cases, no event 
is generated, since there is no relevant change to propagate.  

```csharp
[Publish(nameof(Name))]
public class Customer : BaseEntity
{
    public string Name { get; set; }
}
```

> ⚠️ Avoid overly complex publish path configurations, as they may trigger unnecessary data hydration and impact performance.

The `SubscribeAttribute` defines how an entity reacts to incoming `EntityEvent`s. It is applied to models that should be synchronized with published changes from other applications. The subscribing model 
represents a flattened projection of the publish properties, where each subscribed field is mapped using the name of the final segment in the publish path. When an event is received, the built-in event handling 
pipeline resolves the target entity and applies the incoming changes automatically. Based on the event type, the model is created, modified, or deleted accordingly.  

```csharp
[Subscribe]
public class MyEntity : DefaultEntity
{
    public string Name { get; set; }
}
```

> 💡 Entity eventing can also be applied to identity-based models, enabling seamless synchronization of identity data across applications. For example, a `User` entity can 
publish properties such as `IdentityUser.Email` or `IdentityUser.Phone`, allowing downstream services like email or SMS systems to subscribe and receive all required data upfront. 
This enables each service to independently fulfill its responsibility without additional lookups or coupling to the source system.

Try it out yourself using the **[Api.Data.EntityEvents](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.EntityEvents)**.  
