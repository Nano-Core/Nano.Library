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
  * [Include Annotation](#include-annotation)
* [Audit](#audit)
* [Soft Delete](#soft-delete)
* [Triggers](#triggers)
* [Cache](#cache)
* [Identity](#identity)
* [Entity Events](#entity-events)
* [Health Checks](#health-checks)

## Summary
Nano provides a robust data framework that enables applications to integrate with SQL databases.  
It's based on Entity Framework, and uses the unit-of-work pattern for easy and safe interafction with data, through the `IRepository` interface.  

When the `IDataProvider` is registered during application startup and the `BaseDbContext ` have been implemented, the application is ready to interact with a database. The 
`IRepository` interface gets registered as part of data in Nano, and enables easy access to common methods needed for data storage and retrieval. 

Nano data supports many features not built directly into Entity Framework.

## Registration
The data context and data provider must be registered as dependencies.  
Invoke the method ```.AddDataContext<TProvider, TContext>()```, using the data context and data provider implementations as generic type parameters.  

When registering the data provider, a **[Data Context](#data-context) must also be specified as part the generic type signature. 

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddDataContext<TProvider, TContext>();
})
...
```

By default Nano will use `guid` for all primary key columns. It's recommended to follow that, but Nano also support `string`, `int` and `long` for identity columns. To 
register a custom identity use the follow data registration.

```
...
.ConfigureServices(services =>
{
    services
        .AddDataContext<TProvider, TContext, TIdentity>();
})
...
```

> ⚠️ Using non-default identity requires `TIdentity` to be specified on [Data Models](#data-models), [Data Mappings](#data-mappings), and other abstractions in Nano.

## Configuration
The `Data` section in the configuration defines the data provider and related settings used by the application.

| Setting                         | Type   | Default     | Description                                                                                                                                       |
| ------------------------------- | ------ | ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
|  `BatchSize`                    | int    | 25          | The maximum batch size for queries.                                                                                                               |
|  `BulkBatchSize`                | int    | 500         | The maximum batch size for bulk operations.                                                                                                       |
|  `BulkBatchDelay`               | int    | 1000        | The delay (in milliseconds) between bulk batches.                                                                                                 |
|  `QueryRetryCount`              | int    | 0           | The number of times a query will retry on failure.                                                                                                |
|  `UseLazyLoading`               | bool   | false       | A value indicating whether lazy loading is enabled. ⚠️ Not recommended, use [Nano Include Annotation](#include-annotation)                        |
|  `UseCreateDatabase`            | bool   | false       | A value indicating whether soft deletion is enabled.                                                                                              |
|  `UseMigrateDatabase`           | bool   | false       | A value indicating whether the database should be created automatically using just the mappings and without migrations. ⚠️ Not recommended.       |
|  `UseSoftDeletetion`            | bool   | false       | A value indicating whether database migrations should be applied automatically.                                                                   |
|  `UseSensitiveDataLogging`      | bool   | false       | A value indicating whether sensitive data logging is enabled.                                                                                     |
|  `UseAudit`                     | bool   | false       | A value indicating whether auditing is enabled.                                                                                                   |
|  `QuerySplittingBehavior`       | enum   | SingleQuery | The default query splitting behavior for EF Core queries.                                                                                         |
|  `DefaultCollation`             | string | null        | The default collation for the database.                                                                                                           |
|  `ConnectionString`             | string | null        | The connection string for the database.                                                                                                           |
|  `Repository`                   | object | default     | The cache configuration options. See [Repositories](#repositories).                                                                               |
|  `Repository.UseAutoSave`       | bool   | true        | A value indicating whether automatic saving of changes in repositories is enabled.                                                                |
|  `Repository.QueryIncludeDepth` | int    | 4           | The maximum depth for query includes.                                                                                                             |
|  `Cache`                        | object | null        | The cache configuration options. See [Cache](#cache)                                                                                              |
|  `Identity`                     | object | null        | The identity configuration options. See [Identity](#identity)                                                                                     |
|  `ConnectionPool`               | object | null        | The connection pool configuration options. See [ConnectionPool](#connection-pool)                                                                 |
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
  "Cache": { },
  "Identity": { },
  "ConnectionPool": { },
  "HealthCheck": { }
}
```

## Data Providers
Data providers integrate a SQL database through Entity Framework into your Nano application.  

All data providers in Nano implement the `IDataProvider` interface. This interface is responsible for configuring and setting up the underlying 
data context, as well as providing an implementation of the `IRepository` interface interacting with the `DbContext`. 

To implement a new data provider:
1. Create a class that implements `IDataProvider`.
2. Register all required services and dependencies in the `Configure` method for the data provider.
3. Add your provider to the application using:

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoData<MyProvider, MyDbContext>();
})
...
```

The following data providers are currently supported in Nano:
* [Nano.Data.InMemory](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.InMemory)
* [Nano.Data.MySql](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.MySql)
* [Nano.Data.PostgreSQL](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.PostgreSQL)
* [Nano.Data.SqLite](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.SqLite)
* [Nano.Data.SqlServer](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.SqlServer)

Additional providers can be implemented by following the same pattern, allowing you to extend Nano.

## Data Context
Nano takes care of managing the `DbContext`. 
Simply, derive an implementation from the `BaseDbContext` or `BaseDbContext<TIdentity>` (if you want to use different primary key columns that `Guid`).

```csharp
public class MyDbContext(DbContextOptions contextOptions, IOptionsMonitor<DataOptions> dataOptions, IEventing? eventing = null)
    : BaseDbContext(contextOptions, dataOptions, eventing);
```

You don't actually need to implement anything in `MyDbContext`, Entity Framework just requires the `DbContext` to be located in entry assemlby, where `program.cs` 
is located. Also, Nano will automatically find and configure all [Data Mappings](#data-mappings), so there is no need to override the `OnModelCreating(...)` method. 

> ⚠️ If you choose to override any methods from `BaseDbContext`, remember to call the `base` method, to ensure Nano will work correctly.

Also DbContextFactory needs to be derived from `BaseDbContextFactory`, as shown below.  

```csharp
public class MySqlDbContextFactory : BaseDbContextFactory<MySqlProvider, MySqlDbContext>;
```

Again, you don't need to implement anything, they just need to be located in your application project, same as where `program.cs` resides.  

## Data Models
Models, also referred to as entities, represent the tables in the database.  

To create a model in Nano, derive an implementation from `BaseEntity` or `BaseEntity<TIdentity>`.  
Your model will automatically get the following properties.

| Property      | Type            | Dscription                                                                                       |
| ------------- | --------------- | ------------------------------------------------------------------------------------------------ |
| `Id`          | TIdentity       | The primary key of type `TIdentity` the model. _Automatically instantiated for new instances._   |
| `CreatedAt`   | DateTimeOffset  | The date-time the model was created. _Automatically set to `UtcNow` for new instances._          |
| `IsDeleted`   | int             | Only used when [Soft Delete](#soft-delete) is enabled in configuration. _Default=0._             |

```csharp
public class MyEntity : BaseEntity
{
    // Properties
}
```

If you have specified a `TIdentity` type during data [Registration](#registration) and when deriving the [Data Context](#data-context), you must also specify the same type 
when deriving your concrete entities.

Alternatively, you can also derive you model from `BaseEntityIdentity` or `BaseEntityIdentity<TIdentity>`, to only inherit the `Id` property for your model.
EXPLAIN MORE ABOUT THE Entity interfaces and how if you choose to enherit from `BaseEntityIdentity<TIdentity>`, you manually need to add interfaces for Creatable, Updateable,
Deleteable. OR CREATE more BaseEntiy classes for that. 
MENTION IEntity as topmost interface
AND CONSUMERS SHOULDN'T DERIVE FROM `BaseEntityIdentity`

Nano also supports spatial `Geometry` types from `NetToplogySuite`, and spatial SQL operations.    

## Data Mappings
For every data model in your application a corresponding data mappings must be implemented as well.

```csharp
public class MyEntityMapping : BaseEntityMapping<MyEntity>
{
    public override void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        base.Configure(builder);

        // Entity Framework mappings.
    }
}
```

> ⚠️ Remember, to always invoke the `base.Configure(builder);`, or Nano might not work correctly.  

Data mappings are automatically applied in Nano, and there is no need to manually apply the model data mappings. Only non-abstract, no-generic mapping types 
will be automatically mapped.

> ⚠️ It's highly recommended to fully map all properties on your models in your data mappings.  

Nano also supports mapping read-only views in your application. Simply derive your mapping from `BaseEntityViewMapping<TEntity>`, as shown below.  

```csharp
public class MyEntityMapping : BaseEntityViewMapping<MyEntity>
{
    public override void Map(EntityTypeBuilder<MyEntity> builder)
    {
        base.Map(builder);

        // Entity Framework mappings.
    }
}
```

Nano will update all unique index mappings to include `IsDeleted` property, on order to ensure soft deleted entities can co-exist.   

## Migrations
Migrations in Nano is no different than normal.  

Ensure you have derived an implementation from `BaseDbContextFactory<MySqlProvider, MySqlDbContext>`. Then open powershell and add a new migration.  

```powershell
PM> dotnet ef migrations add Initial --project {project}
```

In `Development` environment migrations are not applied automatically, unelss `UseMigrations` are enabled in the configuration.  

> ⚠️ It's recommended to enable `UseMigrations` configuration only in `Development`.  

In `Staging` and `Production` environment migrations are applied during application deployment in GitHub Actions, as shown below.  

```powershell
dotnet ef database update `
  --no-build `
  --startup-project $env:APP_NAME `
  --connection "$env:MYSQL_MIGRATION_CONNECTIONSTRING";
```

> 📖 Learn more about **EF Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)**

## Repositories
Repositories represents data repositories within an application.  
The ```IRepository``` interface contains methods for getting, querying, adding, updating and deleting models in the application. Inject this, when implementations requires access to
the application data.  
The ```BaseRepository<TContext>``` implements the interface, and ensures data is stored and retrieved through the data context defined by the generic class parameter and injected into 
the constructor when resolved at runtime.  
The ```DefaultRepository``` derives from ```BaseRepository<DefaultDbContext>```. and the ```DefaultDbContext``` contains overridden methods for saving changes, featuring eventing by 
annotation and custom extensions for entity framework.  
The services follow the ```UnitOfWork``` pattern.  

The ```BaseRepository<TContext>``` implementation contains various methods for getting, querying, updating, adding and deleting entities in the data context defined by the generic 
type parameter ```TContext```. This dependency is registered when configuring the data provider.  
The methods definitions of ```IRepository```, has different generic type constraints, depending on the operation. For instance ```AddAsync<TEntity>```, is constrained to models 
implementing ```IEntityCreatable```.  
The ```IRepository``` also contains a property, to access the underlying ```DbSet``` of an entity, 
* ```DbSet<TEntity> GetEntitySet<TEntity>()```   
Use it for advanced operations not directly supported by the repository implementation.  

When adding only the raw entity is returned from IRepository, and not included columns. Use AddAndGet to refersh all the includes.

_Name_ is the name of the migration, _project_ is the project where the ```DbContext``` implementation is located and where the migration script will be saved to. Last, 
the _environment_ is the configuration to use, and it's important the connection-string in the settings file of the environment is valid, otherwise an error occurs and the migration fails.

## Autosave
The repository can be configured for autosave. It's a convinience to not have to call `dbContext.SaveChanges(...)`.  
If you want more fine-grained control of when changes are comitted, disable `UseAutoSave` in configuration.  

## Include Annotation
Annotating a property with the ```IncludeAttribte```, instructs the repository layer to fetch additional data when getting and querying the entity. It works similar to 
the ```IQueryable.Include(...)``` extension, but allows for design-time definition. The property must be a class and have a navigation relations, otherwise the annotation is ignored.   
The maximum query (join) depth for a model having properties decorated with ```IncludeAttribte```, can be set in the data options of the configuration, ```QueryIncludeDepth```.  
When having navigations inside owned models decorated with include annotation, then the owned model property on the parent must also be annotated with Include. This will be ignored 
by entity framework, but allows Nano to trigger the nested include.  
Note, that when creating or updating entities the include is not interpreted.  

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

## Cache
Simple memory caching can be enabled, by setting ```Data.UseMemoryCache = true``` in the configuration. The cache stores queries once executed, for future invocations. 


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

**NOTE**: Entity events are sent through the repository implementation and not the database context. Bypassing the repository and using the database context directly, will also bypass the entity events.  
**NOTE**: Avoid sharing models having ```SubscribeAttribute```, with other Nano services, as the eventing subscription will be initialized unintentionally for that service as well.  

## Health Checks
When enabling health-checks in the data section of the confiugration, the application will be configured with a health-check for the data provider. When the application starts, a check is made to ensure that the data provider is up and running, returning a healthy status code when checked.  
