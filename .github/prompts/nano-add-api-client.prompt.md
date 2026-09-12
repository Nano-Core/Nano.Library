---
mode: agent
description: Wire a Nano Api Client into an application - defines a BaseApiClient subclass for calling another Nano application over HTTP, and its App:Apis configuration entry. Use when the user asks to call another Nano service/API, add an API client, or compose a gateway from internal services in a Nano API, Web, or Console application.
---

# Nano add API client

Wires a typed HTTP client for calling another Nano application into an existing Nano API, Web, or
Console application. Read `AGENTS.md`'s `### Api Clients` section first - it documents the built-in
method groups (`.Entity`/`.Auth`/`.Audit`/`.Identity`), the custom-request attribute shapes, and
authentication forwarding in full; this skill does not repeat that, only how to wire a new client
into this specific app.

**No registration call.** Every `BaseApiClient` subclass in the entry assembly whose class name
matches a key under `App:Apis` is auto-wired - but per AGENTS.md's own gotcha, a client that's
never actually injected anywhere doesn't get registered at all. Define the class, add the config,
then make sure something actually consumes it (a controller or worker constructor parameter) or
none of this takes effect.

## Before making any change, determine

1. **Which target application**, and where its `{TargetName}.Models` project (or published NuGet)
   lives - that's where the client class and any custom request types belong (AGENTS.md: "the
   client class and its custom request types live in the *owning* service's `{name}.Models/Api/`
   project"). If the target is in the same solution, check how any other Api Client in this
   project already references its target's `.Models` project (`ProjectReference` for a same-
   solution/monorepo target, or a NuGet reference for a separate-repo target) and match that
   convention - AGENTS.md explicitly allows either for this case, unlike Nano.Library itself.
2. **Does the target have persistent Identity?** Determines the base class: `BaseApiClient`/
   `BaseApiClient<TIdentity>` (no Identity, or identity type doesn't matter to this client), or
   `BaseIdentityApiClient<TUser[,TIdentity]>` (target has Identity - unlocks the `.Identity`
   method group). Check the target's own `Data:Identity` config / `BaseEntityUser`-derived entity
   if you have access to its source; ask the user if not.
3. **Is a client with this class name already registered?** Check for an existing class matching
   the intended name (the `App:Apis` key must exactly match it - AGENTS.md: "the only link between
   config and DI"). Pick a name that doesn't collide.
4. **Console app?** Per AGENTS.md's `#### Authentication forwarding`: Console workers have no
   inbound `HttpContext`, so they can't transparently forward a caller's JWT - a Console-hosted
   client typically only calls `[AllowAnonymous]` endpoints on the target, or needs `LogInRoot`
   configured if it must call authenticated ones. Ask which applies before adding `LogInRoot`.
5. **Custom endpoints needed beyond `.Entity`/`.Auth`/`.Audit`/`.Identity`?** If so, this also
   means defining request types (see below) - confirm which endpoints on the target before
   guessing at routes.

## Client class

`{TargetName}.Models/Api/{ClientName}.cs` (owning service's Models project, not this consuming
app):

```csharp
// Bare pass-through
public class MyApi(ApiClient apiClient) : BaseApiClient(apiClient);
```
```csharp
// Identity-backed target
public class MyApi(ApiClient apiClient) : BaseIdentityApiClient<MyUser>(apiClient);
```

Add custom methods only if step 5 applies - one method per custom request, calling
`this.InvokeAsync(request, cancellationToken)` (no response) or
`this.InvokeAsync<TRequest, TResponse>(request, cancellationToken)` (typed response).

## Custom requests (only if step 5 applies)

`{TargetName}.Models/Api/Requests/{Name}Request.cs`, one action attribute
(`[GetAction]`/`[PostAction]`/etc.) naming the HTTP verb + relative route, per AGENTS.md's four
request shapes. Set `this.Controller` explicitly in the constructor unless the route naturally
matches the pluralized response type. **Define the route segment as a constant** in a `Consts`
class inside `{TargetName}.Models` and reference it from both this request's action attribute and
the target controller's `[Route(...)]` - per AGENTS.md, nothing else keeps the two sides in sync,
and Nano's own built-in requests avoid drift exactly this way.

## appsettings.json (consuming app)

Add the `App:Apis:{ClientClassName}` section to the base `appsettings.json` - the dictionary key
must be the exact class name from above:

```json
"App": {
  "Apis": {
    "MyApi": {
      "Host": "my-service",
      "Root": "api",
      "Port": 8080,
      "UseSsl": false,
      "Timeout": "00:00:30"
    }
  }
}
```

- `Host` matches the target's Kubernetes service name in Staging/Production (or the docker-compose
  service name locally, if calling another app in the same compose network) - not sensitive,
  stays in the base file.
- Include `HealthCheck: { "UnhealthyStatus": "Unhealthy" }` only if this app's own
  `App:HealthCheck` is enabled (API/Web apps only) - same dead-config rule as every other
  provider's health check.
- **`LogInRoot`** (`Username`/`Password`), if step 4 needs it: **this grants the caller a real,
  full `administrator` identity** on the target - not a lesser scope, the same as any human root
  login. That's exactly why it's worth using instead of just making the target endpoint
  anonymous: an anonymous endpoint has no identity or audit trail at all, while a `LogInRoot`
  call still flows through the target's normal authorization *and* shows up in its audit log as
  root having acted - the right choice whenever the target also serves real authenticated
  end-users, or attribution of machine-to-machine calls matters. But because it's full admin
  access, the credential needs the same secret-handling rigor as anything else that powerful -
  don't hardcode it in the base `appsettings.json`; set the real value only in
  `appsettings.Development.json` locally, and source it from a Kubernetes secret + GitHub secret
  in Staging/Production, the same pattern used for SQL passwords and JWT private keys elsewhere.
  **Don't create a new secret for this app** - `LogInRoot` only works if its credentials match
  the target's own `Jwt.RootLogin`, so this app must reference the *same* secret the target
  creates (conventionally `auth-root-login-secret`, keys `root-login-username`/
  `root-login-password`), mapped into `App__Apis__{ClientName}__LogInRoot__Username`/`Password`
  in `deployment.yaml` - never re-create or duplicate it with a new name. Don't confuse this with
  `Jwt.RootLogin` - see AGENTS.md's explicit warning distinguishing the two; they're on different
  apps, in different
  directions.

## Injecting the client

Inject the client class directly into whatever consumes it - a controller or worker constructor
parameter:

```csharp
public class MyController(ILogger<MyController> logger, MyApi myApi) : BaseController(logger)
{
    // ...
}
```

This is the step that actually makes the `App:Apis` entry take effect - without it, per the
gotcha above, nothing gets registered even though the config exists.

## After making the change

- Show the user every file touched, noting which project each lives in (owning service's
  `.Models` vs. this consuming app).
- Confirm the client is actually injected somewhere - if the request was just "add the client" with
  no specified consumer yet, say explicitly that nothing is wired up until it's referenced.
- If `LogInRoot` was added, restate the Staging/Production secret-handling requirement - don't let
  a real credential sit in the base file.
