# Nano.Data
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.svg)](https://www.nuget.org/packages/Nano.Data/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.svg)](https://www.nuget.org/packages/Nano.Data/)

> _Data provider common implementations for Nano applications._

> ⚠️ This NuGet is transitive and included in other Nano Packages, and is not meant to be included directly.

***

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)
* [Configuration](#configuration)
* [Data Providers](#data-providers)
* [Data Context](#data-context)
* [Data Models](#data-models)
* [Data Mappings](#data-mappings)
* [Migrations](#migrations)
* [Repositories](#repositories)
  * [Autosave](#autosave)
  * [Cache](#cache)
  * [Include Annotation](#include-annotation)
* [Connection Pool](#connection-pool)
* [Audit](#audit)
* [Soft Delete](#soft-delete)
* [Triggers](#triggers)
* [Identity](#identity)
* [Entity Events](#entity-events)
* [Health Checks](#health-checks)

## Summary
Nano provides a data access implementation built on top of **[Entity Framework](https://learn.microsoft.com/en-us/ef/)**, enabling applications to integrate with SQL-based 
databases in a consistent and structured way.

To enable data support in a Nano application, a few core components must be implemented and registered. First, a **[Data Providers](#data-providers)** must be selected. Next, 
the application must define a database context by deriving implementations from `BaseDbContext` and `BaseDbContextFactory`.  

Once the context has been implemented, data services must be registered during application startup by invoking `AddDataContext<TProvider, TContext>()` in `program.cs`. 
After registration, the application can define entity models derived from `BaseEntity`, along with their corresponding mappings derived from `BaseEntityMapping<TEntity>`.  

When Nano data is registered, the `IRepository` interface becomes available for interacting with the database. This abstraction uses the Unit-Of-Work pattern, providing 
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

| Setting                         | Type   | Default     | Description                                                                                                                                       |
| ------------------------------- | ------ | ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
|  `BatchSize`                    | int    | 25          | The maximum batch size for queries.                                                                                                               |
|  `BulkBatchSize`                | int    | 500         | The maximum batch size for bulk operations.                                                                                                       |
|  `BulkBatchDelay`               | int    | 1000        | The delay (in milliseconds) between bulk batches.                                                                                                 |
|  `QueryRetryCount`              | int    | 0           | The number of times a query will retry on failure.                                                                                                |
|  `UseLazyLoading`               | bool   | false       | A value indicating whether lazy loading is enabled. ⚠️ Not recommended, use [Nano Include Annotation](#include-annotation).                       |
|  `UseCreateDatabase`            | bool   | false       | A value indicating whether the database should be created automatically using just the mappings, bypassing migrations.                            |
|  `UseMigrateDatabase`           | bool   | false       | A value indicating whether database migrations should be applied automatically.                                                                   |
|  `UseSoftDeletetion`            | bool   | false       | A value indicating whether soft deletion is enabled.                                                                                              |
|  `UseSensitiveDataLogging`      | bool   | false       | A value indicating whether sensitive data logging is enabled.                                                                                     |
|  `UseAudit`                     | bool   | false       | A value indicating whether auditing is enabled.                                                                                                   |
|  `QuerySplittingBehavior`       | enum   | SingleQuery | The default query splitting behavior for EF Core queries.                                                                                         |
|  `DefaultCollation`             | string | null        | The default collation for the database.                                                                                                           |
|  `ConnectionString`             | string | null        | The connection string for the database.                                                                                                           |
|  `Repository`                   | object | default     | The cache configuration options. See [Repositories](#repositories).                                                                               |
|  `Repository.UseAutoSave`       | bool   | true        | A value indicating whether automatic saving of changes in repositories is enabled.                                                                |
|  `Repository.QueryIncludeDepth` | int    | 4           | The maximum depth for query includes.                                                                                                             |
|  `ConnectionPool`               | object | null        | The connection pool configuration options. See [Connection Pool](#connection-pool).                                                               |
|  `Cache`                        | object | null        | The cache configuration options. See [Cache](#cache).                                                                                             |
|  `Identity`                     | object | null        | The identity configuration options. See [Identity](#identity).                                                                                    |
|  `HealthCheck`                  | object | null        | The options for configuring health checks. See [HealthCheck](#health-check)  _Only relevant for `NanoApiApplication` and `NanoWebApplication`_.   |

```json
"Data": {
  "BatchSize": 25,
  "BulkBatchSize": 500,
  "BulkBatchDelay": 1000,
  "QueryRetryCount": 0,
  "UseLazyLoading": false,
  "UseCreateDatabase": false,
  "UseMigrateDatabase": false,
  "UseSoftDeletetion": false,
  "UseSensitiveDataLogging": false,
  "UseAudit": false,
  "QuerySplittingBehavior": "SingleQuery",
  "DefaultCollation": null,
  "ConnectionString": null,
  "Repository": { 
    "UseAutoSave": false,
    "QueryIncludeDepth": 4
  },
  "ConnectionPool": { },
  "Cache": { },
  "Identity": { },
  "HealthCheck": { }
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

* [Nano.Data.InMemory](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.InMemory)
* [Nano.Data.MySql](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.MySql)
* [Nano.Data.PostgreSQL](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.PostgreSQL)
* [Nano.Data.SqLite](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.SqLite)
* [Nano.Data.SqlServer](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.SqlServer)

## Data Context
Nano provides built-in management for the `DbContext` while still letting you use it as you normally would.  

To integrate your database, simply derive a class from `BaseDbContext` or `BaseDbContext<TIdentity>` if you want to use a primary key type other than `Guid`.

```csharp
public class MyDbContext(DbContextOptions contextOptions, IOptionsMonitor<DataOptions> dataOptions, IEventing? eventing = null)
    : BaseDbContext(contextOptions, dataOptions, eventing);
```
You don’t need to implement any additional logic in `MyDbContext`. Entity Framework only requires that the `DbContext` exists in the entry assembly, where `program.cs` is 
located. Nano will automatically detect and configure all [Data Mappings](#data-mappings), so overriding the `OnModelCreating(...)` method is unnecessary.

> ⚠️ If you override any methods from `BaseDbContext`, always call the `base` method to ensure Nano functions correctly.

In addition, you must derive a concrete implementation from `BaseDbContextFactory`.  

```csharp
public class MySqlDbContextFactory : BaseDbContextFactory<MySqlProvider, MySqlDbContext>;
```

No further implementation is required. The factory class simply needs to exist in your application project alongside `program.cs`, similar to the `BaseDbContext` implementation.

## Data Models
Models in Nano, also referred to as entities, represent the tables in your database.

To create a model, derive a class from `BaseEntity` or `BaseEntity<TIdentity>` if you want to use a custom primary key type. By inheriting from `BaseEntity`, your model 
automatically includes several built-in properties.  

| Property      | Type            | Description                                                                                     |
| ------------- | --------------- | ----------------------------------------------------------------------------------------------- |
| `Id`          | TIdentity       | The primary key of the model. _Automatically assigned for new instances._                       |
| `CreatedAt`   | DateTimeOffset  | The timestamp when the model was created. _Automatically set to `UtcNow` for new instances._    |
| `IsDeleted`   | int             | Used only when [Soft Delete](#soft-delete) is enabled. _Defaults to 0._                         |

```csharp
public class MyEntity : BaseEntity
{
    // Properties
}
```

If you specify a `TIdentity` type during data [Registration](#registration) and in your [Data Context](#data-context), you must use the same type when deriving your 
concrete entities.

Alternatively, you can derive your entity model from one of the specialized CRUD base classes: `BaseEntityCreatable`, `BaseEntityCreatableAndUpdatable`, `BaseEntityUpdatable`, 
or `BaseEntityDeletable`, to restrict the allowed `IRepository` operations for that entity.  

For more advanced scenarios, if you do not want the built-in properties provided by Nano, you can derive your entity model from `BaseEntityIdentity` or 
`BaseEntityIdentity<TIdentity>`. This gives your entity only the `Id` property, but limits most built-in `IRepository` operations. To restore specific operations, your entity 
must implement the corresponding interfaces: `IEntityWritable`, `IEntityCreatable`, `IEntityCreatableAndUpdatable`, `IEntityUpdatable`, or `IEntityDeletable` 
(`IEntityDeletableSoft`). These interfaces mirror the functionality of the CRUD base entity classes.  

> ⚠️ For simplicity and maintainability, it is recommended to derive entity models from `BaseEntity` or one of the specific base classes rather than 
implementing the interfaces directly.  

Nano also supports spatial `Geometry` types from `NetToplogySuite`.  

## Data Mappings
Each data model in your application should have a corresponding data mapping. Data mappings define how your entities are configured in the database and allow you 
to customize Entity Framework behavior.

```csharp
public class MyEntityMapping : BaseEntityMapping<MyEntity>
{
    public override void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        base.Configure(builder);

        // Add custom Entity Framework mappings here.
    }
}
```

> ⚠️ Always call `base.Configure(builder);` in your data mappings to ensure Nano functions correctly.  

Nano automatically applies all data mappings, so there is no need to manually register them. Only non-abstract, non-generic mapping classes are automatically 
detected and applied.  

> ⚠️ It is strongly recommended to map all properties of your entity models within your data mappings to ensure consistency and correctness.

Nano also supports mapping read-only database views. To do this, derive your mapping from `BaseEntityViewMapping<TEntity>`.

```csharp
public class MyEntityViewMapping : BaseEntityViewMapping<MyEntity>
{
    public override void Map(EntityTypeBuilder<MyEntity> builder)
    {
        base.Map(builder);

        // Add custom Entity Framework mappings for the view here.
    }
}
```

Nano automatically updates all unique index mappings to include the `IsDeleted` property. This ensures that soft-deleted entities can coexist without violating 
uniqueness constraints.  

## Migrations
Migrations in Nano work the same way as standard Entity Framework migrations.  

Before creating migrations, ensure you have implemented a `BaseDbContextFactory<MySqlProvider, MySqlDbContext>`. Then, in PowerShell, add a new migration.  

```powershell
dotnet ef migrations add Initial --project {project}
```

Migrations are not applied automatically unless the `UseMigrations` option is enabled in the configuration.

> ⚠️ It is recommended to enable `UseMigrations` **only** in `Development` environments.

In `Staging` and `Production`, migrations are applied during deployment, via GitHub Actions.

```powershell
dotnet ef database update `
  --no-build `
  --startup-project $env:APP_NAME `
  --connection "$env:MYSQL_MIGRATION_CONNECTIONSTRING";
```

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

| Method                                      | Parameters                                                | TEntity Constraint           | Description                                                                        |
| ------------------------------------------- | --------------------------------------------------------- | ---------------------------- |----------------------------------------------------------------------------------- |
| `GetAsync<TEntity, TKey>`                   | key, includeDepth                                         | IEntityIdentity              | Gets an entity by its unique key including related entities to a specified depth.  |
| `GetFirstAsync<TEntity, TCriteria>`         | criteria, includeDepth                                    | IEntity                      | Gets the first entity matching the specified criteria.                             |
| `GetFirstAsync<TEntity>`                    | where, ordering, includeDepth                             | IEntity                      | Gets the first entity matching the predicate.                                      |
| `GetManyAsync<TEntity, TKey>`               | keys, includeDepth                                        | IEntityIdentity              | Gets entities matching the specified keys.                                         |
| `GetManyAsync<TEntity>`                     | query, includeDepth                                       | IEntity                      | Gets entities matching the specified query.                                        |
| `GetManyAsync<TEntity, TCriteria>`          | criteria, includeDepth                                    | IEntity                      | Gets entities matching the specified criteria.                                     |
| `GetManyAsync<TEntity>`                     | where, pagination, ordering, includeDepth                 | IEntity                      | Gets entities matching a predicate with pagination and ordering.                   |
| `GetManyAsync<TEntity, TKey>`               | where, pagination, includeDepth, orderBy, orderDirection  | IEntity                      | Gets entities matching a predicate ordered by a key selector with pagination.      |
| `AddAsync<TEntity>`                         | entity                                                    | IEntityCreatable             | Adds a single entity.                                                              |
| `AddAndGetAsync<TEntity, TKey>`             | entity                                                    | IEntityIdentity              | Adds an entity and reloads it including related entities.                          |
| `AddManyAsync<TEntity>`                     | entities                                                  | IEntityCreatable             | Adds multiple entities.                                                            |
| `AddManyBulkAsync<TEntity>`                 | entities                                                  | IEntityCreatable             | Bulk adds multiple entities using EF Plus Enterprise.                              |
| `UpdateAsync<TEntity>`                      | entity                                                    | IEntityUpdatable             | Updates a single entity.                                                           |
| `UpdateAndGetAsync<TEntity, TKey>`          | entity                                                    | IEntityIdentity              | Updates an entity and reloads it including related entities.                       |
| `UpdateManyAsync<TEntity>`                  | entities                                                  | IEntityUpdatable             | Updates multiple entities.                                                         |
| `UpdateManyAsync<TEntity>`                  | where, propertyUpdates                                    | IEntityUpdatable             | Updates entities matching a predicate.                                        |
| `UpdateManyAsync<TEntity, TCriteria>`       | criteria, propertyUpdates                                 | IEntityUpdatable             | Updates entities based on specified criteria.                                 |
| `UpdateManyBulkAsync<TEntity>`              | entities                                                  | IEntityUpdatable             | Bulk updates multiple entities using EF Plus Enterprise.                           |
| `UpdateManyBulkAsync<TEntity>`              | where, propertyUpdates                                    | IEntityUpdatable             | Bulk updates entities matching a predicate.                                        |
| `UpdateManyBulkAsync<TEntity, TCriteria>`   | criteria, propertyUpdates                                 | IEntityUpdatable             | Bulk updates entities based on specified criteria.                                 |
| `AddOrUpdateAsync<TEntity>`                 | entity                                                    | IEntityCreatableAndUpdatable | Adds or updates a single entity.                                                   |
| `AddOrUpdateManyAsync<TEntity>`             | entities                                                  | IEntityCreatableAndUpdatable | Adds or updates multiple entities.                                                 |
| `DeleteAsync<TEntity, TKey>`                | id                                                        | IEntityIdentity              | Deletes an entity by its key.                                                      |
| `DeleteAsync<TEntity>`                      | entity                                                    | IEntityDeletable             | Deletes a specific entity instance.                                                |
| `DeleteManyAsync<TEntity, TKey>`            | ids                                                       | IEntityIdentity              | Deletes multiple entities by their keys.                                           |
| `DeleteManyAsync<TEntity>`                  | entities                                                  | IEntityDeletable             | Deletes multiple entities.                                                         |
| `DeleteManyAsync<TEntity, TCriteria>`       | criteria                                                  | IEntityDeletable             | Deletes entities matching specified criteria.                                      |
| `DeleteManyAsync<TEntity>`                  | expression                                                | IEntityDeletable             | Deletes entities matching a filter expression.                                     |
| `DeleteManyBulkAsync<TEntity, TKey>`        | ids                                                       | IEntityIdentity              | Bulk deletes entities with specified keys.                                         |
| `DeleteManyBulkAsync<TEntity>`              | entities                                                  | IEntityDeletable             | Bulk deletes specified entities.                                                   |
| `DeleteManyBulkAsync<TEntity, TCriteria>`   | criteria                                                  | IEntityDeletable             | Bulk deletes entities matching specified criteria.                                 |
| `DeleteManyBulkAsync<TEntity>`              | expression                                                | IEntityDeletable             | Bulk deletes entities matching a filter expression.                                |
| `CountAsync<TEntity, TCriteria>`            | criteria                                                  | IEntity                      | Returns the number of entities matching the criteria.                              |
| `CountAsync<TEntity>`                       | expression                                                | IEntity                      | Returns the number of entities matching a filter expression.                       |
| `SumAsync<TEntity>`                         | whereExpr, sumExpr                                        | IEntity                      | Calculates the sum of a numeric expression for matching entities.                  |
| `AverageAsync<TEntity>`                     | whereExpr, avgExpr                                        | IEntity                      | Calculates the average of a numeric expression for matching entities.              |
| `SaveChangesAsync`                          | -                                                         | -                            | Persists all pending changes to the data store.                                    | 

Several methods include overloads, which have been merged here for simplicity.  

One of the most useful parameters is `includeDepth`, which overrides the globally configured include depth and determines how many levels of [Include Annotations](#include-annotation) 
are applied in a query. This allows you to map complex entity models with related entities, while also controlling how much of the entity graph is loaded. Sometimes you may want 
only the plain entity, while other times you may need the full inclusion tree.  

Try it out yourself using the **[Api.Data.Mysql](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.Mysql)** or  
**[Console.Data.Mysql](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Data.Mysql)** examples. Similar examples are available for other data providers as well.  

## Autosave
The repository can be configured for autosave. When enabled, all methods that modify data will automatically persist changes to the database. No need to call 
`dbContext.SaveChanges(...)` manually. Autosave is convenient for smaller queries or when adding/updating entire entity model trees in a single operation, rather than performing 
multiple individual add or update calls.  

If you need more fine-grained control over when changes are committed, you can disable `UseAutoSave` in the repository configuration.  

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

## Audit
Even when audit is disabled the tables are still created. 

Audit->Propertes has INCLUDE.
The user needs to know the AuditEntry<TIdentity> (and the AuditEntryProperty<TIdentity>) to get them through api-client. Maybe this needs to be some place else, in Api docs??


When audit is enabled in the data section of the configuration, changes to all entities deriving from ```IEntity``` will be tracked. 
Auditting happens automatically and is executed asynchronously, to avoid impacting the response time.  

Two additional tables are created in the database for audit data storage. 
* ```__EFAudit```
* ```__EFAuditProperties```  
The tables are created even when ```UseAudit``` is disabled, but they will remain empty. This is in order to be able to switch at a later time, without having to worry about database migrations. The frist, contains a row for each change made to an entity, while the second stores one row for each property of the entity.  

To include a model for audit, just implement the empty interface ```IEntityAuditable```. Though normally this isn't required explicitly as all models is included when deriving from ```DefaultEntity``` and auditing is enabled for the application.  
To exclude models implement the ```IEntityAuditableNegated``` or ```ExcludeAttribute```

The audit implementation is based on the [EntityFramework Plus](https://github.com/zzzprojects/EntityFramework-Plus) project.

## Soft Delete
In order for soft deletion to be enabled, it much be enabled in the data section of the configuration.  
When implementing the interface ```IEntityDeletableSoft```, or when deriving a model implementation from the ```DefaultEntity```, the entity will be soft deleted when removed from the data context. When soft deleted, the data doesn't get removed, but the row gets flagged as deleted, and filtered out in future queries.  

When dealing with soft deleted entities, together with unique indexes, a conflict can arise having one or more deleted entities with duplicate unique values. 
Nano automatically adjusts unique indexes, appending the ```IsDeleted``` property. This is with the exception of the property defined as primary key.

Opposite of using regular delete, soft-deleting entities doesn't support cascading deletes.

## Triggers
Nano supports mapping triggers for the models. 
Note, these triggers are executed on code-level, as part of saving changes to the context of Entity Framework, and not to be confused with database triggers. The implementation is based on the [EntityFramework.Triggers](https://github.com/NickStrupat/EntityFramework.Triggers) library, recommended by Microsoft.  

The triggers are defined as part of the mapping implementation, and may be associated with either insert, update and delete. Furthermore, the action can be invoked either before or after the triggering action.  
```csharp
public class MyEntityMapping : DefaultEntityMapping<MyEntity>
{
    public override void Map(EntityTypeBuilder<MyEntity> builder)
    {
        base.Map(builder);
        
        builder
            .OnUpdated(entry =>
            {
                var dbContext = entry.Context;
                var entity = entry.Entity;

                // action implementation.
            });
    }
}
```

Nano supports the following triggers.  
* On Inserted
* On Inserting
* On Updated
* On Updating
* On Deleted
* On Deleting

For more details about triggers and how to use them, consult the docucmenation of [EntityFramework.Triggers](https://github.com/NickStrupat/EntityFramework.Triggers).  

## Identity
Even when identity is not configured the tables are still created. 

Securing the application is about user identity, and authentication and authorization.  
Nano provides everything required to effectively manage user identities, and still leveraging full control for customizing polices, claims and roles. The default database created by Nano includes all the required tables, and contains all the technical identity data. This is extended with a custom generic user model during signup.  
The ```DefaultIdentityManager``` exposes methods for handling all the interaction with user identities, such as login, signup, change email, etc. The ```TransientIdentityManager``` exposes methods for authentication against non-persisted user identities. This could be through external provider login, or with the built-in administrator user.  
**NOTE:** Without a configured ```IDataProvider```, the identity features are highly limited. Only Transient operations will be available.  

Security contains many models, handling everything from login to change password, or logging in with an external provider. Most are straight forward, and is used as parameter for just one method in the ```IdentityManager```.  
When adding the initial database migration snapshot, models and mappings related to identity is injected. The models are based on ```Microsoft.AspNetCore.Identity``` library.  
Normally, you would derive your custom user from the ```IdentityUser<T>```, when building a store for user identity. This approach is not possible when encapsulating functionality, as the consumer would have to deal with too many factors, such as generic parameters and constraints. By using a composite user model, where the identity and user is separated, Nano is able to manage the identity part without having to worry about custom properties. The Signup methods in Nano automatically links the two tables, and when a ```CustomUser``` is retrieved, the related ```IdentityUser``` data is retrieved as well.  

The ```DefaultIdentityManager```, encapsulates features of Microsoft Identity (```UserManager``` and ```SignInManager```), exposes atomic methods for managing user identity, and simplifies using custom user identities, by separating the identity from the user.  
The ```TransientIdentityManager``` contains methods for logging in users without having a identity store. This can be used to login transiently using the  administrator user defined in the configuration, or by using one of the supported external providers. Transient logins can't be refreshed.  

Also Nano exposes a `SecurePasswordGenerator` class.

Identity Configuration:
| Setting                         | Type   | Default     | Description                                                                                                      |
| ------------------------------- | ------ | ----------- | ---------------------------------------------------------------------------------------------------------------- |
|  `User`                         | object | default     |                                                                                                                  |

```json
"Identity": {
  "User": {
    "IsUniqueEmailAddressRequired": true,
    "IsUniquePhoneNumberRequired": false,
    "AllowedUserNameCharacters": null,
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
    "RequireUppercase": false,
    "RequiredLength": 5,
    "RequiredUniqueCharacters": 0
  },
  "Authentication": {
    "ApiKey": {
      "Secret": null
    }
  }
}
```

Model:
The identity is associated with the user through a simple foreign key navigation, and is included when the user is queried, by deriving the custom user model from the ```DefaultEntityUser```.
 ```csharp
public class DefaultEntityUser : DefaultEntity
{
    [MaxLength(128)]
    public virtual string IdentityUserId { get; set; }

    [Include]
    public virtual IdentityUser IdentityUser { get; set; }
}
```

This isolates and hides all functionality related to the account of a user, and allows to work solely with the user  relevant to the application.
**NOTE:** All identity email addresses, user names and phone numbers must be unique or null. At least one must not be null.  

Mapping:
When mapping the user model, derive the mapping implementation from ```DefaultEntityUserMapping<TEntity>```. Besides that, mapping is no different than models not having an identity associated.  
```csharp
public class DefaultEntityUserMapping<TEntity> : DefaultEntityMapping<TEntity> 
    where TEntity : DefaultEntityUser
{
    public override void Map(EntityTypeBuilder<TEntity> builder)
    {
        base.Map(builder);
`
        builder
            .HasOne(x => x.IdentityUser)
            .WithOne()
            .IsRequired();
    }
}
```

When identity has been configured the following roles are automatically added

| Name          | Description                          |
| ------------- | ------------------------------------ |
| reader        | Authorized to read.                  | 
| writer        | Authorized to read and write.        | 
| creator       | Authorized to create.                | 
| editor        | Authorized to update.                | 
| deleter       | Authorized to create.                | 
| identity      | Authorized to use identity actions.  | 
| Administrator | Full access to everything.           | 

## Entity Events
Adding eventing annotations to model implementations, provides a way of synchronizing entities between applications. 

When building micro-service applications, managing relations and dependencies of shared models, becomes a challenge. The eventing attributes provides a very simple method publishing change notifications in one application, and subscribing to it in another.  
Eventing attributes is similar to database foreign-key relations, just in between services. The ```PublishAttribute``` publishes an ```EntityEvent``` whenever an instance is either created, updated or deleted. When receiving an event subscribed to by a model annotated with the ```SubscribeAttribute```, the built-in ```EntityEventHandler``` handles the event, and likewise creates, updates or deletes the instance.  
The contract between the publisher and the subscriber is the type ```EntityEvent```, using the entity type name (```Type.Name```, and not ```Type.FullName```), when routing the event. The subscriber doesn't have any knowledge about the event being fired, and shouldn't. By convention, the relationship is loosely coupled.  
```csharp
[Publish(params)]
[Subscribe]
public class MyEntity : DefaultEntity
{
    [InternationalPhone]
    public virtual string PhoneNumber { get; set; }
}
```
If the ```Publish``` is used on a base class, that entity type will be used when publishing the entity event. This allows discriminated types in the master service to be published as the base to subscribing services. Also, it's possible through the ```params``` parameter of the ```Publish``` annotation, to pass additional properties in the event. Also nested classes are supported by using the ```.``` operator. Property names can be split on derived and base class Subscribe annotation, and when publishing the derived class, the property names of both itself and the base class will be concatenated. Be aware that changes to properties on related entities, won't trigger a publish. In this case you need to trigger the update of the entity after the related entity has been saved.  
Publish/Subscribe can also work bi-directionally, but it would required the models in each service to have the same required properties, and probably best to keep them identical in this case.  

**NOTE**: Avoid sharing models having ```SubscribeAttribute```, with other Nano services, as the eventing subscription will be initialized unintentionally for that service as well.  

## Health Checks
When enabling health-checks in the data section of the confiugration, the application will be configured with a health-check for the data provider. When the application starts, a check is made to ensure that the data provider is up and running, returning a healthy status code when checked.  
