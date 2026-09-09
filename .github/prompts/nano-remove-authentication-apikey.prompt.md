---
mode: agent
description: Remove Nano's built-in API-key authentication (Data:Identity:ApiKey) from a Nano.Library-based application - unregisters the ApiKey configuration and removes its Staging/Production secret/CI wiring. Use when the user asks to remove API-key authentication or the X-Api-Key header scheme from a Nano API or Web application - not for removing JWT authentication by itself, that's nano-remove-authentication-jwt.
---

# Nano remove API-key authentication

Fully removes Nano's API-key authentication from an existing Nano API/Web application - the
counterpart to `nano-add-authentication-apikey`. Read that skill first - this one undoes exactly
what it adds.

## Before making any change, determine

1. **Is API-key authentication currently configured?** Check the base `appsettings.json` for
   `Data:Identity:ApiKey:Secret`. If not present, say so and stop.
2. **Is JWT authentication also configured on this app** (`App:Authentication:Jwt`/an existing
   `AuthController`)? This determines what removal actually does - surface it before proceeding:
   - **Also configured**: nothing dramatic - `AuthController` doesn't depend on `ApiKeyOptions`
     at all, so it keeps working exactly as before. `/auth/login/apikey` simply becomes hidden
     again (`ConditionalActionsConvention` gates its visibility purely on
     `Data:Identity:ApiKey:Secret`), and the scheme reverts from `JWT_OR_APIKEY` to JWT-only. No
     file besides config/K8s/CI needs touching.
   - **Not configured (pure API-key mode)**: this was the app's **only** authentication scheme -
     removing it leaves the app with no authentication at all, every endpoint anonymous by
     default (AGENTS.md). Confirm this is intended before proceeding; it's a security-relevant
     change, not just a config cleanup, and there's no controller here to hint at it either (pure
     API-key mode never had one).

## appsettings.json

Remove `Data:Identity:ApiKey:Secret` from the base `appsettings.json`, and from
`appsettings.Development.json` too if a local convenience value was set there (per
`nano-add-authentication-apikey`'s note that this is the one place a Development override might
exist, unlike the shared JWT key pair).

## Kubernetes / GitHub Actions

Unlike `auth-jwt-secret`, this secret is always per-app (never shared across services), so
there's no issuer/validator distinction to worry about here - always safe to remove:

- Delete `.kubernetes/auth-api-key-secret.yaml`.
- Remove its apply step from the `Kubernetes Deploy` workflow step.
- Remove the `AUTH_API_KEY_SECRET` workflow env var.
- Remove the `Data__Identity__ApiKey__Secret` entry from `.kubernetes/deployment.yaml`'s
  container `env`.

## After making the change

- Show the user every file touched/deleted.
- Restate step 2's outcome now that it's done - either "JWT auth still works, the key-exchange
  endpoint is gone" or "this app now has no authentication at all, every endpoint is anonymous" -
  whichever applies. Worth a second, explicit confirmation, not just a line in a file list.
