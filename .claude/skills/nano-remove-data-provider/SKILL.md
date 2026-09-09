---
name: nano-remove-data-provider
description: Remove a Nano data provider (MySql, PostgreSQL, SqlServer, SqLite, or InMemory) from a Nano.Library-based application - unregisters it in Program.cs and removes the DbContext/DbContextFactory, Data configuration, docker-compose database service, and (for MySql/PostgreSQL/SqlServer) the Staging/Production migration CI step and Kubernetes secret, or (for SqLite) the persistent volume. Use when the user asks to remove a database, drop persistence, or strip a data provider out of a Nano API, Web, or Console application.
---

# Nano remove data provider

Fully removes a Nano data provider from an existing Nano API, Web, or Console application — the
counterpart to `nano-add-data-provider`. Read that skill first — this one undoes exactly what it
adds, file for file; refer back to it for the shapes/locations of anything unclear here rather
than re-deriving them.

## Before making any change, determine

1. **Which provider is currently registered?** Check `Program.cs` for `.AddNanoData<TProvider,
   TContext>()`. If none, say so and stop.
2. **What depends on it?** This is the step most worth getting right — removing a Data provider
   out from under dependent features leaves them broken, not just unused. As with eventing, the
   real risk here is a startup crash, not just dead code — `IRepository` and the concrete
   `DbContext` are both registered by `AddNanoData<TProvider,TContext>()` (AGENTS.md's
   `### Repositories` section), and any class with either as a **required** constructor
   parameter fails DI resolution the instant the provider is gone:
   - **`IRepository` or the `DbContext` injected directly.** Search the project for both,
     anywhere — controllers, services, workers. A scaffolded controller
     (`nano-scaffold-entity`'s own template) always takes `IRepository` as a required parameter,
     so any existing entity's controller is a guaranteed hit — the app won't start at all with
     it left in place and the provider gone.
   - **Entities.** Beyond the controller-crash risk above, `BaseEntity`-derived classes and their
     mappings/query criteria become dead weight with nothing to persist them — not a crash by
     themselves, but still worth surfacing.
   - **Identity/Master auth.** If `Data:Identity` is configured (see AGENTS.md's `#### Identity`)
     or a JWT auth setup depends on the Identity store this context provides, removing the
     provider breaks authentication entirely.
   If any of these apply, tell the user exactly what removing the provider will do (crash vs.
   dead code) and confirm before proceeding — don't remove out from under them without saying so.
3. **Is the package reference this skill's to remove?** Same check as the logging-remove skill:
   if the project references `NanoCore`/`Nano.All`, leave it alone (it covers unrelated
   features too). Otherwise remove the `Nano.Data.<Provider>` `PackageReference` from the
   application project.

## Program.cs

Remove the `using Nano.Data.Extensions;`, `using Nano.Data.<Provider>;`, and `using
<Namespace>.Data;` lines, and the `.AddNanoData<...>()` call. Same empty-lambda cleanup rule as
the logging-remove skill: if nothing else is left in `.ConfigureServices(...)`, restore the
blank-app placeholder and the `_` discard parameter.

## Files to delete

- `Data/<Name>DbContext.cs`
- `Data/<Name>DbContextFactory.cs` (if present — `InMemory` never had one)
- `Migrations/` folder (if present — dead without the factory that constructs the context for
  `dotnet ef`; keeping stale migration files around with no way to run them is just clutter)

## appsettings.json

Remove the `Data` section entirely from the base `appsettings.json`, and from
`appsettings.Development.json` if it has its own `Data` override there (the common case — see
`nano-add-data-provider`'s placement rules for what that override normally contains).

**Exception — SqLite**: its `Data` section lives entirely in the base file (no Development
override, per `nano-add-data-provider`'s SqLite section) — remove it from there instead.

## docker-compose.yml

Comment out the `database` service block (don't delete it) — matching the established
convention of keeping all providers' blocks available for future reference, just inactive. Also
remove `depends_on: [database]` from the app's own service entry, since nothing is left to
depend on. Skip this step entirely for `InMemory` and `SqLite` — neither ever had a `database`
service.

## SqLite-specific cleanup

If the provider was `SqLite`, additionally:
- Delete `.kubernetes/data-storageclass.yaml` and `.kubernetes/data-pvc.yaml`.
- Remove their `Get-Content | ExpandEnvironmentVariables | kubectl apply` block from the
  `Kubernetes Deploy` workflow step.
- Remove the `volumeMounts`/`volumes` entries referencing `%SERVICE_NAME%-volume` from
  `.kubernetes/deployment.yaml`.
- Remove the `SQL_SIZE` workflow env var, if nothing else uses it.

## Staging/Production cleanup (MySql, PostgreSQL, SqlServer only)

Skip entirely for `SqLite`/`InMemory` (covered above / never applicable).

1. **Workflow steps** — remove `<Provider> Database Migration`. For `SqlServer` specifically,
   also remove `SQL Server Create Database` (the two steps `nano-add-data-provider` always adds
   together for that provider).
2. **Workflow env vars** — remove `SQL_TYPE`, `SQL_AUTH_TYPE`, `SQL_NAME`. Only remove
   `AZURE_GROUP_DATABASE`/`AZURE_GROUP_LOGS`/`DOTNET_EF_TOOLS_VERSION` if nothing else in the
   workflow still references them (`AZURE_GROUP_LOGS` in particular is also used by an
   Availability Check step, if one exists — check before removing).
3. **Kubernetes secret** — delete `.kubernetes/auth-sql-secret.yaml` and remove its apply block
   from the `Kubernetes Deploy` step.
4. **ConfigMap** — remove `Data__AuthenticationType: %SQL_AUTH_TYPE%` from
   `.kubernetes/configmap.yaml`.
5. **Deployment** — remove the `Data__ConnectionString` `secretKeyRef` entry from
   `.kubernetes/deployment.yaml`'s container `env`.

## After making the change

- Show the user every file touched/deleted, grouped by concern (app code, local docker-compose,
  Staging/Production CI + K8s) — same reasoning as the add skill: too many files for a flat list
  to be easy to sanity-check.
- Restate anything flagged in step 2 — required `IRepository`/`DbContext` injections that will
  now crash the app, plus orphaned entities or broken Identity/auth — one more time here, even
  if the user already confirmed it; worth a second visible reminder once the removal is done.
- If a step was skipped because the project uses `NanoCore`/`Nano.All`, or because a Staging/
  Production section never existed to begin with, say so explicitly.
