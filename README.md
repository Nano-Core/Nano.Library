## .NET Core Nano-Services
Seamlessly launch services without...

### Implementating
```csharp
// program.cs
WebApplication
	.GetWebHostBuilder<MyApplication>()
	.ConfigureServices(x =>
	{
		// Add additional services ...
	})
	.AddLogging<MyLogging>();
	.AddEventing<MyEventing>();
	.AddDataContext<MyDataContext>();
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
  "Hosting": {
    "EnableSession": true,
    "EnableGzipCompression": true,
    "EnableRequestLocalization": true
  },
  "Data": {
    "Provider": "MySql",
    "BatchSize": "25",
    "IsolationLevel": "READ UNCOMMITTED",
    "UseMemoryCache": true,
    "ConnectionString": "Server=mysql;Database=myDb;Uid=myUser;Pwd=myPassword"
  },
  "Logging": {
    "IncludeScopes": true,
    "IncludeLogContext": true,
    "IncludeHttpRequestIdentifier": true,
    "LogLevel": "Information",
    "LogLevelOverrides": [
      {
        "Namespace": "System",
        "LogLevel": "Warning"
      }
    ]
  },
  "Eventing": {
    "Provider": "RabbitMQ",
    "Host": "rabbitmq",
    "VHost": "/",
    "Port": 5672,
    "SslPort": 5671,
    "Username": "myUser",
    "Password": "myPassword"
  }
}
```
