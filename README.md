## .NET Core Nano-Services 0.1.60-pre-alpha
Work in progress...


### Implementating
```csharp
// program.cs
WebApplication
	.ConfgureApp<MyApplication>()
	.ConfigureServices(x =>
	{
		x.AddDataContext<SqlServerDataProvider, MyDbContext>();
		x.AddEventing<RabbitMqEventing>();
		
		// Add additional services ...
	})
	.Build()
	.Run();

public class MyEntity : BaseEntity { } // Model Implementation.
public class MyCriteria : Criteria { } // Query Criteria Implementation.
public class MyDbContext : BaseDbContext { } // IDbContext Implementation.
public class MyController : DefaultController<IService, MyEntity> { } // Controller Implementation.
public class MyApplication : DefaultApplication { } // IApplication Implementation.
```
 
### Configuration
```json
{
   "App": {
    "Name": "Examples",
    "Version": "1.0.0",
    "Description": null,
    "TermsUrl": null,
    "LicenseUrl": null,
    "Cultures": {
      "Default": "en-US",
      "Supported": [
        "da-DK",
        "en-US"
      ]
    }
  },
  "Hosting": {
    "Path": "app",
    "EnableSession": true,
    "EnableDocumentation": true,
    "EnableGzipCompression": true,
    "EnableHttpContextLogging": true,
    "EnableHttpContextIdentifier": true,
    "EnableHttpContextLocalization": true
  },
  "Data": {
    "BatchSize": "25",
    "UseMemoryCache": true,
    "ConnectionString": "Server=mysql;Database=myDb;Uid=myUser;Pwd=myPassword"
  },
  "Logging": {
    "LogLevel": "Debug",
    "Sinks": [
      "Console"
    ],
    "LogLevelOverrides": 
    [
        {
          "Namespace": "System",
          "LogLevel": "Debug"
        }
      ]
  },
  "Eventing": {
    "Host": "rabbitmq",
    "VHost": "/",
    "Port": 5672,
    "AuthenticationCredential": {
      "Username": "myUser",
      "Password": "myPassword",
      "Token": null
    },
    "UseSsl": false,
    "Timeout": 30,
    "Heartbeat": 10
  }
}
```
