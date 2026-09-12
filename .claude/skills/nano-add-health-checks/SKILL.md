---
name: nano-add-health-checks
description: Enable Nano's built-in health checks (App:HealthCheck) on a Nano API or Web application - adds the config plus the Kubernetes liveness/readiness probes a fresh app ships without. Use when the user asks to add health checks, a /healthz endpoint, or liveness/readiness probes to a Nano API or Web application.
---

# Nano add health checks

Enables Nano's built-in `/healthz` endpoint on an existing Nano API or Web application. Read
AGENTS.md's `#### Health Checks` section first — it documents the response shape and the
health-is-a-tree propagation model in full; this skill is config plus the Kubernetes wiring a
fresh app doesn't have yet.

**API/Web only.** Console apps have no HTTP pipeline at all — there's nothing to expose `/healthz`
on. If the target is a Console app, stop and say so rather than adding dead config.

**This is not just a config flip.** `UseNanoHealthChecks` no-ops entirely when `App:HealthCheck`
isn't configured — `/healthz` genuinely doesn't exist without it. A minimal Nano app ships with
**no Kubernetes liveness/readiness probes at all** (verified: `nanocore-api-minimal`'s
`deployment.yaml` has none), specifically because probing a path that doesn't exist would fail
forever and crash-loop the pod. So enabling health checks means adding the config **and** the
probes together — never one without the other.

## Before making any change, determine

1. **Application type.** Confirm API or Web via `Program.cs`. Stop for Console.
2. **Is `App:HealthCheck` already configured?** Check the base `appsettings.json`. If present,
   check whether `.kubernetes/deployment.yaml`/`stateful-set.yaml` already has the matching
   probes — if the config exists but the probes don't (or vice versa), that's the broken
   half-state described above; fixing it is this skill's job even though nothing needs "adding"
   config-wise.
3. **Any provider health checks waiting to activate?** Check for `HealthCheck` blocks already
   present under `Data`/`Eventing`/`Storage`/`App:Apis:{Client}` config — those are dead
   configuration until `App:HealthCheck` exists (AGENTS.md: "must also be enabled at the
   `App:HealthCheck` level"). Not a blocker, just worth mentioning — they'll start working the
   moment this change lands.

## appsettings.json

Add to the base `appsettings.json`, sibling of `App:Version`/`App:Hosting`:

```json
"App": { "HealthCheck": { } }
```

No options — presence alone enables it. Same in every environment; no Development-specific
override needed.

## Kubernetes

Add liveness and readiness probes to `.kubernetes/deployment.yaml`'s (or `stateful-set.yaml`'s)
container spec:

```yaml
livenessProbe:
  httpGet:
    path: /healthz
    port: 8080
    scheme: HTTP
  periodSeconds: 10
  initialDelaySeconds: 30
  timeoutSeconds: 2
readinessProbe:
  httpGet:
    path: /healthz
    port: 8080
    scheme: HTTP
  periodSeconds: 5
  initialDelaySeconds: 20
  timeoutSeconds: 2
```

These values (period/delay/timeout) match every existing Nano app with health checks enabled —
match them rather than inventing different numbers unless the user asks for something specific.

## After making the change

- Show the user every file touched.
- If step 3 found dormant provider health checks, tell the user explicitly which ones just
  became active — they'll now appear in the `/healthz` response tree and can affect the overall
  reported status.
- If step 2 found a broken half-state (config without probes, or probes without config), say
  clearly what was actually wrong before this fix, not just what was added.
