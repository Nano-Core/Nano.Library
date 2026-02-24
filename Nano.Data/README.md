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
* [Migration](#migration)
* [Repositories](#repositories)
* [Audit](#audit)
* [Spacial Types](#spacial-types)
* [Identity](#identity)
* [Soft Delete](#soft-delete)
* [Triggers](#triggers)
* [Cache](#cache)
* [Entity Eventing](#entity-events)
* [Health Checks](#health-checks)
* [Special Annotations](#special-annotations)
* [Examples](#examples)

THIS NUGET SHOULD NOT BE INSTALLED DIRECTLY SEE Providers

## Summary
Data access management in Nano.  
That includes database creation, transaction management and object-relation-mapping.  

Nano uses Entity Framework Core for managing object relation mapping, and handling data contexts within the application.  The main parts of data in Nano, evolves around the data provider and the data context.  

The ```IDataProvider``` is registered during startup, and the implementing type defines the data provider used in the application.  

The ```DbContext``` of Entity Framework is used in Nano, by the inheriting abstract class ```BaseDbContext```. Furthermore, the class ```DefaultDbContext``` derives from ```BaseDbContext```, and your custom data context implementation should derive from that. Both Nano data context implementations, overrides certain aspects of Entity Framework, in order to  extends it's functionality and to circumvent missing features.  

Besides the above, the data context and the data provider must be initialized during application startup, and models mapped to corresponding data mapping implementations.  

When starting the application, the database of the data context will be (if enabled). Additionally, any pending migrations will in-turn be applied to the database.  

## Registration
The data context and data provider must be registered as dependencies.  
Invoke the method ```.AddDataContext<TProvider, TContext>()```, using the data context and data provider implementations as generic type parameters.  

By default, the ```BaseDbContext``` dependency is registered to resolve to ```DefaultDbContext```. When registering a custom data context implementation, the registration is mitigated, and both base classes will resolve to the ```TContext``` generic type parameter implementation.  

```csharp
.ConfigureServices(x =>
{
    x.AddDataContext<MySqlProvider, MyDbContext>(); // With default Guid type for Identity.
})

// with string as custom type for Identity type.
.ConfigureServices(x =>
{
    x.AddDataContext<MySqlProvider, MyDbContext, string>(); // With user defined type for Identity.
})
```

## Configuration
The ```Data``` section in the configuration defines the data provider and related settings used by the application.

| Setting                         | Type   | Default     | Description                                                                           |
| ------------------------------- | ------ | ----------- | ------------------------------------------------------------------------------------- |
|  `BatchSize`                    | int    | 25          | The maximum batch size for queries.                                                   |
|  `BulkBatchSize`                | int    | 500         | The maximum batch size for bulk operations.                                           |
|  `BulkBatchDelay`               | int    | 1000        | The delay (in milliseconds) between bulk batches.                                     |
|  `QueryRetryCount`              | int    | 0           | The number of times a query will retry on failure.                                    |
|  `QueryIncludeDepth`            | int    | 4           | The maximum depth for query includes.                                                 |
|  `UseAutoSave`                  | bool   | true        | A value indicating whether automatic saving of changes in repositories is enabled.    |
|  `UseLazyLoading`               | bool   | false       | A value indicating whether lazy loading is enabled.                                   |
|  `UseCreateDatabase`            | bool   | false       | A value indicating whether soft deletion is enabled.                                  |
|  `UseMigrateDatabase`           | bool   | false       | A value indicating whether the database should be created automatically.              |
|  `UseSoftDeletetion`            | bool   | false       | A value indicating whether database migrations should be applied automatically.       |
|  `UseSensitiveDataLogging`      | bool   | false       | A value indicating whether sensitive data logging is enabled.                         |
|  `UseAudit`                     | bool   | false       | A value indicating whether auditing is enabled.                                       |
|  `QuerySplittingBehavior`       | enum   | SingleQuery | The default query splitting behavior for EF Core queries.                             |
|  `DefaultCollation`             | string | null        | The default collation for the database.                                               |
|  `ConnectionString`             | string | null        | The connection string for the database.                                               |
|  `Cache`                        | object | null        | The cache configuration options. See [Cache](#cache)                                  |
|  `Identity`                     | object | null        | The identity configuration options. See [Identity](#identity)                            |
|  `ConnectionPool`               | object | null        | The connection pool configuration options. See [ConnectionPool](#connection-pool)              |
|  `HealthCheck`                  | object | null        | The options for configuring health checks. See [HealthCheck](#health-check)  _Only relevant for `NanoApiApplication` and `NanoWebApplication`_.                 |

```json
"Data": {
  "BatchSize": 25,
  "BulkBatchSize": 500,
  "BulkBatchDelay": 1000,
  "QueryRetryCount": 0,
  "QueryIncludeDepth": 4,
  "UseAutoSave": false,
  "UseLazyLoading": true,
  "UseCreateDatabase": true,
  "UseMigrateDatabase": true,
  "UseSoftDeletetion": true,
  "UseSensitiveDataLogging": false,
  "UseAudit": false,
  "QuerySplittingBehavior": "SingleQuery",
  "DefaultCollation": null,
  "ConnectionString": null,
  "Cache": { },
  "Identity": { },
  "ConnectionPool": { },
  "HealthCheck": { }
},
```

## Data Providers
Data providers integrate a SQL database into your Nano application and provide easy access to mapped directories.  

All data providers implement the `IDataProvider` interface. 
This interface is responsible for handling all configuration and setup required for the storage provider.  

To implement a new storage provider:

1. Create a class that implements `IDataProvider`.
2. Ensure that all required services are registered in `Configure` methods.
3. Register your provider in the application using `AddNanoData<MyProvider, MyDbContext>()`.

The following data providers are currently supported:
* [Nano.Data.InMemory](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.InMemory)
* [Nano.Data.MySql](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.MySql)
* [Nano.Data.PostgreSQL](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.PostgreSQL)
* [Nano.Data.SqLite](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.SqLite)
* [Nano.Data.SqlServer](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.SqlServer)

## Data Context
Create the data context implementation, by deriving a class from ```DefaultDbContext```.  

Later, associations between models and their mappings will be declared in the method ```OnModelCreating(...)```. Remember to invoke base class method!

```csharp
// Deriving from default db-context.
public class MyDbContext : DefaultDbContext
{
    public MyDbContext(DbContextOptions options)
        : base(options)
    {  }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}

// Deriving from base db-context, specifying custom identity type.
public class MyDbContext : BaseDbContext<string>
{
    public MyDbContext(DbContextOptions options)
        : base(options)
    {  }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}

```

You also need to implement DbContextFactory for use with migrations. Derived from Default- or BaseDbContextFactory
```csharp
public class MyDbContextFactory : BaseDbContextFactory<MySqlProvider, MyDbContext>
{

}
```

## Data Models
Models, also referred to as entities, represent the definitions of data in the application.   

Derive a model implementation from ```DefaultEntity```, and inherit the default probabilities of Nano models. 
Alternatively, derive models directly from the underlying interfaces and abstractions implemented by ```DefaultEntity```, for greater flexibility and control over the model implementations. 
Through the data mapping implementations, models are associated with a data context. Model represents tables nd their properties columns, in the database.  

The object model, from which custom models can be derived, consists of a few abstractions and a set of interfaces.  

The top-most interface definition for a model, is ```IEntity```. Many parts of Nano requires a model to be of that type, so it should always be implemented by any model.  
Most of the other interfaces defines the kind of operation is supported by the model. The repository pattern implemented by ```IRepository```, expects models to implements the interfaces associated with the create, read, update and delete (CRUD) operations. See [Services Operations](services#operations) for further details on operations used with models.  
The interface ```IEntityIdentity<TIdentity>```, defines an entity with an identifier property - ```Id```. The ```TIdentity``` generic type parameter can be any desired type, though it's recommended to use ```System.Guid```, which is also the default when deriving models from ```DefaultEntity```. Note, that it's possible to specify the identifier when creating entities through controller create action.

## Data Mappings
The models in the application, needs to be mapped and known to the data context.  
When having both the model and the mapping, those needs to be associated. In the overridden method ```OnModelCreating(...)``` of the data context implementation, the method ```.AddMapping<MyEntity, MyEntityMapping>()``` to be invoked for each model / mappings pair in the application.  

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    builder
        .AddMapping<MyEntity, MyEntityMapping>();
}
```
Nano uses Entity Framework for mapping models to database tables.  

The [Data Context](Data#data-context) defines the class for managing the database session, and as explained associates models with their mappings, through the ```.OnModelCreating(...)``` method. The mapping implementations themselves, should derive from either ```BaseEntityMapping<TEntity>``` or ```DefaultEntityMapping<TEntity>```, depending which base class the model is deriving from.  

The base mappings implementations, maps the properties of which they are responsible, and additionally set a query filter for ```IsDeleted=0```. 

##### Table mapping
```csharp
public class MyEntityMapping : DefaultEntityMapping<MyEntity>
{
    public override void Map(EntityTypeBuilder<MyEntity> builder)
    {
        base.Map(builder);

        // Entity Framework mapping.
    }
}
```
##### View mapping
```csharp
public class MyEntityMapping : BaseEntityViewMapping<MyEntity>
{
    public override void Map(EntityTypeBuilder<MyEntity> builder)
    {
        base.Map(builder);

        // Entity Framework mapping.
    }
}
```

## Migration
Creating migration scripts at design-time, which in-turn are applied when running the application, can be accomplished simply by including an implementation of ```BaseDbContextFactory<TProvider, TContext>```. It's similar to the registration done in startup, and no additional implementation is needed.

Entity framework's support for database migration is made easy with Nano.  
Open NuGet PM console from `VS -> Tools -> NuGet Package Manager -> Package Manager Console`, and run the following command (replace parameters).  

```
PM> Add-Migration -name {name} -StartUpProject {project}
```  

## Repositories
Repositories represents data repositories within an application.  
The ```IRepository``` interface contains methods for getting, querying, adding, updating and deleting models in the application. Inject this, when implementations requires access to the application data.  
The ```BaseRepository<TContext>``` implements the interface, and ensures data is stored and retrieved through the data context defined by the generic class parameter and injected into the constructor when resolved at runtime.  
The ```DefaultRepository``` derives from ```BaseRepository<DefaultDbContext>```. and the ```DefaultDbContext``` contains overridden methods for saving changes, featuring eventing by annotation and custom extensions for entity framework.  
The services follow the ```UnitOfWork``` pattern.  

The ```BaseRepository<TContext>``` implementation contains various methods for getting, querying, updating, adding and deleting entities in the data context defined by the generic type parameter ```TContext```. This dependency is registered when configuring the data provider.  
The methods definitions of ```IRepository```, has different generic type constraints, depending on the operation. For instance ```AddAsync<TEntity>```, is constrained to models implementing ```IEntityCreatable```.  
The ```IRepository``` also contains a property, to access the underlying ```DbSet``` of an entity, 
* ```DbSet<TEntity> GetEntitySet<TEntity>()```   
Use it for advanced operations not directly supported by the repository implementation.  

When adding only the raw entity is returned from IRepository, and not included columns. Use AddAndGet to refersh all the includes.

_Name_ is the name of the migration, _project_ is the project where the ```DbContext``` implementation is located and where the migration script will be saved to. Last, the _environment_ is the configuration to use, and it's important the connection-string in the settings file of the environment is valid, otherwise an error occurs and the migration fails.

## Audit
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

## Identity
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

### Identity Configuration
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

### Model
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

### Mapping
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

## Soft Delete
In order for soft deletion to be enabled, it much be enabled in the data section of the configuration.  
When implementing the interface ```IEntityDeletableSoft```, or when deriving a model implementation from the ```DefaultEntity```, the entity will be soft deleted when removed from the data context. When soft deleted, the data doesn't get removed, but the row gets flagged as deleted, and filtered out in future queries.  

##### Unique Indexes
When dealing with soft deleted entities, together with unique indexes, a conflict can arise having one or more deleted entities with duplicate unique values. Nano automatically adjusts unique indexes, appending the ```IsDeleted``` property. This is with the exception of the property defined as primary key.

##### Cascade Delete
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

## Entity Eventing
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

## Annotations

### Entity Eventing Annotations
Publish and subscribe
Readme [Entity Eventing](#entity-eventng)

### Include Annotations
Annotating a property with the ```IncludeAttribte```, instructs the repository layer to fetch additional data when getting and querying the entity. It works similar to the ```IQueryable.Include(...)``` extension, but allows for design-time definition. The property must be a class and have a navigation relations, otherwise the annotation is ignored.   
The maximum query (join) depth for a model having properties decorated with ```IncludeAttribte```, can be set in the data options of the configuration, ```QueryIncludeDepth```.  
When having navigations inside owned models decorated with include annotation, then the owned model property on the parent must also be annotated with Include. This will be ignored by entity framework, but allows Nano to trigger the nested include.  
Note, that when creating or updating entities the include is not interpreted.  

### UX Exception Annotations
Annotating a model with the ```UxExceptionAttribte```, instructs the exception handling middleware to return a custom translated error response, when a database unique index exception occurs, that matching the defined properties.  
The attribute constructor takes two parameters. First, the custom error message to use when the Unique index exception is thrown. Second, an array of ordered properties, that aggregated should match the columns in the unique index.  
It can make it easier to catch duplicate database entry exceptions.  
```csharp
[UxException("Duplicate name", nameof(Name)]
public class MyEntity : DefaultEntity
{
    public virtual string Name { get; set; }
}
```

## Examples
See examples of Nano applications with data registered here:
* [Nano.Templates.Web.Data](https://github.com/Nano-Core/Nano.Templates/tree/master/Web.Data)
* [Nano.Templates.Console.Data](https://github.com/Nano-Core/Nano.Templates/tree/master/Console.Data)


## Model Annotations
Nano provides a set of useful validation annotations that can be applied to Nano entity models.
These annotations simplify common validation tasks and ensure consistency across your models.

Nano comes with some useful validation annotations they may be used together with Nano entity models.  

| Annotation                     | Description                                                                                                                                                |
| ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `InternationalPhoneAttribute`  | Validates that a string contains a valid international phone number. Works with properties and parameters.                                                 |
| `RequiredOneOfAttribute`       | Validates that at least one of the specified members, including the decorated member, has a non-null value. Works with properties, fields, and parameters. |
| `UrlAttribute`                 | Validates that a string contains a valid URL. Works with properties, fields, and parameters.                                                               |
