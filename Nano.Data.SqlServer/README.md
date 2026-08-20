# Nano.Data.SqlServer
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.SqlServer.svg)](https://www.nuget.org/packages/Nano.Data.SqlServer/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.SqlServer.svg)](https://www.nuget.org/packages/Nano.Data.SqlServer/)

> Sql Server data provider for Nano applications._

*** 

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/blob/master/README.md#nanolibrary)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
* **[Docker-compose](#docker-compose)**
* **[Kubernetes](#kubernetes)**
* **[GitHub Actions](#github-actions)**

## Summary
Data Provider implementation for Sql Server data access.  

> 📖 Learn more about **[Nano Data](https://github.com/Nano-Core/Nano.Library/blob/master/Nano.Data/README.md#nanodata)**.
> 📖 Learn more about **[Nano Azure Sql Server](https://github.com/Nano-Core/Nano.Azure/blob/master/Nano.Azure.SqlServer/README.md#nanoazuresqlserver)**.  

Try it out yourself using the **[Api.Data.SqlServer](https://github.com/Nano-Core/Nano.Lessons/blob/master/Api.Data.SqlServer)**, or 
**[Console.Data.SqlServer](https://github.com/Nano-Core/Nano.Lessons/blob/master/Console.Data.SqlServer)** example.  

## Registration
Install the **[Nano.Data.SqlServer](https://www.nuget.org/packages/Nano.Data.SqlServer)** NuGet package.  

```powershell
dotnet add package Nano.Data.SqlServer;
```

Register the `SqlServerProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
    .ConfigureServices(services =>
    {
        services
            .AddNanoData<SqlServerProvider, TContext>();
    })
```

A `BaseDbContext` and `BaseDbContextFactory` must also be implemented and used as `TContext`, and also an initial migration added.

```powershell
dotnet ef migrations add Initial --project {project-name}
```

## Configuration
Add the data configuration to `appsettings.json`.  

```json
"Data": {
  "BatchSize": 25,
  "BulkBatchSize": 500,
  "BulkBatchDelay": 1000,
  "QueryRetryCount": 0,
  "UseLazyLoading": false,
  "StartupAction": "None",
  "UseSensitiveDataLogging": false,
  "QuerySplittingBehavior": "SingleQuery",
  "DefaultCollation": null,
  "ConnectionString": null,
  "Repository": {
    "UseAutoSave": false,
    "QueryIncludeDepth": 4
  },
  "Identity": null,
  "ConnectionPool": null,
  "HealthCheck": {
    "UnhealthyStatus": "Unhealthy"
  }
}
```

...and `appsettings.Development.json`

```json
"Data": {
  "UseMigrateDatabase": true,
  "ConnectionString": "Server=host.docker.internal,1433;Database=nanoDb;User Id=sa;Password=myPassword_123;Encrypt=False;"
}
```

## Docker Compose
Add Sql Server as a service dependency in `docker-compose.yml`.  

```yaml
services:
  {service-name}:
    depends_on:
      - database

  database:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports:
      - 1433:1433
    networks:
      - network
    environment:
      SA_PASSWORD: myPassword_123
      ACCEPT_EULA: Y
      MSSQL_PID: Developer
```

## Kubernetes
Add the `auth-sql-secret.yaml` for the connectionstring to the `deployment.yaml`.  

```json
spec:
  template:
    spec:
      containers:
        env:
        - name: Data__ConnectionString
          valueFrom:
            secretKeyRef:
              name: %SERVICE_NAME%-sql-auth-secret
              key: data-connectionstring
```

## GitHub Actions
Add the following environment variables to the `buid-and-deply.yml`.  

```yaml
env:
  DOTNET_EF_TOOLS_VERSION: "10.0"
  AZURE_GROUP_DATABASE : ${{ vars.AZURE_RESOURCE_GROUP_DATABASE }}
  SQL_NAME: nanoDb
  SQL_USER: api-data-sqlserver-user
  SQL_PASSWORD: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_SQL_NANO_DB_PASSWORD || secrets.STAGING_SQL_NANO_DB_PASSWORD }}
  SQL_ADMIN_PASSWORD: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_SQL_ADMIN_PASSWORD || secrets.STAGING_SQL_ADMIN_PASSWORD }}
```

Additionally, two steps have been added: one to create the application's database if it doesn't already exist, and one to ensure database migrations are applied and the application 
database user has been created before the application is deployed.  

```yaml
- name: Create Database
  shell: pwsh
  run: |
    $env:SQL_SERVER_NAME = az sql server list -g $env:AZURE_GROUP_DATABASE --query "[0].name" -o tsv;
    $env:SQL_DB_EXISTS = az sql db show -g $env:AZURE_GROUP_DATABASE -s $env:SQL_SERVER_NAME -n $env:SQL_NAME --query name -o tsv 2>$null;

    if (-not $env:SQL_DB_EXISTS)
    {
        az sql db create `
            -n $env:SQL_NAME `
            -s $env:SQL_SERVER_NAME `
            -g $env:AZURE_GROUP_DATABASE `
            --edition $env:SQL_EDITION `
            --service-objective $env:SQL_SERVICE_OBJECTIVE `
            --max-size $env:SQL_MAX_SIZE `
            --backup-storage-redundancy Geo `
            --zone-redundant true;

        $env:MAINTENANCE_CONFIG_ID = az maintenance public-configuration list --query "[?name=='SQL_Default_1'].id" -o tsv;

        az sql db update `
            -n $env:SQL_NAME `
            -s $env:SQL_SERVER_NAME `
            -g $env:AZURE_GROUP_DATABASE `
            --maint-config-id $env:MAINTENANCE_CONFIG_ID;

        $env:DIAGNOSTIC_SETTINGS_NAME = "diagnostics-" + $env:SQL_NAME;
        $env:WORKSPACE_ID = az monitor log-analytics workspace list -g $env:AZURE_GROUP_LOGS --query [0].[id] -o tsv;
        $env:SQLDB_ID = az sql db show -g $env:AZURE_GROUP_DATABASE -s $env:SQL_SERVER_NAME -n $env:SQL_NAME --query id -o tsv;

        az monitor diagnostic-settings create `
            --name $env:DIAGNOSTIC_SETTINGS_NAME `
            --resource $env:SQLDB_ID `
            --workspace $env:WORKSPACE_ID `
            --logs '@.azure/.diagnostic-settings/logs.json' `
            --metrics '@.azure/.diagnostic-settings/metrics.json';

        $env:ACTION_GROUP = az monitor action-group list -g $env:AZURE_GROUP_LOGS --query [0].[id] -o tsv;

        az monitor metrics alert create `
            --name "High CPU Usage" `
            --resource-group $env:AZURE_GROUP_DATABASE `
            --scopes $env:SQLDB_ID `
            --condition "avg cpu_percent > 80" `
            --window-size PT5M `
            --evaluation-frequency PT1M `
            --action $env:ACTION_GROUP `
            --severity 2 `
            --description "Alert when CPU usage is above 80% for 5 minutes.";

        az monitor metrics alert create `
            --name "High Memory/Worker Usage" `
            --resource-group $env:AZURE_GROUP_DATABASE `
            --scopes $env:SQLDB_ID `
            --condition "avg workers_percent > 80" `
            --window-size PT5M `
            --evaluation-frequency PT1M `
            --action $env:ACTION_GROUP `
            --severity 2 `
            --description "Alert when worker/session usage is above 80% for 5 minutes.";

        az monitor metrics alert create `
            --name "High Number Of Connections" `
            --resource-group $env:AZURE_GROUP_DATABASE `
            --scopes $env:SQLDB_ID `
            --condition "total connection_successful > 100" `
            --window-size PT5M `
            --evaluation-frequency PT1M `
            --action $env:ACTION_GROUP `
            --severity 2 `
            --description "Alert when the number of successful connections exceeds 100 in 5 minutes.";

        az monitor metrics alert create `
            --name "High Storage IO" `
            --resource-group $env:AZURE_GROUP_DATABASE `
            --scopes $env:SQLDB_ID `
            --condition "avg io_consumption_percent > 80" `
            --window-size PT5M `
            --evaluation-frequency PT1M `
            --action $env:ACTION_GROUP `
            --severity 2 `
            --description "Alert when Storage IO consumption is above 80% for 5 minutes.";

        az monitor metrics alert create `
            --name "High Storage Percent" `
            --resource-group $env:AZURE_GROUP_DATABASE `
            --scopes $env:SQLDB_ID `
            --condition "avg storage_percent > 80" `
            --window-size PT5M `
            --evaluation-frequency PT1M `
            --action $env:ACTION_GROUP `
            --severity 2 `
            --description "Alert when Storage usage exceeds 80% for 5 minutes.";

        az sql db str-policy set `
            -g $env:AZURE_GROUP_DATABASE `
            -s $env:SQL_SERVER_NAME `
            -n $env:SQL_NAME `
            --retention-days $env:SQL_BACKUP_RETENTION;

        if ($LastExitCode -ne 0)
        { 
            throw "error";
        };
    };
```

```yaml
- name: Database Migration & User
  shell: pwsh
  run: |
    $env:SQL_HOST = az sql server list -g $env:AZURE_GROUP_DATABASE --query "[0].fullyQualifiedDomainName" -o tsv;
    $env:SQL_PORT = "1433"
    $env:SQL_ADMIN_USER = az sql server list -g $env:AZURE_GROUP_DATABASE --query "[0].administratorLogin" -o tsv;

    $env:DATA__CONNECTIONSTRING = "Server=$env:SQL_HOST,$env:SQL_PORT;Database=$env:SQL_NAME;User Id=$env:SQL_ADMIN_USER;Password=$env:SQL_ADMIN_PASSWORD;Encrypt=True;TrustServerCertificate=True;";

    & "/opt/ef-tools/$env:DOTNET_EF_TOOLS_VERSION/dotnet-ef" database update `
    --no-build `
    --configuration Release `
    --startup-project $env:APP_NAME `
    -- `
    --environment $env:ASPNETCORE_ENVIRONMENT;

    if ($LastExitCode -ne 0)
    { 
        throw "error";
    };
          
    apt-get update
    apt-get install -y mssql-tools unixodbc-dev

    $loginExists = sqlcmd `
    -S "$env:SQL_HOST,$env:SQL_PORT" `
    -U $env:SQL_ADMIN_USER `
    -P $env:SQL_ADMIN_PASSWORD `
    -d main `
    -h -1 `
    -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.server_principals WHERE name = '$env:SQL_USER';"

    if ($loginExists -eq 0)
    {
        sqlcmd `
        -S "$env:SQL_HOST,$env:SQL_PORT" `
        -U $env:SQL_ADMIN_USER `
        -P $env:SQL_ADMIN_PASSWORD `
        -d main `
        -Q "CREATE LOGIN [$env:SQL_USER] WITH PASSWORD = '$env:SQL_PASSWORD';"
    };

    $userExists = sqlcmd `
    -S "$env:SQL_HOST,$env:SQL_PORT" `
    -U $env:SQL_ADMIN_USER `
    -P $env:SQL_ADMIN_PASSWORD `
    -d $env:SQL_NAME `
    -h -1 `
    -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.database_principals WHERE name = '$env:SQL_USER';"

    if ($userExists -eq 0)
    {
        sqlcmd `
        -S "$env:SQL_HOST,$env:SQL_PORT" `
        -U $env:SQL_ADMIN_USER `
        -P $env:SQL_ADMIN_PASSWORD `
        -d $env:SQL_NAME `
        -Q "CREATE USER [$env:SQL_USER] FOR LOGIN [$env:SQL_USER];
            ALTER ROLE db_datareader ADD MEMBER [$env:SQL_USER];
            ALTER ROLE db_datawriter ADD MEMBER [$env:SQL_USER];"
    };

    echo "SQL_HOST=$env:SQL_HOST" >> $env:GITHUB_ENV;
    echo "SQL_PORT=$env:SQL_PORT" >> $env:GITHUB_ENV;
```

Last, before applying the new Kubernetes templates, these environmental variables must be set.

```powershell
$env:SQL_CONNECTIONSTRING = "Server=$env:SQL_HOST,$env:SQL_PORT;Database=$env:SQL_NAME;User Id=$env:SQL_USER;Password=$env:SQL_PASSWORD;Encrypt=True;TrustServerCertificate=True;";
```

Finally, apply the templates.
