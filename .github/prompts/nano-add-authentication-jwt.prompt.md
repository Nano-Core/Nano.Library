---
mode: agent
description: Configure Nano's built-in JWT authentication (App:Authentication:Jwt) on a Nano.Library-based API/Web application - adds the Jwt configuration, the AuthController, the Development key setup, and (for the token-issuing app) the Staging/Production key-generation and Kubernetes secret. Use when the user asks to add login, sign-in, or JWT authentication to a Nano API or Web application - not for adding a user store by itself (that's nano-add-identity) or for API-key authentication by itself (that's nano-add-authentication-apikey, which works standalone without any of this).
---

# Nano add JWT authentication

Configures Nano's built-in JWT authentication on an existing Nano API/Web application. Read
`AGENTS.md`'s `#### Authentication` section first - it documents the full `Configuration` table,
the `AuthController`'s sub-repository table, and the persistent-vs-transient distinction in
detail; this skill does not repeat that, only how to apply it.

**This skill is JWT-specific.** API-key authentication is a genuinely independent auth mode in
Nano - it does not require `Jwt` at all, has no `AuthController`, and is `nano-add-authentication-apikey`'s
job, not this one. See step 6 below for what happens when both are configured on the same app.

**No `Program.cs` registration call.** Unlike every other add-provider skill, Authentication is
pure config plus one controller - `IAuthRepository`'s sub-repositories self-populate based on
whichever config sections exist (`Jwt.RootLogin` → root login, [Identity](nano-add-identity) →
persistent login, `Jwt.ExternalLogins` with no Identity → transient login). There's nothing to
add to `.ConfigureServices(...)`.

**Two request shapes.** A request naming this skill is one of:
1. **Persistent auth** - `Jwt` config + `AuthController` on top of already-configured Identity.
2. **Transient auth** - `Jwt` config + `AuthController`, but with `Jwt.ExternalLogins` instead of
   Identity.
Figure out which one applies before touching anything - steps 1–5 below are how. Either can be
layered with API-key auth by running `nano-add-authentication-apikey` before or after this skill - see
step 6.

## Before making any change, determine

1. **Does this app issue tokens, or only validate them?** Ask if the request doesn't say.
   - **Issuer**: needs both `PublicKey` and `PrivateKey` in Staging/Production, and creates the
     `auth-jwt-secret` Kubernetes secret from real GitHub secrets.
   - **Validator-only**: needs only `PublicKey` in Staging/Production, and must **not** create or
     re-apply the secret - it references the one the issuer app already created. See the
     Kubernetes section below; getting this backwards silently corrupts the shared secret with
     unexpanded placeholder values - a real bug found and fixed this way in this codebase, so
     don't repeat it.
   - This distinction **does not apply to Development** - see below.
2. **Persistent or transient auth?** Check whether [Identity](nano-add-identity) (`Data:Identity`)
   is already configured.
   - **Persistent** (Identity present): `AuthIdentityRepository` auto-populates and backs
     `/auth/login`, `/auth/login/refresh`, `/auth/logout` - nothing further to wire beyond the
     `Jwt` config and controller below.
   - **Transient** (no Identity): needs `Jwt.ExternalLogins` configured (built-in Facebook/
     Google/Microsoft, or a custom provider per AGENTS.md's `##### Custom external provider`) -
     ask which, and whether a custom provider implementation is needed, before proceeding.
   - If the user wants persistent auth but Identity isn't registered yet, stop and point them at
     `nano-add-identity` first.
3. **Is Authentication already configured?** Check the base `appsettings.json` for
   `App:Authentication:Jwt`, or an existing `AuthController`. If present, say so before changing
   anything.
4. **Application type.** The `AuthController` is API/Web only - a Console app has no HTTP surface
   to expose it on. A Console app can still have `Jwt` configured, but only for its own outbound
   Api Client authentication (`Apis:{Client}:LogInRoot`, a different, already-documented AGENTS.md
   concern) - not something this skill scaffolds a controller for.
5. **Is this a Console app whose only use of `Jwt` is outbound Api Client auth?** Skip the
   controller step below in that case, per step 4.
6. **Is API-key authentication already configured** (`Data:Identity:ApiKey:Secret` set)? Check
   the base `appsettings.json`. If so, this app was previously in pure API-key-only mode - no
   `Jwt`, no `AuthController` (see `nano-add-authentication-apikey`: that controller would fail to resolve
   `IAuthRepository` without `Jwt` configured, so it genuinely didn't exist yet). Adding `Jwt` now
   changes two things **automatically, from config alone** - nothing extra to build, but tell the
   user about both:
   - Nano's scheme selection (`AddNanoAuthentication`, based on whether `Jwt`/`ApiKeyOptions` are
     each present) switches from API-key-only to `JWT_OR_APIKEY` - existing API-key callers keep
     working unchanged, requests carrying a JWT `Authorization` header now also work.
   - The `AuthController` this skill adds will immediately expose `/auth/login/apikey` (its
     visibility is gated purely on `Data:Identity:ApiKey:Secret` being set, per
     `ConditionalActionsConvention`) - callers can now trade an API key for a JWT once instead of
     presenting the key on every request.

## appsettings.json - Jwt

Base `appsettings.json` (sibling of `App:Version`/`App:Hosting`, per AGENTS.md's `##### Configuration`
example): `Issuer`/`Audience`/`PublicKey`/`PrivateKey` all `null`, `Expiration`/`RefreshExpiration`
at their framework defaults (`01:00:00`/`72:00:00`). Leave `RootLogin`/`ExternalLogins` out
entirely unless configuring them now - they're opt-in additions, not blank placeholders.

**`appsettings.Development.json` - use the existing shared key pair, don't generate a new one.**
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
  `Jwt.RootLogin` for isolated local testing (the common case for an internal service - AGENTS.md:
  "useful in Development when testing a service in isolation"). Root login self-issues a JWT,
  which needs a private key regardless of the app's Staging/Production role. Only omit
  `PrivateKey` in Development for an app that genuinely never self-issues locally (e.g. a
  pure public-facing gateway with no isolated-testing story of its own).
- `Expiration: "24:00:00"` (vs. the base file's `01:00:00`) is the established convention for
  Development - longer-lived tokens are less annoying to work with locally. Not required, but
  match it unless the user asks otherwise.
- Add a `RootLogin` block (`Username`/`Password`) alongside `Jwt` in Development if this app
  should support isolated local testing - ask for credentials, or use a placeholder like
  `admin@domain.com` / a throwaway password if the user doesn't care.

**`appsettings.Staging.json` / `appsettings.Production.json`** - only `Issuer`/`Audience`
overrides, no keys (those come from the Kubernetes secret, never a static file):

```json
"App": { "Authentication": { "Jwt": { "Issuer": "nano.staging", "Audience": "nano.staging" } } }
```
```json
"App": { "Authentication": { "Jwt": { "Issuer": "nano.production", "Audience": "nano.production" } } }
```

## AuthController (API/Web only)

`Controllers/AuthController.cs`, main app project:

```csharp
public class AuthController(ILogger<AuthController> logger, IAuthRepository authRepository)
    : BaseAuthController(logger, authRepository);
```

Nothing to implement - every endpoint the current config enables (per AGENTS.md's sub-repository
table) is provided. For a non-`Guid` identity type, use `BaseAuthController<TIdentity>` and
`IAuthRepository<TIdentity>` to match (same rule as every other controller in this ecosystem).

## Kubernetes / GitHub Actions (Staging/Production) - issuer app only

Only the app that **issues** tokens does this. A validator-only app does **not** create or
re-apply this secret - it only references the `auth-jwt-secret` the issuer already created (see
its `deployment.yaml` entry below). Re-applying it from an app with no real key values set pushes
unexpanded placeholder text into the shared secret, silently corrupting the real one - don't do
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
   Apply it in the `Kubernetes Deploy` step, before `deployment.yaml`/`stateful-set.yaml`.

## Kubernetes - every app (issuer and validator)

Reference the secret in `.kubernetes/deployment.yaml`'s container `env` - issuer apps map both
keys, validator-only apps map `PublicKey` only:

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

Drop the `PrivateKey` entry entirely for a validator-only app.

## API-key authentication

Not this skill's job - see `nano-add-authentication-apikey`, which works whether or not `Jwt` is
configured on this app. If the user asked for both in one request, run both skills; step 6 above
covers the one thing each needs to know about the other.

## Generating real keys (Staging/Production, or a deliberate Development change)

Never hardcode Staging/Production keys - generate a unique pair and store both halves as
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
  controller, and - for the issuer app - Staging/Production CI + K8s).
- Point them at the snippet above for generating real Staging/Production keys - never the
  hardcoded Development pair.
- If they want to change the Development key pair from the shared default, warn explicitly: it
  must change **identically across every app** in the solution, or apps stop being able to
  validate each other's locally-issued tokens.
- If step 2 stopped the skill early for a missing Identity prerequisite, that's the whole
  response - don't partially wire persistent auth while waiting on it.
- If step 6 applied (API-key was already configured), restate the automatic scheme-switch and
  the newly-visible `/auth/login/apikey` endpoint one more time - it's a real behavior change on
  an app that already had callers, worth a second confirmation, not just a note in passing.
