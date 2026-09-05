# AGENTS.md — Nano Framework

Implementation reference for building applications with Nano. Structured to mirror the module READMEs in this
repository (`Nano.App`, `Nano.App.Api`, `Nano.App.Console`, `Nano.App.Web`, `Nano.Logging`, `Nano.Data`,
`Nano.Eventing`, `Nano.Storage`), so a section here maps 1:1 to a section there.

---

## Solution Structure

Every Nano application — Api, Web, or Console — follows the same predictable solution layout. `{name}` is the
application's own name (e.g. `Svc.Accounts`); `{name}.Models` is a **separate, sibling project**, not nested
inside `{name}/`.

| Directory / File                                       | API | WEB | CON | Description                                                                                                          |
| -------------------------------------------------------- | --- | --- | --- | ------------------------------------------------------------------------------------------------------------------------- |
| `{name}.sln`                                                | ✓   | ✓   | ✓   | The Visual Studio solution file, at the solution root.                                                                      |
| `{name}/{name}.csproj`                                      | ✓   | ✓   | ✓   | The application project file.                                                                                               |
| `{name}/Program.cs`                                         | ✓   | ✓   | ✓   | Entry point — where the application is configured, built, and run.                                                          |
| `{name}/Properties/InternalsVisibleTo.cs`                   | ✓   | ✓   | ✓   | Exposes internal types to the test project.                                                                                 |
| `{name}/appsettings.json`                                   | ✓   | ✓   | ✓   | Default application configuration.                                                                                          |
| `{name}/appsettings.{environment}.json`                     | ✓   | ✓   | ✓   | Overrides for `Development`, `Staging`, `Production`.                                                                       |
| `{name}/Controllers/`                                       | ✓   | ✓   | ✗   | Concrete controllers (conventional location, not a hard requirement).                                                       |
| `{name}/Data/`                                              | ✓   | ✓   | ✓   | `DbContext`, `DbContextFactory`, and `Mappings/` (conventional location).                                                    |
| `{name}/Migrations/`                                        | ✓   | ✓   | ✓   | EF Core migrations (conventional location, when a SQL data provider is used).                                              |
| `{name}/wwwroot/`                                           | ✓   | ✓   | ✗   | Static/dynamic web content root.                                                                                            |
| `{name}/Dockerfile.Local`                                   | ✓   | ✓   | ✓   | Used by Docker Compose in `Development`; must stay in the application project folder.                                       |
| `{name}.Models/{name}.Models.csproj`                        | ✓   | ✓   | ✗   | Sibling project holding entity models, query criteria, and API client (Requests/Api). Publishable as its own NuGet for sharing models + API client with consumers. Should reference at minimum `Nano.App`. |
| `{name}.Models/Data/`                                       | ✓   | ✓   | ✗   | Entity models (conventional location).                                                                                      |
| `{name}.Models/Criterias/`                                  | ✓   | ✓   | ✗   | Query criteria classes (conventional location).                                                                             |
| `{name}.Models/Api/`                                        | ✓   | ✓   | ✗   | API client + `Requests/` (conventional location, for apps exposing a typed client to consumers).                            |
| `.tests/Tests.{name}/Tests.{name}.csproj`                   | ✓   | ✓   | ✓   | Test project — empty by default, demonstrates where unit/integration tests belong.                                          |
| `.tests/Tests.{name}/Properties/DoNotParallelize.cs`        | ✓   | ✓   | ✓   | Ensures tests are not parallelized.                                                                                          |
| `.docker/docker-compose.dcproj`                             | ✓   | ✓   | ✓   | Docker Compose project used by Visual Studio for local orchestration.                                                       |
| `.docker/docker-compose.yml`                                | ✓   | ✓   | ✓   | Docker Compose spec for local (`Development`) orchestration.                                                                |
| `.kubernetes/configmap.yaml`                                | ✓   | ✓   | ✓   | Kubernetes ConfigMap.                                                                                                        |
| `.kubernetes/autoscaler.yaml`                               | ✓   | ✓   | ✗   | Kubernetes Horizontal Pod Autoscaler.                                                                                        |
| `.kubernetes/deployment.yaml`                               | ✓   | ✓   | ✗   | Kubernetes Deployment.                                                                                                       |
| `.kubernetes/service.yaml`                                  | ✓   | ✓   | ✗   | Kubernetes Service.                                                                                                          |
| `.kubernetes/httproute.yaml`                                | (✓) | (✓) | ✗   | Kubernetes HTTPRoute _(optional, public-facing apps only)_.                                                                  |
| `.kubernetes/cronjob.yaml`                                  | ✗   | ✗   | ✓   | Kubernetes CronJob (Console apps run as scheduled jobs, not long-running Deployments).                                       |
| `.github/config/slack.yml`                                  | ✓   | ✓   | ✓   | Build/deploy Slack notifications _(optional)_.                                                                              |
| `.github/workflows/build-and-deploy.yml`                    | ✓   | ✓   | ✓   | CI/CD workflow — build, test, publish, deploy.                                                                              |
| `Dockerfile`                                                | ✓   | ✓   | ✓   | Container image build for `Staging`/`Production`, at the solution root.                                                     |
| `.dockerignore` / `.gitignore`                              | ✓   | ✓   | ✓   | Solution root.                                                                                                               |
| `README.md` / `icon.png` / `LICENSE`                        | (✓) | (✓) | (✓) | Solution root, optional — used for the repo and any published NuGet packages.                                                |

Folder names like `Controllers/`, `Data/`, `Criterias/`, `Api/`, and `Migrations/` are convention, not a
framework requirement — Nano discovers controllers, mappings, and data providers by type, not by folder
location. As each feature section below is filled in, it will also note where new files of that kind
conventionally belong.

**NuGet packages**: for a quick start, add `NanoCore` (all-inclusive) to `{name}.Models` only — since `{name}`
references `{name}.Models` via `ProjectReference`, every Nano package flows into the app project transitively, so
no Nano package reference is needed there directly. This is what Nano.Templates itself does. Once you know which
providers you're actually using, switch to referencing only the specific packages you need (e.g.
`Nano.Data.PostgreSQL` instead of the whole graph) — smaller dependency footprint, and it makes provider choices
explicit in the `.csproj` rather than implicit via a meta-package.

**Non-`Guid` identity**: Nano defaults every generic surface to `Guid` via a non-generic shorthand
(`BaseEntity` = `BaseEntity<Guid>`, `IRepository` = `Repository<TContext, Guid>`, etc.). ⭐ It's highly
recommended to just use `Guid` throughout — it's the path every non-generic shorthand and every real example in
this doc is built around. Using a different identity type (`int`, `long`, `string`, or a custom
`IEquatable<T>`) means threading the same `TIdentity` through **every** one of these consistently — there's no
single place that "sets" it once:

- **Data**: `AddNanoData<TProvider, TContext, TIdentity>()`, `BaseDbContext<TIdentity>`, every entity base class
  (`BaseEntity<TIdentity>`, etc.) and mapping base class.
- **Repository**: the concrete `Repository<TContext, TIdentity>` registered behind `IRepository`.
- **Controllers**: `BaseEntityController<TEntity, TIdentity, TCriteria>` and siblings, `BaseAuthController<TIdentity>`,
  `BaseAuditController<TIdentity>`.
- **Authentication**: `IAuthRepository<TIdentity>`, `IAuthIdentityRepository<TIdentity>`, `IIdentityRepository<TIdentity>`.
- **Api Client**: `BaseApiClient<TIdentity>`, `BaseIdentityApiClient<TUser, TIdentity>`, and generic requests
  (`DetailsRequest<TIdentity>`, `DeleteRequest<TIdentity>`, etc.).
- **Audit**: `AuditEntry<TIdentity>`, `AuditEntryProperty<TIdentity>`.
- **Identity entity models**: `IdentityUserEx<TIdentity>`, `IdentityRole<TIdentity>`, etc.

Mixing identity types across these — e.g. an `int`-keyed entity registered against a `Guid`-typed repository —
doesn't compile or bind correctly. If everything stays `Guid`, none of this matters; it's only relevant the
moment one non-default identity type is chosen anywhere in the app.

---

## Nano.App

Common services shared by every Nano application type (Api, Console, Web). Transitive — never referenced
directly by an app project.

### Environment

Nano is environment-neutral: behavior differs only through `appsettings.{environment}.json`, never through
environment-specific code. The environment is read from `DOTNET_ENVIRONMENT` or `ASPNETCORE_ENVIRONMENT`,
defaulting to `Development`.

| Environment   | Type   | Description                  |
| ------------- | ------ | ----------------------------- |
| `Development` | Local  | Local development machine.    |
| `Staging`     | Cloud  | Cloud Kubernetes deployment.  |
| `Production`  | Cloud  | Cloud Kubernetes deployment.  |

### Configuration

Standard .NET configuration providers, with precedence (later overrides earlier):

1. `appsettings.json`
2. `appsettings.{environment}.json`
3. Command-line arguments
4. Environment variables
5. User secrets (`Development` only)

Two deviations from stock .NET behavior:
- An **empty** configuration section is mapped with all default values — it is not treated as absent.
- Setting a section to **`null`** in an environment-specific file removes/overrides a section defined in the
  base `appsettings.json` (stock .NET silently ignores a `null` override; Nano honors it as a deletion).

### Null Logger

If no logging provider is registered (see [Nano.Logging](#nanologging)), Nano still registers `ILoggerFactory`,
`ILogger`, and `ILogger<T>` — backed by a `NullLogger` that discards everything. This is a safety fallback so
code that injects `ILogger` never fails to resolve, even with no logging provider configured.

### Api Clients

This is the mechanism for one Nano application to call another over HTTP with a typed, strongly-modeled client —
full CRUD against the target's entities, authentication, and identity management, without hand-building HTTP
requests. It's how internal services expose their models/entities to other applications (typically via a NuGet
built from their `{name}.Models` project — see [Solution Structure](#solution-structure)), and how a
publicly-exposed gateway API composes several internal services into one façade.

**Where the code lives**: the client class and its custom request types live in the *owning* service's
`{name}.Models/Api/` project (e.g. `MyService.Models/Api/MyApi.cs`, with custom requests under
`Api/Requests/`). A consuming application references that project (or its published NuGet) and injects the
client class directly — no manual DI registration needed.

#### Defining a client

Derive from `BaseApiClient` (`Guid` identity), `BaseApiClient<TIdentity>` (custom identity type), or — if the
target application has Identity configured — `BaseIdentityApiClient<TUser>`/`BaseIdentityApiClient<TUser,
TIdentity>`, where `TUser` is the target's `IEntityUser` model. The constructor must take exactly `ApiClient`.

Three shapes:

```csharp
// Bare pass-through — no custom methods, relies entirely on the built-in .Entity/.Auth/.Audit groups
public class MyApi(ApiClient apiClient) : BaseApiClient(apiClient);
```

```csharp
// Custom methods only, wrapping one hand-defined request each
public class MyOtherApi(ApiClient apiClient) : BaseApiClient(apiClient)
{
    // No response — InvokeAsync<TRequest>
    public virtual Task MyMethodAsync(MyModel model, CancellationToken cancellationToken = default)
        => this.InvokeAsync(new MyRequest { Model = model }, cancellationToken);

    // Typed response — InvokeAsync<TRequest, TResponse>; MyResponse is a plain POCO, no base type required
    public virtual Task<MyResponse?> GetMyResponseAsync(MyRequest request, CancellationToken cancellationToken = default)
        => this.InvokeAsync<MyRequest, MyResponse>(request, cancellationToken);
}
```

```csharp
// Identity-backed target — adds the .Identity method group
public class MyApi(ApiClient apiClient) : BaseIdentityApiClient<MyUser>(apiClient)
{
    public virtual Task<MyUser?> GetByEmailAsync(string emailAddress, CancellationToken cancellationToken = default)
        => this.InvokeAsync<GetByEmailRequest, MyUser>(new GetByEmailRequest { EmailAddress = emailAddress }, cancellationToken);
}
```

#### Built-in method groups

Available as properties on the client instance — no implementation needed, just call them:

| Group        | Available on                          | Covers                                                                          |
| -------------- | ---------------------------------------- | ------------------------------------------------------------------------------------ |
| `.Entity`         | `BaseApiClient`                             | Full CRUD against any entity of the target app: `GetAsync`, `GetManyAsync`, `QueryAsync`, `QueryFirstAsync`, `QueryCountAsync`, `CreateAsync`/`CreateOrEditAsync`/`CreateOrGetAsync`/`CreateAndGetAsync`/`CreateManyAsync`(`Bulk`), `EditAsync`/`EditAndGetAsync`/`EditManyAsync`(`Bulk`)/`EditQueryAsync`(`Bulk`), `DeleteAsync`/`DeleteManyAsync`(`Bulk`)/`DeleteQueryAsync`(`Bulk`). Mirrors the entity controller route table 1:1 — see [Controllers § Full CRUD route table](#controllers). |
| `.Auth`           | `BaseApiClient`                             | `LogInAsync`, `LogInRootAsync`, `LogInApiKeyAsync`, `LogInExternalAsync`, `LogInRefreshAsync`, `LogOutAsync`, `GetExternalSchemesAsync`. |
| `.Audit`          | `BaseApiClient`                             | Read-only access to the target's `AuditEntry<TIdentity>` log: `GetAsync`/`GetManyAsync`/`QueryAsync`/`QueryFirstAsync`/`QueryCountAsync`. |
| `.Identity`       | `BaseIdentityApiClient<TUser[,TIdentity]>`  | Sign-up, password set/change/reset (+ token generation), email/phone change/confirm (+ token generation), roles, claims, external logins, refresh tokens, API keys. |

An endpoint not enabled on the target application (e.g. `.Auth` when the target has no authentication configured)
returns `404` — surfaced as `null`, not an exception (see Gotchas below).

Real usage — a controller composing multiple clients (an identity-backed `MyApi` plus a custom-methods-only
`MyOtherApi`) into one gateway endpoint:

```csharp
public class MyUserController(ILogger<MyUserController> logger, MyApi myApi, MyOtherApi myOtherApi)
    : BaseController(logger)
{
    public virtual async Task<IActionResult> GetMyUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await myApi.Entity.GetAsync<MyUser>(id, cancellationToken);
        return entity == null ? this.NotFound() : this.Ok(entity);
    }

    public virtual async Task<IActionResult> SignUpAsync([FromBody][Required] MyUser entity, CancellationToken cancellationToken = default)
    {
        var user = await myApi.Identity.SignUpAsync(new SignUpRequest<MyUser> { SignUp = new SignUp<MyUser> { User = entity } }, cancellationToken);

        await myOtherApi.MyMethodAsync(new MyModel { UserId = user.Id }, cancellationToken);

        return this.Created("signup", user);
    }
}
```

#### Custom requests (endpoints beyond CRUD/Auth/Identity)

1. Derive a request from `BaseRequest`, annotated with an action attribute naming the HTTP verb + relative route:
   `[GetAction]`, `[PostAction]`, `[PutAction]`, `[DeleteAction]`, `[PatchAction]`, `[QueryAction]`, `[HeadAction]`,
   `[OptionsAction]`, `[ConnectAction]`.
2. Annotate properties with parameter attributes:

| Attribute   | Purpose                                                                                                       |
| ------------- | ------------------------------------------------------------------------------------------------------------------ |
| `[Route(Order = n)]`  | Positional route-template substitution (`{n}` placeholders in the action's route string, filled in `Order` sequence). |
| `[Query]`             | Querystring parameter (scalar types); optional `Name` override.                                                       |
| `[Body]`              | The JSON request body (one complex object).                                                                           |
| `[Form]`              | A `multipart/form-data` field — scalar, or `IFormFile`/`FileInfo`/`FileStream`/`Stream`/`NamedStream`; complex objects need `[FromFormBody]` server-side. Mutually exclusive with `[Body]`. |
| `[Header(Name=..., ValuePrefix=...)]` | An HTTP header key/value.                                                                              |

Four shapes, covering every parameter attribute:

```csharp
[GetAction("all")]
public class GetAllRequest : BaseRequest;   // no params — controller inferred from TResponse (e.g. IEnumerable<MyEntity> -> "MyEntities")

[GetAction("by-name")]
public class MyQueryRequest : BaseRequest
{
    [Query] public virtual string Name { get; set; } = null!;
}

[GetAction("{id}/file/{type}")]
public class MyFileRequest : BaseRequest
{
    [Route(Order = 0)] public virtual Guid Id { get; set; }
    [Route(Order = 1)] public virtual MyEnum Type { get; set; }

    public MyFileRequest() { this.Controller = "MyEntities"; }   // explicit override — route doesn't match a pluralized TResponse
}

[PostAction("{id}/file/set")]
public class SetMyFileRequest : BaseRequest
{
    [Route] public virtual Guid Id { get; set; }
    [Form] public virtual IFormFile File { get; set; } = null!;

    public SetMyFileRequest() { this.Controller = "MyEntities"; }
}
```

3. Add a method to the client, calling `InvokeAsync<TRequest>` (no response) or `InvokeAsync<TRequest, TResponse>`
   (typed response — use `NamedStream` or `Stream` for file downloads).

**Controller resolution**: if a request doesn't set `this.Controller` explicitly in its constructor, it's
inferred as the pluralized `TResponse` type name (e.g. `IEnumerable<MyEntity>` → `MyEntities`). Set it explicitly
whenever the route doesn't naturally match the response type, or the request has no typed response at all.

**Keep the route string in sync with the server.** Both sides declare the same route segment independently — the
action attribute here, and `[Route(...)]` on the target controller's action — with nothing enforcing they match.
Nano's own built-in requests avoid this by referencing shared constants (`Nano.Common.Consts.ActionRoutes`) from
both sides, e.g. `BaseEntityViewController` uses `[Route(ActionRoutes.INDEX)]` while the built-in `IndexRequest`
uses `[PostAction(ActionRoutes.INDEX)]` — one string, referenced twice, so a rename can't silently break the
client without also breaking the build. For your own custom endpoints, define the route segment as a constant in
a `Consts` class inside the shared `{name}.Models` project (visible to both the owning API project and any
client-Api consumer) and reference it from both the request's action attribute and the controller's `[Route(...)]`,
instead of retyping the same literal string in two places.

#### Configuration

Registered automatically — no `services.AddNanoApiClient<T>()` call needed. Every `BaseApiClient` subclass in
the entry assembly whose class name matches a key under `App:Apis` gets wired up (`HttpClient` + `ApiClient` +
the client instance) and becomes injectable.

| Setting                  | Type     | Default   | Description                                                                    |
| ---------------------------- | -------- | --------- | ------------------------------------------------------------------------------------ |
| `Host`                          | string   | localhost | Target API host.                                                                       |
| `Root`                          | string   | api       | Root path segment.                                                                     |
| `Port`                          | int      | 80        | Target port.                                                                           |
| `UseSsl`                        | bool     | false     | Use HTTPS.                                                                             |
| `Timeout`                       | TimeSpan | 00:00:30  | Request timeout.                                                                       |
| `LogInRoot.Username`            | string   | null      | Optional — auto-login as root if no inbound JWT is available to forward.               |
| `LogInRoot.Password`            | string   | null      | Optional — paired with `LogInRoot.Username`.                                            |
| `HealthCheck.UnhealthyStatus`   | enum     | Unhealthy | Status reported when the target is unreachable. API/Web apps only.                     |

```json
"App": {
  "Apis": {
    "MyApi": {
      "Host": "my-service",
      "Root": "api",
      "Port": 8080,
      "UseSsl": false,
      "Timeout": "00:00:30",
      "HealthCheck": { "UnhealthyStatus": "Unhealthy" }
    }
  }
}
```

⚠ The dictionary key **must exactly match the client's class name** (`MyApi` above) — this is the only
link between config and DI; there's no other place to declare which config entry a client uses.

#### Authentication forwarding

Outbound JWT is resolved in this order: `request.JwtTokenOverride` (explicit per-request override) → the
current inbound request's own JWT (so a call made from inside a controller/worker action transparently forwards
the caller's identity — this is how a gateway application's controllers stay authenticated end-to-end into an
internal service) → if `LogInRoot` is configured, an automatic root login (cached for the process lifetime). A
set of headers (`X-Api-Key`, `X-Forwarded-*`, request id, `Accept-Language`, timezone) is also forwarded
automatically from the inbound `HttpContext`, so locale/tenant/tracing context survives across service calls.

Console workers (which have no inbound `HttpContext`) typically call only anonymous/unauthenticated endpoints to
avoid needing `LogInRoot` credentials — a worker with no `LogInRoot` configured at all can still call target
endpoints that are `[AllowAnonymous]`.

#### Gotchas

- A configured-but-never-injected client is **not** registered — Nano only wires up clients actually referenced
  somewhere in the app.
- `404` responses return `null`, never throw — always null-check rather than try/catch.
- Other non-success responses throw `ProblemDetailsException` (structured `ProblemDetails`) or, if the body
  isn't parseable as `ProblemDetails`, `ApiClientException` (raw body + status code).
- Every generic `.Entity` read method accepts an `includeDepth` parameter — thread your own controller's
  `[FromQuery] int? includeDepth` through to it for end-to-end include-depth control, see [Include
  Annotation](#include-annotation).

### Start-Up Tasks

One-time initialization work that must complete before the application starts accepting traffic (or, for
Console apps, before workers start) — cache warm-up, external dependency checks, or similar. Not the same
mechanism as the built-in database migration task Nano runs for a configured data provider.

#### Defining a task

Implement `IStartupTask` (`OnStartAsync`/`OnStopAsync`), or derive from `BaseStartupTask` to only need
`OnStartAsync` — its `OnStopAsync` defaults to `Task.CompletedTask`.

```csharp
public class MyStartupTask(ILogger<MyStartupTask> logger) : BaseStartupTask(logger)
{
    public override async Task OnStartAsync(CancellationToken cancellationToken = default)
    {
        // one-time init — cache warm-up, external dependency check, etc.
    }

    // optional — only override if you need it; see the timing note below before relying on it
    public override async Task OnStopAsync(CancellationToken cancellationToken = default)
    {
        // cleanup for what OnStartAsync acquired — runs right after OnStartAsync completes, not at real shutdown
    }
}
```

No registration needed — just define the class in the entry assembly. Every non-abstract `IStartupTask`
implementation is discovered by reflection and registered `Scoped`; any other registered service, including
scoped ones, can be injected into the constructor.

#### Execution

All registered tasks' `OnStartAsync` run **concurrently** (`Task.WhenAll`), in one shared service scope, before
the application accepts requests. If any task throws, the exception propagates and **the application fails to
start** — a startup task is not allowed to fail silently.

⚠ **`OnStopAsync` is not "runs at application shutdown."** Immediately after all `OnStartAsync` calls complete,
Nano's internal hosted service calls its own stop routine right away — which invokes every task's `OnStopAsync`
and decrements a shared readiness counter (`StartupTaskContext`). So `OnStopAsync` actually fires right after
`OnStartAsync` finishes, as a completion/cleanup hook — not tied to real application shutdown (though the host's
real shutdown sequence may also invoke it again). Use it for cleanup that belongs immediately after the task's
own startup work, not for logic that must run when the application actually stops.

#### Readiness integration

The same readiness counter backs the built-in *self* startup health check: once [Health Checks](#health-checks)
are enabled, the application isn't reported healthy/ready until every startup task's `OnStartAsync` **and**
`OnStopAsync` have completed. In Console apps, workers don't start until this same counter reaches zero — startup
tasks always run to completion before the first worker starts.

Conventionally placed in a `Startup/` folder in the application project (not a hard requirement — discovered by
type, not location).

### Custom Services

Standard ASP.NET Core dependency injection — nothing Nano-specific beyond the extension point. Register anything
in the `ConfigureServices(...)` step alongside the `AddNanoX<...>()` provider calls:

```csharp
NanoApiApplication
    .ConfigureApp()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IMyService, MyService>();
    })
    .Build()
    .Run();
```

### Custom Middleware

Add middleware to the `IApplicationBuilder` delegate passed to `Build(...)`:

```csharp
NanoApiApplication
    .ConfigureApp()
    .ConfigureServices(services => { /* ... */ })
    .Build(builder =>
    {
        builder.Use((context, next) =>
        {
            context.Response.Headers["MyHeader"] = "MyValue";

            return next();
        });
    })
    .Run();
```

⚠ Custom middleware is **appended to the end** of Nano's own middleware pipeline — it can't run earlier in the
pipeline than Nano's built-in middleware.

⚠ Only API and Web applications support this — Console applications ignore the `Build(builder => ...)` delegate
entirely, since there's no HTTP pipeline to add middleware to.

### Custom Configuration Section

Define an options model, add a matching section to `appsettings.json`, and bind it with
`AddNanoConfigSection<TSection>(name, out options)`:

```csharp
public class MySectionModel
{
    // Properties...
}
```

```json
{
  "MySection": { }
}
```

```csharp
.ConfigureServices(services =>
{
    services.AddNanoConfigSection<MySectionModel>("MySection", out var options);
})
```

`options` is the bound instance, available immediately for further service registration in the same
`ConfigureServices` call; the section is also registered for standard `IOptions<T>`/`IOptionsMonitor<T>`
injection anywhere else. Binding uses the same validation as every other Nano section —
`ValidateDataAnnotationsRecursively().ValidateOnStart()` — so a `[Required]` property left unset fails at host
startup, not on first use.

⚠ The section name must actually **exist** in configuration, even if empty (`"MySection": { }`) — an entirely
missing section throws `InvalidOperationException` at startup, it doesn't silently bind an all-defaults instance.

Section names must not collide with Nano's own built-in sections: `App`, `Logging`, `Data`, `Eventing`, `Storage`.

---

## Nano.App.Api

`NanoApiApplication` — the ready-to-use API host template.

### Registration

```powershell
dotnet add package Nano.App.Api;
```

```csharp
NanoApiApplication
    .ConfigureApp()
    .ConfigureServices(x =>
    {
        // Your services...
    })
    .Build()
    .Run();
```

### Configuration

The `App` section defines application-level behavior.

| Setting               | Type       | Default | Description                                                   |
| ---------------------- | ---------- | ------- | --------------------------------------------------------------- |
| `Version`               | string     | 1.0.0.0 | Application version identifier.                                 |
| `ShutdownTimeout`       | int        | 10      | Seconds to wait after SIGTERM before shutting down.              |
| `Hosting`               | object     | default | See [Hosting](#hosting).                                        |
| `HttpPolicyHeaders`     | object     | default | See [Http Policy Headers](#http-policy-headers).                |
| `ResponseCache`         | object     | null    | See [Response Cache](#response-cache).                          |
| `ResponseCompression`   | object     | null    | See [Response Compression](#response-compression).              |
| `Session`               | object     | null    | See [Session](#session).                                        |
| `TimeZone`              | object     | null    | See [TimeZone](#timezone).                                      |
| `Localization`          | object     | null    | See [Localization](#localization).                              |
| `Documentation`         | object     | null    | Swagger config. See [Documentation](#documentation).            |
| `HealthCheck`           | object     | null    | See [Health Checks](#health-checks).                            |
| `Metrics`               | object     | null    | See [Metrics (OpenTelemetry)](#metrics-opentelemetry).           |
| `VirusScan`             | object     | null    | See [Virus Scan](#virus-scan).                                  |
| `ErrorHandling`         | object     | default | See [Error Handling](#error-handling).                          |
| `Authentication`        | object     | default | See [Authentication](#authentication).                          |
| `Apis`                  | dictionary | []      | Named Nano API client configurations. See [Nano.App § Api Clients](#api-clients). |

```json
"App": {
  "Version": "1.0.0.0",
  "ShutdownTimeout": 10,
  "Hosting": { },
  "HttpPolicyHeaders": { },
  "ResponseCache": null,
  "ResponseCompression": null,
  "Session": null,
  "TimeZone": null,
  "Localization": null,
  "Documentation": null,
  "HealthCheck": null,
  "VirusScan": null,
  "ErrorHandling": { },
  "Authentication": { },
  "Apis": []
}
```

#### Hosting

How the API is hosted on Kestrel.

| Setting            | Type   | Default | Description                                     |
| -------------------- | ------ | ------- | -------------------------------------------------- |
| `Root`                | string | api     | Root route prefix for application endpoints.       |
| `Http`                | object | default | See [Http](#http).                                 |
| `Https`               | object | null    | See [Https](#https).                               |
| `MultipartLimits`     | object | null    | See [MultiPart Limits](#multipart-limits).          |

```json
"App": {
  "Hosting": {
    "Root": "api",
    "Http": { },
    "Https": null,
    "MultipartLimits": null
  }
}
```

##### Http

| Setting               | Type   | Default | Description                              |
| ------------------------ | ------ | ------- | ------------------------------------------- |
| `Ports`                   | array  | []      | List of ports for HTTP.                     |
| `UseHttpsRedirection`     | bool   | false   | Enforce HTTPS redirect for all requests.    |

```json
"App": {
  "Hosting": {
    "Http": {
      "Ports": [],
      "UseHttpsRedirection": false
    }
  }
}
```

⚠ At least one HTTP or HTTPS port must be specified. Avoid the default port 80 — it may trigger security
warnings in Kubernetes.

##### Https

Requires at least one port plus a certificate path and password. Intended primarily for local development —
`Staging`/`Production` TLS is handled at the gateway/cert-manager level, not via this config.

| Setting                | Type   | Default | Description                              |
| ------------------------- | ------ | ------- | -------------------------------------------- |
| `Ports`                    | array  | []      | List of ports for HTTPS.                     |
| `UseHttpsRequired`         | bool   | false   | Enforce HTTPS for all requests.              |
| `Certificate.Path`         | string | null    | Required. File path to the certificate.      |
| `Certificate.Password`     | string | null    | Required. Password for the certificate.      |

```json
"App": {
  "Hosting": {
    "Http": { "UseHttpsRedirection": true },
    "Https": {
      "Ports": [4443],
      "Certificate": {
        "Path": "/root/.dotnet/https/localhost.pfx",
        "Password": "password"
      },
      "UseHttpsRequired": true
    }
  }
}
```

⚠ Avoid the default HTTPS port 443 — it may trigger security warnings in Kubernetes. Configure this only in
`appsettings.Development.json`.

##### Routing

No configuration — routing is fully automatic. Routes are derived from the base controller a concrete controller
derives from; API versioning is integrated into the route automatically. All routes are normalized to lowercase.

##### MultiPart Limits

| Setting             | Type  | Default  | Description                                       |
| ---------------------- | ----- | -------- | ------------------------------------------------- |
| `MaxUploadBytes`        | int   | 33554432 | Maximum upload size in bytes (default 32 MB).      |
| `KeepAliveTimeout`      | int   | 00:02:10 | Timeout for slow uploads.                          |

```json
"App": {
  "Hosting": {
    "MultipartLimits": {
      "MaxUploadBytes": 33554432,
      "KeepAliveTimeout": 130
    }
  }
}
```

⚠ Leaving this `null` allows unlimited uploads — fine if limits are enforced at the orchestration level,
otherwise set explicit limits.

#### Http Policy Headers

Parent config object for headers such as HSTS, XSS protection, CSP, CORS, and other browser-security policies.

| Setting             | Type   | Default | Description                                      |
| ---------------------- | ------ | ------- | ---------------------------------------------------- |
| `ContentType`           | object | null    | See [Content Type Options](#content-type-options).   |
| `ReferrerPolicy`        | object | null    | See [Referrer Policy](#referrer-policy).             |
| `FrameOptions`          | object | null    | See [Frame Options](#frame-options).                 |
| `XssProtection`         | object | null    | See [Xss Protection](#xss-protection).               |
| `Csp`                   | object | null    | See [Content Security Policy (CSP)](#content-security-policy-csp). |
| `Cors`                  | object | null    | See [Cors](#cors).                                   |
| `Hsts`                  | object | null    | See [Strict Transport Security (Hsts)](#strict-transport-security-hsts). |
| `Robots`                | object | null    | See [Robots](#robots).                               |
| `ForwardedHeaders`      | object | null    | See [Forwarded Headers](#forwarded-headers).          |

```json
"App": {
  "HttpPolicyHeaders": {
    "ContentType": null,
    "ReferrerPolicy": null,
    "FrameOptions": null,
    "XssProtection": null,
    "Csp": null,
    "Cors": null,
    "Hsts": null,
    "Robots": null,
    "ForwardedHeaders": null
  }
}
```

##### Content Type Options

Sets the `X-Content-Type-Options` response header to prevent MIME type sniffing.

| Setting     | Type | Default | Description                             |
| ------------- | ---- | ------- | ---------------------------------------- |
| `NoSniff`     | bool | false   | If true, prevents MIME type sniffing. ⭐ recommended: `true`. |

```json
"App": {
  "HttpPolicyHeaders": {
    "ContentType": { "NoSniff": false }
  }
}
```

##### Referrer Policy

Sets the `Referrer-Policy` response header, controlling how much referrer information is sent with requests.

| Setting                  | Type | Default  | Description                     |
| --------------------------- | ---- | -------- | --------------------------------- |
| `ReferrerPolicyHeader`       | enum | Disabled | See policy values below.          |

```json
"App": {
  "HttpPolicyHeaders": {
    "ReferrerPolicy": { "ReferrerPolicyHeader": "Disabled" }
  }
}
```

| Policy                            | Description |
| ---------------------------------- | ------------ |
| `Disabled`                          | Header not set. |
| `NoReferrer`                        | No referrer information sent. |
| `NoReferrerWhenDowngrade`           | Full referrer unless HTTPS → HTTP. |
| `SameOrigin` ⭐                     | Full referrer for same-origin, none for cross-origin. |
| `Origin`                            | Only origin (no path/query) sent, always. |
| `StrictOrigin`                      | Origin only, and never HTTPS → HTTP. |
| `OriginWhenCrossOrigin`             | Full for same-origin, origin-only for cross-origin. |
| `StrictOriginWhenCrossOrigin`       | Full for same-origin, origin-only cross-origin, none HTTPS → HTTP. |
| `UnsafeUrl`                         | Full referrer always, including HTTPS → HTTP. Unsafe. |

The `[ReferrerPolicy]` action/controller attribute overrides the global setting per endpoint.

##### Frame Options

Sets `X-Frame-Options`, guarding against clickjacking by controlling `<iframe>`/`<frame>` embedding.

| Setting                        | Type | Default  | Description |
| ---------------------------------- | ---- | -------- | ------------ |
| `FrameOptionsPolicyHeader`          | enum | Disabled | See values below. |

```json
"App": {
  "HttpPolicyHeaders": {
    "FrameOptions": { "FrameOptionsPolicyHeader": "Disabled" }
  }
}
```

| Policy         | Description |
| ---------------- | ------------ |
| `Disabled`         | Header not set. |
| `Deny` ⭐          | Page never displayed in an iframe. |
| `SameOrigin`       | Displayed in an iframe only if same-origin. |

##### Xss Protection

Sets `X-XSS-Protection` (legacy — only honored by older browsers; a strong CSP with no `unsafe-inline` supersedes it).

| Setting                      | Type   | Default | Description                    |
| -------------------------------- | ------ | ------- | --------------------------------- |
| `XssProtectionPolicyHeader`        | enum   | null    | See values below.                 |
| `ReportingUrl`                     | string | null    | URL to report XSS attempts.       |

```json
"App": {
  "HttpPolicyHeaders": {
    "XssProtection": {
      "XssProtectionPolicyHeader": "Disabled",
      "ReportingUrl": null
    }
  }
}
```

| Policy                       | Description |
| ------------------------------ | ------------ |
| `FilterDisabled`                 | IE XSS filter explicitly disabled. |
| `FilterEnabled`                  | IE XSS filter explicitly enabled. |
| `FilterEnabledBlockMode` ⭐       | Filter enabled, BlockMode on. |
| `ProtectionReport`               | Report is sent. |

##### Content Security Policy (CSP)

Sets the `Content-Security-Policy` response header, controlling which resource origins the browser is allowed to
load for a page — the primary defense against cross-site scripting.

| Setting                    | Type   | Default | Description                                                                          |
| ------------------------------ | ------ | ------- | ------------------------------------------------------------------------------------------ |
| `ReportOnly`                      | bool   | false   | Enforce in report-only mode — violations reported, not blocked.                            |
| `UpgradeInsecureRequests`         | bool   | false   | Upgrade all HTTP requests to HTTPS automatically.                                          |
| `ReportTo`                        | object | null    | See [Report-To Directive](#report-to-directive).                                           |
| `Defaults`                        | object | null    | `default-src` — fallback for any unspecified directive below. [Common Directive](#common-directive). |
| `Scripts` / `ScriptsElem` / `ScriptsAttr` | object | null | `script-src` / `script-src-elem` / `script-src-attr`. [Scripts Directive](#scripts-directive). |
| `Styles` / `StylesElem` / `StylesAttr`    | object | null | `style-src` / `style-src-elem` / `style-src-attr`. [Styles Directive](#styles-directive).     |
| `Objects`                         | object | null    | `object-src`. [Common Directive](#common-directive).                                        |
| `Images`                          | object | null    | `img-src`. [Common Directive](#common-directive).                                           |
| `Media`                           | object | null    | `media-src`. [Common Directive](#common-directive).                                         |
| `Frames`                          | object | null    | `frame-src`. [Common Directive](#common-directive).                                         |
| `FencedFrames`                    | object | null    | `fenced-frame-src`. [Common Directive](#common-directive).                                  |
| `FrameAncestors`                  | object | null    | `frame-ancestors` — who may embed this document. [Common Directive](#common-directive).      |
| `Fonts`                           | object | null    | `font-src`. [Common Directive](#common-directive).                                          |
| `Connections`                     | object | null    | `connect-src` — fetch/XHR/WebSocket/EventSource targets. [Common Directive](#common-directive). |
| `BaseUris`                        | object | null    | `base-uri`. [Common Directive](#common-directive).                                           |
| `Children`                        | object | null    | `child-src`. [Common Directive](#common-directive).                                         |
| `Forms`                           | object | null    | `form-action`. [Common Directive](#common-directive).                                       |
| `Manifests`                       | object | null    | `manifest-src`. [Common Directive](#common-directive).                                      |
| `Workers`                         | object | null    | `worker-src`. [Common Directive](#common-directive).                                         |
| `TrustedTypes`                    | object | null    | `trusted-types`. [TrustedTypes Directive](#trustedtypes-directive).                          |
| `Sandbox`                         | object | null    | `sandbox`. [Sandbox Directive](#sandbox-directive).                                          |
| `PermissionsPolicy`               | object | null    | `permissions-policy`. [Permissions Policy Directive](#permissions-policy-directive).         |

```json
"App": {
  "HttpPolicyHeaders": {
    "Csp": {
      "ReportOnly": false,
      "UpgradeInsecureRequests": false,
      "ReportTo": null,
      "Defaults": null,
      "Scripts": null,
      "ScriptsElem": null,
      "ScriptsAttr": null,
      "Styles": null,
      "StylesElem": null,
      "StylesAttr": null,
      "Objects": null,
      "Images": null,
      "Media": null,
      "Frames": null,
      "FencedFrames": null,
      "FrameAncestors": null,
      "Fonts": null,
      "Connections": null,
      "BaseUris": null,
      "Children": null,
      "Forms": null,
      "Manifests": null,
      "Workers": null,
      "TrustedTypes": null,
      "Sandbox": null,
      "PermissionsPolicy": null
    }
  }
}
```

###### Common Directive

Shape shared by `Defaults`, `Objects`, `Images`, `Media`, `Frames`, `FencedFrames`, `FrameAncestors`, `Fonts`,
`Connections`, `BaseUris`, `Children`, `Forms`, `Manifests`, `Workers`:

| Setting   | Type  | Default | Description                                        |
| ----------- | ----- | ------- | -------------------------------------------------------- |
| `IsNone`      | bool  | false   | Only `'none'` allowed — all other sources ignored.        |
| `IsSelf`      | bool  | false   | `'self'` allowed as a source. ⭐                          |
| `Sources`     | array | []      | Custom allowed sources.                                    |

```json
"{Directive}": { "IsNone": false, "IsSelf": false, "Sources": [] }
```

###### Scripts Directive

`Scripts`/`ScriptsElem` share one shape (`script-src`/`script-src-elem`); `ScriptsAttr` (`script-src-attr`) is a
reduced variant.

| Setting                    | Type  | Default | Description                                                                         | `ScriptsAttr`? |
| ------------------------------- | ----- | ------- | ------------------------------------------------------------------------------------------ | -------------- |
| `IsNone`                          | bool  | false   | Only `'none'` allowed.                                                                       | ✓ |
| `IsSelf`                          | bool  | false   | `'self'` allowed. ⭐                                                                          | ✓ |
| `IsUnsafeInline`                  | bool  | false   | Allow inline scripts (`'unsafe-inline'`).                                                    | ✓ |
| `IsUnsafeEval`                    | bool  | false   | Allow `eval()`-like constructs (`'unsafe-eval'`).                                            | ✗ |
| `IsUnsafeWasmEval`                | bool  | false   | Allow WebAssembly unsafe evaluation.                                                          | ✗ |
| `IsTrustedTypesEval`              | bool  | false   | Allow undo of trusted-type evaluation.                                                        | ✗ |
| `StrictDynamic`                   | bool  | false   | Enable `'strict-dynamic'` behavior.                                                           | ✗ |
| `IsUnsafeHashes`                  | bool  | false   | Allow unsafe hashes for inline scripts. `script-src` only for the full shape.                 | ✓ |
| `UnsafeHashedAttributes`          | bool  | false   | Allow unsafe hashed attributes. `script-src` only for the full shape.                         | ✓ |
| `UnsafeAllowRedirects`            | bool  | false   | Allow redirects from unsafe sources.                                                          | ✗ |
| `InlineSpeculationRules`          | bool  | false   | Allow inline speculation rules.                                                               | ✗ |
| `Sources`                         | array | []      | Custom sources.                                                                                | ✓ |
| `Nonces`                          | array | []      | Nonces allowed for inline scripts.                                                             | ✗ |
| `Hashes`                          | array | []      | SHA hashes allowed for inline script content — prefix `sha256-`/`sha384-`/`sha512-`.           | ✗ |
| `RequireTrustedTypes`             | bool  | false   | Require Trusted Types for script execution. `script-src` only.                                | ✗ |
| `RequireSri`                      | bool  | false   | Require Subresource Integrity. `script-src` only.                                              | ✗ |
| `ReportSample`                    | bool  | false   | Include a sample in violation reports.                                                        | ✓ |

###### Styles Directive

`Styles`/`StylesElem` share one shape (`style-src`/`style-src-elem`); `StylesAttr` (`style-src-attr`) is a
reduced variant — same idea as Scripts, without the eval/trusted-types/strict-dynamic settings:

| Setting            | Type  | Default | Description                                                                | `StylesAttr`? |
| ---------------------- | ----- | ------- | ---------------------------------------------------------------------------------- | -------------- |
| `IsNone`                 | bool  | false   | Only `'none'` allowed.                                                              | ✓ |
| `IsSelf`                 | bool  | false   | `'self'` allowed. ⭐                                                                | ✓ |
| `IsUnsafeInline`         | bool  | false   | Allow inline styles.                                                                 | ✓ |
| `IsUnsafeHashes`         | bool  | false   | Allow unsafe hashes for inline styles. `style-src` only for the full shape.          | ✓ |
| `Sources`                | array | []      | Custom sources.                                                                       | ✓ |
| `Nonces`                 | array | []      | Nonces allowed for inline styles. Not on `StylesAttr`.                                | ✗ |
| `Hashes`                 | array | []      | SHA hashes for inline style content. Not on `StylesAttr`.                             | ✗ |
| `RequireSri`             | bool  | false   | Require Subresource Integrity. `style-src` only. Not on `StylesAttr`.                | ✗ |
| `ReportSample`           | bool  | false   | Include a sample in violation reports.                                              | ✓ |

###### TrustedTypes Directive

Allowlists Trusted Type policy names a page may create via `trustedTypes.createPolicy()`.

| Setting             | Type  | Default | Description                              |
| ----------------------- | ----- | ------- | ---------------------------------------------- |
| `IsNone`                  | bool  | false   | Only `'none'` allowed.                          |
| `AllowDuplicates`         | bool  | false   | Allow duplicate policy names.                   |
| `Policies`                | array | []      | Allowed Trusted Types policy names.             |

###### Sandbox Directive

Restricts page behavior similar to an `<iframe sandbox>` attribute — every flag defaults `false` (most
restrictive) and must be explicitly enabled:

`AllowDownloads`, `AllowForms`, `AllowModals`, `AllowOrientationLock`, `AllowPointerLock`, `AllowPopups`,
`AllowPopupsToEscapeSandbox`, `AllowPresentation`, `AllowSameOrigin`, `AllowScripts`,
`AllowStorageAccessByUserActivation`, `AllowTopNavigation`, `AllowTopNavigationByUserActivation`,
`AllowTopNavigationToCustomProtocols` — all `bool`, default `false`.

###### Permissions Policy Directive

Sets the `Permissions-Policy` header, allowing/denying browser features per-directive. Each directive named
below takes the same shape as [Common Directive](#common-directive) (`IsNone`/`IsSelf`/`Sources`):

```json
"PermissionsPolicy": {
  "{Directive}": { "IsNone": false, "IsSelf": false, "Sources": [] }
}
```

Directive names (one per browser feature): `Accelerometer`, `AmbientLightSensor`, `AriaNotify`, `AutoPlay`,
`Bluetooth`, `Battery`, `Camera`, `CapturedSurfaceControl`, `HighEntropyValues`, `ComputePressure`,
`CrossOriginIsolated`, `DeferredFetch`, `DeferredFetchMinimal`, `DisplayCapture`, `DocumentDomain`,
`EncryptedMedia`, `ExecutionWhileNotRendered`, `ExecutionWhileOutOfViewport`, `FullScreen`, `Gamepad`,
`Geolocation`, `Gyroscope`, `Hid`, `IdentityCredentialsGet`, `IdleDetection`, `LanguageDetector`, `LocalFonts`,
`LayoutAnimations`, `LegacyImageFormats`, `Magnetometer`, `Microphone`, `Midi`, `OnDeviceSpeechRecognition`,
`OtpCredentials`, `NavigationOverride`, `OversizedImages`, `Payment`, `PictureInPicture`,
`PrivateStateTokenIssuance`, `PrivateStateTokenRedemption`, `PublickeyCredentialsCreate`,
`PublicKeyCredentialsGet`, `ScreenWakeLock`, `Serial`, `SpeakerSelection`, `StorageAccess`, `Translator`,
`Summarizer`, `SyncXhr`, `UnoptimizedImages`, `UnsizedMedia`, `Usb`, `WebShare`, `WindowManagement`,
`XrSpatialTracking`.

###### Report-To Directive

Emits both `Report-To` and `Reporting-Endpoints` headers when configured.

| Setting       | Type   | Default      | Description                                                                              |
| ----------------- | ------ | ------------ | ----------------------------------------------------------------------------------------------- |
| `Group`             | string | csp-reports  | Reporting group name referenced by the CSP.                                                      |
| `MaxAge`            | int    | 10886400     | Max age in seconds for the report group.                                                         |
| `Endpoints`         | array  | []           | URLs to receive reports. Empty ⇒ defaults to Nano's own built-in `/csp/report-to` endpoint, which logs the violation. |

⚠ **Interaction with Swagger UI**: if CSP is configured strictly (no broad `'unsafe-inline'` on styles), add the
style hash `sha256-RL3ie0nH+Lzz2YNqQN83mnU0J1ot4QL7b99vMdIX99w=` to `Styles.Hashes` — otherwise the
[Documentation](#documentation) (Swagger UI) page's icons/images fail to render. This is the one place CSP
config depends on another feature's configuration.

##### Cors

Cross-Origin Resource Sharing. Nano intercepts preflight `OPTIONS` requests and answers with the correct
`Access-Control-*` headers based on this policy.

| Setting                    | Type   | Default | Description                                                                    |
| ------------------------------ | ------ | ------- | ---------------------------------------------------------------------------------- |
| `AllowedOrigins`                 | array  | []      | Allowed origins. Empty = all.                                                      |
| `AllowedHeaders`                 | array  | []      | Allowed headers. Empty = all.                                                      |
| `AllowedMethods`                 | array  | []      | Allowed methods. Empty = all.                                                      |
| `AllowCredentials`               | bool   | false   | Whether credentials are allowed.                                                   |
| `Origin.EmbedderPolicy`          | enum   | default | COEP header. Values: `UnsafeNone` ⭐, `RequireCorp`, `Credentialless`.             |
| `Origin.OpenerPolicy`            | enum   | default | COOP header. Values: `SameOrigin` ⭐, `UnsafeNone`, `SameOriginAllowPopups`.       |
| `Origin.ResourcePolicy`          | enum   | default | CORP header. Values: `SameOrigin` ⭐, `SameSite`, `CrossOrigin`.                   |
| `ExposedHeaders`                 | array  | default | Additional exposed headers. Nano exposes `TZ`, `RequestId`, `Content-Disposition`, `api-supported-versions` by default. |

```json
"App": {
  "HttpPolicyHeaders": {
    "Cors": {
      "AllowedOrigins": [],
      "AllowedHeaders": [],
      "AllowedMethods": [],
      "AllowCredentials": false,
      "Origin": { "EmbedderPolicy": null, "OpenerPolicy": null, "ResourcePolicy": null },
      "ExposedHeaders": []
    }
  }
}
```

`[EnableCors]`/`[DisableCors]` action/controller attributes override the global policy per endpoint.

##### Strict Transport Security (Hsts)

Forces browsers to only interact with the site over HTTPS.

| Setting              | Type     | Default      | Description                                                        |
| ------------------------ | -------- | ------------ | ---------------------------------------------------------------------- |
| `MaxAge`                   | TimeSpan | 182:00:00:00 | Max age (default 182 days).                                            |
| `UsePreload`                | bool     | false        | Enable the preload directive (only used if `MaxAge` > 7 weeks).        |
| `IncludeSubdomains`         | bool     | false        | Include subdomains in the policy.                                      |

```json
"App": {
  "HttpPolicyHeaders": {
    "Hsts": {
      "MaxAge": "182:00:00:00",
      "UsePreload": false,
      "IncludeSubdomains": false
    }
  }
}
```

##### Robots

Sets `X-Robots-Tag`, a de-facto standard for instructing crawlers.

| Setting             | Type | Default | Description                                                    |
| ---------------------- | ---- | ------- | -------------------------------------------------------------------- |
| `UseNoIndex`             | bool | false   | Don't index the page.                                                |
| `UseNoFollow`            | bool | false   | Don't follow links on the page.                                      |
| `UseNoSnippet`           | bool | false   | Don't show a snippet in search results.                              |
| `UseNoArchive`           | bool | false   | Don't offer a cached version.                                        |
| `UseNoOdp`               | bool | false   | Don't use Open Directory Project info.                               |
| `UseNoTranslate`         | bool | false   | Don't offer translation (Google only).                               |
| `UseNoImageIndex`        | bool | false   | Don't index images on the page (Google only).                        |

```json
"App": {
  "HttpPolicyHeaders": {
    "Robots": {
      "UseNoIndex": false,
      "UseNoFollow": false,
      "UseNoSnippet": false,
      "UseNoArchive": false,
      "UseNoOdp": false,
      "UseNoTranslate": false,
      "UseNoImageIndex": false
    }
  }
}
```

##### Forwarded Headers

Recovers the original client IP/host/protocol when the app runs behind a proxy or load balancer.

| Setting                    | Type   | Default | Description                                                                          |
| ------------------------------ | ------ | ------- | ------------------------------------------------------------------------------------------ |
| `Headers`                        | enum   | All     | Which headers to process: `None`, `XForwardedFor`, `XForwardedHost`, `XForwardedPort`, `XForwardedProto`, `XForwardedPrefix`, `All`. |
| `RequireHeaderSymmetry`          | bool   | false   | Only process forwarded headers if the full set is present for that hop.                    |

```json
"App": {
  "HttpPolicyHeaders": {
    "ForwardedHeaders": {
      "Headers": "All",
      "RequireHeaderSymmetry": false
    }
  }
}
```

| Header               | HttpContext effect                              |
| ---------------------- | -------------------------------------------------- |
| `X-Forwarded-Proto`      | Sets `HttpContext.Request.Scheme`.                  |
| `X-Forwarded-Host`       | Sets `HttpContext.Request.Host`.                    |
| `X-Forwarded-Port`       | Sets `HttpContext.Request.Host.Port`.               |
| `X-Forwarded-For`        | Sets `HttpContext.Connection.RemoteIpAddress`.      |
| `X-Forwarded-Prefix`     | Ignored — not transferred to `HttpContext`.         |

⚠ Safe only when traffic always passes through a trusted proxy/load balancer (the normal case in cloud
deployments) — otherwise these headers can be spoofed by the client.

#### Response Cache

HTTP response caching, so the origin server doesn't reprocess identical requests.

| Setting       | Type     | Default  | Description                                    |
| --------------- | -------- | -------- | -------------------------------------------------- |
| `MaxSize`         | int      | 1024     | Max cache size in KB (default 1 MB).               |
| `MaxBodySize`     | int      | 102400   | Max cached body size in KB (default 100 MB).       |
| `MaxAge`          | TimeSpan | 00:20:00 | Max cache duration (default 20 min).               |

```json
"App": {
  "ResponseCache": {
    "MaxSize": 1024,
    "MaxBodySize": 102400,
    "MaxAge": "00:20:00"
  }
}
```

Recommended: enable globally in config, disable per-action via `[ResponseCache(...)]` where needed.

#### Response Compression

| Setting     | Type | Default | Description                     |
| ------------- | ---- | ------- | ---------------------------------- |
| `UseGzip`       | bool | true    | Enable Gzip compression.           |
| `UseBrotli`     | bool | true    | Enable Brotli compression.         |

```json
"App": {
  "ResponseCompression": {
    "UseGzip": true,
    "UseBrotli": true
  }
}
```

#### Session

Server-side session state tracked via a `.AspNetCore.Session` cookie.

⚠ Discouraged — breaks statelessness and complicates horizontal scaling.

| Setting     | Type     | Default  | Description                                |
| ------------- | -------- | -------- | ---------------------------------------------- |
| `Timeout`       | TimeSpan | 00:20:00 | Session timeout (default 20 min).              |

```json
"App": {
  "Session": { "Timeout": "00:20:00" }
}
```

#### Cookies

Not configurable — Nano enforces a fixed, secure-by-default cookie policy on every cookie it creates. If a
cookie's own options already meet or exceed the policy they're left alone; if they violate it, Nano adjusts them
before the response is sent.

| Setting              | Value          | Description                                                                          |
| ----------------------- | -------------- | ----------------------------------------------------------------------------------------- |
| `HttpOnly`                | Always         | Inaccessible to `document.cookie`.                                                        |
| `CookieSecurePolicy`      | SameAsRequest  | HTTPS-provided cookies only return over HTTPS; HTTP-provided cookies return over HTTP/HTTPS. |
| `SameSiteMode`            | Strict         | Restricted to same-site requests (mitigates CSRF).                                        |

#### TimeZone

Built on the external `Vivet.AspNetCore.RequestTimeZone` package (Nano wires it up via `AddNanoRequestTimeZone`).
`DateTimeOffset` values in requests/querystrings are automatically converted to UTC internally; response
`DateTimeOffset` properties are converted back to the caller's timezone — falling back to `DefaultTimeZone` if
none was specified on the request.

⚠ `DateTime` values are **not** converted, only `DateTimeOffset`.

| Setting            | Type   | Default | Description                             |
| ---------------------- | ------ | ------- | -------------------------------------------- |
| `DefaultTimeZone`        | string | UTC     | Fallback timezone when a request specifies none. |

```json
"App": { "TimeZone": { "DefaultTimeZone": "UTC" } }
```

Specify the timezone on a request via any of: HTTP header (`tz=Europe/Copenhagen`), querystring parameter
(`tz=Europe/Copenhagen`), or cookie (`.AspNetCore.TimeZone=Europe/Copenhagen`). Propagated automatically across
layered Nano APIs via the [Api Client](#api-clients)'s automatic header forwarding.

To get the current date/time respecting this feature:

```csharp
var local = DateTimeInfo.Now;    // current date-time in the request's (or DefaultTimeZone's) timezone
var utc   = DateTimeInfo.UtcNow; // current date-time in UTC
```

#### Localization

Built on standard ASP.NET Core request localization (`RequestLocalizationMiddleware`) — no third-party package
involved. Language/culture selection per request.

| Setting                | Type   | Default | Description                                                                |
| -------------------------- | ------ | ------- | ---------------------------------------------------------------------------- |
| `DefaultCulture`             | string | en-US   | Default culture used by the application.                                     |
| `SupportedCultures`          | array  | []      | Supported cultures; unsupported cultures fall back to `DefaultCulture`.      |

```json
"App": {
  "Localization": {
    "DefaultCulture": "en-US",
    "SupportedCultures": []
  }
}
```

Culture can be specified per-request via, in order: `Accept-Language` header, `culture` query parameter, or the
`.AspNetCore.Culture` cookie. Propagated automatically across layered Nano APIs via the API client.

#### Versioning

No configuration section — built on `Asp.Versioning` (`AddApiVersioning`). Declare every version a controller
supports with `[ApiVersion]` at the class level, then map each action to the version(s) it serves with
`[MapToApiVersion]`:

```csharp
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class MyController(ILogger<MyController> logger) : BaseController(logger)
{
    [HttpGet]
    [Route("my-route")]
    [MapToApiVersion("1.0")]
    public virtual async Task<IActionResult> GetV1Async(CancellationToken cancellationToken = default)
        => this.Ok("v1");

    [HttpGet]
    [Route("my-route")]
    [MapToApiVersion("2.0")]
    public virtual async Task<IActionResult> GetV2Async(CancellationToken cancellationToken = default)
        => this.Ok("v2");
}
```

The version is resolved, in this order: route segment (`v{version}`) → `Api-Version` HTTP header → `api-version`
query parameter. Every response carries `Api-Version` (the version actually served) and
`Api-Supported-Versions` (everything the endpoint supports) automatically.

`App:Version`'s major.minor is the **default** version — routes targeting it need no version attributes at all.
Only major.minor are considered (`v1`/`v1.0` valid, `v1.0.0` is not).

⚠ Use sparingly — prefer evolving the API in a backward-compatible way and reserve versioning for genuinely
breaking changes; every additional version is ongoing maintenance surface.

#### Documentation

Enables the Swagger UI at `/docs`.

| Setting               | Type   | Default  | Description                                                                                                  |
| ------------------------- | ------ | -------- | ------------------------------------------------------------------------------------------------------------- |
| `Name`                      | string | Nano App | Application/API name.                                                                                          |
| `Description`               | string | null     | Description.                                                                                                   |
| `TermsOfServiceUrl`         | string | null     | Must be a valid URL.                                                                                            |
| `Contact.Name`              | string | null     | Contact name.                                                                                                   |
| `Contact.Email`             | string | null     | Contact email (must be valid).                                                                                  |
| `Contact.Url`               | string | null     | Contact URL (must be valid).                                                                                    |
| `License.Name`              | string | null     | License name.                                                                                                   |
| `License.Identifier`        | string | null     | SPDX license expression. Mutually exclusive with `License.Url`.                                                 |
| `License.Url`                | string | null     | License URL.                                                                                                     |
| `HideDefaultVersion`        | bool   | true     | Hide the default (`App:Version`) API version's routes in Swagger, showing only the non-versioned default routes. |

```json
"App": {
  "Documentation": {
    "Name": "Application",
    "Description": null,
    "TermsOfServiceUrl": null,
    "Contact": { "Name": null, "Email": null, "Url": null },
    "License": { "Name": null, "Url": null },
    "HideDefaultVersion": true
  }
}
```

⚠ When using a strict CSP, add style hash `sha256-RL3ie0nH+Lzz2YNqQN83mnU0J1ot4QL7b99vMdIX99w=` — otherwise
Swagger UI's icons/images fail to render.

#### Health Checks

Exposes `/healthz`. No configuration options — enable with an empty object.

```json
"App": { "HealthCheck": { } }
```

A built-in *self* startup health check waits for all startup tasks to complete before reporting ready. Every
other registered Nano provider/service with health-check support (Data, Eventing, Storage, custom API clients)
appears automatically once its own `HealthCheck` config is enabled.

Response shape:

```json
{
  "status": "Healthy",
  "checks": [
    { "name": "{name}", "status": "Healthy", "duration": 1.000 }
  ]
}
```

Health is a tree — a failing dependency's status propagates up. Custom service health checks can be registered
alongside the built-in ones in `ConfigureServices(...)`.

#### Metrics (OpenTelemetry)

Exposes `/metrics` (Prometheus-compatible, via OpenTelemetry — ASP.NET Core, HTTP client, and .NET runtime
metrics). No configuration options — enable with an empty object. Requires `HealthCheck` to also be enabled.

```json
"App": { "Metrics": { } }
```

#### Virus Scan

Built on the external `Vivet.AspNetCore.RequestVirusScan` package (Nano wires it up via `AddNanoVirusScan`).
Scans uploaded files via a connected ClamAV service. Infected uploads are rejected with `500 Internal Server
Error`, naming the detected virus and offending file(s).

⚠ ClamAV has no authentication of its own — only run it on an internal/trusted network.

| Setting                        | Type   | Default   | Description                             |
| ----------------------------------- | ------ | --------- | ------------------------------------------ |
| `Host`                                | string | clamav    | Hostname of the virus scanning service.    |
| `Port`                                | int    | 3310      | Port of the virus scanning service.        |
| `HealthCheck.UnhealthyStatus`         | enum   | Unhealthy | Reported status when the service is down.  |

```json
"App": {
  "VirusScan": {
    "Host": "clamav",
    "Port": 3310,
    "HealthCheck": { "UnhealthyStatus": "Unhealthy" }
  }
}
```

#### Content Negotiation

No configuration. `application/json` is the only supported response format (files bypass negotiation). If
`Accept` is omitted, JSON is assumed.

#### Request Tracing

No configuration. An `X-Request-Id` is generated by the first Nano instance in the call chain and propagated
through all layers via the API client; it can also be set by the caller/frontend. Available in controllers
(deriving from `BaseController`) via the `RequestId` property, and included in every response header and in all
request/response logs when logging is enabled.

#### Error Handling

Required section; auto-populated with defaults if omitted.

| Setting          | Type | Default | Description                                                                       |
| ------------------- | ---- | ------- | -------------------------------------------------------------------------------------- |
| `ExposeErrors`        | bool | false   | Expose detailed error info on `500`s. ⚠ Not recommended in `Production`.               |

```json
"App": {
  "ErrorHandling": { "ExposeErrors": false }
}
```

All errors are returned as `ProblemDetails` (RFC 7807) via a centralized middleware. Built-in exception → status
mappings:

| Exception                            | Status                     | Notes                                                          |
| ---------------------------------------- | -------------------------- | ------------------------------------------------------------------ |
| `IdentityException`                        | 400 Bad Request              | Sets `IsTranslated=true`.                                          |
| `UnauthorizedException`                    | 401 Unauthorized             |                                                                      |
| `PermissionDeniedException`                | 403 Forbidden                |                                                                      |
| `OperationCanceledException`               | 408 Request Timeout          |                                                                      |
| `VirusScanException`                       | 422 Unprocessable Entity     | Sets `IsTranslated=true`.                                          |
| `AggregateException`                       | varies                       | Status depends on inner exceptions.                                 |
| `Exception`                                | 500 Internal Server Error    | Fallback default.                                                   |
| `ProblemDetailsException`                  | any                           | Throw a fully-defined `ProblemDetails` directly.                    |
| `BadRequestException`                      | 400 Bad Request              | Can be `IsCoded` (machine-readable code) or `IsTranslated` (server-translated message). |
| `NotFoundException`                        | 404 Not Found                | For null/missing-resource errors.                                    |
| `UniqueConstraintViolationException`       | 409 Conflict                 | Thrown when a unique constraint is violated.                        |

⚠ In a layered Nano architecture, controllers must return `ProblemDetails` or no body for error responses —
custom error objects won't propagate through the API client, which falls back to a generic `500` otherwise.

#### Static Files

No configuration. Static assets are served from `wwwroot` directly by the web host, bypassing the endpoint
pipeline.

#### Authentication

Cookie-less — JWT (`Authorization` header) and/or API key (`X-Api-Key` header) are the only two schemes. If
neither is configured, every endpoint is anonymous by default. Custom `IAuthenticationHandler`s are technically
possible but not the intended extension point — external identity providers are integrated by implementing
`BaseAuthExternalRepository<TFlow>` (below), not by writing your own ASP.NET Core auth handler.

**This is what makes auth work across a layered Nano architecture.** A JWT issued by one app is just a signed
token — any other app configured with the same `Issuer`/`Audience`/`PublicKey` can validate it, with no shared
session or database involved. The [Api Client](#api-clients)'s automatic JWT forwarding is what actually carries
a caller's identity through a chain of internal service calls: an inbound request's JWT is forwarded unchanged
to every downstream call made through it, so a user who authenticated once against a gateway stays authenticated
all the way down into whichever internal service ultimately handles the request — see [Api Clients §
Authentication forwarding](#authentication-forwarding).

**Persistent vs. transient**: with [Data Identity](#identity) configured, Nano stores and manages users, roles,
and claims in the database (persistent). Without it, users authenticate only through an external provider and
get a JWT with **transient** roles/claims assigned at login time — nothing is stored. To reach any base
controller action under transient auth, the `administrator` role must be included in the transient claims.

| Role            | Grants                                    |
| ------------------- | ---------------------------------------------- |
| `reader`              | Read.                                            |
| `writer`              | Read and write (create, update, delete).         |
| `creator`             | Create.                                          |
| `editor`              | Update.                                          |
| `deleter`             | Delete.                                          |
| `identity`            | Identity actions (see [Controllers § Identity user controller](#controllers)). |
| `administrator`       | Everything.                                      |

Transient roles/claims can also be layered on top of persistent auth for per-login data that shouldn't be
stored permanently (e.g. a legal name fetched at login rather than kept as a persistent claim).

##### Configuration

| Setting                            | Type     | Default  | Description                                                          |
| --------------------------------------- | -------- | -------- | ---------------------------------------------------------------------------- |
| `Jwt.Issuer`                              | string   | null     | Required.                                                                      |
| `Jwt.Audience`                            | string   | null     | Required.                                                                      |
| `Jwt.PublicKey`                           | string   | null     | Base64 RSA public key. Required to validate tokens.                           |
| `Jwt.PrivateKey`                          | string   | null     | Base64 RSA private key. Required only on the app that **issues** tokens.      |
| `Jwt.Expiration`                          | TimeSpan | 01:00:00 | Access token lifetime.                                                        |
| `Jwt.RefreshExpiration`                   | TimeSpan | 72:00:00 | Refresh token lifetime.                                                       |
| `Jwt.RootLogin.Username`                  | string   | null     | Optional — enables root login (see below).                                    |
| `Jwt.RootLogin.Password`                  | string   | null     | Paired with `Jwt.RootLogin.Username`.                                         |
| `Jwt.ExternalLogins.Facebook.AppId`/`.AppSecret`/`.Scopes`   | string/string/array | null/null/[] | Required if using the built-in Facebook provider.  |
| `Jwt.ExternalLogins.Google.ClientId`/`.ClientSecret`/`.Scopes` | string/string/array | null/null/[] | Required if using the built-in Google provider.  |
| `Jwt.ExternalLogins.Microsoft.TenantId`/`.ClientId`/`.ClientSecret`/`.Scopes` | string/string/string/array | null/null/null/[] | Required if using the built-in Microsoft provider. |

```json
"App": {
  "Authentication": {
    "Jwt": {
      "Issuer": null,
      "Audience": null,
      "PublicKey": null,
      "PrivateKey": null,
      "Expiration": "01:00:00",
      "RefreshExpiration": "72:00:00",
      "RootLogin": { "Username": null, "Password": null },
      "ExternalLogins": null
    }
  }
}
```

⚠ Only the app that **issues** tokens needs `PrivateKey`; every app that only **validates** tokens needs just
`PublicKey`. Store both as GitHub/Kubernetes secrets, never in `appsettings.json` directly.

Generate an RSA key pair (run once, e.g. in a throwaway Console app):

```csharp
using var rsa = RSA.Create();
var publicKey = rsa.ExportRSAPublicKeyPem().Replace("-----BEGIN RSA PUBLIC KEY-----", "").Replace("-----END RSA PUBLIC KEY-----", "").Replace("\n", "");
var privateKey = rsa.ExportRSAPrivateKeyPem().Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "").Replace("\n", "");
```

**Root login** is a statically-configured, transient JWT login — no identity store involved. Useful in
`Development` when testing a service in isolation, or for console apps authenticating via the API client with no
specific user account. Logging in as root auto-assigns the `administrator` role.

⚠ Don't confuse this with `App:Apis:{ClientClassName}:LogInRoot` in [Api Clients](#api-clients) — same idea,
different direction. `Jwt.RootLogin` here lets **callers of this app** log in as root against `/auth/login/root`.
`Apis:{Client}:LogInRoot` is configured on a *different* app entirely — it's the credentials **that app's own
outbound API client** uses to auto-authenticate itself as root against *this* app, when it has no inbound JWT
to forward.

**API key auth** (`X-Api-Key` header) requires [Data Identity](#identity) with `Data:Identity:ApiKey:Secret`
configured — it's an identity-store feature, not a standalone scheme. JWT and API key can be enabled
side-by-side; Nano picks the handler based on which header is present, defaulting to JWT if both could apply.
In a layered architecture, a gateway in front of your services must exchange an API key for a JWT itself (via the
built-in `/auth/login/apikey` endpoint) before forwarding — services behind it don't accept raw API keys directly
over the wire from end users, they still expect a JWT.

##### Controller

Derive from `BaseAuthController`/`BaseAuthController<TIdentity>` — every endpoint is provided, nothing to
implement:

```csharp
public class MyAuthController(ILogger<MyAuthController> logger, IAuthRepository authRepository)
    : BaseAuthController(logger, authRepository);
```

The controller depends on one aggregate, `IAuthRepository`/`IAuthRepository<TIdentity>`, whose sub-repositories
are all nullable — each is populated only if the matching config exists, and the corresponding endpoints return
`404` when it's `null`:

| Sub-repository                | Populated when                                  | Backs                                              |
| ---------------------------------- | ------------------------------------------------------ | --------------------------------------------------------- |
| `AuthRootRepository`                 | `Jwt.RootLogin` configured                              | `/auth/login/root`                                          |
| `AuthIdentityRepository`              | [Data Identity](#identity) configured                    | `/auth/login`, `/auth/login/apikey`, `/auth/login/refresh`, `/auth/logout` |
| `AuthTransientRepository`             | `Jwt.ExternalLogins` configured, Identity **not** configured | `/auth/login/external/{providerName}/transient`       |
| `AuthExternalRepositoryAggregator`    | Always available                                        | `/auth/external/schemes`, external login resolution for both identity and transient repositories |

##### Custom external provider

Derive from `BaseAuthExternalRepository<TFlow>`, implement the two abstract methods, and give it a provider
name via the constructor — no registration needed, it's auto-discovered like everything else in Nano:

```csharp
public class MyExternalRepository() : BaseAuthExternalRepository<ImplicitFlow>("MyProvider")
{
    public override async Task<ExternalAuthenticationData> AuthenticateAsync(ImplicitFlow flow, CancellationToken cancellationToken = default)
    {
        // call the external provider, map its response to ExternalAuthenticationData
        return new ExternalAuthenticationData
        {
            Id = "external-id",
            Username = "MyUser",
            EmailAddress = "user@domain.com",
            Name = "My User",
            ExternalToken = new ExternalAuthenticationToken { Name = this.ProviderName, Token = "token", RefreshToken = "refresh-token" }
        };
    }

    public override async Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // refresh against the external provider
        return new ExternalAuthenticationToken { Name = this.ProviderName, Token = "token", RefreshToken = "refresh-token" };
    }
}
```

`TFlow` is `ImplicitFlow` or `AuthCodeFlow` (both derive `BaseAuthFlow`) — pick whichever matches the provider's
OAuth flow, or derive your own from `BaseAuthFlow` for something else entirely. Every `IAuthExternalRepository<TFlow>`
implementation is exposed through `AuthExternalRepositoryAggregator`, which resolves the right one by
`ProviderName` when multiple are registered. Built-in providers (Facebook/Google/Microsoft) exist purely as
config (see above) — no repository implementation needed for those.

##### Access tokens & claims

Every login method returns a consistent `AccessToken`:

```json
{
  "AppId": null,
  "UserId": null,
  "Token": null,
  "ExpireAt": "2026-03-06T12:00:00Z",
  "IsExpired": false,
  "RefreshToken": { "Token": null, "ExpireAt": "2026-03-07T12:00:00Z", "IsExpired": false }
}
```

`HttpContext` extensions read the current token's claims (all return `null` when unauthenticated):
`GetJwtAppId()` (default `"Default"`), `GetJwtUserId<TIdentity>()`, `GetJwtUserName()`, `GetJwtUserEmail()`,
`GetJwtToken()`. `AppId` scopes logins/refresh-tokens per application/platform, so a user can hold independent
sessions across multiple client apps.

⚠ In claims/roles terminology: **claims** carry user information, **roles** drive authorization. Nano's base
controllers are role-based by default; override `[Authorize]` and register a custom policy in `ConfigureServices`
for anything more elaborate (see [Authorization](#authorization)).

#### Authorization

If both JWT and API key are configured and both headers are present on a request, JWT takes precedence. If
**no** authentication scheme is configured at all, every one of Nano's own policies (below) resolves to
anonymous-allow — every base-controller endpoint is open to anyone, not locked down by default.

Nano's own entity/audit/identity base controllers gate their actions with these policies (role-based,
`RequireRole`/`RequireAuthenticatedUser`):

| Policy               | Required role (any one of)                                            | Used by                                    |
| ------------------------ | -------------------------------------------------------------------------- | ------------------------------------------------ |
| `NanoRead`                 | `administrator`, `writer`, `creator`, `editor`, `deleter`, `reader`           | Read actions on entity/audit controllers — see [Controllers](#controllers). |
| `NanoAdd`                  | `administrator`, `writer`, `creator`                                         | Create actions.                                   |
| `NanoEdit`                 | `administrator`, `writer`, `editor`                                          | Edit actions.                                     |
| `NanoAddOrEdit`             | `administrator`, `writer`, or (`creator` **and** `editor`)                    | Create-or-edit (upsert) actions.                  |
| `NanoDelete`                | `administrator`, `writer`, `deleter`                                         | Delete actions.                                   |
| `NanoAudit`                 | `administrator` only                                                          | The audit controller.                             |
| `NanoIdentity`              | `administrator`, `identity`                                                  | Identity-management actions on user controllers.  |

⚠ These policy name constants are `internal` to Nano — you cannot reference them from your own code. If you
need the literal string (e.g. to compose a custom policy alongside one of these), use the value directly (e.g.
`"NanoRead"`), there's no shared constant to import.

Claims carry user information; roles drive authorization. To add a custom authorization strategy beyond roles,
register an ASP.NET Core policy in `ConfigureServices` and apply it with `[Authorize(Policy = "...")]` — no
Nano-specific abstraction on top of the standard mechanism:

```csharp
// ConfigureServices
services.AddAuthorization(x =>
{
    x.AddPolicy("MyPolicy", y => y.RequireClaim("MyClaim"));
});
```

```csharp
// Controller action
[HttpGet]
[Route("my-route")]
[Authorize(Policy = "MyPolicy")]
public virtual async Task<IActionResult> MyActionAsync(CancellationToken cancellationToken = default)
    => this.Ok("my-response");
```

`AddPolicy` works well for a small, fixed set of named policies known at startup. For many fine-grained,
dynamically-named policies (e.g. a permission system with one policy per permission string), replace the
default policy provider instead — this resolves a policy on demand from the policy name rather than requiring
each one pre-registered:

```csharp
public class MyAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider fallback = new(Options.Create(new AuthorizationOptions()));

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        => Task.FromResult(new AuthorizationPolicyBuilder().RequireClaim(policyName).Build());

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => this.fallback.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => this.fallback.GetFallbackPolicyAsync();
}
```

```csharp
// ConfigureServices — replaces the default provider entirely, so AddPolicy(...) calls are no longer consulted
services.AddSingleton<IAuthorizationPolicyProvider, MyAuthorizationPolicyProvider>();
```

⚠ There is only one `IAuthorizationPolicyProvider` in the container — registering your own replaces ASP.NET
Core's default, so any policy name not handled by your provider's logic needs an explicit fallback (as shown
above) rather than silently working like before.

#### Api Clients

See [Nano.App § Api Clients](#api-clients) — no API-specific behavior beyond what's documented there.

### Controllers

Every controller must inherit from `BaseController`, directly or indirectly — it establishes routing,
versioning, model validation, authorization, and response handling. `BaseController` fixes the base route to
`http(s)://{host}:{port}/{root}/{controller}` (plus a versioned variant); **don't** put a `[Route]` attribute at
the controller level yourself, only on actions.

```csharp
[ApiController]
[Route("[controller]")]
[Route("{version}/[controller]")]
[Authorize]
public abstract class BaseController : Controller
```

`BaseController` itself requires **auth by default** (bare `[Authorize]`) and exposes `Logger` and `RequestId`
(the `X-Request-Id` header value — see [Request Tracing](#request-tracing)).

#### Entity controller hierarchy

For entities backed by [Nano.Data](#nanodata), pick the narrowest base class that matches the
capability you want to expose — each is gated by its own [authorization policy](#authorization):

| Controller                                                     | Get | Query | Create | Edit | Delete | Policy applied |
| --------------------------------------------------------------- | --- | ----- | ------ | ---- | ------ | -------------------- |
| `BaseEntityViewController<TEntity, TCriteria>`                     | ✗   | ✓     | ✗      | ✗    | ✗      | `NanoRead` (query/count only, no single-get) |
| `BaseEntityReadOnlyController<TEntity, TCriteria>`                 | ✓   | ✓     | ✗      | ✗    | ✗      | `NanoRead`             |
| `BaseEntityCreatableController<TEntity, TCriteria>`                | ✓   | ✓     | ✓      | ✗    | ✗      | `NanoRead` + `NanoAdd` |
| `BaseEntityEditableController<TEntity, TCriteria>`                 | ✓   | ✓     | ✗      | ✓    | ✗      | `NanoRead` + `NanoEdit` |
| `BaseEntityCreatableAndEditableController<TEntity, TCriteria>`     | ✓   | ✓     | ✓      | ✓    | ✗      | adds `NanoAddOrEdit` for the upsert action |
| `BaseEntityDeletableController<TEntity, TCriteria>`                | ✓   | ✓     | ✗      | ✗    | ✓      | `NanoRead` + `NanoDelete` |
| `BaseEntityController<TEntity, TCriteria>`                         | ✓   | ✓     | ✓      | ✓    | ✓      | full set, adds `NanoDelete` on top |

Each has a `<TEntity, TIdentity, TCriteria>` overload for a non-`Guid` identity type — the two-generic form shown
above is just `<TEntity, Guid, TCriteria>`. `TEntity` must satisfy the matching capability interface from
[Data Models](#data-models) — e.g. `BaseEntityController` requires `IEntityWritable`. Use
`BaseEntityViewController` specifically for entities mapped from a SQL view.

Constructor is always `(ILogger<T> logger, IRepository repository, IEventing? eventing = null)` — `IEventing` is
optional and only needed if the controller publishes custom events from an action.

```csharp
public class MyEntitysController(ILogger<MyEntitysController> logger, IRepository repository, IEventing? eventing)
    : BaseEntityController<MyEntity, MyEntityQueryCriteria>(logger, repository, eventing);
```

⚠ Naming convention: a concrete entity controller must be named the **pluralized entity name** — `MyEntity` →
`MyEntitysController` — this is how the route segment is derived.

#### Query criteria

The second generic parameter defines what's queryable. Derive from `BaseQueryCriteria` (already contributes
`CreatedAfter`/`CreatedBefore` against `BaseEntity.CreatedAt`), add one property per queryable field, and
override `GetExpressions()` — always calling `base.GetExpressions()` first:

```csharp
public class MyEntityQueryCriteria : BaseQueryCriteria
{
    public virtual string? Name { get; set; }

    public override IList<CriteriaExpression> GetExpressions()
    {
        var expressions = base.GetExpressions();
        var expression = expressions.FirstOrDefault() ?? new CriteriaExpression();

        if (!string.IsNullOrEmpty(this.Name))
        {
            expression.StartsWith(nameof(MyEntity.Name), this.Name);
        }

        expressions.Add(expression);
        return expressions;
    }
}
```

Built on the [DynamicExpression](https://github.com/vivet/DynamicExpression) library — criteria properties are
compiled into LINQ expressions against the entity, not hand-written `Where` clauses.

##### Available operations

Every `CriteriaExpression` method takes `(string property, TType value, LogicalType logicalType = And)` (or two
values for `Between`, no value for the `Is*` checks) — `property` is a string name, not a strongly-typed
expression, so it's checked at runtime, not compile time. Not every operation is valid for every property type —
constructing one that isn't throws `InvalidOperationException` immediately:

| Property type                                              | Valid operations                                                                                          |
| ------------------------------------------------------------ | ---------------------------------------------------------------------------------------------------------------- |
| Text (`string`, `char`)                                        | `Equal`, `NotEqual`, `StartsWith`, `EndsWith`, `Contains`, `NotContains`, `In`, `NotIn`, `IsNull`, `IsNotNull`, `IsEmpty`, `IsNotEmpty`, `IsNullOrWhiteSpace`, `IsNotNullOrWhiteSpace` |
| Number (any numeric type), Date (`DateOnly`/`TimeOnly`/`DateTime`/`DateTimeOffset`) | `Equal`, `NotEqual`, `GreaterThan`, `GreaterThanOrEqual`, `LessThan`, `LessThanOrEqual`, `Between`   |
| `bool`, `Guid`                                                  | `Equal`, `NotEqual`                                                                                                |
| Enum                                                            | `Equal`, `NotEqual`, `In`, `NotIn`, `Contains`, `NotContains`                                                      |
| Array / `IEnumerable<T>`                                        | `In`, `NotIn`, `Contains`, `NotContains` only                                                                       |
| `Nullable<T>`                                                   | Everything the underlying `T` supports, **plus** `IsNull`/`IsNotNull`                                              |
| Spatial (`Geometry` and subtypes, via NetTopologySuite)         | `Covers`, `Crosses`, `Touches`, `Overlaps`, `CoveredBy`, `Disjoint`, `Intersects`, `Within`, `IsWithinDistance` (takes an extra `distance` parameter) |

##### Combining criteria — sequential, not grouped

Every operation call takes a `LogicalType` (`And` default, or `Or`). Multiple calls on the **same**
`CriteriaExpression` combine strictly **left to right** — each new condition's `LogicalType` says how it joins
with everything accumulated *so far*, not with what comes after it. There is no parenthesized grouping within
one `CriteriaExpression`:

```csharp
expression.Equal(nameof(MyEntity.A), a);              // A
expression.Equal(nameof(MyEntity.B), b);               // (A) AND B
expression.Equal(nameof(MyEntity.C), c, LogicalType.Or); // (A AND B) OR C  -- NOT A AND (B OR C)
```

To get `A AND (B OR C)`, build `(B OR C)` as one `CriteriaExpression` and put `A` in a **separate**
`CriteriaExpression` — the `IList<CriteriaExpression>` returned by `GetExpressions()` is itself combined with
**AND across the list**, so multiple list entries behave like top-level AND-ed groups:

```csharp
var groupA = new CriteriaExpression();
groupA.Equal(nameof(MyEntity.A), a);

var groupBC = new CriteriaExpression();
groupBC.Equal(nameof(MyEntity.B), b);
groupBC.Equal(nameof(MyEntity.C), c, LogicalType.Or);

return new[] { groupA, groupBC };   // A AND (B OR C)
```

This is why every real criteria class in Nano.Templates/Nano.Lessons keeps to a single `CriteriaExpression` with
only `And` (the default) — as soon as an `Or` is needed, the grouping above is what's actually required to get
correct results, not just adding another `.Or(...)` call in the same chain.

##### Nested and collection properties

The `property` string supports dotted paths to navigate into nested/owned objects: `"Address.City.Name"`. To
filter by a property on a **collection navigation**, wrap the nested property in `[...]` — this compiles to
`.Any(...)` over the collection, not a direct member access:

```csharp
expression.GreaterThan("Orders[Total]", 100m);   // x.Orders.Any(o => o.Total > 100)
```

##### Ordering and pagination

`Ordering` (`IQuery.Order`) — `By` (default `"Id"`, supports the same dotted-path navigation as criteria
properties), optional `ThenBy` secondary sort key, `Direction` (`Asc`/`Desc`, default `Asc`).

`Pagination` (`IQuery.Paging`) — `Number` (1-based page number, default `1`) + `Count` (page size, default `25`,
max `25000`); or set `Skip` directly to override page-number-based calculation entirely.

#### Full CRUD route table

Available on `BaseEntityController<TEntity, TCriteria>` (narrower base classes expose the matching subset only):

| Endpoint                       | Method        | Role    | Repository call                              |
| ------------------------------ | ------------- | ------- | ---------------------------------------------------- |
| `/{entity}s/create`             | POST          | creator | `AddAsync` + save                                     |
| `/{entity}s/create/get`         | POST          | creator | `AddOrGetAsync`                                       |
| `/{entity}s/create/reload`      | POST          | creator | `AddAndGetAsync`                                      |
| `/{entity}s/create/edit`        | POST          | creator | `AddOrUpdateAsync` (only on the CreatableAndEditable/full tiers, policy `NanoAddOrEdit`) |
| `/{entity}s/create/many`        | POST          | creator | `AddManyAsync`                                        |
| `/{entity}s/create/many/bulk`   | POST          | creator | `AddManyBulkAsync`                                    |
| `/{entity}s/{id}/details`       | GET           | reader  | `GetAsync` — `includeDepth` query param               |
| `/{entity}s/details/many`       | GET, POST     | reader  | `GetManyAsync` (by ids) — `includeDepth` query param  |
| `/{entity}s/index`              | GET, POST     | reader  | `GetManyAsync` (by query) — `includeDepth` query param |
| `/{entity}s/query`              | GET, POST     | reader  | `GetManyAsync` (by criteria) — `includeDepth`         |
| `/{entity}s/query/first`        | GET, POST     | reader  | `GetFirstAsync` — `includeDepth`                       |
| `/{entity}s/query/count`        | GET, POST     | reader  | `CountAsync`                                           |
| `/{entity}s/edit`               | PUT, POST     | editor  | `UpdateAsync` + save                                   |
| `/{entity}s/edit/reload`        | PUT, POST     | editor  | `UpdateAndGetAsync`                                    |
| `/{entity}s/edit/many`          | PUT, POST     | editor  | `UpdateManyAsync`                                      |
| `/{entity}s/edit/many/bulk`     | PUT, POST     | editor  | `UpdateManyBulkAsync`                                  |
| `/{entity}s/edit/query`         | PUT, POST     | editor  | `UpdateManyAsync` by criteria (body: update-query)     |
| `/{entity}s/edit/query/bulk`    | PUT, POST     | editor  | `UpdateManyBulkAsync` by criteria                      |
| `/{entity}s/{id}/delete`        | POST, DELETE  | deleter | `DeleteAsync` + save                                   |
| `/{entity}s/delete/many`        | POST, DELETE  | deleter | `DeleteManyAsync`                                      |
| `/{entity}s/delete/many/bulk`   | POST, DELETE  | deleter | `DeleteManyBulkAsync`                                  |
| `/{entity}s/delete/query`       | POST, DELETE  | deleter | `DeleteManyAsync` by criteria                          |
| `/{entity}s/delete/query/bulk`  | POST, DELETE  | deleter | `DeleteManyBulkAsync` by criteria                      |

⚠ Never set `includeDepth` in a request higher than the app's configured [Include Annotation](#include-annotation)
depth — serialization only honors the configured value regardless.

#### Identity user controller

When [Identity](#identity) is configured, `BaseEntityUserController<TEntity, TCriteria>` (deriving
`BaseEntityEditableController`) adds identity-management actions on top of the standard CRUD set, for an entity
deriving `BaseEntityUser`/`BaseEntityUser<TIdentity>`. It takes a second constructor dependency,
`IIdentityRepository<TIdentity>`, alongside `IRepository`. Endpoints not matching the current configuration
(e.g. API keys when API-key auth isn't enabled) aren't registered at all.

| Endpoint (relative to `/{entity}s`)         | Method        | Role          |
| ------------------------------------------------- | ------------- | ------------- |
| `{id}/details/deactivated`                          | GET           | identity      |
| `password/options`                                  | GET           | Anonymous     |
| `email/is-taken`, `phone/is-taken`                   | GET           | Anonymous     |
| `signup`, `signup/external/{providerName}`           | POST          | Anonymous     |
| `{id}/username/set`                                 | POST          | Anonymous     |
| `{id}/password/set`, `{id}/password/change`         | POST          | identity      |
| `{id}/password/reset`, `password/reset/token`        | POST          | Anonymous     |
| `{id}/email/change[/token]`, `{id}/email/confirm[/token]` | POST     | identity      |
| `{id}/phone/change[/token]`, `{id}/phone/confirm[/token]` | POST     | identity      |
| `{id}/custom-purpose/confirm[/token]`               | POST          | identity      |
| `{id}/activate`, `{id}/deactivate`                   | POST/DELETE   | identity      |
| `{id}/roles`, `{id}/roles/assign`, `{id}/roles/remove` | GET/POST/DELETE | identity   |
| `{id}/claims[/assign\|replace\|assign-or-replace\|remove]` | GET/POST/PUT/DELETE | identity |
| `{id}/external-logins[/add\|remove/{providerName}]` | GET/POST/DELETE | identity    |
| `{id}/refresh-tokens[/active]`, `refresh-tokens/{id}` | GET/DELETE   | identity      |
| `{id}/api-keys`, `{id}/api-keys/create`, `api-keys/{id}/edit\|revoke` | GET/POST/PUT/DELETE | identity |
| `api-keys/{id}/roles[/assign\|remove]`, `api-keys/{id}/claims[/assign\|replace\|assign-or-replace\|remove]` | various | identity |
| `roles`, `roles/create`, `roles/delete`             | GET/POST/DELETE | **administrator** |
| `roles/{id}/claims[/assign\|replace\|assign-or-replace\|remove]` | various | **administrator** |

⚠ **Security**: `password/reset/token` and `{id}/password/reset` are anonymous by design, for internal use.
Never expose this controller directly to untrusted clients without a gateway in front.

#### Auth and audit controllers

`BaseAuthController`/`BaseAuthController<TIdentity>` — see [Authentication](#authentication), needs only
`IAuthRepository`. `BaseAuditController`/`BaseAuditController<TIdentity>` — read-only over the built-in
`AuditEntry<TIdentity>` log (mirrors the read subset of the CRUD table above, at `/audit/...`), gated by
`NanoAudit` (administrator only), requires a data provider to be registered.

Conventionally placed in a `Controllers/` folder in the application project (not a hard requirement).

#### Request Validation

Automatic on any controller deriving from `BaseController` — a failing model returns `400 Bad Request` with
validation errors, using standard ASP.NET Core `DataAnnotations` validation. Additional Nano-provided validation
attributes:

| Annotation                       | Description                                                                      |
| ------------------------------------ | -------------------------------------------------------------------------------------- |
| `InternationalPhoneAttribute`          | Valid international phone number. Properties and action parameters.                    |
| `RequiredOneOfAttribute`               | At least one of the specified members (incl. the decorated one) must be non-null.      |
| `UrlAttribute`                         | Valid URL. Properties, fields, and parameters.                                         |
| `FileExtensionValidationAttribute`     | Uploaded files must have an allowed extension.                                         |

#### Request Multipart JSON

For an action that needs both a file and structured JSON in one request, bind the JSON field with
`[FromFormBody]` alongside a plain `IFormFile` parameter — no special request wrapper type needed:

```csharp
[HttpPost]
[Route("my-route")]
public virtual async Task<IActionResult> MyActionAsync(IFormFile file, [Required][FromFormBody] MyBody body, CancellationToken cancellationToken = default)
{
    // file.FileName, body.Text, etc.
    return this.Ok("my-response");
}

public class MyBody
{
    [Required]
    public string Text { get; set; } = null!;
}
```

The client sends `multipart/form-data` with the JSON as a string-valued form field whose name matches the
parameter name (`body`). `[FromFormBody]` deserializes that field's JSON into `MyBody` and validates it against
its `ValidationAttribute`s, populating `ModelState` the same way normal model binding would.

⚠ The form field name must match the parameter name exactly.

#### Response Serialization

Newtonsoft.Json, case-insensitive, supporting Nano's own types and NetTopologySuite `Geometry` types.

Nano adds exactly one rule, and it only applies to entity navigation properties — every non-entity property
(scalars, DTOs, any type not `IEntity`/`IEnumerable<IEntity>`) serializes completely normally, no extra
behavior. For a property typed as an entity (`IEntity`) or a collection of entities (`IEnumerable<IEntity>`):
it's only ever serialized if it's tagged `[Include]` — regardless of whether it was actually loaded in memory.
An untagged navigation is always dropped from the response, even if some other code path happened to load it
(e.g. via lazy loading), which is exactly the point: it prevents accidental reference-cycle or over-fetching
leaks into API responses. See [Include Annotation](#include-annotation) for the full attribute, including the
second, independent depth check enforced here at serialize time.

### Start-Up Tasks

See [Nano.App § Start-Up Tasks](#start-up-tasks). For API/Web apps, completion gates the built-in self
readiness health check.

---

## Nano.App.Console

`NanoConsoleApplication` — a console-worker host template, typically run as a Kubernetes CronJob.

### Registration

```powershell
dotnet add package Nano.App.Console;
```

```csharp
NanoConsoleApplication
    .ConfigureApp()
    .ConfigureServices(x =>
    {
        // Your services...
    })
    .Build()
    .Run();
```

### Configuration

| Setting          | Type       | Default | Description                                   |
| ------------------- | ---------- | ------- | -------------------------------------------------- |
| `Version`             | string     | 1.0.0.0 | Application version identifier.                     |
| `Localization`        | object     | null    | See [Localization](#localization-1).                |
| `Apis`                | dictionary | []      | Named Nano API client configurations. See [Nano.App § Api Clients](#api-clients). |

```json
"App": {
  "Version": "1.0.0.0",
  "Localization": null,
  "Apis": null
}
```

#### Localization

Sets `CultureInfo.DefaultThreadCurrentCulture` directly — a plain .NET setting, not ASP.NET Core request
localization (there's no HTTP request to localize in a console app).

| Setting            | Type   | Default | Description                            |
| ---------------------- | ------ | ------- | -------------------------------------------- |
| `DefaultCulture`         | string | en-US   | Default culture used by the application.     |

```json
"Localization": { "DefaultCulture": "en-US" }
```

#### Exception Handling

No configuration. Exceptions thrown by one worker are handled internally and don't affect other workers; when a
logging provider is registered, the failure is logged automatically.

#### Api Clients

See [Nano.App § Api Clients](#api-clients). Console workers have no inbound `HttpContext`, so prefer calling
target endpoints that are `[AllowAnonymous]` rather than configuring `LogInRoot` credentials just to
authenticate.

### Console Workers

A console app's actual job — run-to-completion background work, run once per process launch (typically as a
Kubernetes CronJob), not a long-running daemon loop.

```csharp
public class MyWorker(ILogger<MyWorker> logger) : BaseWorker(logger)
{
    public override async Task OnStartAsync(CancellationToken cancellationToken = default)
    {
        // the actual work
    }

    // optional — only override if you need cleanup; see the exception-handling note below
    public override Task OnStopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
```

No registration needed — every non-abstract `IWorker` in the entry assembly is discovered and registered
`Scoped`, same mechanism as [Start-Up Tasks](#start-up-tasks).

**Lifecycle**: all workers wait for every [startup task](#start-up-tasks) to finish first, then every worker's
`OnStartAsync` runs **concurrently** (`Task.WhenAll`). Once all have finished starting, every worker's
`OnStopAsync` runs concurrently, and then the application deliberately shuts itself down
(`IHostApplicationLifetime.StopApplication()`) — this is the entire reason a console app process exits on its
own once the work is done.

⚠ **Unlike startup tasks, a worker that throws does not abort anything.** The exception is caught and logged,
the worker's task is treated as complete, and every other worker still runs to completion — a failing worker
doesn't stop its siblings or prevent the graceful shutdown sequence. If a failure should actually be surfaced
(e.g. as a non-zero exit code for a CronJob to alert on), you need to handle that yourself inside `OnStartAsync`.

Conventionally placed in a `Workers/` folder in the application project (not a hard requirement).

### Startup Tasks

See [Nano.App § Start-Up Tasks](#start-up-tasks). In Console apps, workers wait for all startup tasks to
complete before any worker starts.

---

## Nano.App.Web

`NanoWebApplication<TRoot>` — a Nano Web application is the same as a Nano Api application, except that it
additionally registers frontend services (Razor Pages/Blazor support). ⚠ Experimental.

### Registration

```powershell
dotnet add package Nano.App.Web;
```

```csharp
NanoWebApplication<TRoot>
    .ConfigureApp()
    .ConfigureServices(x =>
    {
        // Your services...
    })
    .Build()
    .Run();
```

### Configuration

No additional configuration beyond the [Nano.App.Api § Configuration](#configuration-2) `App` section — Web
applications use the same shape.

### Razor

Not yet documented/implemented in the source README.

### Blazor

Not yet documented/implemented in the source README.

---

## Nano.Logging

Enables `ILoggerFactory`/`ILogger`/`ILogger<T>` via a pluggable provider. All providers write to console by
default; in `Staging`/`Production`, Kubernetes intercepts stdout/stderr for centralized log collection — prefer
letting the collector own routing rather than configuring it per app.

### Registration

```csharp
.ConfigureServices(services =>
{
    services.AddNanoLogging<TProvider>();
})
```

### Configuration

| Setting                       | Type   | Default     | Description                                                              |
| --------------------------------- | ------ | ----------- | ------------------------------------------------------------------------------ |
| `LogLevel`                          | enum   | Information | Default minimum log level: `Debug`, `Information`, `Warning`, `Error`, `Fatal`. |
| `LogLevelOverrides[].Namespace`     | string | null        | Namespace to override (supports `*` prefix wildcard).                          |
| `LogLevelOverrides[].LogLevel`      | enum   | Warning     | Log level for that namespace.                                                  |

```json
"Logging": {
  "LogLevel": "Information",
  "LogLevelOverrides": [
    { "Namespace": "Microsoft", "LogLevel": "Warning" }
  ]
}
```

### Logging Providers

Each provider is a one-line registration; all write the same concise console format (timestamp, level, message,
exception).

| Provider          | Package                    | Registration                                  |
| -------------------- | --------------------------- | -------------------------------------------------- |
| Serilog                | `Nano.Logging.Serilog`        | `services.AddNanoLogging<SerilogProvider>();`        |
| Log4Net                | `Nano.Logging.Log4Net`        | `services.AddNanoLogging<Log4NetProvider>();`        |
| NLog                   | `Nano.Logging.NLog`           | `services.AddNanoLogging<NLogProvider>();`           |
| Microsoft               | `Nano.Logging.Microsoft`      | `services.AddNanoLogging<MicrosoftProvider>();`      |

To implement a custom provider: implement `ILoggingProvider` (single `Configure(...)` method), register all
required services there, then register with `.AddNanoLogging<TProvider>()`.

---

## Nano.Data

Entity Framework–based data access. Transitive — reference one of the concrete provider packages instead.

### Configuration

| Setting                          | Type   | Default     | Description                                                                                     |
| ------------------------------------- | ------ | ----------- | ------------------------------------------------------------------------------------------------- |
| `BatchSize`                             | int    | 25          | Max batch size for queries.                                                                        |
| `BulkBatchSize`                         | int    | 500         | Max batch size for bulk operations.                                                                |
| `BulkBatchDelay`                        | int    | 1000        | Delay (ms) between bulk batches.                                                                    |
| `QueryRetryCount`                       | int    | 0           | Retry count on query failure.                                                                        |
| `UseLazyLoading`                        | bool   | false       | ⚠ Not recommended — prefer [Include Annotation](#include-annotation). See also [Lazy Loading](#lazy-loading). |
| `StartupAction`                         | enum   | None        | `None`, `Create`, or `Migrate`.                                                                      |
| `UseSensitiveDataLogging`               | bool   | false       | Enable sensitive data logging.                                                                       |
| `QuerySplittingBehavior`                | enum   | SingleQuery | Default EF Core query splitting behavior.                                                           |
| `DefaultCollation`                      | string | null        | ⚠ Affects only new migrations, not existing tables/columns.                                          |
| `ConnectionString`                      | string | null        | Required.                                                                                             |
| `AuthenticationType`                    | enum   | Credentials | `Credentials` or `Azure` (Kubernetes Workload Identity).                                             |
| `Repository.UseAutoSave`                | bool   | true        | Auto-persist changes made through repositories. See [Repositories](#repositories).                    |
| `Repository.QueryIncludeDepth`          | int    | 4           | Max include-annotation depth. See [Include Annotation](#include-annotation).                          |
| `ConnectionPool`                        | object | null        | See [Connection Pool](#connection-pool).                                                             |
| `Identity`                              | object | null        | See [Identity](#identity).                                                                            |
| `HealthCheck`                           | object | null        | See [Health Checks](#health-checks-1). Only relevant for `NanoApiApplication`/`NanoWebApplication`.  |

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
  "Repository": { "UseAutoSave": true, "QueryIncludeDepth": 4 },
  "ConnectionPool": null,
  "Identity": null,
  "HealthCheck": null
}
```

#### Connection Pool

Optional pooling of `DbContext` instances for reduced allocation overhead.

| Setting     | Type | Default | Description                    |
| ------------- | ---- | ------- | ---------------------------------- |
| `PoolSize`      | int  | 1024    | Size of the connection pool.        |

#### Identity

Configures the data store for authentication/authorization — users, roles, and related security data.

| Setting                                    | Type     | Default          | Description                                                              |
| ----------------------------------------------- | -------- | ---------------- | ------------------------------------------------------------------------------ |
| `TokensExpiration`                                | TimeSpan | 24:00:00          | Token expiration.                                                               |
| `UseAudit`                                        | enum     | None              | Which identity models to audit — see values below. Multiple allowed (CSV).      |
| `User.IsUniqueEmailAddressRequired`               | bool     | true              | Require unique email per user.                                                  |
| `User.IsUniquePhoneNumberRequired`                | bool     | false             | Require unique phone per user.                                                  |
| `User.AllowedUserNameCharacters`                  | string   | a-zA-Z0-9-._@+    | Allowed username characters.                                                    |
| `User.DefaultRoles`                               | array    | [administrator]   | Roles assigned to new users. `null` ⇒ automatically Administrator.              |
| `SignIn.RequireConfirmedEmail`                    | bool     | false             | Require confirmed email to sign in.                                            |
| `SignIn.RequireConfirmedPhoneNumber`               | bool     | false             | Require confirmed phone to sign in.                                            |
| `Lockout.AllowedForNewUsers`                      | bool     | true              | Allow lockout for new users.                                                    |
| `Lockout.MaxFailedAccessAttempts`                 | int      | 3                 | Failed attempts before lockout.                                                 |
| `Lockout.DefaultLockoutTimeSpan`                  | TimeSpan | 00:30:00          | Lockout duration.                                                               |
| `Password.RequireDigit`                           | bool     | true              | Require a digit.                                                                |
| `Password.RequireNonAlphanumeric`                 | bool     | true              | Require a non-alphanumeric character.                                          |
| `Password.RequireLowercase`                       | bool     | true              | Require a lowercase letter.                                                    |
| `Password.RequireUppercase`                       | bool     | true              | Require an uppercase letter.                                                   |
| `Password.RequiredLength`                          | int      | 12                | Minimum length.                                                                 |
| `Password.RequiredUniqueCharacters`               | int      | 3                 | Minimum unique characters.                                                      |
| `ApiKey.Secret`                                   | string   | null              | Required (if API keys used). Secret for creating/validating API keys.          |

```json
"Data": {
  "Identity": {
    "TokensExpiration": "24:00:00",
    "UseAudit": "None",
    "User": {
      "IsUniqueEmailAddressRequired": true,
      "IsUniquePhoneNumberRequired": false,
      "AllowedUserNameCharacters": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+",
      "DefaultRoles": ["administrator"]
    },
    "SignIn": { "RequireConfirmedEmail": false, "RequireConfirmedPhoneNumber": false },
    "Lockout": { "AllowedForNewUsers": true, "MaxFailedAccessAttempts": 3, "DefaultLockoutTimeSpan": "00:30:00" },
    "Password": {
      "RequireDigit": true,
      "RequireNonAlphanumeric": true,
      "RequireLowercase": true,
      "RequireUppercase": true,
      "RequiredLength": 12,
      "RequiredUniqueCharacters": 3
    },
    "ApiKey": { "Secret": null }
  }
}
```

`UseAudit` accepts one or more (comma-separated) of: `None`, `Standard` (User, UserRole, ApiKey, ApiKeyRole),
`All`, `User`, `UserRole`, `UserClaim`, `UserLogin`, `Role`, `RoleClaim`, `ApiKey`, `ApiKeyClaim`, `ApiKeyRole`.
Sensitive/technical properties are always excluded from audit regardless of this setting.

When Identity is configured, these roles are auto-created:

| Role            | Description                          |
| ------------------ | ---------------------------------------- |
| `reader`              | Authorized to read.                       |
| `writer`               | Authorized to read and write.             |
| `creator`              | Authorized to create.                     |
| `editor`               | Authorized to update.                     |
| `deleter`              | Authorized to delete.                     |
| `identity`             | Authorized to use identity actions.       |
| `administrator`        | Full access to everything.                |

⚠ Even when Identity is not configured, the identity tables are still created, so it can be enabled later
without a schema change.

#### Health Checks

| Setting                        | Type | Default   | Description                                                                       |
| ----------------------------------- | ---- | --------- | ---------------------------------------------------------------------------------------- |
| `HealthCheck.UnhealthyStatus`          | enum | Unhealthy | Status reported when the data provider is unavailable. API/Web apps only.                |

```json
"Data": {
  "HealthCheck": { "UnhealthyStatus": "Unhealthy" }
}
```

### Data Providers

All Nano data providers implement `IDataProvider`. Registration is always the same shape regardless of which
provider is chosen:

```csharp
.ConfigureServices(services =>
{
    services.AddNanoData<TProvider, TContext>();
})
```

| Provider          | Package                  | Registration                                              | Notes                                        |
| -------------------- | -------------------------- | ---------------------------------------------------------------- | -------------------------------------------------- |
| InMemory                | `Nano.Data.InMemory`         | `AddNanoData<InMemoryProvider, TContext>()`                        | No migrations, no `BaseDbContextFactory` needed.     |
| MySql                   | `Nano.Data.MySql`            | `AddNanoData<MySqlProvider, TContext>()`                            | Needs `BaseDbContextFactory` + initial migration.    |
| PostgreSQL              | `Nano.Data.PostgreSQL`       | `AddNanoData<PostgreSqlProvider, TContext>()`                        | Spatial via NetTopologySuite (`postgis`), vector search via Pgvector (`vector`) — both must be installed/allow-listed on the server. |
| SqLite                  | `Nano.Data.SqLite`           | `AddNanoData<SqLiteProvider, TContext>()`                            | ⚠ No native spatial support; `mod_spatialite` unreliable. |
| SqlServer               | `Nano.Data.SqlServer`        | `AddNanoData<SqlServerProvider, TContext>()`                         | Needs `BaseDbContextFactory` + initial migration.    |

```json
"Data": {
  "ConnectionString": "..."
}
```

Example config shape (identical across providers except `ConnectionString` format and provider-specific fields):

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
  "Repository": { "UseAutoSave": true, "QueryIncludeDepth": 4 },
  "Identity": null,
  "ConnectionPool": null,
  "HealthCheck": { "UnhealthyStatus": "Unhealthy" }
}
```

#### Custom data provider

Implement `IDataProvider`'s two static members (C# static-abstract interface members — the type itself, not an
instance, is the provider):

```csharp
public sealed class MyProvider : IDataProvider
{
    public static void Configure(IServiceCollection services, DataOptions options)
    {
        // register provider-specific services here, e.g. a health check
    }

    public static void Configure(DbContextOptionsBuilder builder, DataOptions options)
    {
        builder.UseMyDatabaseEngine(options.ConnectionString);
    }
}
```

Then use it exactly like a built-in provider: `services.AddNanoData<MyProvider, MyDbContext>();`. The first
`Configure` overload wires DI (health checks, exception translators, anything else the underlying EF Core
provider needs); the second configures the `DbContextOptionsBuilder` itself — connection string, retry policy,
batching, spatial/vector extensions, whatever the target database supports.

### Data Context

Derive a thin subclass from `BaseDbContext` (`Guid` identity) or `BaseDbContext<TIdentity>` — no logic to add,
the constructor shape matters more than anything else:

```csharp
public class MyDbContext(DbContextOptions contextOptions, IOptionsMonitor<DataOptions> dataOptions)
    : BaseDbContext(contextOptions, dataOptions);
```

⚠ The constructor **must** be exactly `(DbContextOptions, IOptionsMonitor<DataOptions>)` — the design-time
factory (below) creates the context via `Activator.CreateInstance`, not a DI-resolved call, so this shape is a
hard requirement, not just convention.

Conventionally placed in a `Data/` folder in the application project (not a hard requirement — it just needs to
live in the entry assembly).

`BaseDbContext<TIdentity>` extends ASP.NET Core Identity's `IdentityDbContext<...>` (the full 8-generic-argument
set — user/role/claims/logins/tokens) and also implements `IDataProtectionKeyContext`, so the same context
doubles as ASP.NET Core Data Protection's key store. **Never override `OnModelCreating`** — it already:
- applies `DefaultCollation` if configured,
- auto-discovers and applies every `BaseMapping<T>`-derived mapping in the entry assembly (`MapEntities<TIdentity>()`),
- maps the identity entities (`MapIdentityEntities<TIdentity>`),
- and maps the built-in `AuditEntry`/`AuditEntryProperty` tables.

If you must override something, always call `base.OnModelCreating(modelBuilder)` first.

`SaveChanges`/`SaveChangesAsync` are overridden to wrap `EntityFrameworkCore.Triggers`' trigger pipeline (see
[Triggers](#triggers)) with pre/post audit capture (see [Audit](#audit)). ⚠ **If anything was captured for
audit, `SaveChanges` issues a second round-trip to the database** to persist the audit rows — a single call to
`SaveChangesAsync()` is not guaranteed to be exactly one `INSERT`/`UPDATE` statement batch when auditing is
active.

`Update`/`UpdateRange` are also overridden — before EF tracks the update, they walk the entity graph
(`EntityGraphHydrator.HydrateAudit`) so audit correctly captures original vs. new values even for entities that
were modified while detached from the context.

A custom `AddOrUpdate<TEntity>(entity)` helper (used internally by `IRepository.AddOrUpdateAsync`) checks the
change tracker first (by reference), then falls back to a primary-key lookup (`Find` + `SetValues`), and finally
adds the entity if neither matched.

#### Design-time factory (migrations)

Also derive a `BaseDbContextFactory<TProvider, TContext>` subclass — required for `dotnet ef` tooling, since
migrations run outside the normal app host/DI:

```csharp
public class MyDbContextFactory : BaseDbContextFactory<MyProvider, MyDbContext>;
```

It builds `DataOptions` directly from configuration (bypassing the rest of app bootstrap), and in `Development`
automatically rewrites `host.docker.internal` → `localhost` in the connection string, so `dotnet ef` commands
run correctly from a local shell even though the app's own `appsettings.Development.json` is written for
Docker Compose networking.

Conventionally placed alongside the DbContext (same `Data/` folder).

### Data Models

An entity is a class deriving from `BaseEntity` (or one of the narrower capability base classes below), which
also determines what `IRepository` operations are available on it — matching the entity capability interfaces
used throughout [Controllers](#controllers) and [Repositories](#repositories):

```
IEntity
 └─ IEntityIdentity<TIdentity>            → Id
     └─ IEntityReadOnly<TIdentity>        → + IsDeleted, CreatedAt   (every base class below has this)
IEntityCreatable / IEntityUpdatable / IEntityDeletable      (capability markers, no members)
IEntityCreatableAndUpdatable = IEntityCreatable + IEntityUpdatable
IEntityWritable              = IEntityCreatableAndUpdatable + IEntityDeletable
IEntitySoftDeletable         : IEntityDeletable, redeclares IsDeleted   (opt-in)
```

`IEntitySoftDeletable` is opt-in — implement it to switch an entity from hard-delete to soft-delete, see
[Soft Delete](#soft-delete).

| Base class                                  | Capability                     | Notes                                                                |
| ---------------------------------------------- | ----------------------------------- | --------------------------------------------------------------------------- |
| `BaseEntity` / `BaseEntity<TIdentity>`            | Full CRUD (`IEntityWritable`)          | `Id` auto-assigned (`Guid.NewGuid()`) in the constructor for the `Guid` variant. |
| `BaseEntityReadOnly` / `<TIdentity>`               | None — `Id`/`IsDeleted`/`CreatedAt` only | ⚠ Not intended to be used directly; it's the common ancestor every other base class below derives from. |
| `BaseEntityCreatable` / `<TIdentity>`              | `IEntityCreatable` only                | |
| `BaseEntityUpdatable` / `<TIdentity>`              | `IEntityUpdatable` only                | |
| `BaseEntityDeletable` / `<TIdentity>`              | `IEntityDeletable` only                | |
| `BaseEntityCreatableAndUpdatable` / `<TIdentity>`   | Create + update, no delete             | |
| `BaseEntityUser` / `<TIdentity>`                   | Update + delete (`IEntityUser<TIdentity>`) | Adds `IdentityUser` (`IdentityUserEx<TIdentity>`), tagged `[Include]` + `[ValidateNever]` + `[SwaggerRequestIgnore]` — always eager-loaded, never validated as input, never shown in Swagger request bodies. See [Identity](#identity). |
| `BaseEntityView`                                   | None — bare `IEntity`, no `Id`/`IsDeleted`/`CreatedAt` at all | Non-generic only. For entities mapped to a SQL view — you define every property yourself, including any identifier. |
| `BaseEntityIdentity` / `<TIdentity>`                | `Id` only, nothing else                | For advanced cases that don't want the built-in `IsDeleted`/`CreatedAt` — implement whichever capability interface (`IEntityCreatable`, `IEntityWritable`, etc.) yourself to restore the operations you need. |

```csharp
public class MyEntity : BaseEntity
{
    public virtual string Name { get; set; } = null!;
}
```

Using a non-`Guid` identity type? See [Solution Structure § Non-Guid identity](#solution-structure) — `TIdentity`
must be threaded consistently across every generic surface in the app, not just entities.

Spatial `Geometry` types (NetTopologySuite) are supported as regular properties, mapped provider-specifically
(see each [Data Provider](#data-providers)'s own spatial support).

### Data Mappings

Each entity gets a matching `IEntityTypeConfiguration<TEntity>`-style mapping class, auto-discovered and applied
— never registered manually, never call `ApplyConfiguration` yourself.

```csharp
public class MyEntityMapping : BaseEntityMapping<MyEntity>
{
    public override void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        base.Configure(builder);   // always first — wires soft-delete filter + CreatedAt/IsDeleted indexes

        builder.Property(x => x.Name).HasMaxLength(128).IsRequired();
        builder.HasIndex(x => x.Name);
        builder.HasOne(x => x.Parent).WithMany(x => x.Children).IsRequired();
    }
}
```

| Base class                          | Derives from                              | Adds                                                                 |
| --------------------------------------- | ------------------------------------------- | ---------------------------------------------------------------------------- |
| `BaseMapping<TEntity>`                    | `IEntityTypeConfiguration<TEntity>`            | Nothing — the root; only entities with **no** `Id` skip everything below it. |
| `BaseEntityIdentityMapping<TEntity,TIdentity>` | `BaseMapping<TEntity>`                    | `HasKey(Id)` + a value generator (`GuidValueGenerator` for `Guid`; a generic `ValueGenerator<TIdentity>` type otherwise — verify this actually works for your chosen non-`Guid` identity type before relying on it). |
| `BaseEntityMapping<TEntity>` / `<TEntity,TIdentity>` | `BaseEntityIdentityMapping<...>`     | Soft-delete query filter (`IsDeleted == 0`), `CreatedAt` (auto-generated on add, ignored after save, indexed), `IsDeleted` (default `0`, indexed). |
| `BaseEntityUserMapping<TEntity>` / `<TEntity,TIdentity>` | `BaseEntityIdentityMapping<...>` (not `BaseEntityMapping`) | Same `CreatedAt`/`IsDeleted` config as above, **plus** a query filter requiring `IdentityUser.IsActive`, **plus** a required 1:1 relationship to `IdentityUser` with cascade delete. |
| `BaseEntityViewMapping<TEntity>`           | `BaseMapping<TEntity>`                       | `ToView(typeof(TEntity).Name).HasNoKey()` — maps to a SQL view by entity type name, no key at all. |

⚠ Always call `base.Configure(builder)` **first** in an override — every behavior in the table above depends on
it running before your own configuration.

#### Auto-discovery mechanics

`BaseDbContext<TIdentity>.OnModelCreating` calls `modelBuilder.MapEntities<TIdentity>()`, which reflects the
entry assembly for every non-abstract, non-generic class deriving `BaseMapping<TEntity>` — **a raw
`IEntityTypeConfiguration<T>` not derived from `BaseMapping<T>` is not picked up.** For each one found, it
instantiates and calls `Configure`, then unconditionally applies two automatic index adjustments to every mapped
entity:

- **Every unique index gets renamed** to a canonical `UX_{table}_{col1}_{col2}...` database name, regardless of
  what name (if any) you gave it in `Configure`.
- **If the entity implements `IEntitySoftDeletable`**, every unique index that doesn't already include
  `IsDeleted` (and isn't just the primary key) is rebuilt to add `IsDeleted` as an extra composite column — this
  is the actual mechanism behind [Soft Delete](#soft-delete)'s "unique indexes are adjusted automatically" rule.

⚠ Mapping classes typed with a **different** identity type than the app's own `AddNanoData<...,TIdentity>`/
`BaseDbContext<TIdentity>` won't bind correctly — the discovery mechanism always plugs in the app-wide
`TIdentity`, not whatever the mapping class happens to declare (see [Solution Structure § Non-`Guid`
identity](#solution-structure)).

Conventionally placed in a `Data/Mappings/` folder in the application project (not a hard requirement).

### Migrations

Standard EF Core migrations — the only Nano-specific requirement is a [`BaseDbContextFactory<TProvider,
TContext>`](#data-context) subclass, since `dotnet ef` needs to construct the `DbContext` outside the normal
app host/DI.

```powershell
dotnet ef --version                                      # must match the EF Core version Nano itself uses
dotnet ef migrations add Initial --project {project}
```

#### Runtime behavior (`Data:StartupAction`)

| Value       | What happens                                                                 |
| ------------- | ----------------------------------------------------------------------------------- |
| `None` (default) | Nothing — the app doesn't touch schema at all at startup.                        |
| `Create`          | `Database.EnsureCreatedAsync()` — creates the schema directly from the current model, bypassing migrations entirely. |
| `Migrate`         | `Database.MigrateAsync()` — applies any pending migrations, retried up to 3 times (2s apart) on failure. |

This runs **synchronously, blocking app startup** — during `Build()` for API apps, during startup for Console
apps — before the app starts accepting requests or running workers.

⚠ **A migration that fails all 3 retries does not crash the app.** The exception is logged as a warning and
swallowed — the application starts up regardless, potentially against a database schema that doesn't match the
current model. This is silent; there's no health-check or readiness signal tied to migration success. If this
matters for your deployment, monitor logs for migration warnings rather than assuming a running app means a
successfully migrated database.

If [Data Identity](#identity) is configured, migration is immediately followed by seeding the 7 built-in roles
(`reader`, `writer`, `creator`, `editor`, `deleter`, `identity`, `administrator`) — only roles that don't already
exist are created.

⚠ Only enable `Create`/`Migrate` in `Development`. In `Staging`/`Production`, apply migrations during deployment
(`dotnet ef database update ...` in CI/CD), not at app startup.

⚠ EF Core migrations don't manage SQL views, stored procedures, or functions — those must be added or edited
into migration files by hand.

Conventionally placed in a `Migrations/` folder in the application project (standard `dotnet ef` behavior, not
Nano-specific).

### Repositories

`IRepository` is the data-access surface app code is expected to use — inject it rather than the `DbContext`.
Injecting the `DbContext` directly is a supported fallback for cases the repository genuinely doesn't cover, not
forbidden, but reach for `IRepository` first. Registered automatically by `AddNanoData<TProvider,TContext>()`; the concrete implementation is
`Repository<TContext,TIdentity>` (thin subclass of `BaseRepository<TContext,TIdentity>`), but nothing in app
code ever references either by name.

```csharp
public class MyEntitysController(ILogger<MyEntitysController> logger, IRepository repository)
    : BaseEntityController<MyEntity, MyEntityQueryCriteria>(logger, repository);
```

Every read pipeline builds the same way internally: `dbContext.Set<TEntity>().IncludeAnnotations(includeDepth).Where(criteria/predicate).Order(...).Limit(...)`.
`includeDepth` defaults to `Data:Repository:QueryIncludeDepth` and can be overridden per call — see [Include
Annotation](#include-annotation).

#### Method surface

Every method is `async`, ends with `CancellationToken cancellationToken = default`, and constrains `TEntity` to
the matching capability interface from [Data Models](#data-models):

| Group          | Representative signatures                                                                 | `TEntity` requires |
| ---------------- | --------------------------------------------------------------------------------------------- | ---------------------- |
| Get one            | `GetAsync<TEntity,TKey>(key, [includeDepth])` + `int`/`long`/`string`/`Guid`-keyed shorthand overloads | `IEntityIdentity<TKey>` |
| Get first          | `GetFirstAsync<TEntity,TCriteria>(IQuery<TCriteria>, [includeDepth])`, `GetFirstAsync<TEntity>(where, [ordering], [includeDepth])` | `IEntity` |
| Get many           | `GetManyAsync<TEntity,TKey>(keys, [includeDepth])`, `GetManyAsync<TEntity>(IQuery, [includeDepth])`, `GetManyAsync<TEntity,TCriteria>(IQuery<TCriteria>, [includeDepth])`, `GetManyAsync<TEntity>(where, [pagination], [ordering], [includeDepth])` | `IEntity` (or `IEntityIdentity<TKey>` for the by-keys overload) |
| Add                | `AddAsync` (save gated by `UseAutoSave`), `AddOrGetAsync`/`AddAndGetAsync` (**always** save + reload with includes), `AddManyAsync`, `AddManyBulkAsync` (⚠ needs paid EF+ Enterprise) | `IEntityCreatable` |
| Update             | `UpdateAsync`, `UpdateAndGetAsync` (reload with includes), `UpdateManyAsync(IEnumerable<TEntity>)`, `UpdateManyAsync<TEntity,TCriteria>(criteria, propertyUpdates)` (in-memory, loads then sets), `UpdateManyBulkAsync(criteria/predicate, propertyUpdates)` (native EF Core `ExecuteUpdateAsync`, no paid package needed) | `IEntityUpdatable` |
| AddOrUpdate        | `AddOrUpdateAsync`, `AddOrUpdateManyAsync` — tracked-entry check, then PK lookup, else add | `IEntityCreatableAndUpdatable` |
| Delete             | `DeleteAsync<TEntity,TKey>(id)` (+ shorthand overloads), `DeleteAsync(entity)`, `DeleteManyAsync` (by keys/entities/criteria/predicate), `DeleteManyBulkAsync` (criteria/predicate variants use native `ExecuteDeleteAsync`, no paid package) | `IEntityDeletable` |
| Aggregate          | `CountAsync`, `SumAsync`/`AverageAsync` (hard-coded `decimal`) | `IEntity` |
| Raw SQL            | `ExecuteProcedureAsync<T>`, `ExecuteProcedureListAsync<T>`, `ExecuteProcedureScalarAsync<T>` | — |
| Persistence        | `SaveChangesAsync(ct)` | — |

#### Transactions and `UseAutoSave`

`Data:Repository:UseAutoSave` (default `true`) governs whether each mutating call commits immediately. To batch
several repository calls into one transaction, set `UseAutoSave: false` and call `Repository.SaveChangesAsync()`
once yourself at the end — `IRepository` has no explicit `BeginTransaction`/`Commit` API of its own.

⚠ `AddAndGetAsync`/`UpdateAndGetAsync` **always** save regardless of `UseAutoSave` — they need the row persisted
before they can reload it with `[Include]`d navigations populated.

#### Gotchas

- **`AddManyBulkAsync` / `UpdateManyBulkAsync(IEnumerable<TEntity>)` / `DeleteManyBulkAsync(IEnumerable<TEntity>)`
  require the paid `Z.EntityFramework.Plus` Enterprise package.** The `*Bulk*(criteria)`/`*Bulk*(predicate)`
  overloads instead use EF Core's free, native `ExecuteUpdateAsync`/`ExecuteDeleteAsync` — no paid package
  needed for those.
- `DeleteAsync<TEntity,TKey>` only round-trips to load the entity first when it implements
  `IEntitySoftDeletable` (needed so [Soft Delete](#soft-delete)'s interceptor can convert the removal into an
  update); a hard-deletable entity is stubbed (`new TEntity { Id = id }`) and removed directly, with no prior
  load — any logic that depends on loaded state won't run for hard deletes by id.

Also provides a parallel identity-focused surface, `IIdentityRepository`/`IIdentityRepository<TIdentity>` — see
[Authentication](#authentication) and the identity-management endpoints in [Controllers](#controllers) for how
it's used (login, sign-up, password/email/phone management, roles, claims, API keys, refresh tokens).

#### Autosave

Controlled by `Data:Repository:UseAutoSave` (default `true`) — see [Repositories § Transactions and
`UseAutoSave`](#repositories) for the full behavior, including which reload methods (`AddAndGetAsync`,
`UpdateAndGetAsync`) always save regardless of this setting.

#### Cache

Not currently supported. Planned: in-memory + Redis-backed distributed caching.

#### Include Annotation

Nano's replacement for hand-written `.Include(...)` chains — mark a navigation property once on the entity,
and every `IRepository` read call eager-loads it automatically, recursively, up to a configurable depth.

```csharp
[AttributeUsage(AttributeTargets.Property)]
public class IncludeAttribute(QuerySplitBehavior querySplitBehavior = QuerySplitBehavior.SingleQuery) : Attribute
{
    public QuerySplitBehavior QuerySplitBehavior { get; set; } = querySplitBehavior;
}
```

```csharp
public class Customer : BaseEntity
{
    public virtual CustomerProfile Profile { get; set; } = null!;             // NOT [Include] — never eager-loaded or serialized

    [Include(QuerySplitBehavior.SplitQuery)]                                    // collection nav — split query avoids a cartesian-product join
    public virtual ICollection<Order> Orders { get; set; } = [];
}

public class Order : BaseEntity
{
    public virtual Customer Customer { get; set; } = null!;                    // intentionally NOT [Include] — avoids an include cycle back to Customer
    [Include] public virtual Payment? Payment { get; set; }                    // depth-2 from Customer: Customer.Orders.Payment
}
```

At query time, `IRepository` calls `IncludeAnnotations(maxDepth)`, which reflects over `[Include]`-tagged
properties and builds up a dotted EF Core `.Include("Orders.Payment")` string, recursing one level per
`[Include]` found and decrementing `maxDepth` each level; it stops once depth reaches `0`. `QuerySplitBehavior`
(`SingleQuery` default, or `SplitQuery`) is applied per-property via `.AsSplitQuery()` — mixing single- and
split-query navigations in the same graph is fine, each level uses its own setting.

`maxDepth` defaults to `Data:Repository:QueryIncludeDepth` (default `4`) but can be overridden per call via the
`includeDepth` parameter present on every `IRepository` read method and every entity controller action
(`?includeDepth=` query param) — see
[Controllers](#controllers).

⚠ **Depth is enforced a second time, independently, at serialization.** See [Response
Serialization](#response-serialization) — a navigation loaded via `[Include]` at query time still won't appear
in the JSON response unless the property itself is `[Include]`-tagged (it always is, since that's what caused it
to load) **and** the object-graph nesting level at serialize time is also within the configured depth. In a
normal tree the two line up, but they're two separate mechanisms computed differently (property-nesting depth vs.
serialized-object-nesting depth).

⚠ **No selective `$expand`.** Callers can only dial the recursion *depth* via `includeDepth` — they cannot pick
*which* navigations to expand. If a property isn't `[Include]`-tagged by the entity author, no `includeDepth`
value will ever surface it.

⚠ **Avoid include cycles.** Nano does not detect reference cycles between `[Include]` properties — attribute
only one direction of a bidirectional relationship (as `Order.Customer` above is deliberately left un-annotated
even though `Customer.Orders` is), or a query/serialize can recurse indefinitely.

⚠ **Owned types**: if a navigation *inside* an owned type also needs including, the parent property referencing
the owned type must itself carry `[Include]` too — Nano won't traverse into an owned type it wasn't told to
include in the first place.

⚠ Be cautious annotating large collection navigations — `[Include]` on a big collection can pull back a lot of
data; prefer `SplitQuery` for these, and keep an eye on whether the collection really needs to be part of the
default eager-load graph at all.

### Audit

Built on the `Z.EntityFramework.Plus` (EF+) `Audit` feature — `BaseDbContext`'s `SaveChanges*` overrides always
run the audit pipeline, but by default **every entity is excluded**; only entities implementing
`IEntityAuditable` are actually captured. Exclude individual properties (sensitive/technical fields) with
`[AuditExclude]` — an EF+ attribute, not a Nano one.

```csharp
public class MyEntity : BaseEntity, IEntityAuditable
{
    public virtual string Name { get; set; } = null!;

    [AuditExclude]
    public virtual string InternalNotes { get; set; } = null!;
}
```

Two built-in entities store the log: `AuditEntry<TIdentity>` (one row per changed entity — `CreatedBy`,
`EntityKey`, `EntityTypeName`, `EntityState`, `RequestId`, and a `Properties` collection) and
`AuditEntryProperty<TIdentity>` (one row per changed property — `PropertyName`, `OldValue`/`NewValue` as
strings). Both are always mapped and their tables always created, even if no entity opts in, so audit can be
enabled later without a schema migration.

`EntityState` is one of `Added`, `Deleted`, `Modified`, `SoftAdded`, `SoftDeleted`, `RelationshipAdded`,
`RelationshipDeleted`, `Current` — note **soft deletes are captured as `SoftDeleted`, distinct from a real
`Deleted`**, so the log correctly shows what actually happened at the database level (see [Soft
Delete](#soft-delete)).

`CreatedBy` is the current JWT user id if an authenticated `HttpContext` is available, otherwise the literal
string `"Anonymous"`. `RequestId` is `HttpContext.TraceIdentifier` — which Nano's own request-id middleware
keeps in sync with the `X-Request-Id` header (see [Request Tracing](#request-tracing)), so it's the same value
you'd see in logs and response headers for that request.

Only entities with `Data:Identity:UseAudit` include the built-in identity models (`User`, roles, claims, API
keys, etc.) — see [Identity](#identity) for the exact allowed values.

Read the log back through `BaseAuditController`/`BaseAuditController<TIdentity>` (see
[Controllers](#controllers)) or directly via `IRepository` against `AuditEntry<TIdentity>` like any other
entity.

⚠ Audit capture is what causes [Data Context](#data-context)'s `SaveChanges` to sometimes issue a second
database round-trip in one call — see the note there.

### Soft Delete

Implement `IEntitySoftDeletable` to switch an entity from hard-delete to soft-delete — no other registration
needed, it's wired unconditionally for every app using [Nano.Data](#nanodata):

```csharp
public class MyEntity : BaseEntity, IEntitySoftDeletable
{
    public virtual string Name { get; set; } = null!;
}
```

Deleting such an entity through `IRepository`/`DbContext.Remove(...)` doesn't issue a SQL `DELETE`. An EF Core
`SaveChangesInterceptor` runs just before `SaveChanges` executes: it finds every tracked entry implementing
`IEntitySoftDeletable` in the `Deleted` state, flips it to `Modified`, and sets `IsDeleted` to the current Unix
epoch time in **milliseconds**. The actual SQL is an `UPDATE`, not a `DELETE`.

`BaseEntity`/`BaseEntityReadOnly` (and everything deriving from them) already has the `IsDeleted` property —
implementing `IEntitySoftDeletable` is what makes it *mean* something; without it, `IsDeleted` is just an inert
column that always stays `0`.

Soft-deleted rows are automatically excluded from queries via the mapping's query filter (`IsDeleted == 0`,
applied by `BaseEntityMapping<TEntity>` — see [Data Mappings](#data-mappings)) — you don't need to add
`.Where(x => x.IsDeleted == 0)` yourself anywhere.

⚠ Soft-deleting does **not** cascade — related entities aren't automatically soft-deleted alongside their
parent, unlike a real cascading SQL delete.

⚠ Unique indexes need care with soft-delete: since a soft-deleted row still physically exists, two "deleted"
rows with the same value could otherwise violate a unique constraint. Nano handles this automatically at mapping
time by rewriting every unique index (except the primary key) on a soft-deletable entity to also include
`IsDeleted` as a composite column — see [Data Mappings § Auto-discovery mechanics](#data-mappings).

See [Audit](#audit) for how a soft delete is distinguished from a real delete in the audit log
(`AuditState.SoftDeleted` vs `AuditState.Deleted`).

### Lazy Loading

`Data:UseLazyLoading: true` turns on EF Core's `UseLazyLoadingProxies()` — every `virtual` navigation property
(which is all of them, by Nano convention) becomes lazily loadable, not just ones tagged `[Include]`.

⚠ Discouraged, and independent from [Include Annotation](#include-annotation) — the two solve different
problems and don't need each other. In practice this is mainly useful as a safety net: if a response needs an
`[Include]`-tagged navigation that wasn't eager-loaded (e.g. `includeDepth` was too low), accessing that
property during serialization triggers a lazy-load query so it still comes back populated, instead of
serializing as `null`/empty. The cost is an extra, synchronous query per access — the classic N+1 problem if it
happens across a collection. Prefer getting `[Include]`/`includeDepth` right at query time over relying on this.

Lazy loading applies to **every** virtual navigation, not just `[Include]`-tagged ones — but [Response
Serialization](#response-serialization) still only ever serializes `[Include]`-tagged properties, so lazy-loading
a non-`[Include]` navigation (e.g. from custom server-side code) works fine, it just never appears in an API
response.

### Triggers

Code-level hooks around save operations — not SQL triggers. Built on the external `EntityFrameworkCore.Triggers`
package; register them inside a [Data Mapping](#data-mappings)'s `Configure(builder)`, not in the entity itself.

| Trigger         | Timing  | `TEntity` requires  |
| ------------------- | ------- | ------------------------ |
| `OnInserting`         | Before  | `IEntityCreatable`         |
| `OnInserted`          | After   | `IEntityCreatable`         |
| `OnInsertFailed`      | On error| `IEntityCreatable`         |
| `OnUpdating`          | Before  | `IEntityUpdatable`         |
| `OnUpdated`           | After   | `IEntityUpdatable`         |
| `OnUpdateFailed`      | On error| `IEntityUpdatable`         |
| `OnDeleting`          | Before  | `IEntityDeletable`         |
| `OnDeleted`           | After   | `IEntityDeletable`         |
| `OnDeleteFailed`      | On error| `IEntityDeletable`         |

Define the action as a **static field or method**, not an inline lambda, and reference it from the mapping:

```csharp
internal static class MyEntityTriggers
{
    internal static Action<IInsertingEntry<MyEntity>> Inserting = x =>
    {
        x.Entity.UpdatedAt = DateTimeOffset.UtcNow;   // x.Entity — the entity instance
        x.Context.Add(new MyAuditRow { EntityId = x.Entity.Id });  // x.Context — the current DbContext
    };
}

public class MyEntityMapping : BaseEntityMapping<MyEntity>
{
    public override void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        base.Configure(builder);
        builder.OnInserting(MyEntityTriggers.Inserting);
    }
}
```

⚠ **Use a static field/method, not `x => { ... }` written inline in `Configure`.** Registration is deduplicated
by delegate equality against a process-wide, per-entity-type registry (`Triggers<TEntity>` is a **static** event,
shared across every `DbContext` instance in the app, not scoped per instance) — an inline lambda is a new
delegate reference every time `Configure` runs, defeating the dedup if the model is ever built more than once
and silently double-registering the trigger. A static field/method is always the same delegate reference, so
`AddOnce` correctly registers it exactly once.

`entry.Entity` gives the entity instance, `entry.Context` gives the current `DbContext` — from either you can add
other rows, resolve services via the context's service provider, etc. Delete triggers fire correctly for [Soft
Delete](#soft-delete)d entities too (`OnDeleting`/`OnDeleted` still run — the underlying operation just becomes
an `UPDATE`, not a `DELETE`).

⚠ Don't call `SaveChanges` yourself inside a trigger — Nano handles persisting whatever the trigger adds/changes
automatically. Avoid modifying the entity that caused the trigger from an *after*-save trigger
(`OnInserted`/`OnUpdated`/`OnDeleted`) — that can cause duplicate update invocations; do that kind of work in
`OnInserting`/`OnUpdating` instead.

Keep triggers small and self-contained (e.g. stamping `UpdatedAt`, writing a companion audit-style row). Put
more complex logic in a dedicated service instead.

### Entity Events

Declarative cross-service data replication: mark an entity `[Publish]` in the owning app, mark a matching
flattened DTO `[Subscribe]` in a consuming app, and Nano keeps the two in sync automatically whenever the source
entity is created, updated, or deleted — no manual event handler to write. Requires [Eventing](#nanoeventing) to
be configured in **both** apps (silently does nothing otherwise — see Gotchas).

#### Publishing

```csharp
[Publish(nameof(Name), nameof(ProfileId), $"{nameof(Profile)}.{nameof(Profile.AddressId)}")]
public class MyEntity : BaseEntity
{
    public virtual string Name { get; set; } = null!;
    public virtual Guid? ProfileId { get; set; }

    [Include]
    public virtual MyProfile? Profile { get; set; }
}
```

- Applied to a class; lists the **property paths** to publish, as `nameof`-built strings (plain names, or dotted
  navigation chains like `Profile.AddressId`).
- If no paths given, only `Id` and `CreatedAt` are published.
- **Inherited across the type hierarchy** — a derived class inherits its base class's publish paths too. The
  *most specific* type carrying `[Publish]` in the hierarchy determines the published `TypeName`.
- Path rules: the final segment must be a scalar EF property; intermediate segments must be reference/owned
  navigations — **collection navigations can't appear in a path** (publish the child entity separately instead
  if you need that data). The final (leaf) property name must be **unique across all paths** on the entity — you
  can't publish both `BillingAddress.Street` and `DeliveryAddress.Street` on the same entity, since the
  subscriber's flattened shape uses only the leaf name.

#### Subscribing

```csharp
[Subscribe]
public class MyEntity : BaseEntity
{
    public virtual string Name { get; set; } = null!;      // from MyEntity.Name
    public virtual Guid ProfileId { get; set; }             // from MyEntity.ProfileId
    public virtual Guid? AddressId { get; set; }            // from Profile.AddressId
}
```

- Class name must match the publisher's published `TypeName` (simple class name, no namespace — typically a
  different namespace in a different solution/app).
- Property names match the **leaf** segment of each publish path — a flattened, denormalized shape, not a
  structural copy of the source entity.
- No handler to write — the built-in `EntityEventingHandler` finds the matching local model by `TypeName` and
  creates/updates/deletes it automatically.

#### What happens on save

1. On `SaveChanges`, an interceptor hydrates the changed entity's publish-path navigations (loading anything not
   already in memory) and diffs which watched properties actually changed.
2. If a *dependent* entity changed (e.g. `Profile.AddressId`, not `MyEntity` itself), Nano walks the relationship
   graph in reverse to find the owning publish-root (`MyEntity`) and treats that as the changed entity — so
   editing a nested navigation still republishes the parent correctly.
3. After the save commits, one `EntityEvent` per affected root is published — `Added`/`Modified`/`Deleted`, with
   `Data` populated from the (possibly reloaded, to pick up DB-generated values) current values.
4. The subscriber's `EntityEventingHandler` matches `TypeName`, then: `Added`/`Modified` → find-by-id-or-create,
   apply `Data`, save; `Deleted` → find-by-id and remove (no-op if already absent).

#### Wire format

```csharp
public sealed class EntityEvent(object id, string typeName, string state)
{
    public object Id { get; init; }
    public string TypeName { get; set; }              // simple class name — the routing discriminator
    public string State { get; set; }                  // "Added" | "Modified" | "Deleted"
    public Dictionary<string, object?> Data { get; set; } = new();
}
```

Every entity event, for every `[Publish]`/`[Subscribe]` pair in every app, travels as this one generic envelope
type — there's no per-entity message contract.

#### Gotchas

- **Silent no-op without Eventing configured.** If `[Publish]`/`[Subscribe]` are present but
  `AddNanoEventing<TProvider>()` was never called, the SaveChanges interceptor never attaches — no error, no
  event, ever. Easy to miss in local testing.
- With RabbitMQ specifically: all entity events (across every entity type, every app) flow through **one shared
  fanout exchange**, differentiated only by which **queue** a subscriber binds (named after the entity
  `TypeName`) — the exchange itself doesn't filter by routing key. A subscriber whose local model doesn't
  recognize an incoming `TypeName` throws inside the handler rather than silently ignoring it.
- Deletes carry no `Data` — only `Id`/`TypeName`/`State`.
- `Modified` events where nothing in the publish paths actually changed are suppressed — no event, no bus
  traffic, for unrelated field updates.
- Only entities implementing `IEntityIdentity<TIdentity>` with `TIdentity` of `int`, `long`, `Guid`, or `string`
  can participate.

Also works for identity-backed models — e.g. publishing `IdentityUser.Email`/`IdentityUser.Phone` off a
`BaseEntityUser` lets a downstream service (email, SMS) receive everything it needs upfront, without an extra
lookup call back to the source service.

---

## Nano.Eventing

Publish/subscribe messaging between applications. Transitive — reference a concrete provider package.

### Registration

```csharp
.ConfigureServices(services =>
{
    services.AddNanoEventing<TProvider>();
})
```

### Configuration

| Setting                | Type     | Default  | Description                                                                     |
| --------------------------- | -------- | -------- | -------------------------------------------------------------------------------------- |
| `Host`                         | string   | null     | Required. Broker hostname/IP.                                                           |
| `VHost`                        | string   | /        | Virtual host/namespace on the broker.                                                   |
| `Port`                         | ushort   | 5672     | Broker port.                                                                             |
| `Timeout`                      | TimeSpan | 00:00:30 | Connection timeout.                                                                      |
| `UseSsl`                       | bool     | false    | Use SSL/TLS.                                                                              |
| `Heartbeat`                    | ushort   | 60       | Heartbeat interval in seconds; `0` disables it.                                          |
| `PrefetchCount`                | ushort   | 50       | Messages fetched at once for processing.                                                 |
| `Credentials.Id`               | string   | null     | Required (if authenticating). Username.                                                  |
| `Credentials.Secret`           | string   | null     | Required (if authenticating). Password.                                                  |
| `HealthCheck`                  | object   | null     | See [Health Checks](#health-checks-2). API/Web apps only.                                |

```json
"Eventing": {
  "Host": null,
  "VHost": null,
  "Port": 5672,
  "Timeout": 30,
  "UseSsl": false,
  "Heartbeat": 60,
  "PrefetchCount": 50,
  "Credentials": { "Id": null, "Secret": null },
  "HealthCheck": null
}
```

#### Health Checks

| Setting                        | Type | Default   | Description                                                          |
| ----------------------------------- | ---- | --------- | -------------------------------------------------------------------------- |
| `HealthCheck.UnhealthyStatus`          | enum | Unhealthy | Status reported when the broker is unavailable. API/Web apps only.          |

```json
"Eventing": {
  "HealthCheck": { "UnhealthyStatus": "Unhealthy" }
}
```

### Serialization

Newtonsoft.Json, case-insensitive. Supports Nano base types and derived types, plus NetTopologySuite `Geometry`
types. Keep event contracts small and simple — eventing is not designed for large payloads.

### Eventing Providers

All eventing providers implement `IEventingProvider`.

| Provider   | Package                    | Registration                                       |
| ------------ | ----------------------------- | ------------------------------------------------------- |
| RabbitMq       | `Nano.Eventing.RabbitMq`         | `services.AddNanoEventing<RabbitMqProvider>();`            |

```json
"Eventing": {
  "Host": null,
  "VHost": null,
  "Port": 5672,
  "Timeout": 30,
  "UseSsl": false,
  "Heartbeat": 60,
  "PrefetchCount": 50,
  "Credentials": { "Id": null, "Secret": null },
  "HealthCheck": { "UnhealthyStatus": "Unhealthy" }
}
```

#### Custom eventing provider

Implement `IEventingProvider`'s single static member:

```csharp
public sealed class MyEventingProvider : IEventingProvider
{
    public static void Configure(IServiceCollection services, EventingOptions options)
    {
        services.AddSingleton<IEventing, MyEventing>();   // your IEventing implementation for this broker
        // optionally register a health check, serialization config, etc. here too
    }
}
```

Then use it exactly like a built-in provider: `services.AddNanoEventing<MyEventingProvider>();`.

⚠ Unlike [Storage](#nanostorage), there's no generic fallback `IEventing` registered anywhere else — your
provider's `Configure` **must** register the `IEventing` implementation itself, or nothing will be able to
publish or subscribe.

### Publish and Subscribe

The general-purpose eventing mechanism — for arbitrary messages, independent of EF Core change tracking (contrast
with [Entity Events](#entity-events), which is specifically tied to entity saves).

Define a plain message contract — no base type required:

```csharp
public class MyEvent
{
    public string Text { get; set; } = null!;
}
```

Publish from anywhere with `IEventing` injected:

```csharp
public class MyController(ILogger<MyController> logger, IEventing eventing) : BaseController(logger)
{
    public virtual async Task<IActionResult> MyActionAsync(CancellationToken cancellationToken = default)
    {
        await eventing.PublishAsync(new MyEvent { Text = "..." }, cancellationToken: cancellationToken);
        return this.Ok();
    }
}
```

Consume by deriving from `BaseEventHandler<TEvent>` — no registration needed, discovered and subscribed
automatically at startup:

```csharp
public class MyEventHandler : BaseEventHandler<MyEvent>
{
    public override async Task CallbackAsync(MyEvent @event, bool isRedelivered, CancellationToken cancellationToken = default)
    {
        // handle @event; isRedelivered is true if the broker is retrying a previously-failed delivery
    }
}
```

Optionally scope a handler to a specific routing key, and/or override the globally-configured prefetch count, by
hiding the base interface's static members on your handler class:

```csharp
public class MyEventHandler : BaseEventHandler<MyEvent>
{
    public static string RoutingKey => "my-routing-key";
    public static ushort OverridePrefetchCount => 10;

    public override async Task CallbackAsync(MyEvent @event, bool isRedelivered, CancellationToken cancellationToken = default) { /* ... */ }
}
```

Pass the matching `routing` argument to `PublishAsync` to target that handler specifically:

```csharp
await eventing.PublishAsync(new MyEvent { Text = "..." }, "my-routing-key", cancellationToken);
```

Most handlers need neither — `RoutingKey` is for the uncommon case where the same message contract has multiple,
selectively-targeted consumers; `OverridePrefetchCount` is for a handler whose processing is heavier than most
and shouldn't be handed as many concurrent messages as the app-wide default.

⚠ `BaseEventHandler<TEvent>` implementations **must be non-generic** — a handler class that is itself an open
generic type is silently skipped during discovery.

With RabbitMQ, the exchange name is derived from `TEvent`'s type name (a fanout exchange — every bound queue
receives every message published to it, regardless of routing key), and the queue name is derived from the
consuming app + `TEvent` + routing key. Share the message contract as a referenced project or NuGet between
publisher and subscriber to keep the shape in sync — there's no schema negotiation.

---

## Nano.Storage

Provider-agnostic file storage layer. Transitive — reference a concrete provider package.

### Registration

```csharp
.ConfigureServices(services =>
{
    services.AddNanoStorage<TProvider>();
})
```

Registering a provider also registers `IPathProvider`, injectable anywhere, exposing the storage root and a
temporary (`tmp`) directory.

### Configuration

| Setting        | Type   | Default | Description                                          |
| ---------------- | ------ | ------- | ---------------------------------------------------------- |
| `ShareName`         | string | null    | Logical container/share/bucket name.                        |
| `Credentials`       | object | null    | Optional. Provider-specific account/credentials.            |
| `HealthCheck`       | object | null    | See [Health Checks](#health-checks-3). API/Web apps only.    |

```json
"Storage": {
  "ShareName": null,
  "HealthCheck": null
}
```

#### Health Checks

| Setting              | Type   | Default   | Description                                                                          |
| ------------------------ | ------ | --------- | ------------------------------------------------------------------------------------------ |
| `HealthCheck.AccountName`    | string | null      | Storage account name, used by some providers' health checks.                               |
| `HealthCheck.UnhealthyStatus`| enum   | Unhealthy | Status reported when storage is unavailable. API/Web apps only.                            |

```json
"Storage": {
  "HealthCheck": { "AccountName": null, "UnhealthyStatus": "Unhealthy" }
}
```

⚠ Health checks must also be enabled at the `App:HealthCheck` level for storage health checks to take effect.

### Storage Providers

All storage providers implement `IStorageProvider`.

| Provider    | Package               | Registration                                            |
| ------------- | ------------------------ | -------------------------------------------------------------- |
| Local           | `Nano.Storage.Local`        | `services.AddNanoStorage<LocalFileShareProvider>();`              |
| Azure           | `Nano.Storage.Azure`        | `services.AddNanoStorage<AzureFileshareProvider>();`              |

```json
"Storage": {
  "ShareName": null,
  "HealthCheck": { "UnhealthyStatus": "Unhealthy" }
}
```

#### Custom storage provider

Implement `IStorageProvider`'s single static member:

```csharp
public sealed class MyStorageProvider : IStorageProvider
{
    public static void Configure(IServiceCollection services, StorageOptions options)
    {
        if (options.HealthCheck == null)
        {
            return;
        }

        services.AddHealthChecks().AddDefaultStorageHealthCheck(options.HealthCheck.UnhealthyStatus.GetHealthStatus());
    }
}
```

Then use it exactly like a built-in provider: `services.AddNanoStorage<MyStorageProvider>();`.

Unlike Eventing, `IPathProvider` is registered generically by `AddNanoStorage<TProvider>()` itself — **the same
implementation regardless of which provider you choose** — so both built-in providers (`Local`, `Azure`) only
use `Configure` to register a health check; there's no per-provider client/SDK to wire up, because both
represent storage already mounted into the container's filesystem and accessed identically through
`IPathProvider`. A genuinely different storage backend (e.g. calling a blob/object storage API directly instead
of a mounted filesystem path) would need its own `IPathProvider` implementation too, registered from `Configure`
to override the default.
