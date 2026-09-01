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
  "AuthenticationType": "Credentials",
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

Also add the following variable to the `configmap.yaml`.

```yaml
data:
  Data__AuthenticationType: %SQL_AUTH_TYPE%
```

Last, the secret `auth-sql-secret.yaml` for the connectionstriong must be applied as well.

## GitHub Actions
Add the following environment variables to the `buid-and-deply.yml`.  

```yaml
env:
  AZURE_GROUP_LOGS : ${{ vars.AZURE_RESOURCE_GROUP_LOGS }}
  DOTNET_EF_TOOLS_VERSION: "10.0"
  AZURE_GROUP_DATABASE : ${{ vars.AZURE_RESOURCE_GROUP_DATABASE }}
  SQL_AUTH_TYPE: Azure
  SQL_NAME: nanoDb
  SQL_SERVICE_OBJECTIVE: GP_Gen5_2
  SQL_EDITION: GeneralPurpose
  SQL_MAX_SIZE: 64GB
  SQL_BACKUP_RETENTION: 35
```

Additionally, these steps ensure the database exists, migrations are applied, and the application database user is created (using the application's managed identity) before the application is deployed.

```yaml
- name: Managed Identity
  shell: pwsh
  run: |
    $env:IDENTITY_NAME = $env:SERVICE_NAME + "-identity";
    $env:IDENTITY_PRINCIPAL_ID = az identity show -g $env:AZURE_GROUP_KUBERNETES -n $env:IDENTITY_NAME --query principalId -o tsv;
    $env:KUBERNETES_ISSUER_URL = az aks list -g $env:AZURE_GROUP_KUBERNETES --query [0].['oidcIssuerProfile.issuerUrl'] -o tsv;

    if (-not $env:IDENTITY_PRINCIPAL_ID)
    {
        az identity create `
            -g $env:AZURE_GROUP_KUBERNETES `
            -n $env:IDENTITY_NAME;

        if ($LastExitCode -ne 0)
        {
            throw "error";
        };

        $env:IDENTITY_PRINCIPAL_ID = az identity show -g $env:AZURE_GROUP_KUBERNETES -n $env:IDENTITY_NAME --query principalId -o tsv;
    }
          
    $env:IDENTITY_CLIENT_ID = az identity show -g $env:AZURE_GROUP_KUBERNETES -n $env:IDENTITY_NAME --query clientId -o tsv;

    az identity federated-credential create `
        --name $env:SERVICE_NAME-credentials `
        --resource-group $env:AZURE_GROUP_KUBERNETES `
        --identity-name $env:IDENTITY_NAME `
        --issuer $env:KUBERNETES_ISSUER_URL `
        --subject "system:serviceaccount:${env:KUBERNETES_NAMESPACE}:${env:SERVICE_NAME}-service-account" `
        --audience api://AzureADTokenExchange;

    if ($LastExitCode -ne 0)
    {
        throw "error";
    };
          
    echo "IDENTITY_NAME=$env:IDENTITY_NAME" >> $env:GITHUB_ENV;
    echo "IDENTITY_CLIENT_ID=$env:IDENTITY_CLIENT_ID" >> $env:GITHUB_ENV; 
    echo "IDENTITY_PRINCIPAL_ID=$env:IDENTITY_PRINCIPAL_ID" >> $env:GITHUB_ENV; 

- name: SQL Server Create Database
  shell: pwsh
  run: |
    $env:SQL_SERVICE_OBJECTIVE = "GP_Gen5_2";
    $env:SQL_EDITION = "GeneralPurpose";
    $env:SQL_MAX_SIZE = "64GB";
    $env:SQL_BACKUP_RETENTION = "35"
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
                  
        $env:MAINTENANCE_CONFIG_ID = "/subscriptions/$env:AZURE_SUBSCRIPTION_ID/providers/Microsoft.Maintenance/publicMaintenanceConfigurations/SQL_Default";

        az sql db update `
            -n $env:SQL_NAME `
            -s $env:SQL_SERVER_NAME `
            -g $env:AZURE_GROUP_DATABASE `
            --maint-config-id $env:MAINTENANCE_CONFIG_ID;

        $env:DIAGNOSTIC_SETTINGS_NAME = "diagnostics-" + $env:SQL_NAME;
        $env:SQL_LOGS_PATH = "sql-diagnostic-logs.json";
        $env:SQL_METRICS_PATH = "sql-diagnostic-metrics.json";
        $env:WORKSPACE_ID = az monitor log-analytics workspace list -g $env:AZURE_GROUP_LOGS --query [0].[id] -o tsv;
        $env:SQLDB_ID = az sql db show -g $env:AZURE_GROUP_DATABASE -s $env:SQL_SERVER_NAME -n $env:SQL_NAME --query id -o tsv;

        $logsJson = @"
        [
          { "category": "QueryStoreRuntimeStatistics", "enabled": true },
          { "category": "SQLSecurityAuditEvents", "enabled": true }
        ]
    "@;

        $logsJson | Set-Content $env:SQL_LOGS_PATH;

        $metricsJson = @"
        [
          { "category": "Basic", "enabled": true },
          { "category": "InstanceAndAppAdvanced", "enabled": true },
          { "category": "WorkloadManagement", "enabled": true }
        ]
    "@;

        $metricsJson | Set-Content $env:SQL_METRICS_PATH;

        az monitor diagnostic-settings create `
            --name $env:DIAGNOSTIC_SETTINGS_NAME `
            --resource $env:SQLDB_ID `
            --workspace $env:WORKSPACE_ID `
            --logs "@$env:SQL_LOGS_PATH" `
            --metrics "@$env:SQL_METRICS_PATH";

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
            --name "High Memory And Worker Usage" `
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
            --condition "avg physical_data_read_percent > 80" `
            --window-size PT5M `
            --evaluation-frequency PT1M `
            --action $env:ACTION_GROUP `
            --severity 2 `
            --description "Alert when data IO usage is above 80% for 5 minutes.";

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

- name: SQL Server Database Migration
  shell: pwsh
  run: |
    $env:SQL_HOST = az sql server list -g $env:AZURE_GROUP_DATABASE --query [0].fullyQualifiedDomainName -o tsv;
    $env:SQL_PORT = 1433;
    $env:SQL_SERVER = az sql server list -g $env:AZURE_GROUP_DATABASE --query [0].name -o tsv;

    $env:DATA__CONNECTIONSTRING = "Server=$env:SQL_HOST,$env:SQL_PORT;Database=$env:SQL_NAME;Authentication=Active Directory Service Principal;User Id=$env:AZURE_CLIENT_ID;Password=$env:AZURE_CLIENT_SECRET;Encrypt=True;TrustServerCertificate=True;";

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

    $env:APP_USER_SQL_PATH = "app-database-user.sql";

    $sql = @"
      IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = '$env:IDENTITY_NAME')
      BEGIN
          CREATE USER [$env:IDENTITY_NAME] FROM EXTERNAL PROVIDER;
      END

      IF NOT EXISTS (SELECT 1 FROM sys.database_role_members drm
          JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
          JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
          WHERE r.name = 'db_datareader' AND m.name = '$env:IDENTITY_NAME')
      BEGIN
          ALTER ROLE db_datareader ADD MEMBER [$env:IDENTITY_NAME];
      END

      IF NOT EXISTS (SELECT 1 FROM sys.database_role_members drm
          JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
          JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
          WHERE r.name = 'db_datawriter' AND m.name = '$env:IDENTITY_NAME')
      BEGIN
          ALTER ROLE db_datawriter ADD MEMBER [$env:IDENTITY_NAME];
      END
    "@;

    $sql | Set-Content $env:APP_USER_SQL_PATH;

    $env:SQL_TOKEN = az account get-access-token --resource "https://database.windows.net/" --query accessToken -o tsv;

    Invoke-Sqlcmd `
        -ServerInstance $env:SQL_HOST `
        -Database $env:SQL_NAME `
        -AccessToken $env:SQL_TOKEN `
        -InputFile $env:APP_USER_SQL_PATH;

    if ($LastExitCode -ne 0)
    {
        throw "error";
    };

    $env:SQL_CONNECTIONSTRING = "Server=$env:SQL_HOST,$env:SQL_PORT;Database=$env:SQL_NAME;User Id=$env:IDENTITY_CLIENT_ID;Encrypt=True;TrustServerCertificate=True;";
    echo "SQL_CONNECTIONSTRING=$env:SQL_CONNECTIONSTRING" >> $env:GITHUB_ENV;
```

Finally, apply the templates.
