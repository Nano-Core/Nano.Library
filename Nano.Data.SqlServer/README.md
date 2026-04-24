# Nano.Data.SqlServer
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.SqlServer.svg)](https://www.nuget.org/packages/Nano.Data.SqlServer/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.SqlServer.svg)](https://www.nuget.org/packages/Nano.Data.SqlServer/)

> Sql Server data provider for Nano applications._

*** 

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library#nano-library)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
* **[Docker-compose](#docker-compose)**
* **[Kubernetes](#kubernetes)**
* **[GitHub Actions](#github-actions)**

## Summary
Data Provider implementation for Sql Server data access.  

> 📖 Learn more about **[Nano Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)**.
> 📖 Learn more about **[Nano Azure Sql Server](https://github.com/Nano-Core/Nano.Kubernetes/tree/master/Nano.Azure.Sql/SqlServer)**.  

Try it out yourself using the **[Api.Data.SqlServer](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.SqlServer)**, or 
**[Console.Data.SqlServer](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Data.SqlServer)** example.  

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
Add the `%SERVICE_NAME%-secret` for the connectionstring to the `deployment.yaml`.  

```json
spec:
  template:
    spec:
      containers:
        env:
        - name: Data__ConnectionString
          valueFrom:
            secretKeyRef:
              name: %SERVICE_NAME%-data-secret
              key: data-connectionstring
```

## GitHub Actions
Add the following environment variables to the `buid-and-deply.yml`.  

```yaml
env:
  DATA_HOST: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_SQLSERVER_HOST || secrets.STAGING_SQLSERVER_HOST }}
  DATA_NAME: nanoDb
  DATA_USER: api-data-sqlserver-user
  DATA_PASSWORD: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_SQLSERVER_NANO_DB_PASSWORD || secrets.STAGING_SQLSERVER_NANO_DB_PASSWORD }}
  DATA_ADMIN_USER: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_SQLSERVER_ADMIN_USER || secrets.STAGING_SQLSERVER_ADMIN_USER }}
  DATA_ADMIN_PASSWORD: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_SQLSERVER_ADMIN_PASSWORD || secrets.STAGING_SQLSERVER_ADMIN_PASSWORD }}
  DATA_CONNECTIONSTRING: Server=${{ env.DATA_HOST }},${{ vars.DATA_SQLSERVER_PORT }};Database=${{ env.DATA_NAME }};User Id=${{ env.DATA_USER }};Password=${{ env.DATA_PASSWORD }};
  DATA_MIGRATION_CONNECTIONSTRING: Server=${{ env.DATA_HOST }},${{ vars.DATA_SQLSERVER_PORT }};Database=${{ env.DATA_NAME }};User Id=${{ env.DATA_ADMIN_USER }};Password=${{ env.DATA_ADMIN_PASSWORD }};
```

Additionally, this step has been added to ensure database migrations are applied, and the application database user has been created before the application is deployed.  

```yaml
- name: Database Migration
  shell: pwsh
  run: |
    dotnet ef database update `
      --no-build `
      --startup-project $env:APP_NAME `
      --connection "$env:DATA_MIGRATION_CONNECTIONSTRING" `;

    if ($LastExitCode -ne 0)
    { 
        throw "error";
    };
         
    sudo apt-get update
    sudo apt-get install -y mssql-tools unixodbc-dev

    $loginExists = sqlcmd `
        -S "$env:DATA_HOST,$env:DATA_SQLSERVER_PORT" `
        -U $env:DATA_ADMIN_USER `
        -P $env:DATA_ADMIN_PASSWORD `
        -d master `
        -h -1 `
        -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.server_principals WHERE name = '$env:DATA_USER';"

    if ($loginExists -eq 0)
    {
        sqlcmd `
            -S "$env:DATA_HOST,$env:DATA_SQLSERVER_PORT" `
            -U $env:DATA_ADMIN_USER `
            -P $env:DATA_ADMIN_PASSWORD `
            -d master `
            -Q "CREATE LOGIN [$env:DATA_USER] WITH PASSWORD = '$env:DATA_PASSWORD';"
    }

    $userExists = sqlcmd `
        -S "$env:DATA_HOST,$env:DATA_SQLSERVER_PORT" `
        -U $env:DATA_ADMIN_USER `
        -P $env:DATA_ADMIN_PASSWORD `
        -d $env:DATA_NAME `
        -h -1 `
        -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.database_principals WHERE name = '$env:DATA_USER';"

   if ($userExists -eq 0)
   {
       sqlcmd `
           -S "$env:DATA_HOST,$env:DATA_SQLSERVER_PORT" `
           -U $env:DATA_ADMIN_USER `
           -P $env:DATA_ADMIN_PASSWORD `
           -d $env:DATA_NAME `
           -Q "CREATE USER [$env:DATA_USER] FOR LOGIN [$env:DATA_USER];
               ALTER ROLE db_datareader ADD MEMBER [$env:DATA_USER];
               ALTER ROLE db_datawriter ADD MEMBER [$env:DATA_USER];"
   }
```

Last, the application connectionstring must be added in a secret in Kubernetes in the `Kubernetes Deploy` step.  

```yaml
sudo kubectl create secret generic $env:SERVICE_NAME-data-secret ` --from-literal=data-connectionstring=$env:DATA_CONNECTIONSTRING --save-config --dry-run=client -o yaml | sudo kubectl apply -f -;
if ($LastExitCode -ne 0)
{ 
    throw "error";
};
```
