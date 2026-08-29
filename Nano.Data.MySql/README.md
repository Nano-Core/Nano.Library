# Nano.Data.MySql
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.MySql.svg)](https://www.nuget.org/packages/Nano.Data.MySql/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.MySql.svg)](https://www.nuget.org/packages/Nano.Data.MySql/)

> MySql data provider for Nano applications._

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
Data Provider implementation for MySql data access.  

> 📖 Learn more about **[Nano Data](https://github.com/Nano-Core/Nano.Library/blob/master/Nano.Data/README.md#nanodata)**.
> 📖 Learn more about **[Nano Azure MySql](https://github.com/Nano-Core/Nano.Azure/blob/master/Nano.Azure.MySql/README.md#nanoazuremysql)**.  

Try it out yourself using the **[Api.Data.MySql](https://github.com/Nano-Core/Nano.Lessons/blob/master/Api.Data.MySql)**, or 
**[Console.Data.MySql](https://github.com/Nano-Core/Nano.Lessons/blob/master/Console.Data.MySql)** example.  

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
      MYSQL_ROOT_HOST: '%'
      MYSQL_ROOT_PASSWORD: myPassword_123
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

- name: MySQL Database Migration
  shell: pwsh
  run: |
    $env:SQL_HOST = az mysql flexible-server list -g $env:AZURE_GROUP_DATABASE --query [0].fullyQualifiedDomainName -o tsv;
    $env:SQL_PORT = az mysql flexible-server list -g $env:AZURE_GROUP_DATABASE --query [0].databasePort -o tsv;
    $env:SQL_SERVER = az mysql flexible-server list -g $env:AZURE_GROUP_DATABASE --query [0].name -o tsv;
    $env:SQL_USER = az mysql flexible-server ad-admin list -g $env:AZURE_GROUP_DATABASE -s $env:SQL_SERVER --query "[0].login" -o tsv;
    $env:SQL_TOKEN = az account get-access-token --resource-type oss-rdbms --query accessToken -o tsv;

    $env:DATA__CONNECTIONSTRING = "Server=$env:SQL_HOST;Port=$env:SQL_PORT;Database=$env:SQL_NAME;Uid=$env:SQL_USER;Pwd=$env:SQL_TOKEN;SslMode=Required";

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

    $env:APP_USER_SQL_PATH = Join-Path $env:USERPROFILE "app-database-user.sql";

    $sql = @"
      CREATE AADUSER IF NOT EXISTS '$env:IDENTITY_NAME' IDENTIFIED BY '$env:IDENTITY_CLIENT_ID';
      GRANT SELECT, INSERT, UPDATE, DELETE ON $env:SQL_NAME.* TO '$env:IDENTITY_NAME'@'%';
      FLUSH PRIVILEGES;
    "@;

    $sql | Set-Content $env:APP_USER_SQL_PATH;

    az mysql flexible-server execute `
        -n $env:SQL_SERVER `
        -u $env:SQL_USER `
        -p $env:SQL_TOKEN `
        --file-path $env:APP_USER_SQL_PATH;

    if ($LastExitCode -ne 0)
    {
        throw "error";
    };

    $env:SQL_CONNECTIONSTRING = "Server=$env:SQL_HOST;Port=$env:SQL_PORT;Database=$env:SQL_NAME;Uid=$env:IDENTITY_NAME;SslMode=Required";
    echo "SQL_CONNECTIONSTRING=$env:SQL_CONNECTIONSTRING" >> $env:GITHUB_ENV;
```

Finally, apply the templates.
