---
name: nano-remove-eventing-provider
description: Remove a Nano eventing provider (currently only RabbitMq) from a Nano.Library-based application - unregisters it in Program.cs and removes the Eventing configuration, local docker-compose broker service, and Kubernetes secret reference. Use when the user asks to remove eventing, pub/sub messaging, or a message broker from a Nano API, Web, or Console application.
---

# Nano remove eventing provider

Fully removes a Nano eventing provider from an existing Nano API, Web, or Console application —
the counterpart to `nano-add-eventing-provider`. Read that skill first — this one undoes exactly
what it adds.

## Before making any change, determine

1. **Is an eventing provider currently registered?** Check `Program.cs` for
   `.AddNanoEventing<...>()`. If none, say so and stop.
2. **What depends on it?** Two distinct risks, different severities — check for both:
   - **Startup crash.** Search for `IEventing` used as a constructor parameter (injected via DI)
     anywhere in the project, and check whether it's required or optional (`IEventing?
     eventing`, the pattern the entity-scaffold skill uses when an eventing provider isn't
     registered). A class with a **required** `IEventing` parameter fails DI resolution the
     moment the provider is gone — the app won't start at all, not even a runtime error deep in
     some request path. This is the more urgent of the two checks.
   - **Silent no-op.** Per AGENTS.md's `### Entity Events` section, `[Publish]`/`[Subscribe]`
     entity replication **requires Eventing configured, and silently does nothing without it** —
     no exception, no error, it just stops syncing. Also check for any class deriving
     `BaseEventHandler<TEvent>` (general pub/sub, not entity events) — its handler simply never
     fires again, with no signal that it stopped.
   If either exists, tell the user exactly what removing the provider will do to it (crash vs.
   silent no-op) and confirm before proceeding — don't remove out from under them without saying
   so.
3. **Is the package reference this skill's to remove?** Same check as the other remove skills:
   leave `NanoCore`/`Nano.All` alone if present; otherwise remove the `Nano.Eventing.RabbitMq`
   `PackageReference` from the application project.

## Program.cs

Remove `using Nano.Eventing.Extensions;`, `using Nano.Eventing.RabbitMq;`, and the
`.AddNanoEventing<...>()` call. Same empty-lambda cleanup as the other remove-provider skills:
restore the blank-app placeholder and `_` parameter if nothing else is left in
`.ConfigureServices(...)`.

## appsettings.json

Remove the `Eventing` section from the base `appsettings.json`, and its `Credentials` override
from `appsettings.Development.json` if present (per `nano-add-eventing-provider`'s placement —
only `Credentials` lives in the Development file, the rest of the section is base-only).

## docker-compose.yml

Remove the `eventing` service from `.docker/docker-compose.yml` entirely (delete, don't
comment out — unlike the data-provider skill's multiple-alternatives convention, there's only
one eventing provider, so there's no sibling variant worth preserving as a reference). Also
remove it from the app's own service's `depends_on` list.

## Existing entity controllers

The counterpart to `nano-add-eventing-provider`'s retrofit step: remove the `IEventing? eventing`
constructor parameter (and the corresponding base-constructor argument) from every entity
controller that has one, across the full `BaseEntity*Controller`/`BaseEntityUserController`
hierarchy — it's dead weight once nothing can ever populate it. This is separate from, and safe
regardless of, the crash risk already flagged in step 2: a controller with the **nullable**
`IEventing?` form just loses an unused parameter here; one with a **required** `IEventing`
parameter (the crash case) still needs that constructor fixed by hand as part of addressing step
2 — removing the parameter here is what actually resolves it, once the user has confirmed that's
acceptable.

## Kubernetes

Remove the four `Eventing__*` env entries (`Eventing__Host`, `Eventing__Port`,
`Eventing__Credentials__Id`, `Eventing__Credentials__Secret`) from
`.kubernetes/deployment.yaml`'s container `env`. There's no secret file to delete — the
`rabbitmq-default-user` secret is shared/cluster-wide and outlives this app regardless.

## After making the change

- Show the user every file touched, including every controller that had `IEventing? eventing`
  removed, and restate anything flagged in step 2 — required `IEventing` injections that will now
  crash the app, plus any orphaned `[Publish]`/`[Subscribe]` entities or dead event handlers — one
  more time now that the removal is actually done, not just as the earlier confirmation.
- If a step was skipped because the project uses `NanoCore`/`Nano.All`, say so explicitly.
