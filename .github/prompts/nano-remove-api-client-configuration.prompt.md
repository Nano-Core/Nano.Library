---
mode: agent
description: Stop consuming a Nano Api Client from this application - removes the App:Apis configuration entry and the client's injection site, without touching the client's definition in the owning service. Use when the user asks to remove a call to another Nano service/API, stop consuming an internal service, or drop an API client from this app's Public API composition in a Nano API, Web, or Console application.
---

# Nano remove API client configuration

Removes this application's *consumption* of a Nano Api Client - the `App:Apis` config entry and
the injection site - without touching the client's definition in the owning service's `.Models`
project. The counterpart to `nano-add-api-client-configuration`. Read that skill first - this one
undoes exactly what it adds, and nothing more.

If the actual goal is to delete the client's definition entirely (so *no* application can consume
it anymore), that's `nano-remove-api-client`'s job instead, on the owning service's side - this
skill never deletes a `.Models` project's client class, since that class may still be consumed by
other applications this skill has no visibility into.

## Before making any change, determine

1. **Is the `App:Apis` entry for this client actually present in this app?** Check the base
   `appsettings.json` for `App:Apis:{ClientClassName}`. If none, say so and stop.
2. **What depends on it in this app?** Search for the client class used as a constructor
   parameter (controller or worker) in *this* app only. This isn't a startup-crash risk - the
   client itself has no required-service semantics beyond normal C# compilation - but removing
   the config while something still injects the class simply **won't compile** (or, if the class
   still resolves some other way, silently stops working). Find every injection site in this app
   first.
3. **Is anything else in this app still using the target's `.Models` project?** Once every
   injection site from step 2 is gone, check whether this app's `.csproj` reference to the
   target's `.Models` project (`ProjectReference` or `PackageReference` - see
   `nano-add-api-client-configuration`'s note that private-feed `PackageReference` is the more common
   real-world case) has any other reason to exist - a directly-referenced entity/DTO type,
   another client targeting the same package, etc. If nothing else uses it, the reference is
   safe to remove too; if something does, leave it and say so explicitly rather than guessing.

## appsettings.json (this app)

Remove the `App:Apis:{ClientClassName}` section from the base `appsettings.json`, and its
`LogInRoot`/other overrides from `appsettings.Development.json`, if present.

**`LogInRoot`'s Staging/Production cleanup is a `deployment.yaml` env-entry removal, not a
secret deletion.** Per `nano-add-api-client-configuration`'s design, this app never creates its own secret for
`LogInRoot` - it only maps into the *target's* existing `auth-root-login-secret` via a
`secretKeyRef`. So there's no Kubernetes secret or GitHub secret on this app's side to delete;
just remove the `App__Apis__{ClientClassName}__LogInRoot__Username`/`Password` `secretKeyRef`
entries from `.kubernetes/deployment.yaml`. The target's `auth-root-login-secret` itself is
unaffected - it's shared/owned by the target app, not this one.

## Injection sites (this app)

Remove the constructor parameter and field from every controller/worker in this app that took
this client, per step 2 - this is a compile-breaking change if left in place after the config is
gone.

## .csproj reference (this app)

If step 3 found nothing else in this app uses the target's `.Models` project, remove the
`ProjectReference`/`PackageReference` too. If step 3 found something else still uses it, leave it
and say so.

## docker-compose.yml (local Development)

Mirror of `nano-add-api-client-configuration`'s docker-compose step - remove the target's nested
service *only* if nothing else in this app's compose file still needs it:

1. **Is anything else in this app still consuming the target?** If step 3 found another client
   still using the target's `.Models` project (or any other reason the target is still called),
   leave the nested service block, its `DependentServiceSources` entry, and its line in
   `publish-dependencies.ps1` alone - say so explicitly.
2. Otherwise, remove: the target's service block from `.docker/docker-compose.yml`, its key from
   this app's own primary service's `depends_on`, its `DependentServiceSources` `ItemGroup` entry
   from `.docker/docker-compose.dcproj`, and its `dotnet publish` line from
   `publish-dependencies.ps1`.
3. **Don't remove the shared `database`/`eventing` services** just because this one dependency is
   gone - they're shared across every nested dependency in the compose file; only remove one if
   step 2 removes the *last* dependency that needed it (check every remaining nested service's
   `depends_on` first).
4. If this was the *only* dependency this app ever nested, remove `publish-dependencies.ps1`
   entirely, and the `PublishDependentServices` target and `DependentServiceSources` `ItemGroup` from
   `.docker/docker-compose.dcproj`.

## After making the change

- Show the user every file touched in this app, including the `.csproj` reference if it was
  removed (or why it was kept, per step 3), and every docker-compose/dcproj file touched (or left
  alone, and why) by the section above.
- Note explicitly that the client's own definition in the owning service's `.Models` project was
  **not** touched - other applications may still consume it. If the user's actual intent was to
  delete the definition entirely, point them at `nano-remove-api-client` next.
- If step 1 or 2 stopped the skill early (no entry present, or unresolved injection sites), that's
  the whole response - don't leave broken constructor parameters behind.
