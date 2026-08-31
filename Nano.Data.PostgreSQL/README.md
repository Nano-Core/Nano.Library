# Nano.Data.PostgreSQL
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.PostgreSQL.svg)](https://www.nuget.org/packages/Nano.Data.PostgreSQL/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.PostgreSQL.svg)](https://www.nuget.org/packages/Nano.Data.PostgreSQL/)

> PostgreSQL data provider for Nano applications._

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
Data Provider implementation for PostgreSQL data access.  

> 📖 Learn more about **[Nano Data](https://github.com/Nano-Core/Nano.Library/blob/master/Nano.Data/README.md#nanodata)**.
> 📖 Learn more about **[Nano Azure PostgreSQL](https://github.com/Nano-Core/Nano.Azure/blob/master/Nano.Azure.PostgreSql/README.md#nanoazurepostgresql)**.  

Try it out yourself using the **[Api.Data.PostgreSQL](https://github.com/Nano-Core/Nano.Lessons/blob/master/Api.Data.PostgreSQL)**, or 
**[Console.Data.PostgreSQL](https://github.com/Nano-Core/Nano.Lessons/blob/master/Console.Data.PostgreSQL)** example.  

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
  DOTNET_EF_TOOLS_VERSION: "10.0"
  AZURE_GROUP_DATABASE : ${{ vars.AZURE_RESOURCE_GROUP_DATABASE }}
  SQL_AUTH_TYPE: Azure
  SQL_NAME: nanoDb
```

Additionally, these steps ensure database migrations are applied and the application database user is created, using the application's managed identity, before the application is deployed.

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

- name: PostgreSQL Database Migration
  shell: pwsh
  run: |
    $env:SQL_HOST = az postgres flexible-server list -g $env:AZURE_GROUP_DATABASE --query [0].fullyQualifiedDomainName -o tsv;
    $env:SQL_PORT = 5432;
    $env:SQL_SERVER = az postgres flexible-server list -g $env:AZURE_GROUP_DATABASE --query [0].name -o tsv;
    $env:SQL_USER = az postgres flexible-server ad-admin list -g $env:AZURE_GROUP_DATABASE -s $env:SQL_SERVER --query "[0].principalName" -o tsv;
    $env:SQL_TOKEN = az account get-access-token --resource-type oss-rdbms --query accessToken -o tsv;

    $env:DATA__CONNECTIONSTRING = "Host=$env:SQL_HOST;Port=$env:SQL_PORT;Database=$env:SQL_NAME;Username=$env:SQL_USER;Password=$env:SQL_TOKEN;SSL Mode=Require;Trust Server Certificate=true";

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

    $env:PRINCIPAL_SQL_PATH = "app-database-principal.sql";
    $env:GRANTS_SQL_PATH = "app-database-grants.sql";

    $principalSql = @"
      DO `$`$
      BEGIN
          IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = '$env:IDENTITY_NAME') THEN
          PERFORM pgaadauth_create_principal('$env:IDENTITY_NAME', false, false);
          END IF;
      END
      `$`$;
    "@;

    $principalSql | Set-Content $env:PRINCIPAL_SQL_PATH;

    az postgres flexible-server execute `
        -n $env:SQL_SERVER `
        -u $env:SQL_USER `
        -p $env:SQL_TOKEN `
        -d postgres `
        --file-path $env:PRINCIPAL_SQL_PATH;

    if ($LastExitCode -ne 0)
    {
        throw "error";
    };

    $grantsSql = @"
      GRANT CONNECT ON DATABASE $env:SQL_NAME TO "$env:IDENTITY_NAME";
      GRANT USAGE ON SCHEMA public TO "$env:IDENTITY_NAME";
      GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO "$env:IDENTITY_NAME";
      ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO "$env:IDENTITY_NAME";
    "@;

    $grantsSql | Set-Content $env:GRANTS_SQL_PATH;

    az postgres flexible-server execute `
        -n $env:SQL_SERVER `
        -u $env:SQL_USER `
        -p $env:SQL_TOKEN `
        -d $env:SQL_NAME `
        --file-path $env:GRANTS_SQL_PATH;

    if ($LastExitCode -ne 0)
    {
        throw "error";
    };

    $env:SQL_CONNECTIONSTRING = "Host=$env:SQL_HOST;Port=$env:SQL_PORT;Database=$env:SQL_NAME;Username=$env:IDENTITY_NAME;SSL Mode=Require;Trust Server Certificate=true";
    echo "SQL_CONNECTIONSTRING=$env:SQL_CONNECTIONSTRING" >> $env:GITHUB_ENV;
```

Finally, apply the templates.
