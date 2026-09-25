---
name: nano-add-api-client-configuration
description: Wire an existing Nano Api Client into this application - adds the App:Apis configuration entry, injects the client into a controller/worker, and nests the target service into this app's local docker-compose (with its own incremental publish step) so it's actually runnable end-to-end. Use when the user asks to call another Nano service/API from this app, add an API client to a Public API, or compose internal services together in a Nano API, Web, or Console application.
---

# Nano add API client configuration

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
which case that's `nano-add-api-client`'s job, in that other application's own project, not
this skill's. Either way: stop, warn the user plainly that the client doesn't exist where expected,
and ask which is true — don't invoke the other skill automatically, and don't proceed on the
assumption a missing class is just an item to create in passing.

**No registration call.** Every `BaseApiClient` subclass in the entry assembly whose class name
matches a key under `App:Apis` is auto-wired — but per AGENTS.md's own gotcha, a client that's
never actually injected anywhere doesn't get registered at all. Add the config, then make sure
something actually consumes it (a controller or worker constructor parameter), or none of this
takes effect.

## Before making any change, determine

0. **Is the client already referenced by this application, and if not, where is it?** Do this first.
   - **Already referenced:** if this application's project (`{name}/{name}.csproj`) already has a
     `ProjectReference`/`PackageReference` that provides the client class, nothing to add - continue.
   - **Not referenced, and the user named the target service** (e.g. "add api client configuration for
     `Svc.Something`"): search the **root directory** - one level up from this application's own solution
     directory, where every sibling solution lives - for a solution folder with that name and its
     `{TargetName}.Models` project. If found, add the `ProjectReference` (step 2) without asking again.
   - **Not referenced, and the user did not name a target service** (only the client class): tell the user
     the client could not be found in this application's references, and - if a sibling `{TargetName}.Models`
     project containing it is found under the root directory - offer to add a project reference to it,
     naming that project. Wait for their answer.
   - **Not found anywhere locally, or several matches:** say so and ask - don't guess, and never invent a
     package reference or version.
   - **Whenever a project reference is (or is about to be) added, say upfront** that it is a
     development-time reference: it must be replaced with the real NuGet `PackageReference` once the target
     service's package is published, before this application is deployed.
1. **Does the client class already exist?** Check the target service's `{TargetName}.Models/Api/`
   project (or its published NuGet) for the `BaseApiClient`/`BaseIdentityApiClient` subclass the
   user means. If it doesn't exist yet, stop — don't create it inline, and don't invoke
   `nano-add-api-client` automatically. Warn the user explicitly that the client isn't defined
   where expected, and ask two things: is this actually the right target/application to be
   wiring into right now, and if so, did they mean to create the client first (on the owning
   service's own project, a separate task from this one)? Let them answer both before doing
   anything else.
2. **Always add the reference to the consuming application's own project (`{name}/{name}.csproj`), never
   to a `.Models` project** - even if this application happens to have one. A `.Models` project is shared
   (published as a NuGet) with other applications, so a reference to another service's `.Models` placed
   there would leak into every consumer of it, which it shouldn't. Most consumers (a Public API, a Console
   application) have no `.Models` project at all.
   - Use a relative `ProjectReference` to the target's `{TargetName}.Models.csproj` found in step 0. Match
     how the project's other Api Client references are written where that applies.
   - **Never write a `PackageReference` or a version yourself** - the real version is injected by CI/CD, so
     there is nothing reliable to read locally, and the package is on a private feed you have no business
     querying. Switching to the package is the user's step (see "After making the change").
   - **Actually add the reference if it isn't already there** - a missing reference is a real, previously
     observed gap (caught only when the build failed).
   - If the application already references other targets by `PackageReference`, still add the
     `ProjectReference` for this one, and say the same replace-later note applies.
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
  creates, never re-create or duplicate it with a new name. **But don't assume that secret
  already exists** — per `nano-add-authentication-jwt`, a Staging/Production `RootLogin` is not
  something the target gets by default; it's an opt-in a human has to hand-wire on the target
  app specifically, which is a different repo this skill has no visibility into. Ask the user to
  confirm the target actually has `auth-root-login-secret` (keys `root-login-username`/
  `root-login-password`) wired up in Staging/Production before adding a reference to it here —
  don't wire a `secretKeyRef` that may point at a secret nothing creates. Once confirmed, map it
  into `App__Apis__{ClientName}__LogInRoot__Username`/`Password` in `deployment.yaml`. Don't
  confuse this with `Jwt.RootLogin` — see AGENTS.md's explicit warning distinguishing the two;
  they're on different apps, in different directions.

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

## docker-compose.yml (local Development) — do this automatically, every time

The target must actually run locally alongside this app, or `Host` in the config above resolves to
nothing when you `docker compose up`. Read AGENTS.md's `#### Local Development (docker-compose)`
section under Api Clients first — this is not an optional follow-up step, it's part of what
"add an Api Client configuration" means; do it in the same change as the config/injection above,
without being asked separately. **Applies to Console apps too** — a worker consuming an Api Client
needs its target runnable locally the same way an API/Web consumer does; the only difference is a
Console app's own compose service has no `ports` of its own to worry about colliding with.

1. **Is the target already nested in this app's `.docker/docker-compose.yml`?** (Check for a
   service block whose `hostname`/`image` matches the target — e.g. `svc-mytarget`.) If yes,
   nothing to do here.
2. **Does the target have a Data and/or Eventing provider configured?** Check the target's own
   `Program.cs`/`appsettings.json` (or its own standalone `.docker/docker-compose.yml`, which
   already reflects this) — determines whether the nested block gets `depends_on: [database,
   eventing]` or neither. Add a shared `database`/`eventing` service to *this* app's compose file
   only if not already present — one instance serves every nested dependency, never one per
   dependency.
3. **Add the nested service block**, per AGENTS.md's template — `dockerfile_inline` copying from
   `./bin/publish/.`, a host port that doesn't collide with this app's own or any other nested
   service's port, and `depends_on` wired both onto this app's own primary service (add the new
   `svc.*` key there) and, per step 2, onto `database`/`eventing` if applicable. **Also add
   `env_file: [../../{TargetName}/.docker/.env]`, unconditionally** — every Nano app template
   ships with an empty `.docker/.env` alongside its `docker-compose.yml` (see AGENTS.md's Local
   Development section), so the file always exists even if the target has no secrets yet today.
   Wiring the reference now means a secret added to the target later needs nothing done on this
   app's side to pick it up — don't skip this because the target doesn't currently have any
   secrets in its `.env`.
4. **Wire the publish step into `.docker/docker-compose.dcproj`**, per AGENTS.md's template — **one MSBuild
   target per nested dependency, never one shared target for all of them**:
   - If `publish-dependencies.ps1` doesn't exist yet in `.docker/`, create it (per AGENTS.md's template — it
     takes `-Project`/`-StampName` parameters and publishes exactly one project per invocation, no dependency
     list of its own).
   - Whether this is the first dependency or the app already consumes others, add a **new, separate**
     `{Target}Sources` `ItemGroup` entry (the target's main project + its `.Models` project, `.cs`/`.csproj`
     globs, excluding `bin`/`obj`, **plus `**\*.json` on the target's main project only** — its `.Models`
     project doesn't ship any) and a **new, separate** `Publish{Target}` MSBuild target with its own
     `Inputs="@({Target}Sources)"` and its own `Outputs=".../bin/publish-{target}.stamp"` — never add to an
     existing dependency's `ItemGroup`/`Inputs`/stamp, and never fold two dependencies into one target. This
     per-dependency split is what lets MSBuild republish (and let Docker rebuild) only the one dependency that
     actually changed, instead of every nested dependency on every build — see AGENTS.md's own ⚠ on this.
     Skipping the `.json` glob has the same failure mode as before: a config-only `appsettings.*.json` edit
     silently never triggers a republish, leaving stale config baked into the image indefinitely.
5. No `.gitignore` entry is needed for any stamp file the script writes — they all live under `.docker/bin/`,
   already covered by the solution's standard `**/bin` ignore rule.
6. **Tell the user Rebuild → F5 is the required workflow after changing a nested dependency**, not F5 alone —
   see AGENTS.md's own ⚠ on this: a plain F5 while nothing is running doesn't reliably re-invoke these targets,
   even though an explicit Rebuild does. This isn't something a project file can fix; it's Visual Studio's own
   build-vs-launch trigger behavior for `docker-compose` projects.

## After making the change

- **If a `ProjectReference` was added, remind the user:** remember to replace the project reference with
  the real NuGet `PackageReference` once the target service's package is published, before deploying.
  The skill never writes the package reference or version itself.
- Show the user every file touched in *this* app — the `.csproj` reference (if one was added),
  the `appsettings.json` addition, the injection site, and every docker-compose/dcproj/gitignore
  file touched by the section above.
- Confirm the client is actually injected somewhere — if the request was just "add the client" with
  no specified consumer yet, say explicitly that nothing is wired up until it's referenced.
- Confirm the target is runnable locally: nested in `docker-compose.yml`, and covered by
  `publish-dependencies.ps1`/the incremental MSBuild target — don't leave `docker compose up`
  producing an unreachable host for the new client.
- If `LogInRoot` was added, restate the Staging/Production secret-handling requirement — don't let
  a real credential sit in the base file — and whether the target's `auth-root-login-secret` was
  confirmed to actually exist or is still an open prerequisite on that other app.
- If restore failed due to private feed authentication, say so plainly and stop there — that's
  the user's `nuget.config`/feed credentials to fix, not something to route around.
