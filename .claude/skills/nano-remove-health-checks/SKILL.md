---
name: nano-remove-health-checks
description: Remove Nano's built-in health checks (App:HealthCheck) from a Nano API or Web application - removes the config and the Kubernetes liveness/readiness probes together, since leaving one without the other breaks the pod. Use when the user asks to remove health checks or the /healthz endpoint from a Nano API or Web application.
---

# Nano remove health checks

Removes Nano's `/healthz` endpoint from an existing Nano API or Web application — the counterpart
to `nano-add-health-checks`. Read that skill first — this one undoes exactly what it adds, and
the same "never do one half without the other" rule applies in reverse here.

## Before making any change, determine

1. **Is `App:HealthCheck` currently configured?** Check the base `appsettings.json`. If absent,
   say so and stop.
2. **What depends on it?**
   - **Kubernetes probes will start failing if left behind.** Removing `App:HealthCheck` without
     also removing the `livenessProbe`/`readinessProbe` in `deployment.yaml`/`stateful-set.yaml`
     means Kubernetes keeps probing a `/healthz` path that no longer exists — the pod gets marked
     unhealthy and crash-loops. Both must be removed together; this is not optional cleanup.
   - **Provider health checks go dark, not broken.** If `Data`/`Eventing`/`Storage`/
     `App:Apis:{Client}`'s own `HealthCheck` blocks are configured, they become dead config once
     `App:HealthCheck` is gone (same dependency as at add-time, just now unsatisfied). Not a
     crash, but tell the user — leaving those blocks in place with no effect is confusing without
     an explanation.
   - **`Metrics`, if enabled, is unaffected** — it has no dependency on `HealthCheck` in either
     direction; don't touch it.

## Kubernetes

Remove the `livenessProbe` and `readinessProbe` entries from `.kubernetes/deployment.yaml`'s (or
`stateful-set.yaml`'s) container spec.

## appsettings.json

Remove `App:HealthCheck` from the base `appsettings.json`.

## After making the change

- Show the user every file touched.
- Restate step 2's provider-health-check note if applicable — which blocks are now dead config,
  without functional effect.
- If step 1 stopped the skill early, that's the whole response.
