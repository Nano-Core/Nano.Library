---
mode: agent
description: Configure Nano's built-in API-key authentication (Data:Identity:ApiKey) on a Nano.Library-based API/Web application that already has Identity registered - works standalone, with no JWT/App:Authentication involved, or layered on top of an existing nano-add-authentication-jwt setup. Use when the user asks to add API-key authentication, an X-Api-Key header scheme, or machine-to-machine auth to a Nano API or Web application.
---

# Nano add API-key authentication

Configures Nano's built-in API-key authentication on an existing Nano API/Web application. Read
`AGENTS.md`'s `#### Identity` and `#### Authentication` sections first - `ApiKey.Secret` lives
under `Data:Identity`, but its actual authentication behavior is documented in `Authentication`.

**API-key auth does not require JWT.** This is the key thing that distinguishes it from
`nano-add-authentication-jwt`, and the reason it's a separate skill: per Nano's own
`AddNanoAuthentication` registration logic, the default scheme is chosen from
`(Jwt configured, ApiKeyOptions configured)` - `(false, true)` selects API-key-only. In that mode
there is **no `AuthController`** (it requires `IAuthRepository`, which is only registered when
`Jwt != null` - adding the controller without `Jwt` would fail DI resolution) and **no login
endpoint** - `ApiKeyAuthenticationHandler` validates the `X-Api-Key` header directly against the
identity store on every single request, with no token step at all.

## Before making any change, determine

1. **Is Identity already configured?** Check the base `appsettings.json` for `Data:Identity`, and
   `Program.cs`/the project for an `.AddNanoData<...>()` + identity entity. API-key auth is an
   identity-store feature (AGENTS.md), not usable without it - if missing, stop and point the
   user at `nano-add-identity` first.
2. **Is API-key auth already configured?** Check for `Data:Identity:ApiKey:Secret` already set.
   If so, say so and stop.
3. **Is JWT authentication already configured on this app?** Check the base `appsettings.json`
   for `App:Authentication:Jwt`, or an existing `AuthController`.
   - **Not configured** - this app will end up in **pure API-key mode**: no `AuthController`, no
     `Jwt` config, `X-Api-Key` is the only credential, checked on every request. Don't add a
     controller "just in case" - it would crash DI (see above).
   - **Already configured** (`nano-add-authentication-jwt` already ran) - this app moves from
     JWT-only to `JWT_OR_APIKEY` **automatically, from config alone**, nothing to change in the
     existing `AuthController`. Its already-existing `LogInApiKeyAsync` action (visibility gated
     purely on `Data:Identity:ApiKey:Secret` being set, per `ConditionalActionsConvention`)
     becomes reachable at `/auth/login/apikey` the moment this skill sets the config - tell the
     user this new endpoint just appeared, it's a real behavior change on an app that may already
     have callers, not just an implementation detail.
4. **Application type.** No controller involved either way in pure mode; in the JWT-paired case
   the controller already exists (added by `nano-add-authentication-jwt`). Nothing API/Web-specific
   for this skill to gate on beyond that.

## appsettings.json

Base `appsettings.json`: add `Data:Identity:ApiKey:Secret: null` (sibling of the rest of
`Identity` - see `nano-add-identity`). No local Development value needed by default - API keys
are normally created per-user via the identity-management endpoints
(`{id}/api-keys/create`, per AGENTS.md's `#### Identity user controller` table) rather than
hardcoded, unlike the shared JWT Development key pair. If the user wants a fixed key for local
testing convenience, set one in `appsettings.Development.json` instead of the base file.

## Kubernetes / GitHub Actions (Staging/Production)

1. **Workflow env var**:
   ```yaml
   AUTH_API_KEY_SECRET: ${{ github.ref == 'refs/heads/main' && secrets.PRODUCTION_AUTH_API_KEY_SECRET || secrets.STAGING_AUTH_API_KEY_SECRET }}
   ```
2. **`.kubernetes/auth-api-key-secret.yaml`** (new file):
   ```yaml
   apiVersion: v1
   kind: Secret
   metadata:
     name: auth-api-key-secret
     namespace: %KUBERNETES_NAMESPACE%
   type: Opaque
   stringData:
     apikey-secret: %AUTH_API_KEY_SECRET%
   ```
   Apply it in the `Kubernetes Deploy` step, same `Get-Content | ExpandEnvironmentVariables |
   kubectl apply` pattern as every other secret - before `deployment.yaml`/`stateful-set.yaml`.
   Unlike `auth-jwt-secret.yaml`, this one is per-app, not shared across services - every app
   with API-key auth creates and applies its own.
3. **`.kubernetes/deployment.yaml`** env entry:
   ```yaml
   - name: Data__Identity__ApiKey__Secret
     valueFrom:
       secretKeyRef:
         name: auth-api-key-secret
         key: apikey-secret
   ```
   The env var name has a trailing `__Secret` - verify against an existing `deployment.yaml` in
   the project if one has this wired already; a stale Lessons README once dropped that suffix,
   so don't copy it from documentation without cross-checking a real manifest.

## After making the change

- Show the user every file touched.
- State plainly which mode this app ended up in - pure API-key (no controller, no login step) or
  paired with existing JWT (`/auth/login/apikey` now live) - from step 3. Don't leave this
  implicit; it's the one thing genuinely worth double-checking landed as intended.
- If step 1 stopped the skill early for a missing Identity prerequisite, that's the whole
  response.
