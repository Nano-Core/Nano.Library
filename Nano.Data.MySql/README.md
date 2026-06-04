# Nano.Data.MySql
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.MySql.svg)](https://www.nuget.org/packages/Nano.Data.MySql/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.MySql.svg)](https://www.nuget.org/packages/Nano.Data.MySql/)

> MySql data provider for Nano applications._

*** 

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/tree/master/README.md#nanolibrary)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
* **[Docker-compose](#docker-compose)**
* **[Kubernetes](#kubernetes)**
* **[GitHub Actions](#github-actions)**

## Summary
Data Provider implementation for MySql data access.  

> 📖 Learn more about **[Nano Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#nanodata)**.
> 📖 Learn more about **[Nano Azure MySql](https://github.com/Nano-Core/Nano.Kubernetes/tree/master/Nano.Azure.MySql/README.md#nanoazuremysql)**.  

Try it out yourself using the **[Api.Data.MySql](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.MySql)**, or 
**[Console.Data.MySql](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Data.MySql)** example.  

## Registration
Install the **[Nano.Data.MySql](https://www.nuget.org/packages/Nano.Data.MySql)** NuGet package.  

```powershell
dotnet add package Nano.Data.MySql;
```

Register the `MySqlProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
    .ConfigureServices(services =>
    {
        services
            .AddNanoData<MySqlProvider, TContext>();
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
  "ConnectionString": "Server=host.docker.internal;Database=nanoDb;Uid=sa;Pwd=myPassword_123"
}
```

## Docker Compose
Add MySql as a service dependency in `docker-compose.yml`.  

```yaml
services:
  {service-name}:
    depends_on:
      - database

  database:
    image: mysql/mysql-server:latest
    ports:
      - 3306:3306
    networks:
      - network
    environment:
      MYSQL_USER: sa
      MYSQL_PASSWORD: myPassword_123
      MYSQL_ROOT_PASSWORD: myPassword_123
      MYSQL_DATABASE: nanoDb
      MYSQL_ROOT_HOST: '%'
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
              name: %SERVICE_NAME%-sql-auth-secret
              key: data-connectionstring
```

## GitHub Actions
Add the following environment variables to the `buid-and-deply.yml`.  

```yaml
env:
  SQL_NAME: nanoDb
  SQL_USER: api-data-mysql-user
  SQL_PASSWORD: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_SQL_NANO_DB_PASSWORD || secrets.STAGING_SQL_NANO_DB_PASSWORD }}
  SQL_ADMIN_PASSWORD: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_SQL_ADMIN_PASSWORD || secrets.STAGING_SQL_ADMIN_PASSWORD }}
```

Additionally, this step must be added to ensure database migrations are applied, and the application database user has been created before the application is deployed.  

```yaml
- name: Database Migration & User
  shell: pwsh
  run: |
    $env:SQL_HOST = az mysql flexible-server list -g $env:AZURE_GROUP_DATABASE --query [0].fullyQualifiedDomainName;
    $env:SQL_PORT = az mysql flexible-server list -g $env:AZURE_GROUP_DATABASE --query [0].databasePort;
    $env:SQL_ADMIN_USER = az mysql flexible-server list -g $env:AZURE_GROUP_DATABASE --query [0].administratorLogin;
    $env:SQL_MIGRATION_CONNECTIONSTRING = "Server=$env:SQL_HOST;Port=$env:SQL_PORT;Database=$env:SQL_NAME;Uid=$env:SQL_ADMIN_USER;Pwd=$env:SQL_ADMIN_PASSWORD;SslMode=Preferred";

    dotnet ef database update `
      --no-build `
      --startup-project $env:APP_NAME `
      --connection "$env:SQL_MIGRATION_CONNECTIONSTRING ";

    if ($LastExitCode -ne 0)
    { 
        throw "error";
    };
          
    apt-get update;
    apt-get install -y mysql-client;

    $userExists = mysql --batch -e "SELECT EXISTS(SELECT 1 FROM mysql.user WHERE user = '$env:SQL_USER');" $env:SQL_MIGRATION_CONNECTIONSTRING;

    if ($userExists -eq 0) 
    {
        mysql --connect-expired-password -e " `
            CREATE USER '$env:SQL_USER'@'%' IDENTIFIED BY '$env:SQL_PASSWORD'; `
            GRANT SELECT, INSERT, UPDATE, DELETE ON $database.* TO '$env:SQL_USER'@'%'; `
            FLUSH PRIVILEGES;" $env:SQL_MIGRATION_CONNECTIONSTRING;
    }
```

Last, the application connectionstring must be added in a secret in Kubernetes in the `Kubernetes Deploy` step.  
