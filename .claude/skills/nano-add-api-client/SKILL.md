---
name: nano-add-api-client
description: Wire an existing Nano Api Client into this application - adds the App:Apis configuration entry and injects the client into a controller/worker so it can call another Nano service. Use when the user asks to call another Nano service/API from this app, add an API client to a gateway, or compose internal services together in a Nano API, Web, or Console application.
---

# Nano add API client

Wires an *already-defined* Api Client — a `BaseApiClient` subclass living in the target
service's `{Name}.Models` project — into this consuming application: the `App:Apis` config entry
and the injection site that actually makes it callable. Read AGENTS.md's `### Api Clients`
section first — it documents the built-in method groups (`.Entity`/`.Auth`/`.Audit`/`.Identity`)
and authentication forwarding in full; this skill does not repeat that, only how to consume a
client from this app.

If the client class doesn't exist yet, don't treat that as a choice to offer — treat it as a sign
something may be wrong. Either the user is pointed at the wrong application (the client is
expected to already exist, defined on the *owning* service's side — check this isn't simply the
wrong project before going further), or the owning service genuinely hasn't defined it yet, in
which case that's `nano-define-api-client`'s job, in that other application's own project, not
this skill's. Either way: stop, warn the user plainly that the client doesn't exist where expected,
and ask which is true — don't invoke the other skill automatically, and don't proceed on the
assumption a missing class is just an item to create in passing.

**No registration call.** Every `BaseApiClient` subclass in the entry assembly whose class name
matches a key under `App:Apis` is auto-wired — but per AGENTS.md's own gotcha, a client that's
never actually injected anywhere doesn't get registered at all. Add the config, then make sure
something actually consumes it (a controller or worker constructor parameter), or none of this
takes effect.

## Before making any change, determine

1. **Does the client class already exist?** Check the target service's `{TargetName}.Models/Api/`
   project (or its published NuGet) for the `BaseApiClient`/`BaseIdentityApiClient` subclass the
   user means. If it doesn't exist yet, stop — don't create it inline, and don't invoke
   `nano-define-api-client` automatically. Warn the user explicitly that the client isn't defined
   where expected, and ask two things: is this actually the right target/application to be
   wiring into right now, and if so, did they mean to define the client first (on the owning
   service's own project, a separate task from this one)? Let them answer both before doing
   anything else.
2. **How does this app reference the target's `.Models` project?** Check how any other Api
   Client in this project already references its target (`ProjectReference` for a
   same-solution/monorepo target, or a NuGet reference for a separate-repo target) and match that
   convention — AGENTS.md explicitly allows either here. If this is the first Api Client in the
   project, ask which applies.
3. **Is a client with this class name already registered?** Check for an existing `App:Apis` key
   matching the class name — if one's already there pointing at a different host/target, confirm
   with the user before overwriting it.
4. **Console app?** Per AGENTS.md's `#### Authentication forwarding`: Console workers have no
   inbound `HttpContext`, so they can't transparently forward a caller's JWT — a Console-hosted
   client typically only calls `[AllowAnonymous]` endpoints on the target, or needs `LogInRoot`
   configured if it must call authenticated ones. Ask which applies before adding `LogInRoot`.

## appsettings.json (this app)

Add the `App:Apis:{ClientClassName}` section to the base `appsettings.json` — the dictionary key
must be the exact class name from step 1:

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
  service name locally, if calling another app in the same compose network) — not sensitive,
  stays in the base file.
- Include `HealthCheck: { "UnhealthyStatus": "Unhealthy" }` only if this app's own
  `App:HealthCheck` is enabled (API/Web apps only) — same dead-config rule as every other
  provider's health check.
- **`LogInRoot`** (`Username`/`Password`), if step 4 needs it: **this grants the caller a real,
  full `administrator` identity** on the target — not a lesser scope, the same as any human root
  login. That's exactly why it's worth using instead of just making the target endpoint
  anonymous: an anonymous endpoint has no identity or audit trail at all, while a `LogInRoot`
  call still flows through the target's normal authorization *and* shows up in its audit log as
  root having acted — the right choice whenever the target also serves real authenticated
  end-users, or attribution of machine-to-machine calls matters. But because it's full admin
  access, the credential needs the same secret-handling rigor as anything else that powerful —
  don't hardcode it in the base `appsettings.json`; set the real value only in
  `appsettings.Development.json` locally, and source it from a Kubernetes secret + GitHub secret
  in Staging/Production, the same pattern used for SQL passwords and JWT private keys elsewhere.
  **Don't create a new secret for this app** — `LogInRoot` only works if its credentials match
  the target's own `Jwt.RootLogin`, so this app must reference the *same* secret the target
  creates (conventionally `auth-root-login-secret`, keys `root-login-username`/
  `root-login-password`), mapped into `App__Apis__{ClientName}__LogInRoot__Username`/`Password`
  in `deployment.yaml` — never re-create or duplicate it with a new name. Don't confuse this with
  `Jwt.RootLogin` — see AGENTS.md's explicit warning distinguishing the two; they're on different
  apps, in different directions.

## Injecting the client

Inject the client class directly into whatever consumes it — a controller or worker constructor
parameter:

```csharp
public class MyController(ILogger<MyController> logger, MyApi myApi) : BaseController(logger)
{
    // ...
}
```

This is the step that actually makes the `App:Apis` entry take effect — without it, per the
gotcha above, nothing gets registered even though the config exists.

## After making the change

- Show the user every file touched in *this* app — the `appsettings.json` addition and the
  injection site. Note that the client class itself lives in the target service's `.Models`
  project, not here.
- Confirm the client is actually injected somewhere — if the request was just "add the client" with
  no specified consumer yet, say explicitly that nothing is wired up until it's referenced.
- If `LogInRoot` was added, restate the Staging/Production secret-handling requirement — don't let
  a real credential sit in the base file.
