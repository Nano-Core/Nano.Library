---
name: nano-remove-authentication-microsoft
description: Remove Nano's built-in Microsoft external login provider (App:Authentication:Jwt:ExternalLogins:Microsoft) from a Nano.Library-based application - unregisters the config and removes the Staging/Production app-registration/CI/Kubernetes wiring, without touching JWT authentication itself. Use when the user asks to remove "Sign in with Microsoft", Entra ID, or Azure AD external login from a Nano API or Web application while keeping regular JWT login - not for removing JWT authentication entirely, that's nano-remove-authentication-jwt (whose own removal already covers any ExternalLogins provider along with it).
---

# Nano remove Microsoft authentication

Removes just the `Microsoft` external login provider (`Jwt.ExternalLogins.Microsoft`) from an
existing Nano API/Web application — the counterpart to `nano-add-authentication-microsoft`. Read
that skill first — this one undoes exactly what it adds. `Jwt` itself, the `AuthController`, and
any other configured provider (`Facebook`/`Google`) are left untouched.

If the user actually wants JWT authentication removed entirely, stop and point them at
`nano-remove-authentication-jwt` instead — its own removal already takes `ExternalLogins` (whichever
providers are configured) down with it; running this skill first would just be redundant.

## Before making any change, determine

1. **Is Microsoft external login currently configured?** Check the base `appsettings.json` for
   `Jwt.ExternalLogins.Microsoft`. If not present, say so and stop.
2. **Was this wired up for Staging/Production, or Development only?** Check
   `.kubernetes/auth-microsoft-secret.yaml`, the `Setup App Registration` workflow step, and the
   `AUTH_MICROSOFT_*` workflow env vars — if none exist, this was Development-only and the
   Kubernetes/CI section below doesn't apply.
3. **Are other external login providers (`Facebook`/`Google`) still configured?** Doesn't change
   what this skill does — each provider's config/Kubernetes/CI is independent — but worth confirming
   so the user isn't surprised those keep working unchanged.
4. **Any custom external-login repository or frontend code referencing Microsoft specifically?**
   This skill only removes the built-in provider's config and infrastructure; a hand-written MSAL.js
   sign-in flow on the frontend, or a custom class deriving `BaseAuthExternalRepository<TFlow>` for
   Microsoft, isn't something this skill can find or remove — flag that the frontend sign-in button
   and redirect flow need removing separately.

## appsettings.json

Remove the `Microsoft` object from `Jwt.ExternalLogins` in the base `appsettings.json` (per
`nano-add-authentication-microsoft`, this is the only file it's ever added to — `appsettings.Development.json`
may also have a developer's own local override values under the same path; remove those too if
present). If `ExternalLogins` is now empty, remove the empty `ExternalLogins` object as well rather
than leaving a dangling `{}`.

## Staging/Production — only if step 2 found it wired up

- Remove the `Setup App Registration` workflow step.
- Remove the `AUTH_MICROSOFT_REDIRECT_URI`/`AUTH_MICROSOFT_SIGN_IN_AUDIENCE` workflow-level env vars.
- Remove the `auth-microsoft-secret.yaml` apply line from the `Kubernetes Deploy` step.
- Delete `.kubernetes/auth-microsoft-secret.yaml`, and remove its
  `.kubernetes\auth-microsoft-secret.yaml = .kubernetes\auth-microsoft-secret.yaml` line from
  `{name}.sln`'s `.kubernetes` `SolutionItems` block.
- Remove the `App__Authentication__Jwt__ExternalLogins__Microsoft__TenantId`/`ClientId`/
  `ClientSecret` entries from `.kubernetes/deployment.yaml`'s container `env`.

⚠ This does **not** delete the underlying live resources — removing the workflow/manifest lines
only stops maintaining them going forward, the same class of gap as
`nano-remove-authentication-jwt`'s equivalent note:
- The Entra ID app registration itself (`{service-name}-app` in Azure) stays registered — the
  workflow step that creates/updates it just stops running. Delete it directly in the Azure Portal
  (or `az ad app delete`) if it's no longer needed for anything else.
- The `auth-microsoft-secret` Kubernetes `Secret` already sitting in the cluster from prior deploys.
Say both explicitly rather than letting the user assume "removed the file/workflow lines" means
"removed the actual app registration."

## After making the change

- Show the user every file touched/deleted.
- Confirm JWT authentication (and any other configured provider) is unaffected — sign-in with a
  password/other provider keeps working exactly as before, only Microsoft sign-in disappears.
- If step 2 found Staging/Production wiring, restate the ⚠ above — the live Entra ID app
  registration and Kubernetes secret still exist; only this app's manifest/CI references were
  removed.
- If step 4 found frontend sign-in code, remind the user that removing the backend config alone
  leaves a "Sign in with Microsoft" button that now fails — the frontend piece is outside this
  skill's scope and needs removing separately.
