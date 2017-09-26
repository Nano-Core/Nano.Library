## .NET Core Nano-Services
Seamlessly launch services without...

### Implementating
```csharp
// program.cs
WebApplication
	.GetWebHostBuilder<MyApplication>()
	.ConfigureProviders(x =>
	{
		x.AddLoggingConsole();
        x.AddDataContext<MyDbContext>();
		// ...
	})
	.ConfigureServices(x =>
	{
		x.AddTransient<IMyService, MyService>();
		// ...
	})
	.Build()
	.Run();

public class MyEntity : BaseEntity { } // Entity (Model) Implementation.
public class MyDbContext : DbContext { } // Entity Framework DbContext Implementation.
public interface IAddressService : IService { } // Service Interface.
public class MyController : BaseController<IService, MyEntity> { } // Controller Implementation.
```
 
### Nano-Service Configuration
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
    "LogLevel": "Information",
    "LogLevelOverrides": [
      {
        "Namespace": "System",
        "LogLevel": "Warning"
      },
      {
        "Namespace": "Microsoft",
        "LogLevel": "Information"
      },
      {
        "Namespace": "Microsoft.EntityFrameworkCore",
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
  },
  "Security": {
    "Password": {
      "RequireDigit": true,
      "RequiredLength": "8",
      "RequireNonAlphanumeric": true,
      "RequireUppercase": true,
      "RequireLowercase": true,
      "RequiredUniqueChars": "0"
    },
    "Lockout": {
      "AllowedForNewUsers": true,
      "MaxFailedAccessAttempts": 10,
      "DefaultLockoutTimeSpanInMinutes": 30
    }
  }
}
```
