---
mode: agent
description: Remove Nano's built-in JWT authentication (App:Authentication:Jwt) from a Nano.Library-based application - unregisters the Jwt configuration across every environment, deletes the AuthController, and removes the Staging/Production key secret/CI wiring. Use when the user asks to remove login, sign-in, or JWT authentication from a Nano API or Web application - not for removing API-key authentication by itself, that's nano-remove-authentication-apikey.
---

# Nano remove JWT authentication

Fully removes Nano's JWT authentication from an existing Nano API/Web application - the
counterpart to `nano-add-authentication-jwt`. Read that skill first - this one undoes exactly
what it adds.

**The `AuthController` cannot survive this removal, ever - not a judgment call.**
`BaseAuthController`'s constructor requires `IAuthRepository` as a non-nullable parameter, and
`IAuthRepository` is only registered when `Jwt != null` (`AddNanoAuthentication`). Once `Jwt` is
gone, the controller fails DI resolution at startup if left in place. Delete it unconditionally,
even if API-key auth stays configured afterward - see step 3.

## Before making any change, determine

1. **Is JWT authentication currently configured?** Check the base `appsettings.json` for
   `App:Authentication:Jwt`, or an existing `AuthController`. If neither exists, say so and stop.
2. **Is this app the token issuer or a validator-only app?** Check `.kubernetes/deployment.yaml`
   for whether it maps `App__Authentication__Jwt__PrivateKey` (issuer) or `PublicKey` only
   (validator) - this determines which Kubernetes/CI cleanup applies below.
3. **Is API-key authentication also configured** (`Data:Identity:ApiKey:Secret` set)? This
   changes what removing `Jwt` actually does to the app, and needs surfacing before proceeding:
   - **Not configured**: this app ends up with no authentication at all - every endpoint becomes
     anonymous by default (AGENTS.md: "If no authentication schemes has been configured, all
     endpoints will be accessible anonymously"). Confirm this is intended before proceeding; it's
     a real security-relevant change, not just a code cleanup.
   - **Also configured**: the app doesn't lose authentication - it reverts to **pure API-key
     mode** (per `nano-add-authentication-apikey`), since `ApiKeyAuthenticationHandler` doesn't
     depend on `Jwt` at all. The `AuthController` still gets deleted (per the note above), and
     `/auth/login/apikey` disappears with it - API-key callers keep working unchanged, but lose
     the "trade a key for a JWT once" convenience. Tell the user this explicitly; don't let it
     read as a side effect they weren't told about.
4. **Custom controller logic?** Open `AuthController.cs` before deleting it - if it's still just
   the one-line subclass `nano-add-authentication-jwt` generates, delete freely. If someone added
   custom actions to it since, stop and confirm with the user before removing their code.

## appsettings.json

Remove `App:Authentication:Jwt` (the whole object, including `RootLogin`/`ExternalLogins` if
present) from the base `appsettings.json`, `appsettings.Development.json`,
`appsettings.Staging.json`, and `appsettings.Production.json` - every environment file that has
it, per `nano-add-authentication-jwt`'s placement (`Issuer`/`Audience` overrides live in every
environment file, not just Development).

## AuthController

Delete `Controllers/AuthController.cs` - see the note at the top; this isn't conditional.

## Kubernetes / GitHub Actions - issuer app only

If step 2 found this app is the issuer:

- Delete `.kubernetes/auth-jwt-secret.yaml`.
- Remove its apply step from the `Kubernetes Deploy` workflow step.
- Remove the `AUTH_JWT_PUBLIC_KEY`/`AUTH_JWT_PRIVATE_KEY` workflow env vars.
- If this app was the **only** issuer in the solution, every validator-only app that references
  `auth-jwt-secret` now points at a secret nothing creates anymore - flag this to the user
  explicitly; it's outside this skill's scope (a different app's files), but silently leaving it
  broken elsewhere is worse than mentioning it.

## Kubernetes - every app (issuer and validator)

Remove the `App__Authentication__Jwt__PublicKey`/`App__Authentication__Jwt__PrivateKey` entries
(whichever are present - a validator only ever has `PublicKey`) from
`.kubernetes/deployment.yaml`'s container `env`.

## After making the change

- Show the user every file touched/deleted.
- Restate step 3's outcome one more time now that it's actually done - either "this app now has
  no authentication, every endpoint is anonymous" or "this app is now in pure API-key mode, the
  JWT exchange endpoint is gone" - whichever applies. This is the one thing most worth a second,
  explicit confirmation rather than folding into a file list.
- If step 2 found this app was the sole issuer, restate the warning about now-broken
  validator-only apps elsewhere in the solution.
