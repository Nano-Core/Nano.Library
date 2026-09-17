---
mode: agent
description: Wire an existing Nano Api Client into this application - adds the App:Apis configuration entry, injects the client into a controller/worker, and nests the target service into this app's local docker-compose (with its own incremental publish step) so it's actually runnable end-to-end. Use when the user asks to call another Nano service/API from this app, add an API client to a Public API, or compose internal services together in a Nano API, Web, or Console application.
---

# Nano add API client configuration

Wires an *already-defined* Api Client - a `BaseApiClient` subclass living in the target
service's `{Name}.Models` project - into this consuming application: the `App:Apis` config entry
and the injection site that actually makes it callable. Read AGENTS.md's `### Api Clients`
section first - it documents the built-in method groups (`.Entity`/`.Auth`/`.Audit`/`.Identity`)
and authentication forwarding in full; this skill does not repeat that, only how to consume a
client from this app.

If the client class doesn't exist yet, don't treat that as a choice to offer - treat it as a sign
something may be wrong. Either the user is pointed at the wrong application (the client is
expected to already exist, defined on the *owning* service's side - check this isn't simply the
wrong project before going further), or the owning service genuinely hasn't defined it yet, in
which case that's `nano-add-api-client`'s job, in that other application's own project, not
this skill's. Either way: stop, warn the user plainly that the client doesn't exist where expected,
and ask which is true - don't invoke the other skill automatically, and don't proceed on the
assumption a missing class is just an item to create in passing.

**No registration call.** Every `BaseApiClient` subclass in the entry assembly whose class name
matches a key under `App:Apis` is auto-wired - but per AGENTS.md's own gotcha, a client that's
never actually injected anywhere doesn't get registered at all. Add the config, then make sure
something actually consumes it (a controller or worker constructor parameter), or none of this
takes effect.

## Before making any change, determine

1. **Does the client class already exist?** Check the target service's `{TargetName}.Models/Api/`
   project (or its published NuGet) for the `BaseApiClient`/`BaseIdentityApiClient` subclass the
   user means. If it doesn't exist yet, stop - don't create it inline, and don't invoke
   `nano-add-api-client` automatically. Warn the user explicitly that the client isn't defined
   where expected, and ask two things: is this actually the right target/application to be
   wiring into right now, and if so, did they mean to create the client first (on the owning
   service's own project, a separate task from this one)? Let them answer both before doing
   anything else.
2. **How does this app reference the target's `.Models` project?** Check how any other Api
   Client in this project already references its target (`ProjectReference` for a
   same-solution/monorepo target, or a NuGet/private-feed `PackageReference` for a separate-repo
   target - the more common real-world case, since most Nano apps live in separate repos and
   publish their `.Models` project as a private package) and match that convention - AGENTS.md
   explicitly allows either here. If this is the first Api Client in the project, ask which
   applies.
   - **Then actually add the reference to this app's `.csproj` if it isn't already there** - don't
     stop at determining which kind it should be. A missing reference here is a real, previously
     observed gap (this app referencing a target's Api Client with no corresponding
     `ProjectReference`/`PackageReference` at all, caught only when the build failed).
   - **For a `PackageReference`, determine the version rather than guessing.** If the target's
     source is locally available (a monorepo or multiple repos checked out side by side), read
     its `.csproj`'s `<Version>` directly. If it isn't, ask the user for the version instead of
     inventing one.
   - **Private feed authentication is the user's responsibility, not something to work around.**
     If the target's package lives on a private feed (Azure Artifacts, GitHub Packages, etc.) and
     restore fails for missing credentials, say so plainly and let the user resolve their own
     `nuget.config`/feed auth - don't attempt to supply or guess at credentials yourself, and
     don't silently fall back to a different reference shape to dodge the failure.
3. **Is a client with this class name already registered?** Check for an existing `App:Apis` key
   matching the class name - if one's already there pointing at a different host/target, confirm
   with the user before overwriting it.
4. **Console app?** Per AGENTS.md's `#### Authentication forwarding`: Console workers have no
   inbound `HttpContext`, so they can't transparently forward a caller's JWT - a Console-hosted
   client typically only calls `[AllowAnonymous]` endpoints on the target, or needs `LogInRoot`
   configured if it must call authenticated ones. Ask which applies before adding `LogInRoot`.

## appsettings.json (this app)

Add the `App:Apis:{ClientClassName}` section to the base `appsettings.json` - the dictionary key
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
  creates, never re-create or duplicate it with a new name. **But don't assume that secret
  already exists** - per `nano-add-authentication-jwt`, a Staging/Production `RootLogin` is not
  something the target gets by default; it's an opt-in a human has to hand-wire on the target
  app specifically, which is a different repo this skill has no visibility into. Ask the user to
  confirm the target actually has `auth-root-login-secret` (keys `root-login-username`/
  `root-login-password`) wired up in Staging/Production before adding a reference to it here -
  don't wire a `secretKeyRef` that may point at a secret nothing creates. Once confirmed, map it
  into `App__Apis__{ClientName}__LogInRoot__Username`/`Password` in `deployment.yaml`. Don't
  confuse this with `Jwt.RootLogin` - see AGENTS.md's explicit warning distinguishing the two;
  they're on different apps, in different directions.

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

## docker-compose.yml (local Development) - do this automatically, every time

The target must actually run locally alongside this app, or `Host` in the config above resolves to
nothing when you `docker compose up`. Read AGENTS.md's `#### Local Development (docker-compose)`
section under Api Clients first - this is not an optional follow-up step, it's part of what
"add an Api Client configuration" means; do it in the same change as the config/injection above,
without being asked separately. **Applies to Console apps too** - a worker consuming an Api Client
needs its target runnable locally the same way an API/Web consumer does; the only difference is a
Console app's own compose service has no `ports` of its own to worry about colliding with.

1. **Is the target already nested in this app's `.docker/docker-compose.yml`?** (Check for a
   service block whose `hostname`/`image` matches the target - e.g. `svc-mytarget`.) If yes,
   nothing to do here.
2. **Does the target have a Data and/or Eventing provider configured?** Check the target's own
   `Program.cs`/`appsettings.json` (or its own standalone `.docker/docker-compose.yml`, which
   already reflects this) - determines whether the nested block gets `depends_on: [database,
   eventing]` or neither. Add a shared `database`/`eventing` service to *this* app's compose file
   only if not already present - one instance serves every nested dependency, never one per
   dependency.
3. **Add the nested service block**, per AGENTS.md's template - `dockerfile_inline` copying from
   `./bin/publish/.`, a host port that doesn't collide with this app's own or any other nested
   service's port, and `depends_on` wired both onto this app's own primary service (add the new
   `svc.*` key there) and, per step 2, onto `database`/`eventing` if applicable.
4. **Wire the publish step into `.docker/docker-compose.dcproj`**:
   - If `publish-dependencies.ps1` doesn't exist yet in `.docker/`, create it (per AGENTS.md's
     template) and add the `PublishDependentServices` MSBuild target with `Inputs`/`Outputs`
     incremental-build wiring.
   - If it already exists (this app already consumes at least one other Api Client), add the new
     target's `.csproj` publish line to the existing script, and add a new `DependentServiceSources`
     `ItemGroup` entry for the target (its main project + its `.Models` project, `.cs`/`.csproj`
     globs, excluding `bin`/`obj`) - don't create a second script or a second target.
5. No `.gitignore` entry is needed for the stamp file the script writes - it lives under
   `.docker/bin/`, already covered by the solution's standard `**/bin` ignore rule.

## After making the change

- Show the user every file touched in *this* app - the `.csproj` reference (if one was added),
  the `appsettings.json` addition, the injection site, and every docker-compose/dcproj/gitignore
  file touched by the section above.
- Confirm the client is actually injected somewhere - if the request was just "add the client" with
  no specified consumer yet, say explicitly that nothing is wired up until it's referenced.
- Confirm the target is runnable locally: nested in `docker-compose.yml`, and covered by
  `publish-dependencies.ps1`/the incremental MSBuild target - don't leave `docker compose up`
  producing an unreachable host for the new client.
- If `LogInRoot` was added, restate the Staging/Production secret-handling requirement - don't let
  a real credential sit in the base file - and whether the target's `auth-root-login-secret` was
  confirmed to actually exist or is still an open prerequisite on that other app.
- If restore failed due to private feed authentication, say so plainly and stop there - that's
  the user's `nuget.config`/feed credentials to fix, not something to route around.
