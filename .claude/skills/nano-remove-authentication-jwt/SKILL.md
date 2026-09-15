---
name: nano-remove-authentication-jwt
description: Remove Nano's built-in JWT authentication (App:Authentication:Jwt) from a Nano.Library-based application - unregisters the Jwt configuration across every environment, deletes the AuthController, and removes the Staging/Production key secret/CI wiring. Use when the user asks to remove login, sign-in, or JWT authentication from a Nano API or Web application - not for removing API-key authentication by itself, that's nano-remove-authentication-apikey.
---

# Nano remove JWT authentication

Fully removes Nano's JWT authentication from an existing Nano API/Web application — the
counterpart to `nano-add-authentication-jwt`. Read that skill first — this one undoes exactly
what it adds.

**The `AuthController` cannot survive this removal, ever — not a judgment call.**
`BaseAuthController`'s constructor requires `IAuthRepository` as a non-nullable parameter, and
`IAuthRepository` is only registered when `Jwt != null` (`AddNanoAuthentication`). Once `Jwt` is
gone, the controller fails DI resolution at startup if left in place. Delete it unconditionally,
even if API-key auth stays configured afterward — see step 3.

## Before making any change, determine

1. **Is JWT authentication currently configured?** Check the base `appsettings.json` for
   `App:Authentication:Jwt`, or an existing `AuthController`. If neither exists, say so and stop.
2. **Is this app the token issuer or a validator-only app?** Check `.kubernetes/deployment.yaml`
   for whether it maps `App__Authentication__Jwt__PrivateKey` (issuer) or `PublicKey` only
   (validator) — this determines which Kubernetes/CI cleanup applies below.
3. **Is API-key authentication also configured** (`Data:Identity:ApiKey:Secret` set)? This
   changes what removing `Jwt` actually does to the app, and needs surfacing before proceeding:
   - **Not configured**: this app ends up with no authentication at all — every endpoint becomes
     anonymous by default (AGENTS.md: "If no authentication schemes has been configured, all
     endpoints will be accessible anonymously"). Confirm this is intended before proceeding; it's
     a real security-relevant change, not just a code cleanup.
   - **Also configured**: the app doesn't lose authentication — it reverts to **pure API-key
     mode** (per `nano-add-authentication-apikey`), since `ApiKeyAuthenticationHandler` doesn't
     depend on `Jwt` at all. The `AuthController` still gets deleted (per the note above), and
     `/auth/login/apikey` disappears with it — API-key callers keep working unchanged, but lose
     the "trade a key for a JWT once" convenience. Tell the user this explicitly; don't let it
     read as a side effect they weren't told about.
4. **Custom controller logic?** Open `AuthController.cs` before deleting it — if it's still just
   the one-line subclass `nano-add-authentication-jwt` generates, delete freely. If someone added
   custom actions to it since, stop and confirm with the user before removing their code.
5. **Was `RootLogin` wired up for Staging/Production, not just Development?** `nano-add-authentication-jwt`
   only ever adds it as a Development-only convenience by default, but nothing stops someone from
   wiring the full Staging/Production version by hand (per AGENTS.md: an `auth-root-login-secret`
   Kubernetes secret, `{environment}_AUTH_ROOT_LOGIN_USERNAME`/`_PASSWORD` GitHub secrets, and
   `App__Authentication__Jwt__RootLogin__Username`/`Password` in `deployment.yaml`/`cronjob.yaml`).
   Check for `.kubernetes/auth-root-login-secret.yaml` and those deployment env entries — don't
   assume they're absent just because the add-skill doesn't create them by default.
6. **Any custom external-login repository?** Search for classes deriving
   `BaseAuthExternalRepository<TFlow>` (see `nano-add-authentication-jwt`'s "External Login"
   section). These don't fail to compile once `Jwt` is gone — they're plain classes, and
   `AuthExternalRepositoryAggregator`'s registration isn't itself conditional on `Jwt` — but the
   `/auth/login/external/{providerName}/...` route they backed stops existing the moment
   `AuthController` is deleted (step 4's note), since `IAuthRepository`/`AuthController`
   registration is what's actually gated on `Jwt != null`. They become silent dead code, not a
   crash. Don't delete them unasked — they may hold real integration logic worth keeping if `Jwt`
   comes back later — but flag every one found, the same treatment `nano-remove-identity`/
   `nano-remove-eventing-provider` give their own orphaned-code cases. Check its constructor for
   an injected options class too — per the add-skill, a custom provider typically has its own
   config section (API key, base URL, etc.) bound to one; that section becomes equally orphaned
   and is easy to miss since it isn't part of `App:Authentication:Jwt` at all.
7. **Was `Jwt.ExternalLogins` (built-in Facebook/Google/Microsoft) ever configured, and if so, how
   were its `AppSecret`/`ClientSecret` stored for Staging/Production?** Per
   `nano-add-authentication-jwt`, there's no standard Kubernetes/GitHub-secret convention for
   these — the add-skill explicitly asks the user how they want them stored rather than assuming
   one, so whatever exists here is bespoke to this app, not something you can find by checking a
   fixed file/secret name the way `auth-jwt-secret` or `auth-root-login-secret` can be. Search
   `.kubernetes/deployment.yaml` and the workflow for anything referencing
   `App__Authentication__Jwt__ExternalLogins__*` and ask the user to confirm what it is and
   whether it should be removed too — don't assume config-file removal alone caught it.

## appsettings.json

Remove `App:Authentication:Jwt` (the whole object, including `RootLogin`/`ExternalLogins` if
present) from the base `appsettings.json`, `appsettings.Development.json`,
`appsettings.Staging.json`, and `appsettings.Production.json` — every environment file that has
it, per `nano-add-authentication-jwt`'s placement (`Issuer`/`Audience` overrides live in every
environment file, not just Development).

## AuthController

Delete `Controllers/AuthController.cs` — see the note at the top; this isn't conditional.

## Kubernetes / GitHub Actions — issuer app only

If step 2 found this app is the issuer:

- Delete `.kubernetes/auth-jwt-secret.yaml`, and remove its
  `.kubernetes\auth-jwt-secret.yaml = .kubernetes\auth-jwt-secret.yaml` line from `{name}.sln`'s
  `.kubernetes` `SolutionItems` block — `nano-add-authentication-jwt` added it there, and a
  deleted file left in the `.sln` shows up as missing in Visual Studio's Solution Explorer.
- Remove its apply step from the `Kubernetes Deploy` workflow step.
- Remove the `AUTH_JWT_PUBLIC_KEY`/`AUTH_JWT_PRIVATE_KEY` workflow env vars.
- If this app was the **only** issuer in the solution, every validator-only app that references
  `auth-jwt-secret` now points at a secret nothing creates anymore — flag this to the user
  explicitly; it's outside this skill's scope (a different app's files), but silently leaving it
  broken elsewhere is worse than mentioning it.

⚠ This does **not** delete either underlying live resource — removing the workflow/manifest lines
only stops maintaining them going forward, the same class of gap as
`nano-remove-azure-managed-identity` (doesn't delete the Azure identity) and
`nano-remove-availability-check` (doesn't delete the Application Insights resource):
- The `auth-jwt-secret` Kubernetes `Secret` already sitting in the cluster from prior deploys. If
  any validator-only app still references it, the secret staying live is actually what keeps them
  working — don't suggest deleting it (`kubectl delete secret auth-jwt-secret`) unless the user
  confirms every app that reads it is also being removed or reworked.
- The **GitHub repository secrets** themselves (`{ENVIRONMENT}_AUTH_JWT_PUBLIC_KEY`/`_PRIVATE_KEY`
  for both Staging and Production) — removing the workflow's `AUTH_JWT_PUBLIC_KEY`/
  `AUTH_JWT_PRIVATE_KEY` env var lines just stops this workflow from *reading* them; the secrets
  stay stored in the repo/organization's GitHub settings until someone deletes them there
  directly (`gh secret delete` or the Settings UI) — this skill has no way to do that itself.
Say both explicitly rather than letting the user assume "removed the file/workflow lines" means
"removed the actual secret."

## Kubernetes — every app (issuer and validator)

Remove the `App__Authentication__Jwt__PublicKey`/`App__Authentication__Jwt__PrivateKey` entries
(whichever are present — a validator only ever has `PublicKey`) from
`.kubernetes/deployment.yaml`'s container `env`.

## Kubernetes / GitHub Actions — RootLogin, only if step 5 found it wired up

If `.kubernetes/auth-root-login-secret.yaml` or the `RootLogin` deployment env entries exist:

- Delete `.kubernetes/auth-root-login-secret.yaml`, and remove its matching line from
  `{name}.sln`'s `.kubernetes` `SolutionItems` block if it was added there.
- Remove its apply step from the `Kubernetes Deploy` workflow step.
- Remove the `{environment}_AUTH_ROOT_LOGIN_USERNAME`/`_PASSWORD`-sourced workflow env vars.
- Remove the `App__Authentication__Jwt__RootLogin__Username`/`Password` entries from
  `.kubernetes/deployment.yaml`/`cronjob.yaml`'s container `env`.

## After making the change

- Show the user every file touched/deleted.
- Restate step 3's outcome one more time now that it's actually done — either "this app now has
  no authentication, every endpoint is anonymous" or "this app is now in pure API-key mode, the
  JWT exchange endpoint is gone" — whichever applies. This is the one thing most worth a second,
  explicit confirmation rather than folding into a file list.
- If step 2 found this app was the sole issuer, restate the warning about now-broken
  validator-only apps elsewhere in the solution, and the ⚠ that the live `auth-jwt-secret`
  Kubernetes secret still exists in the cluster — only its manifest/CI maintenance was removed.
- If step 5 found a Staging/Production `RootLogin` wired up, confirm it was fully removed
  (secret, CI, deployment env entries) — this was outside the add-skill's default behavior, so
  don't assume the user already knows all three pieces existed.
- If step 6 found any custom external-login repository classes, list them explicitly as now-dead
  code (no route left to invoke them) rather than leaving that for the user to discover later —
  including any options class/config section feeding it, not just the repository class itself.
- If step 7 found built-in `ExternalLogins` credentials stored somewhere, restate what was found
  and confirm with the user whether it was actually removed — there's no fixed convention to
  verify against here, so don't imply this was handled as thoroughly as the other secrets above.
