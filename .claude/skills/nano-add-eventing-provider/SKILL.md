---
name: nano-add-eventing-provider
description: Add a Nano eventing provider (currently only RabbitMq) to a Nano.Library-based application - registers it in Program.cs, adds the Eventing configuration section, the local docker-compose broker service, and the Kubernetes secret reference for Staging/Production. Use when the user asks to add eventing, pub/sub messaging, or a message broker to a Nano API, Web, or Console application.
---

# Nano add eventing provider

Wires a Nano eventing provider into an existing Nano API, Web, or Console application. Read
`AGENTS.md` in the target repo root first — its `## Nano.Eventing` section documents the
`Configuration` table, the provider/package table, and `Publish and Subscribe`/`BaseEventHandler`
usage in full; this skill does not repeat any of that, only how to apply it and wire the
surrounding infrastructure (docker-compose, K8s) without breaking what's already there.

Considerably simpler than the data-provider skill: there's no CI migration step, no Managed
Identity pairing, and no per-app secret to create — RabbitMQ credentials come from a
**pre-existing, shared, cluster-wide** Kubernetes secret, not something this skill provisions.

## Before making any change, determine

1. **Which provider.** Currently only `RabbitMq` (see AGENTS.md's provider table — if the user
   names something else, check whether a custom provider already exists in the project first,
   per AGENTS.md's `#### Custom eventing provider` section).
2. **Is an eventing provider already registered?** Check `Program.cs` for an existing
   `.AddNanoEventing<...>()` call — unlike Data, there's no supported multi-provider case here
   (AGENTS.md: a provider's `Configure` must itself register the single `IEventing`
   implementation). If one is already registered, treat this as a replace and say so, the same
   as the logging skill.
3. **Is a package reference even needed?** Same check as the other add-provider skills: look for
   `NanoCore`/`Nano.All` (directly, or transitively via a `.Models` project). If found, skip the
   package step. Otherwise add `<PackageReference Include="Nano.Eventing.RabbitMq" Version="X.Y.Z" />`
   to the **application project**, matching the version of the project's existing Nano
   application-type package. Never a `ProjectReference` to Nano.Library source.

## Program.cs

```csharp
using Nano.Eventing.Extensions;
using Nano.Eventing.RabbitMq;
```

```csharp
x.AddNanoEventing<RabbitMqProvider>();
```

Same `.ConfigureServices(...)` lambda placement and `_` → `x` placeholder-rename rule as the
other add-provider skills.

## appsettings.json

Add the `Eventing` section from AGENTS.md's `### Configuration` example to the base
`appsettings.json` (sibling of `App`), with one placement split that example doesn't spell out —
unlike the Data provider's `ConnectionString`, most of this section is **not** sensitive:

- `Host` stays filled in (`"rabbitmq"`, the docker-compose service name below) at the **base**
  level — it's not sensitive, and it's already the correct value for local `Development`, so no
  `appsettings.Development.json` override is needed for it. Staging/Production override `Host`
  (and everything else) via the Kubernetes secret below, not a static appsettings file.
- Only `Credentials.Id`/`Credentials.Secret` are secret — leave them `null` in the base file, and
  set the real local values (matching the docker-compose broker's own credentials below) in
  `appsettings.Development.json`.
- Include `HealthCheck` only if the project's `App.HealthCheck` is actually enabled — adding a
  dependency-level health check when nothing reads the app-level `/healthz` endpoint is dead
  configuration that Kubernetes probes would point at without effect. If unsure, check
  `Program.cs`'s `.ConfigureApp()` chain / the base `appsettings.json` for `App.HealthCheck` first.

## docker-compose.yml (local Development)

Add an `eventing` service to `.docker/docker-compose.yml`, and add it to the app's own service's
`depends_on` if not already present:

```yaml
eventing:
  image: rabbitmq:management
  hostname: rabbitmq
  ports:
    - 5671:5671
    - 5672:5672
    - 15671:15671
    - 15672:15672
  networks:
    - network
  environment:
    RABBITMQ_DEFAULT_USER: rabbitmq_user
    RABBITMQ_DEFAULT_PASS: password
    RABBITMQ_DEFAULT_VHOST: /
```

`hostname: rabbitmq` is why the base `appsettings.json`'s `Eventing:Host` can just be
`"rabbitmq"` without a Development-specific override — it resolves directly on the compose
network.

## Existing entity controllers

Per AGENTS.md's `#### Entity controller hierarchy`, every entity controller's constructor
already has a place for `IEventing? eventing = null` — it's optional, so a controller written
before eventing existed simply omits it. Now that a provider is registered, retrofit every
existing entity controller (the full `BaseEntity*Controller`/`BaseEntityUserController` hierarchy
— check each for a constructor that's missing the parameter) to add it, so they can publish
events without a second pass later:

```csharp
public class MyEntitysController(ILogger<MyEntitysController> logger, IRepository repository, IEventing? eventing)
    : BaseEntityController<MyEntity, MyEntityQueryCriteria>(logger, repository, eventing);
```

For a `BaseEntityUserController` (see `nano-add-identity`), `eventing` goes between `repository`
and `identityRepository`, in that order. This is purely additive and safe — the parameter is
nullable, so it doesn't change behavior for a controller that never ends up using it.

## Staging/Production (Kubernetes)

No CI step and no per-app secret to create — RabbitMQ is a **pre-existing, shared, cluster-wide**
broker, referenced by a secret (`rabbitmq-default-user`) that already exists in the cluster
before this app is ever deployed. Don't create a new secret or add a provisioning workflow step;
just wire the reference:

Add to `.kubernetes/deployment.yaml`'s container `env`:

```yaml
- name: Eventing__Host
  valueFrom:
    secretKeyRef:
      name: rabbitmq-default-user
      key: host
- name: Eventing__Port
  valueFrom:
    secretKeyRef:
      name: rabbitmq-default-user
      key: port
- name: Eventing__Credentials__Id
  valueFrom:
    secretKeyRef:
      name: rabbitmq-default-user
      key: username
- name: Eventing__Credentials__Secret
  valueFrom:
    secretKeyRef:
      name: rabbitmq-default-user
      key: password
```

If `rabbitmq-default-user` doesn't exist in the target cluster yet, that's a one-time,
cluster-level provisioning concern — tell the user rather than inventing a new secret name or a
provisioning step for it.

## After making the change

- Show the user the modified `Program.cs` lines, the `appsettings.json` additions (base +
  Development), the docker-compose `eventing` service, the `deployment.yaml` env entries, and
  every entity controller retrofitted with `IEventing? eventing`.
- If a package step was skipped (`NanoCore`/`Nano.All` already covering it), say so explicitly.
- Mention `AGENTS.md`'s `Publish and Subscribe` section as the next read if the user also wants
  to actually publish/handle events, not just have the broker wired — this skill only wires the
  provider, it doesn't scaffold event contracts or handlers.
