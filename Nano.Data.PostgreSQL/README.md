# Nano.Data.PostgreSQL
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.PostgreSQL.svg)](https://www.nuget.org/packages/Nano.Data.PostgreSQL/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.PostgreSQL.svg)](https://www.nuget.org/packages/Nano.Data.PostgreSQL/)

> PostgreSQL data provider for Nano applications._

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
Data Provider implementation for PostgreSQL data access.  

> 📖 Learn more about **[Nano Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#nanodata)**.
> 📖 Learn more about **[Nano Azure PostgreSQL](https://github.com/Nano-Core/Nano.Kubernetes/tree/master/Nano.Azure.PostgreSql/README.md#nanoazurepostgresql)**.  

Try it out yourself using the **[Api.Data.PostgreSQL](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.PostgreSQL)**, or 
**[Console.Data.PostgreSQL](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Data.PostgreSQL)** example.  

## Registration
Install the **[Nano.Data.PostgreSQL](https://www.nuget.org/packages/Nano.Data.PostgreSQL)** NuGet package.  

```powershell
dotnet add package Nano.Data.PostgreSQL;
```

Register the `PostgreSqlProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
    .ConfigureServices(services =>
    {
        services
            .AddNanoData<PostgreSqlProvider, TContext>();
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
  "ConnectionString": "Host=host.docker.internal;Port=5432;Database=nanoDb;Username=sa;Password=myPassword_123"
}
```

## Docker Compose
Add PostgreSQL as a service dependency in `docker-compose.yml`.  

```yaml
services:
  {service-name}:
    depends_on:
      - database

  database:
    image: postgis/postgis:latest
    ports:
      - 5432:5432
    networks:
      - network
    environment:
      POSTGRES_USER: sa
      POSTGRES_PASSWORD: myPassword_123
      POSTGRES_DB: nanoDb
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
  SQL_USER: api-data-postgres-user
  SQL_PASSWORD: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_SQL_NANO_DB_PASSWORD || secrets.STAGING_SQL_NANO_DB_PASSWORD }}
  SQL_ADMIN_PASSWORD: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_SQL_ADMIN_PASSWORD || secrets.STAGING_SQL_ADMIN_PASSWORD }}
```

Additionally, this step has been added to ensure database migrations are applied, and the application database user has been created before the application is deployed.  

```yaml
- name: Database Migration & User
  shell: pwsh
  run: |
    $env:SQL_HOST = az postgres flexible-server list -g $env:AZURE_GROUP_DATABASE --query "[0].fullyQualifiedDomainName" -o tsv;
    $env:SQL_PORT = "5432";
    $env:SQL_ADMIN_USER = az postgres flexible-server list -g $env:AZURE_GROUP_DATABASE --query "[0].username" -o tsv;
    $env:SQL_MIGRATION_CONNECTIONSTRING = "Host=$env:SQL_HOST;Port=$env:SQL_PORT;Database=$env:SQL_NAME;Username=$env:SQL_ADMIN_USER;Password=$env:SQL_ADMIN_PASSWORD;SSL Mode=Prefer;Trust Server Certificate=true";

    dotnet ef database update `
      --no-build `
      --startup-project $env:APP_NAME `
      --connection "$env:SQL_MIGRATION_CONNECTIONSTRING" `;

    if ($LastExitCode -ne 0)
    { 
        throw "error";
    };
         
    apt-get update
    apt-get install -y postgresql-client

    $userExists = psql "$env:SQL_MIGRATION_CONNECTIONSTRING" `
        -tAc "SELECT 1 FROM pg_roles WHERE rolname='$env:SQL_USER';"

    if ($userExists -ne "1")
    {
        psql "$env:SQL_MIGRATION_CONNECTIONSTRING" `
            -c "CREATE ROLE $env:SQL_USER WITH LOGIN PASSWORD '$env:SQL_PASSWORD';"
    }

    $userDbExists = psql "$env:SQL_MIGRATION_CONNECTIONSTRING" `
        -tAc "SELECT 1 FROM pg_roles WHERE rolname='$env:SQL_USER';"

    if ($userDbExists -ne "1")
    {
        psql "$env:SQL_MIGRATION_CONNECTIONSTRING" `
            -c "GRANT CONNECT ON DATABASE $env:SQL_NAME TO $env:SQL_USER;"

        psql "$env:SQL_MIGRATION_CONNECTIONSTRING" `
            -c "GRANT USAGE ON SCHEMA public TO $env:SQL_USER;"

        psql "$env:SQL_MIGRATION_CONNECTIONSTRING" `
            -c "GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO $env:SQL_USER;"

        psql "$env:SQL_MIGRATION_CONNECTIONSTRING" `
            -c "ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO $env:SQL_USER;"
    }
```

Last, the application connectionstring must be added in a secret in Kubernetes in the `Kubernetes Deploy` step.  
