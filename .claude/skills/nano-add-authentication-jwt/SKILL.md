---
name: nano-add-authentication-jwt
description: Configure Nano's built-in JWT authentication (App:Authentication:Jwt) on a Nano.Library-based API/Web application - adds the Jwt configuration, the AuthController, the Development key setup, and (for the token-issuing app) the Staging/Production key-generation and Kubernetes secret. Use when the user asks to add login, sign-in, or JWT authentication to a Nano API or Web application - not for adding a user store by itself (that's nano-add-identity) or for API-key authentication by itself (that's nano-add-authentication-apikey, which works standalone without any of this).
---

# Nano add JWT authentication

Configures Nano's built-in JWT authentication on an existing Nano API/Web application. Read
`AGENTS.md`'s `#### Authentication` section first — it documents the full `Configuration` table,
the `AuthController`'s sub-repository table, and the persistent-vs-transient distinction in
detail; this skill does not repeat that, only how to apply it.

**This skill is JWT-specific.** API-key authentication is a genuinely independent auth mode in
Nano — it does not require `Jwt` at all, has no `AuthController`, and is `nano-add-authentication-apikey`'s
job, not this one. See step 6 below for what happens when both are configured on the same app.

**No `Program.cs` registration call.** Unlike every other add-provider skill, Authentication is
pure config plus one controller — `IAuthRepository`'s sub-repositories self-populate based on
whichever config sections exist (`Jwt.RootLogin` → root login, [Identity](nano-add-identity) →
persistent login, `Jwt.ExternalLogins` with no Identity → transient login). There's nothing to
add to `.ConfigureServices(...)`.

**Two request shapes.** A request naming this skill is one of:
1. **Persistent auth** — `Jwt` config + `AuthController` on top of already-configured Identity.
2. **Transient auth** — `Jwt` config + `AuthController`, but with `Jwt.ExternalLogins` instead of
   Identity.
Figure out which one applies before touching anything — steps 1–5 below are how. Either can be
layered with API-key auth by running `nano-add-authentication-apikey` before or after this skill — see
step 6.

These two aren't the only combination — `Jwt.ExternalLogins` can also layer on top of already-configured
Identity (e.g. "sign in with Google" but the account is still persistent, not transient). See
AGENTS.md's sub-repository table for exactly how `AuthExternalRepositoryAggregator` resolves which
repository backs a given external login in that case — not repeated here.

## Before making any change, determine

1. **Does this app issue tokens, or only validate them?** Ask if the request doesn't say.
   - **Issuer**: needs both `PublicKey` and `PrivateKey` in Staging/Production, and creates the
     `auth-jwt-secret` Kubernetes secret from real GitHub secrets.
   - **Validator-only**: needs only `PublicKey` in Staging/Production, and must **not** create or
     re-apply the secret — it references the one the issuer app already created. See the
     Kubernetes section below; getting this backwards silently corrupts the shared secret with
     unexpanded placeholder values — a real bug found and fixed this way in this codebase, so
     don't repeat it.
   - This distinction **does not apply to Development** — see below.
2. **Persistent or transient auth?** Check whether [Identity](nano-add-identity) (`Data:Identity`)
   is already configured.
   - **Persistent** (Identity present): `AuthIdentityRepository` auto-populates and backs
     `/auth/login`, `/auth/login/refresh`, `/auth/logout` — nothing further to wire beyond the
     `Jwt` config and controller below. **This combination (persistent auth + `AuthController`)
     is an internal-service-only pattern** — per AGENTS.md's [Controllers § Public API vs
     internal service](#public-api-vs-internal-service), a genuine Public API has no `IRepository`
     of its own, so it can't have Identity configured in the first place. If this app is meant to
     be a Public API, stop: it shouldn't have Identity here at all — see `nano-add-identity`'s own
     warning on this, and point the user at composing through the owning internal service's Api
     Client instead.
   - **Transient** (no Identity): needs `Jwt.ExternalLogins` configured (built-in Facebook/
     Google/Microsoft, or a custom provider — see "External Login" below) — ask which, and
     whether a custom provider implementation is needed, before proceeding. **Also ask whether
     this app needs to assert its own server-computed claims/roles on top of the external login**
     (e.g. an `IsAdmin` flag) — if so, see the "AuthController" section's warning below before
     scaffolding a generic `AuthController`; adding one unconditionally here can open a
     caller-controlled claim-injection endpoint.
   - If the user wants persistent auth but Identity isn't registered yet, stop and point them at
     `nano-add-identity` first.
3. **Is Authentication already configured?** Check the base `appsettings.json` for
   `App:Authentication:Jwt`, or an existing `AuthController`. If present, say so before changing
   anything.
4. **Application type.** The `AuthController` is API/Web only — a Console app has no HTTP surface
   to expose it on. A Console app can still have `Jwt` configured, but only for its own outbound
   Api Client authentication (`Apis:{Client}:LogInRoot`, a different, already-documented AGENTS.md
   concern) — not something this skill scaffolds a controller for.
5. **Is this a Console app whose only use of `Jwt` is outbound Api Client auth?** Skip the
   controller step below in that case, per step 4.
6. **Is API-key authentication already configured** (`Data:Identity:ApiKey:Secret` set)? Check
   the base `appsettings.json`. If so, this app was previously in pure API-key-only mode — no
   `Jwt`, no `AuthController` (see `nano-add-authentication-apikey`: that controller would fail to resolve
   `IAuthRepository` without `Jwt` configured, so it genuinely didn't exist yet). Adding `Jwt` now
   changes two things **automatically, from config alone** — nothing extra to build, but tell the
   user about both:
   - Nano's scheme selection (`AddNanoAuthentication`, based on whether `Jwt`/`ApiKeyOptions` are
     each present) switches from API-key-only to `JWT_OR_APIKEY` — existing API-key callers keep
     working unchanged, requests carrying a JWT `Authorization` header now also work.
   - The `AuthController` this skill adds will immediately expose `/auth/login/apikey` (its
     visibility is gated purely on `Data:Identity:ApiKey:Secret` being set, per
     `ConditionalActionsConvention`) — callers can now trade an API key for a JWT once instead of
     presenting the key on every request.

## appsettings.json — Jwt

Base `appsettings.json` (sibling of `App:Version`/`App:Hosting`, per AGENTS.md's `##### Configuration`
example): `Issuer`/`Audience`/`PublicKey`/`PrivateKey` all `null`, `Expiration`/`RefreshExpiration`
at their framework defaults (`01:00:00`/`72:00:00`). Leave `RootLogin`/`ExternalLogins` out
entirely unless configuring them now — they're opt-in additions, not blank placeholders.

**`appsettings.Development.json` — use the existing shared key pair, don't generate a new one.**
Every app in this codebase (issuers and validators alike) uses the exact same hardcoded RSA key
pair locally:

```json
"App": {
  "Authentication": {
    "Jwt": {
      "Issuer": "nano.development",
      "Audience": "nano.development",
      "PublicKey": "MIIBCgKCAQEAv7iVNUS5wT7Fvg/hkmlvvPnOW7Rcyh7dFStJSTtM+7f74+GGVJLl6spXasnsQ7v6rw7vlyb+uVk1UaQsUA38luSNGWfPqc3JAtkeJPWCu1kN79Yo3im7Qx6B1u4gf0AR3n86ClQGz3O5Jxo8M3+zlwveYnlf6bqhBakOVdPS5tX0Bvh/F9lXiEF53EZEcfuHjBjDLik9PUdTjqehPLCPyI1/FbfE8P1Y4S7AEfs2fIqXGxJNXDyoDRvi42qefqXcsmzBUDYtHqvwSHWcDn5DXDRY2FYkyESMvd7RRGwI6U0g8V9k3Qudd4LjQTs8LdBu5u25wvqx37Y1518BPqGQkQIDAQAB",
      "PrivateKey": "MIIEowIBAAKCAQEAv7iVNUS5wT7Fvg/hkmlvvPnOW7Rcyh7dFStJSTtM+7f74+GGVJLl6spXasnsQ7v6rw7vlyb+uVk1UaQsUA38luSNGWfPqc3JAtkeJPWCu1kN79Yo3im7Qx6B1u4gf0AR3n86ClQGz3O5Jxo8M3+zlwveYnlf6bqhBakOVdPS5tX0Bvh/F9lXiEF53EZEcfuHjBjDLik9PUdTjqehPLCPyI1/FbfE8P1Y4S7AEfs2fIqXGxJNXDyoDRvi42qefqXcsmzBUDYtHqvwSHWcDn5DXDRY2FYkyESMvd7RRGwI6U0g8V9k3Qudd4LjQTs8LdBu5u25wvqx37Y1518BPqGQkQIDAQABAoIBAEwNH3sS+RCUIwLC7/sRQhbXjSlJgalX1uFH23lmQaJ0mEIMOyofX37kpwqgcM1pqwZ4SUhPWqoRnhn1ovJaqgD9Ro92Y6T7EarEj7Wfgi1pJSMnc+y05yi32E93BIMV2kDFfTONo2n1gNPnD0xqcsYPGjc76HUh6DADoMEhFr8kHaz4J2daKV0tJjApNt2oabk8BLQEq9Uv22DsLfL+nEOHPhSMk7EmNv3QQgUNH5ugeDNfTNr+A6K8YMbVVrmDalZS/GBWSscnJ9Ma2WrHJ/x2IRQECVMf6U05vrgtKb9imPcN09ccItIzcK/8ZBbSw2v+Gzf1Je447SYT9njAOiUCgYEAzOAsty4cxCLSWt2GBTE58MoThNeiVRBvc6Gw5B1olCCnWkVxRDYwYlPnwvemqa+YsfijrjVkuS0kJmfrGJ/MkV8Wsx2XL6mRyCBXOUog0U/Nh20ANU8kcmEMkGVtxDUM8hr9QQ5qex/LmSiy8YG4c4mfD6s7KvWnRxJcviXmgUMCgYEA75AQssujQtycWx6fZ/aBQLc6+xSlGaY73k2R8XLwMSASAeq1erxCSsuPF5lPRnQ4VZyfSOV9AcOyLgeJCi4ePJEnfZZMcGkKNt2yMsZoWUlJSmHIXhEEfKqu8Qo0TRu4/vQYPKwTVXdpbZJIlDgzztPdC1gOpCg3QQH16wPyL5sCgYAQ5Ygqj14F+w04Oz7bXMT3i+LyOMqFk3Ztpe8t0RMX7F2A/2spAgMZiOv7U2tmYToJq4TsUDD/aK6rkDR+cmdvsdTwbsdSQfzo8WngKrHsMVW1DpNO0jkiSci8e/EClpF7wigS3np/rw6ekhG4A0fQF5CLvUaC84GZRfVqJTwOewKBgQC4oTKNae54oGgMvewjBtOU2eKmEcIwo3JuoSACkw/U/J+ERKz7W85HsNymVmzHotir+pq0ZtHSI03Wtc4DP4nkKgbifoyI8huCL5igE1PmxFms7vGqtbjcj/tmH/QxHVWVgPCRChmYfACQBvbS7QHYvGYW0RXvpGL5QhaSuybTUwKBgG/p/gsj6yUDAiNhEWpSsMWl/3xJeIobcnH1XQrrXWIzL1xZtX1EkcqLM6++Ojjre3UKj96ZDFRpJH4uxTilE9MDOOf+PLoL01rr1rmzaWDr5NsI3nqz2AS6ZSuofO0rs7nQlKtTnQY0vlzPGqfQp4uQ11KPzO2PB9TEGwnZy5HV",
      "Expiration": "24:00:00"
    }
  }
}
```

- **`PrivateKey` goes here even on a validator-only app**, if the app also configures
  `Jwt.RootLogin` for isolated local testing (the common case for an internal service — AGENTS.md:
  "useful in Development when testing a service in isolation"). Root login self-issues a JWT,
  which needs a private key regardless of the app's Staging/Production role. Only omit
  `PrivateKey` in Development for an app that genuinely never self-issues locally (e.g. a
  pure Public API with no isolated-testing story of its own).
- `Expiration: "24:00:00"` (vs. the base file's `01:00:00`) is the established convention for
  Development — longer-lived tokens are less annoying to work with locally. Not required, but
  match it unless the user asks otherwise.
- Add a `RootLogin` block (`Username`/`Password`) alongside `Jwt` in Development if this app
  should support isolated local testing — ask for credentials, or use a placeholder like
  `admin@domain.com` / a throwaway password if the user doesn't care.

**`appsettings.Staging.json` / `appsettings.Production.json`** — only `Issuer`/`Audience`
overrides, no keys (those come from the Kubernetes secret, never a static file):

```json
"App": { "Authentication": { "Jwt": { "Issuer": "nano.staging", "Audience": "nano.staging" } } }
```
```json
"App": { "Authentication": { "Jwt": { "Issuer": "nano.production", "Audience": "nano.production" } } }
```

## AuthController (API/Web only)

**Stop and check this before scaffolding it — it's not always safe to add.** `BaseAuthController`'s
`login`/`login/external` actions bind `TransientClaims`/`TransientRoles` straight from the request
body and merge them **verbatim, with no server-side filtering,** into the minted JWT
(`AuthTransientRepository.LogInExternalAsync`/`BaseAuthIdentityRepository.LogInAsync`/
`LogInExternalAsync`) — this applies to **both persistent and transient auth**, not just transient.
Concretely: adding this controller means **any caller who can reach it can post
`{"transientClaims": {"IsAdmin": "true"}}` at login and receive back a validly-signed token carrying
that claim** — nothing here validates or restricts which claims/roles a caller may assert about
themselves. Refresh does not have this problem: `login/refresh`/the transient external-login
refresh never accept claims/roles from the caller at all — they're always recovered from a manifest
claim embedded at login, so a refresh can never grant more than the original login already did (see
`ClaimTypesExtended.TransientClaimsManifest`/the internal `TransientClaimsManifest` class in
`Nano.Data.Abstractions`). The transient refresh endpoint
(`/auth/login/external/{providerName}/transient/refresh`) also takes no request body at all - the
token being refreshed comes from the Authorization header. The risk below is specific to login, and
to whoever can reach `AuthController` at all.

- **If this app needs to compute its own claims server-side** (an `IsAdmin` flag, an internal
  role, anything not meant to be caller-assignable) **at login, don't add this controller at all.**
  Write a custom controller instead (derive it from this app's own base controller, *not*
  `BaseAuthController`) that calls `IAuthExternalRepositoryAggregator`/`IAuthTransientRepository`/
  `IAuthIdentityRepository` directly and builds the claims/roles itself from trusted data — never
  from caller input. This is a real, load-bearing pattern in this codebase, not a hypothetical — see
  `Api.Admin`'s `AccountsController` (deriving its own `BaseAdminController`), which implements
  `login/microsoft`/`login/refresh`/`me` by hand for exactly this reason.
- Nano additionally auto-maps built-in transient external-login endpoints
  (`/auth/login/external/{provider}/transient` and its `/refresh` counterpart) whenever *any*
  `BaseAuthController`-derived class exists in the app **and** no Identity is configured — see
  `ServiceScopeExtensions.UseNanoEndpoints`'s `!hasIdentity && hasAuthController` gate, checked by
  type scan, not by whether this specific controller is the one deriving it. This is an extra
  exposure specific to transient auth: it means a custom controller alone isn't enough to shield a
  transient app unless `hasAuthController` also stays `false` (i.e. no `BaseAuthController`-derived
  class anywhere in the app) — `Api.Admin`'s custom controller works precisely because it doesn't
  derive `BaseAuthController`. The `/refresh` counterpart is auto-mapped under the same gate, but
  isn't a caller-trust risk the way login is - see above.
- **This risk is sharpest on a publicly-exposed app** (anyone on the internet can reach the
  endpoint), but don't treat an internal-only app as automatically safe either — anything that lets
  a caller assign its own JWT claims is worth a deliberate decision, not a default. `AuthController`
  is an internal-service-only pattern to begin with (see AGENTS.md's [Controllers § Public API vs
  internal service](#public-api-vs-internal-service)), so "internal-only" is the floor, not a reason
  to skip the decision.
- If none of the above applies — no need for server-computed claims beyond what the external
  provider itself asserts, and whoever can reach this app's `AuthController` is already trusted to
  assert login-time claims — the generic controller below is fine as-is.

`Controllers/AuthController.cs`, main app project:

```csharp
public class AuthController(ILogger<AuthController> logger, IAuthRepository authRepository)
    : BaseAuthController(logger, authRepository);
```

Nothing to implement — every endpoint the current config enables (per AGENTS.md's sub-repository
table) is provided. For a non-`Guid` identity type, use `BaseAuthController<TIdentity>` and
`IAuthRepository<TIdentity>` to match (same rule as every other controller in this ecosystem).

## External Login (`Jwt.ExternalLogins`)

Only relevant if step 2 found external login in play — either the transient case, or the hybrid
persistent-plus-external-login case noted above. Two genuinely different kinds of work, not one:

**Built-in provider (Facebook / Google / Microsoft) — pure config, no code.** Add the matching
block under `Jwt.ExternalLogins` in the base `appsettings.json`, per AGENTS.md's `##### Configuration`
table (`Facebook.AppId`/`.AppSecret`/`.Scopes`, `Google.ClientId`/`.ClientSecret`/`.Scopes`,
`Microsoft.TenantId`/`.ClientId`/`.ClientSecret`/`.Scopes`). Treat `AppSecret`/`ClientSecret` as
real secrets, the same class of value as the JWT keys above — `null` in the base file, a real
value only where it's actually safe to have one.

- **Microsoft has its own skill, `nano-add-authentication-microsoft`** — it's the one built-in
  provider whose credentials can be scripted (an Entra ID app registration via the Azure CLI), so
  it has an established, self-rotating Kubernetes-secret/GitHub-Actions convention. If the request
  names Microsoft specifically, use that skill instead of configuring `Jwt.ExternalLogins.Microsoft`
  by hand here.
- **Facebook/Google have no such convention.** Their credentials are created by hand through each
  provider's own developer console — don't invent a Kubernetes/GitHub-secret pattern for them; ask
  the user how they want it stored for Staging/Production rather than assuming one exists.
- **Facebook logins can never be refreshed — don't offer an `offline_access`-style option for it.**
  `AuthExternalFacebookRepository.AuthenticateRefreshAsync` unconditionally throws, regardless of
  config, yet `.../transient/refresh` is still auto-mapped for every registered provider and will
  always 401 for Facebook. If the user asks for refresh support on a Facebook login, say plainly
  that it isn't possible with the built-in provider rather than looking for a config option that
  doesn't exist. Google and Microsoft, by contrast, are both refreshable — see AGENTS.md's
  `#### Authentication` table.
- **`Facebook.Scopes`/`Google.Scopes` are frontend-only — setting them here does nothing server-side.**
  Neither repository reads `options.Scopes` at all; scope negotiation happens in the client-side SDK
  (Facebook) or the frontend's own authorize-URL redirect (Google) before Nano ever sees the
  request. Still add them to config for documentation purposes if the user gives specific scopes,
  but don't imply this app's config is what actually requests them — for Google specifically,
  refresh support also needs the frontend's authorize request to include `access_type=offline`/
  `prompt=consent`, which has nothing to do with this `Scopes` entry either.

**Custom provider — real code, no config entry.** Per AGENTS.md's `##### Custom external provider`,
this is auto-discovered by type, not registered via `Jwt.ExternalLogins` config the way built-in
providers are — there's no appsettings.json entry for it at all.

1. **`TFlow`.** `ImplicitFlow` or `AuthCodeFlow` (both derive `BaseAuthFlow`) — pick whichever
   matches the provider's actual OAuth flow; ask if unclear rather than guessing. Derive a custom
   `BaseAuthFlow` subclass instead only if the provider's flow doesn't fit either built-in shape.
2. **The class**, conventionally `Auth/{Provider}ExternalRepository.cs` in the application
   project (discovery is by type, so the location isn't enforced):
   ```csharp
   public class MyExternalRepository() : BaseAuthExternalRepository<ImplicitFlow>("MyProvider")
   {
       public override async Task<ExternalAuthenticationData> AuthenticateAsync(ImplicitFlow flow, CancellationToken cancellationToken = default)
       {
           // call the external provider, map its response to ExternalAuthenticationData
           return new ExternalAuthenticationData
           {
               Id = "external-id",
               Username = "MyUser",
               EmailAddress = "user@domain.com",
               Name = "My User",
               ExternalToken = new ExternalAuthenticationToken { Name = this.ProviderName, Token = "token", RefreshToken = "refresh-token" }
           };
       }

       public override async Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
       {
           // refresh against the external provider
           return new ExternalAuthenticationToken { Name = this.ProviderName, Token = "token", RefreshToken = "refresh-token" };
       }
   }
   ```
   The constructor's string argument (`"MyProvider"` above) is `ProviderName` — this is what
   `AuthExternalRepositoryAggregator` resolves against, and what appears in the
   `/auth/login/external/{providerName}/...` route, so ask the user what they want it called
   rather than defaulting to the class name.
3. **Whatever credentials/endpoint the provider itself needs** (API key, base URL, etc.) — these
   are this custom repository's own concern, not `Jwt.ExternalLogins`'s. Add them as an options
   class bound from whatever config section makes sense for this provider (same pattern as any
   other custom service in this codebase), then inject it into the repository's constructor. Don't
   try to route them through `Jwt.ExternalLogins` — that section is exclusively for the three
   built-in providers.

Either way, the actual login endpoint this exposes is
`/auth/login/external/{providerName}/transient` when Identity isn't configured, or the
persistent equivalent per AGENTS.md's sub-repository table when it is — this skill doesn't scaffold
that call site, only the repository/config that backs it.

## Kubernetes / GitHub Actions (Staging/Production) — issuer app only

Only the app that **issues** tokens does this. A validator-only app does **not** create or
re-apply this secret — it only references the `auth-jwt-secret` the issuer already created (see
its `deployment.yaml` entry below). Re-applying it from an app with no real key values set pushes
unexpanded placeholder text into the shared secret, silently corrupting the real one — don't do
it for any app but the issuer.

1. **Workflow env vars**:
   ```yaml
   AUTH_JWT_PUBLIC_KEY: ${{ github.ref == 'refs/heads/main' && secrets.PRODUCTION_AUTH_JWT_PUBLIC_KEY || secrets.STAGING_AUTH_JWT_PUBLIC_KEY }}
   AUTH_JWT_PRIVATE_KEY: ${{ github.ref == 'refs/heads/main' && secrets.PRODUCTION_AUTH_JWT_PRIVATE_KEY || secrets.STAGING_AUTH_JWT_PRIVATE_KEY }}
   ```
2. **`.kubernetes/auth-jwt-secret.yaml`** (new file, issuer app only):
   ```yaml
   apiVersion: v1
   kind: Secret
   metadata:
     name: auth-jwt-secret
     namespace: %KUBERNETES_NAMESPACE%
   type: Opaque
   stringData:
     jwt-public-key: %AUTH_JWT_PUBLIC_KEY%
     jwt-private-key: %AUTH_JWT_PRIVATE_KEY%
   ```
   Apply it in the `Kubernetes Deploy` step, before `deployment.yaml`/`stateful-set.yaml`. Also
   add `.kubernetes\auth-jwt-secret.yaml = .kubernetes\auth-jwt-secret.yaml` to `{name}.sln`'s
   `.kubernetes` `SolutionItems` block (see AGENTS.md's Solution Structure note) — new files
   under `.kubernetes/` don't show up in Visual Studio's Solution Explorer otherwise.

## Kubernetes — deployment.yaml

Reference the secret in `.kubernetes/deployment.yaml`'s container `env` — **the two app types get
different entries here, not the same block with one line dropped**:

Issuer app (both keys):
```yaml
- name: App__Authentication__Jwt__PublicKey
  valueFrom:
    secretKeyRef:
      name: auth-jwt-secret
      key: jwt-public-key
- name: App__Authentication__Jwt__PrivateKey
  valueFrom:
    secretKeyRef:
      name: auth-jwt-secret
      key: jwt-private-key
```

Validator-only app (`PublicKey` only — no `PrivateKey` entry at all):
```yaml
- name: App__Authentication__Jwt__PublicKey
  valueFrom:
    secretKeyRef:
      name: auth-jwt-secret
      key: jwt-public-key
```

## API-key authentication

Not this skill's job — see `nano-add-authentication-apikey`, which works whether or not `Jwt` is
configured on this app. If the user asked for both in one request, run both skills; step 6 above
covers the one thing each needs to know about the other.

## Generating real keys (Staging/Production, or a deliberate Development change)

Never hardcode Staging/Production keys — generate a unique pair and store both halves as
GitHub secrets (`{ENVIRONMENT}_AUTH_JWT_PUBLIC_KEY`/`_PRIVATE_KEY`), consumed only via the
Kubernetes secret above. Generate with a throwaway Console app (from AGENTS.md /
`Nano.App.Api/README.md`'s `## Authentication` section):

```csharp
using System.Security.Cryptography;

using var rsa = RSA.Create();

var publicKey = rsa
    .ExportRSAPublicKeyPem()
    .Replace("-----BEGIN RSA PUBLIC KEY-----", "")
    .Replace("-----END RSA PUBLIC KEY-----", "")
    .Replace("\n", string.Empty);

var privateKey = rsa
    .ExportRSAPrivateKeyPem()
    .Replace("-----BEGIN RSA PRIVATE KEY-----", "")
    .Replace("-----END RSA PRIVATE KEY-----", "")
    .Replace("\n", string.Empty);

Console.WriteLine("PUBLIC KEY:");
Console.WriteLine(publicKey);
Console.WriteLine();
Console.WriteLine("PRIVATE KEY:");
Console.WriteLine(privateKey);

Console.Read();
```

## After making the change

- Show the user every file touched, grouped by concern (appsettings per environment, the
  controller, and — for the issuer app — Staging/Production CI + K8s), plus the external-login
  repository class if one was scaffolded.
- If this is transient auth with external login and a generic `AuthController` was added, restate
  explicitly that `/auth/login/external/{provider}/transient` is now live and accepts
  caller-supplied `TransientClaims`/`TransientRoles` verbatim — confirm that's actually acceptable
  for this app before considering the task done. If a custom controller was used instead specifically
  to avoid this, say so, and confirm it does **not** derive `BaseAuthController` anywhere in the app.
- Point them at the snippet above for generating real Staging/Production keys — never the
  hardcoded Development pair.
- If they want to change the Development key pair from the shared default, warn explicitly: it
  must change **identically across every app** in the solution, or apps stop being able to
  validate each other's locally-issued tokens.
- If step 2 stopped the skill early for a missing Identity prerequisite, that's the whole
  response — don't partially wire persistent auth while waiting on it.
- If step 6 applied (API-key was already configured), restate the automatic scheme-switch and
  the newly-visible `/auth/login/apikey` endpoint one more time — it's a real behavior change on
  an app that already had callers, worth a second confirmation, not just a note in passing.
