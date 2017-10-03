## .NET Core Nano-Services 0.1.10
Work in progress...


### Implementating
```csharp
// program.cs
WebApplication
	.GetWebHostBuilder<MyApplication>()
	.ConfigureServices(x =>
	{
		x.AddLogging<ConsoleLogging>();
		x.AddEventing<RabbitMqEventing>();
		x.AddDataContext<SqlServerDataProvider, MyDbContext>();
		
		// Add additional services ...
	})
	.Build()
	.Run();

public class MyEntity : BaseEntity { } // Entity (Model) Implementation.
public class MyDbContext : DbContext { } // Entity Framework DbContext Implementation.
public class MyController : BaseController<IService, MyEntity> { } // Controller Implementation.
public class MyApplication : DefaultApplication { } // Application Implementation.
```
 
### Configuration
```json
{
  "App": {
    "Name": "Globale",
    "Version": "1.0",
    "Description": null,
    "TermsUrl": null,
    "LicenseUrl": null
  },
  "Hosting": {
    "EnableSession": true,
    "EnableDocumentation": true,
    "EnableGzipCompression": true,
    "EnableHttpContextExtension": true,
    "EnableHttpRequestIdentifier": true,
    "EnableHttpRequestLocalization": true
  },
  "Data": {
    "BatchSize": "25",
    "UseMemoryCache": true,
    "ConnectionString": "Server=mysql;Database=myDb;Uid=myUser;Pwd=myPassword"
  },
  "Logging": {
    "LogLevel": "Information",
    "LogLevelOverrides": [
      {
        "Namespace": "System",
        "LogLevel": "Warning"
      },
      {
        "Namespace": "Microsoft",
        "LogLevel": "Warning"
      },
      {
        "Namespace": "Microsoft.EntityFrameworkCore",
        "LogLevel": "Warning"
      }
    ]
  },
  "Eventing": {
    "Host": "rabbitmq",
    "VHost": "/",
    "Port": 5672,
    "SslPort": 5671,
    "Username": "myUser",
    "Password": "myPassword"
  }
}
```
