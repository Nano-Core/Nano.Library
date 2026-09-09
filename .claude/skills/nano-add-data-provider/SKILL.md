---
name: nano-add-data-provider
description: Add a Nano data provider (MySql, PostgreSQL, SqlServer, SqLite, or InMemory) to a Nano.Library-based application - registers it in Program.cs, adds the DbContext/DbContextFactory, the Data configuration section, the local docker-compose database service, and (for MySql/PostgreSQL/SqlServer) the Staging/Production migration CI step and Kubernetes secret. Use when the user asks to add a database, persistence, or a specific data provider to a Nano API, Web, or Console application.
---

# Nano add data provider

Wires a Nano data provider into an existing Nano API, Web, or Console application. Read
`AGENTS.md` in the target repo root first — its `## Nano.Data` section documents the
`Configuration` table, the provider/package table, the exact `DbContext`/`BaseDbContextFactory`
shapes, and `Migrations`/`StartupAction` semantics in full; this skill does not repeat any of
that, only how to apply it and wire the surrounding infrastructure (docker-compose, CI, K8s)
without breaking what's already there.

## Before making any change, determine

1. **Which provider.** One of `MySql`, `PostgreSQL`, `SqlServer`, `SqLite`, `InMemory` (see
   AGENTS.md's provider table for package/type names). Ask the user if not already given.
2. **Is a data provider already registered?** Check `Program.cs` for an existing
   `.AddNanoData<...>()` call. Unlike logging, a second data provider isn't automatically
   wrong (multi-context setups exist), but it's unusual — if one is already registered, confirm
   with the user whether they want to *replace* it (single-context swap) or genuinely add a
   second `DbContext` before proceeding either way.
3. **Is a package reference even needed?** Same check as the logging skill: look for a
   `PackageReference` to `NanoCore` or `Nano.All` (identical, see AGENTS.md) on the application
   project or a `.Models` project it reaches via `ProjectReference`. If found, skip the package
   step. Otherwise add `<PackageReference Include="Nano.Data.<Provider>" Version="X.Y.Z" />` to
   the **application project** (never `.Models`), matching the version of the project's existing
   Nano application-type package. Never add a `ProjectReference` to Nano.Library source.
4. **Entity identity type.** If entities already exist in the project, match their `TIdentity`
   (see the entity-scaffold skill's identity-type step) — the `DbContext`/`AddNanoData<...>`
   generic arguments must agree with it.

## Program.cs

Add the registration inside the existing `.ConfigureServices(...)` lambda (same placeholder/`_`
→ `x` rename rule as the logging skill if the lambda is still the blank-app boilerplate):

```csharp
using Nano.Data.Extensions;
using Nano.Data.<Provider>;
using <Namespace>.Data;
```

```csharp
x.AddNanoData<<Provider>Provider, <Name>DbContext>();
```

## Data Context and design-time factory

Create both files in `Data/` in the application project (per AGENTS.md's `### Data Context`
and `#### Design-time factory (migrations)` — copy those shapes exactly, they're not
provider-specific except for the generic arguments):

- `Data/<Name>DbContext.cs` — thin subclass of `BaseDbContext`/`BaseDbContext<TIdentity>` with
  the exact `(DbContextOptions, IOptionsMonitor<DataOptions>)` constructor AGENTS.md shows. Skip
  this file for `InMemory` only if the project has no other provider-specific needs — check
  AGENTS.md's provider table notes first (`InMemory` still needs a `DbContext`, just no
  `BaseDbContextFactory` or migrations).
- `Data/<Name>DbContextFactory.cs` — subclass of `BaseDbContextFactory<TProvider, TContext>`.
  Skip for `InMemory` (no migrations to design-time-construct against).

## appsettings.json

Add the `Data` section from AGENTS.md's `### Configuration` example to the base
`appsettings.json` (sibling of `App`). A few placements are load-bearing, not arbitrary:

- **`ConnectionString`**: leave `null` in the base file; set the real value only in
  `appsettings.Development.json` (see below) — never commit a real connection string to the
  base file.
- **`AuthenticationType`**: always `"Credentials"` in the base `appsettings.json`, even for
  providers that will use Managed Identity in Staging/Production — `Credentials` is the correct
  *local* value, and it's what the base file should show either way. Don't set `"Azure"` here;
  live environments override it via the Kubernetes ConfigMap (see the Staging/Production
  section below), never via a static appsettings file.
- **`StartupAction`**: `"None"` in the base file. Only `appsettings.Development.json` sets it to
  `"Migrate"` (AGENTS.md: only enable `Create`/`Migrate` in `Development`).

In `appsettings.Development.json`, add:

```json
"Data": {
  "StartupAction": "Migrate",
  "ConnectionString": "<local connection string for the chosen provider>"
}
```

Use `host.docker.internal` as the host in the local connection string, not the docker-compose
service name — `BaseDbContextFactory` specifically rewrites `host.docker.internal` → `localhost`
in `Development` so `dotnet ef` commands from a local shell still work; the docker-compose
service name wouldn't resolve outside the compose network at all. If the project's
`appsettings.Development.json` already has other providers' connection strings commented out,
add the new one active and leave/add the others commented alongside it, matching that structure
rather than replacing it.

## Initial migration

Skip for `InMemory`. Otherwise, after the `DbContext`/factory files exist:

```powershell
dotnet ef migrations add Initial --project {project}
```

Only run this if the user asked you to, or the project's existing workflow clearly expects a
migration per provider setup — check for a `Migrations/` folder precedent first (per the
entity-scaffold skill's same rule).

## docker-compose.yml (local Development)

Add a `database` service to `.docker/docker-compose.yml`, and add `depends_on: [database]` to
the app's own service if not already present. Use the image/env matching the chosen provider —
if the file already has other providers' `database` blocks commented out, activate the matching
one and leave the others commented rather than deleting them:

```yaml
# MySql
database:
  image: mysql/mysql-server:latest
  ports:
    - 3306:3306
  networks:
    - network
  environment:
    MYSQL_ROOT_HOST: '%'
    MYSQL_ROOT_PASSWORD: myPassword_123

# PostgreSQL
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

# SqlServer
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

`SqLite`/`InMemory` need no `database` service — SqLite persists to a local/mounted file, not a
server container.

## SqLite (Kubernetes persistent volume, not a migration CI step)

`SqLite` needs no `SQL_TYPE`-style migration step and no Managed Identity — it's a local file,
not a network database — but unlike `InMemory` it does need K8s storage so the file survives pod
restarts, and it deviates from the base-vs-Development split used elsewhere in this skill:

- **`appsettings.json` (base, not just Development)**: `"StartupAction": "Migrate"` and
  `"ConnectionString": "Data Source=/mnt/data/nanoDb.sqlite"` go directly in the base file, the
  same in every environment. There's no external CI migration path for SqLite the way there is
  for the network providers, so the app must self-migrate at startup everywhere — this is the
  one case where `Migrate` outside `Development` is correct, not an AGENTS.md violation.
- **`.kubernetes/data-storageclass.yaml`** (new file):
  ```yaml
  apiVersion: storage.k8s.io/v1
  kind: StorageClass
  metadata:
    name: %SERVICE_NAME%-data-storage-class
  provisioner: disk.csi.azure.com
  parameters:
    storageaccounttype: Standard_LRS
    kind: Managed
  reclaimPolicy: Retain
  volumeBindingMode: WaitForFirstConsumer
  ```
- **`ReadWriteOnce`, one disk per pod, not one shared disk.** This disk can only attach to a
  single pod — so if the app runs more than one replica, `deployment.yaml`'s `kind: Deployment`
  is wrong (every replica shares one pod template and would race to attach the same static PVC;
  only the first pod to schedule ever becomes ready). Use `stateful-set.yaml`
  (`kind: StatefulSet`) with `volumeClaimTemplates` instead — giving each replica its own
  separate disk, and its own separate SqLite database file (not shared across replicas; if the
  app needs one *shared* database, that's what the network providers above are for).
- **`.kubernetes/stateful-set.yaml`**: add `serviceName: %SERVICE_NAME%-stateful-headless` alongside
  `replicas`/`selector`, mount the volume in the container:
  ```yaml
  volumeMounts:
  - name: %SERVICE_NAME%-volume
    mountPath: /mnt/data
  ```
  and, as a top-level sibling of `template:` (not nested inside `template.spec`):
  ```yaml
  volumeClaimTemplates:
  - metadata:
      name: %SERVICE_NAME%-volume
    spec:
      accessModes:
        - ReadWriteOnce
      storageClassName: %SERVICE_NAME%-data-storage-class
      resources:
        requests:
          storage: %SQL_SIZE%Gi
  ```
  Needs `SQL_SIZE: 10` (a bare number of GB, e.g. `10` — the template above appends `Gi`; or
  whatever size the user wants) added to the workflow env block.
- **`.kubernetes/service-headless.yaml`** (new file) — required by the `StatefulSet`'s
  `serviceName` field, separate from the app's normal `ClusterIP` service:
  ```yaml
  apiVersion: v1
  kind: Service
  metadata:
    name: %SERVICE_NAME%-stateful-headless
    namespace: %KUBERNETES_NAMESPACE%
  spec:
    clusterIP: None
    ports:
    - name: http
      port: 8080
    selector:
      app: %SERVICE_NAME%
  ```
- **`.kubernetes/autoscaler.yaml`** — always present on an API/Web app (per AGENTS.md's Solution
  Structure), the only app types this `StatefulSet` conversion ever applies to: change
  `scaleTargetRef.kind` from `Deployment` to `StatefulSet`.
- **Kubernetes Deploy workflow step**: apply `data-storageclass.yaml` (still needed — referenced
  by name from `volumeClaimTemplates`) and `service-headless.yaml` (same `Get-Content |
  ExpandEnvironmentVariables | Set-Content .tmp.yaml` + `kubectl apply` pattern), before
  `stateful-set.yaml`. There's no separate PVC file to apply — `volumeClaimTemplates` creates one
  per pod automatically.
- No `docker-compose.yml` `database` service, no `auth-sql-secret.yaml`, no `configmap.yaml`
  change — none of the Staging/Production section below applies to SqLite.

## InMemory (nothing further)

No `DbContext`/factory beyond the plain `DbContext` itself, no migrations, no `docker-compose`
service, no Kubernetes changes, no Staging/Production section. `Program.cs` registration and the
base `Data` config (with `ConnectionString` left `null`) are the entire job.

## Staging/Production (MySql, PostgreSQL, SqlServer only — SqLite/InMemory covered above)

This section assumes the app already has **Managed Identity** wired (`nano-add-azure-managed-identity`
— a separate, prerequisite skill: service-account.yaml, workload-identity annotations, the CI
"Managed Identity" step that produces `$env:IDENTITY_NAME`/`$env:IDENTITY_CLIENT_ID`/
`$env:IDENTITY_PRINCIPAL_ID`). If the project doesn't have that yet, point the user at that skill
first rather than wiring a migration step that references identity variables that don't exist.

It also assumes the target Azure database **server** resource already exists (a MySQL/PostgreSQL
Flexible Server, or an Azure SQL **server** — not the same as the individual database on it).
Provisioning that server is out of this skill's scope.

1. **Workflow env vars** — add alongside the existing ones:
   ```yaml
   SQL_TYPE: <mysql|postgresql|sqlserver>
   SQL_AUTH_TYPE: Azure
   SQL_NAME: <database name>
   AZURE_GROUP_DATABASE: ${{ vars.AZURE_RESOURCE_GROUP_DATABASE }}
   DOTNET_EF_TOOLS_VERSION: "10.0"
   ```
2. **Migration step** — add one of the three provider-specific steps below, placed after
   `Managed Identity` and before `Kubernetes Deploy` in the workflow. Each: resolves the Azure
   server, runs `dotnet ef database update` using an elevated/admin credential, then grants the
   app's own Managed Identity minimal (`SELECT, INSERT, UPDATE, DELETE`) permissions and builds
   the final passwordless `SQL_CONNECTIONSTRING` the app itself will use at runtime:

   ```yaml
   - name: MySQL Database Migration
     if: env.SQL_TYPE == 'mysql'
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

       if ($LastExitCode -ne 0) { throw "error"; };

       $env:APP_USER_SQL_PATH = "app-database-user.sql";
       $sql = @"
         CREATE AADUSER IF NOT EXISTS '$env:IDENTITY_NAME' IDENTIFIED BY '$env:IDENTITY_CLIENT_ID';
         GRANT SELECT, INSERT, UPDATE, DELETE ON $env:SQL_NAME.* TO '$env:IDENTITY_NAME'@'%';
         FLUSH PRIVILEGES;
       "@;
       $sql | Set-Content $env:APP_USER_SQL_PATH;

       az mysql flexible-server execute -n $env:SQL_SERVER -u $env:SQL_USER -p $env:SQL_TOKEN --file-path $env:APP_USER_SQL_PATH;
       if ($LastExitCode -ne 0) { throw "error"; };

       $env:SQL_CONNECTIONSTRING = "Server=$env:SQL_HOST;Port=$env:SQL_PORT;Database=$env:SQL_NAME;Uid=$env:IDENTITY_NAME;SslMode=Required";
       echo "SQL_CONNECTIONSTRING=$env:SQL_CONNECTIONSTRING" >> $env:GITHUB_ENV;
   ```

   ```yaml
   - name: PostgreSQL Database Migration
     if: env.SQL_TYPE == 'postgresql'
     shell: pwsh
     run: |
       $env:SQL_HOST = az postgres flexible-server list -g $env:AZURE_GROUP_DATABASE --query [0].fullyQualifiedDomainName -o tsv;
       $env:SQL_PORT = 5432;
       $env:SQL_SERVER = az postgres flexible-server list -g $env:AZURE_GROUP_DATABASE --query [0].name -o tsv;
       $env:SQL_USER = az postgres flexible-server microsoft-entra-admin list -g $env:AZURE_GROUP_DATABASE -s $env:SQL_SERVER --query "[0].principalName" -o tsv;
       $env:SQL_TOKEN = az account get-access-token --resource-type oss-rdbms --query accessToken -o tsv;

       $env:DATA__CONNECTIONSTRING = "Host=$env:SQL_HOST;Port=$env:SQL_PORT;Database=$env:SQL_NAME;Username=$env:SQL_USER;Password=$env:SQL_TOKEN;SSL Mode=Require;Trust Server Certificate=true";

       & "/opt/ef-tools/$env:DOTNET_EF_TOOLS_VERSION/dotnet-ef" database update `
           --no-build `
           --configuration Release `
           --startup-project $env:APP_NAME `
           -- `
           --environment $env:ASPNETCORE_ENVIRONMENT;

       if ($LastExitCode -ne 0) { throw "error"; };

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
       az postgres flexible-server execute -n $env:SQL_SERVER -u $env:SQL_USER -p $env:SQL_TOKEN -d postgres --file-path $env:PRINCIPAL_SQL_PATH;
       if ($LastExitCode -ne 0) { throw "error"; };

       $grantsSql = @"
         GRANT CONNECT ON DATABASE "$env:SQL_NAME" TO "$env:IDENTITY_NAME";
         GRANT USAGE ON SCHEMA public TO "$env:IDENTITY_NAME";
         GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO "$env:IDENTITY_NAME";
         ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO "$env:IDENTITY_NAME";
       "@;
       $grantsSql | Set-Content $env:GRANTS_SQL_PATH;
       az postgres flexible-server execute -n $env:SQL_SERVER -u $env:SQL_USER -p $env:SQL_TOKEN -d $env:SQL_NAME --file-path $env:GRANTS_SQL_PATH;
       if ($LastExitCode -ne 0) { throw "error"; };

       $env:SQL_CONNECTIONSTRING = "Host=$env:SQL_HOST;Port=$env:SQL_PORT;Database=$env:SQL_NAME;Username=$env:IDENTITY_NAME;SSL Mode=Require;Trust Server Certificate=true";
       echo "SQL_CONNECTIONSTRING=$env:SQL_CONNECTIONSTRING" >> $env:GITHUB_ENV;
   ```

   `SqlServer` needs **two** steps, not one — unlike MySQL/PostgreSQL Flexible Server (one
   server hosts many databases, and EF's `database update` can create the database itself),
   Azure SQL treats each database as its own billable resource that must be explicitly created
   first:

   ```yaml
   - name: SQL Server Create Database
     if: env.SQL_TYPE == 'sqlserver'
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

           az monitor metrics alert create --name "High CPU Usage" --resource-group $env:AZURE_GROUP_DATABASE --scopes $env:SQLDB_ID --condition "avg cpu_percent > 80" --window-size PT5M --evaluation-frequency PT1M --action $env:ACTION_GROUP --severity 2 --description "Alert when CPU usage is above 80% for 5 minutes.";
           az monitor metrics alert create --name "High Memory And Worker Usage" --resource-group $env:AZURE_GROUP_DATABASE --scopes $env:SQLDB_ID --condition "avg workers_percent > 80" --window-size PT5M --evaluation-frequency PT1M --action $env:ACTION_GROUP --severity 2 --description "Alert when worker/session usage is above 80% for 5 minutes.";
           az monitor metrics alert create --name "High Number Of Connections" --resource-group $env:AZURE_GROUP_DATABASE --scopes $env:SQLDB_ID --condition "total connection_successful > 100" --window-size PT5M --evaluation-frequency PT1M --action $env:ACTION_GROUP --severity 2 --description "Alert when the number of successful connections exceeds 100 in 5 minutes.";
           az monitor metrics alert create --name "High Storage IO" --resource-group $env:AZURE_GROUP_DATABASE --scopes $env:SQLDB_ID --condition "avg physical_data_read_percent > 80" --window-size PT5M --evaluation-frequency PT1M --action $env:ACTION_GROUP --severity 2 --description "Alert when data IO usage is above 80% for 5 minutes.";
           az monitor metrics alert create --name "High Storage Percent" --resource-group $env:AZURE_GROUP_DATABASE --scopes $env:SQLDB_ID --condition "avg storage_percent > 80" --window-size PT5M --evaluation-frequency PT1M --action $env:ACTION_GROUP --severity 2 --description "Alert when Storage usage exceeds 80% for 5 minutes.";

           az sql db str-policy set -g $env:AZURE_GROUP_DATABASE -s $env:SQL_SERVER_NAME -n $env:SQL_NAME --retention-days $env:SQL_BACKUP_RETENTION;

           if ($LastExitCode -ne 0) { throw "error"; };
       };
   ```

   This step is idempotent (`if (-not $env:SQL_DB_EXISTS)`) — safe to always include, it only
   acts the first time. It needs `AZURE_GROUP_LOGS: ${{ vars.AZURE_RESOURCE_GROUP_LOGS }}` added
   to the workflow env block alongside `AZURE_GROUP_DATABASE` if not already present (diagnostics
   and alerts attach to the Log Analytics workspace/action group there).

   ```yaml
   - name: SQL Server Database Migration
     if: env.SQL_TYPE == 'sqlserver'
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

       if ($LastExitCode -ne 0) { throw "error"; };
   ```

   ⚠ Unlike MySQL/PostgreSQL, this reference implementation doesn't build a passwordless
   `SQL_CONNECTIONSTRING`/grant step for SQL Server afterward — it runs the migration with the
   service principal's own credentials and stops there. If the user wants runtime
   Managed-Identity auth for SQL Server specifically rather than the service-principal
   credentials shown, flag that as a gap to resolve with them rather than inventing the missing
   grant step.

3. **Kubernetes secret** — add `.kubernetes/auth-sql-secret.yaml`:
   ```yaml
   apiVersion: v1
   kind: Secret
   metadata:
     name: %SERVICE_NAME%-sql-auth-secret
     namespace: %KUBERNETES_NAMESPACE%
   type: Opaque
   stringData:
     data-connectionstring: %SQL_CONNECTIONSTRING%
   ```
   Apply it in the `Kubernetes Deploy` step (same `Get-Content | ExpandEnvironmentVariables |
   Set-Content .tmp.yaml` + `kubectl apply` pattern every other manifest in the workflow uses),
   before the app's own `deployment.yaml` is applied.
4. **ConfigMap** — add `Data__AuthenticationType: %SQL_AUTH_TYPE%` to `.kubernetes/configmap.yaml`.
   This is what actually makes the live environment use `Azure` auth — the base
   `appsettings.json` stays `Credentials` always (see above); this env var overrides it at
   runtime.
5. **Deployment** — add to `.kubernetes/deployment.yaml`'s container `env`:
   ```yaml
   - name: Data__ConnectionString
     valueFrom:
       secretKeyRef:
         name: %SERVICE_NAME%-sql-auth-secret
         key: data-connectionstring
   ```

## After making the change

- Show the user every file touched, grouped by concern (app code, local docker-compose,
  Staging/Production CI + K8s) — this skill touches more files than most, so a flat list is
  harder to sanity-check than a grouped one.
- If package/DbContext/migration steps were skipped (`InMemory`, or `NanoCore`/`Nano.All`
  already covering the package), say so explicitly.
- If the Staging/Production section was skipped because Managed Identity isn't set up yet (point
  the user at `nano-add-azure-managed-identity`), or because the SQL Server target database doesn't
  exist, say so explicitly rather than silently doing only the local-dev half of the job.
