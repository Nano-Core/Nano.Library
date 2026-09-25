---
mode: agent
description: Configure Nano's built-in Microsoft external login provider (App:Authentication:Jwt:ExternalLogins:Microsoft) on a Nano.Library-based API/Web application, across all three environments — a one-time local app-registration script for Development, and a self-provisioning, self-rotating Azure AD app registration wired into the GitHub Actions workflow plus a Kubernetes secret for Staging/Production. Use when the user asks to add "Sign in with Microsoft", Entra ID, or Azure AD external login to a Nano API or Web application — requires nano-add-authentication-jwt already configured (Jwt.ExternalLogins lives under that same config), and does not apply to Google/Facebook, which have no CLI-scriptable credential setup and stay manually configured (see AGENTS.md's Authentication section).
---

# Nano add Microsoft authentication

Configures the built-in `Microsoft` external login provider (`Jwt.ExternalLogins.Microsoft`) on an
existing Nano API/Web application. Read AGENTS.md's `#### Authentication` section first, especially
the "External login providers" table - it documents the flow type (`AuthCodeFlow`), why `Scopes`
must include `openid`, and why Microsoft (unlike Facebook/Google) has a scriptable credential setup
at all. This skill does not repeat that, only how to apply it.

**Prerequisite: `Jwt` must already be configured.** `ExternalLogins` is a sub-section of
`Authentication:Jwt`, not a standalone auth mode - if the app has no `Jwt` config or `AuthController`
yet, stop and point the user at `nano-add-authentication-jwt` first (it covers persistent vs.
transient and the `AuthController` itself; this skill only adds the Microsoft-specific piece under
`ExternalLogins`).

**This always wires up all three environments - Development, Staging, and Production - there's no
partial-scope option to choose.** Development gets a one-time local app-registration script;
Staging/Production get the self-provisioning CI step. Don't ask the user which environments to
cover, only the questions below that actually vary per app.

**Facebook/Google are explicitly out of scope for this skill.** They have no CLI/API path for
creating their app credentials (created by hand through each provider's own developer console), so
there's nothing to script the way there is for Microsoft's Entra ID app registration. If the user
asks for Facebook or Google, configure `Jwt.ExternalLogins.Facebook`/`.Google` by hand per AGENTS.md's
table and ask how they want the secret stored for Staging/Production - don't invent a Kubernetes/CI
convention for those the way this skill does for Microsoft.

## Before making any change, determine

1. **Is `Jwt` (and the `AuthController`) already configured?** If not, stop - see the prerequisite
   above.
2. **Persistent or transient login?** Check whether [Identity](nano-add-identity) is configured.
   Doesn't change anything this skill does (the `Jwt.ExternalLogins.Microsoft` config is identical
   either way) - it only changes which endpoint ultimately exposes the login per AGENTS.md's
   sub-repository table (`AuthTransientRepository` vs. the persistent equivalent). Not this skill's
   concern to scaffold, only worth knowing when telling the user where the login endpoint lives.
3. **Is a redirect URI known?** Needed for the Azure AD app registration in every environment (the
   Development script and the Staging/Production CI step each take their own). Ask if the request
   doesn't name a client - don't guess a port/path.
4. **Which accounts should be able to sign in?** Ask - don't assume. This is the app registration's
   `--sign-in-audience`, and it also changes what `TenantId` must hold at runtime, since
   `AuthExternalMicrosoftRepository` interpolates it straight into the token endpoint URL
   (`https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/token`):

   | Who can sign in | `--sign-in-audience` | Runtime `TenantId` |
   | --- | --- | --- |
   | Only users in your own tenant (default, most common) | `AzureADMyOrg` | the real tenant GUID |
   | Users in any Azure AD/Entra org | `AzureADMultipleOrgs` | literal `organizations` |
   | Any org + personal Microsoft accounts | `AzureADandPersonalMicrosoftAccount` | literal `common` |
   | Personal Microsoft accounts only | `PersonalMicrosoftAccount` | literal `consumers` |

   Default to `AzureADMyOrg` if the user has no specific need - it's the least-privilege choice for
   internal, single-tenant auth. Whatever is chosen, remind the user that the client-side code that
   starts the sign-in (MSAL.js or
   equivalent) must be configured with the matching authority, or Azure rejects the sign-in before a
   code is ever issued - that part lives outside Nano and this skill can't set it.

## appsettings.json - Jwt.ExternalLogins.Microsoft

Base `appsettings.json`, nested under the existing `Jwt` block:

```json
"ExternalLogins": {
  "Microsoft": {
    "TenantId": null,
    "ClientId": null,
    "ClientSecret": null,
    "Scopes": [ "openid", "profile", "email", "offline_access" ]
  }
}
```

`offline_access` is included by default since most apps want refresh support, and leaving it in
place is safe even for logins that don't use it: `LogInExternal`/`LogInExternal<TFlow>`'s
`IsRefreshable` flag is the real, per-login-call gate - Nano discards the external refresh token
server-side whenever a specific login request sets `IsRefreshable: false`, regardless of what
`Scopes` requested. Remove `offline_access` from `Scopes` only if this app should never support
refresh at all.

The client-side authorize request's own `scope` parameter must match - include `offline_access`
there too, unconditionally, the same as here; consent is granted once, at that initial redirect, so
this app's own config alone can't retroactively grant it.

**`ExternalLogins.Microsoft` stays `null` in every `appsettings*.json` file, including
`appsettings.Development.json`.** Unlike the shared JWT Development key pair (a throwaway value
every developer can share, so it's worth hardcoding into the tracked Development file), a Microsoft
app registration's `TenantId`/`ClientId`/`ClientSecret` are tied to whatever Entra ID app
registration each individual developer creates for themselves - there's nothing shared to pre-seed,
and nothing that belongs in a tracked config file at all. Per this app's own `.docker/.env`
convention (see AGENTS.md's Local Development docker-compose section), real per-developer secrets
go into `.docker/.env`, gitignored, not into `appsettings.Development.json` - the Local Development
script below produces exactly the three values that file needs. No Graph API permission is needed
beyond the default - `openid`/`profile`/`email`/`offline_access` only affect what lands in the
`id_token` and whether a `refresh_token` is issued alongside it, not access to any resource.

`appsettings.Staging.json`/`appsettings.Production.json` - **nothing**. Per the Kubernetes section
below, `TenantId`/`ClientId`/`ClientSecret` are injected as environment variables from a Kubernetes
secret, the same way `Jwt.PublicKey`/`PrivateKey` are - not present in any config file for those
environments.

## .docker/.env - placeholder keys

**Always add the three empty keys to this app's own `.docker/.env` in the same change that adds the
`appsettings.json` block above - don't leave this for later, and don't skip it just because the
values aren't known yet.** This mirrors the `null` placeholders in `appsettings.json`: the file
already exists (every app ships one, always present per this app's `.docker/.env` convention), so
the keys belong there now, empty, the same way `TenantId`/`ClientId`/`ClientSecret` are `null` in
`appsettings.json` rather than simply absent.

```
App__Authentication__Jwt__ExternalLogins__Microsoft__TenantId=
App__Authentication__Jwt__ExternalLogins__Microsoft__ClientId=
App__Authentication__Jwt__ExternalLogins__Microsoft__ClientSecret=
```

Leave the values empty here - a developer fills them in from the Local Development script's output
below, once they've run it. Key names are the `App:`-prefixed double-underscore config path, matching
this app's actual `appsettings.json` nesting under `App` and the same names the Kubernetes
`secretKeyRef` entries in the Kubernetes section below use.

## Local Development - one-time app registration script

Unlike Staging/Production, there's no CI to run this automatically - each developer runs it once, by
hand, against their own Azure AD/Entra ID tenant. **Don't write this to a file in the app** - it's a
one-time, per-developer, throwaway command, not a project artifact anyone needs to keep around or
version. Show it directly in your response instead, styled like the `deploy.ps1` scripts under
`Nano.Azure/*` (env vars assigned as literals at the top, plain `az` calls below, no existence checks
or `if`/`else` branching - this runs once, by a human, not repeatedly by CI):

```powershell
$env:APP_DISPLAY_NAME = "{service-name}-app-development";
$env:AUTH_MICROSOFT_REDIRECT_URI = "";
$env:AUTH_MICROSOFT_SIGNIN_AUDIENCE = {SignInAudienceLiteral};
$env:AUTH_MICROSOFT_TENANT_ID = {TenantIdLiteral};

az ad app create `
    --display-name $env:APP_DISPLAY_NAME `
    --sign-in-audience $env:AUTH_MICROSOFT_SIGNIN_AUDIENCE `
    --web-redirect-uris $env:AUTH_MICROSOFT_REDIRECT_URI;

$env:AUTH_MICROSOFT_CLIENT_ID = az ad app list --display-name $env:APP_DISPLAY_NAME --query "[0].appId" -o tsv;

$env:AUTH_MICROSOFT_CLIENT_SECRET = az ad app credential reset `
    --id $env:AUTH_MICROSOFT_CLIENT_ID `
    --append `
    --display-name ($env:APP_DISPLAY_NAME + "-secret") `
    --years 2 `
    --query "password" -o tsv;
```

- `{service-name}` is this app's kebab-case `SERVICE_NAME` (the same value the CI workflow already
  uses), with `-app-development` appended - kept distinct from the CI-managed `{service-name}-app`
  registration shared by Staging/Production, since this one belongs to a single developer's own
  tenant, not a deployed environment.
- `{SignInAudienceLiteral}`/`{TenantIdLiteral}` are the exact same literals used in the CI step below
  - fill them in once, from step 4's table above, matching whatever was decided for this app. No
  switch statement here either, same reasoning as the CI step.
- `AUTH_MICROSOFT_REDIRECT_URI` is left empty in the snippet on purpose - the developer fills in
  their own local callback URL before running it. Don't fill in a guessed value.
- After running it, the developer fills the three values into the keys the `.docker/.env` step above
  already placed - not `appsettings.Development.json`:
  ```
  App__Authentication__Jwt__ExternalLogins__Microsoft__TenantId=organizations
  App__Authentication__Jwt__ExternalLogins__Microsoft__ClientId={ClientId}
  App__Authentication__Jwt__ExternalLogins__Microsoft__ClientSecret={ClientSecret}
  ```
  `.env` is already gitignored and already loaded into the app's own compose service via `env_file`,
  so nothing else needs wiring for these three keys to take effect.

## Staging/Production - self-provisioning, self-rotating CI step

This is the part that runs automatically, on every deploy. Add one workflow-level env var:

```yaml
env:
  AUTH_MICROSOFT_REDIRECT_URI: ${{ vars.AUTH_MICROSOFT_REDIRECT_URI }}
```

`vars`, not `secrets` - it isn't sensitive. Ask the user for its value (the real deployed client's
callback URL) rather than defaulting to a placeholder.

**`--sign-in-audience` and `TenantId` are each assigned once, as their own `$env:` variables at the
top of the step, not sourced from a workflow variable and not derived by a runtime switch.** The
audience was already decided once, at authoring time, in step 4 above - there's nothing left to
branch on at runtime. Fill in `$env:AUTH_MICROSOFT_SIGNIN_AUDIENCE` and `$env:AUTH_MICROSOFT_TENANT_ID`
below with whichever pair was chosen, straight from step 4's table (e.g. `"AzureADMyOrg"` paired with
`$env:AZURE_TENANT_ID`, or `"AzureADMultipleOrgs"` paired with `"organizations"`) - the same two
literals used in the Development script above. **`$env:AUTH_MICROSOFT_TENANT_ID` is always set this
way, even for `AzureADMyOrg`** - the Kubernetes secret and `$env:GITHUB_ENV` output below always read
`$env:AUTH_MICROSOFT_TENANT_ID`, never `$env:AZURE_TENANT_ID` directly, so the two stay
interchangeable regardless of which audience a given app uses.

Add a **"Setup App Registration"** step, after `Build & Push Image` and before `Kubernetes Deploy`
(needs `AZURE_TENANT_ID`/an authenticated `az` session from `Azure Login`, and its output feeds the
Kubernetes step):

```yaml
- name: Setup App Registration
  shell: pwsh
  run: |
    $env:APP_DISPLAY_NAME = $env:SERVICE_NAME + "-app";
    $env:SECRET_DISPLAY_NAME = $env:APP_DISPLAY_NAME + "-secret-" + (Get-Date -Format "yyyyMMddHHmmss");
    $env:AUTH_MICROSOFT_SIGNIN_AUDIENCE = {SignInAudienceLiteral};
    $env:AUTH_MICROSOFT_TENANT_ID = {TenantIdLiteral};
    $env:AUTH_MICROSOFT_CLIENT_ID = az ad app list --display-name $env:APP_DISPLAY_NAME --query "[0].appId" -o tsv;

    if (-not $env:AUTH_MICROSOFT_CLIENT_ID)
    {
        az ad app create `
            --display-name $env:APP_DISPLAY_NAME `
            --sign-in-audience $env:AUTH_MICROSOFT_SIGNIN_AUDIENCE `
            --web-redirect-uris $env:AUTH_MICROSOFT_REDIRECT_URI;

        $env:AUTH_MICROSOFT_CLIENT_ID = az ad app list --display-name $env:APP_DISPLAY_NAME --query "[0].appId" -o tsv;
    }
    else
    {
        az ad app update `
            --id $env:AUTH_MICROSOFT_CLIENT_ID `
            --sign-in-audience $env:AUTH_MICROSOFT_SIGNIN_AUDIENCE `
            --web-redirect-uris $env:AUTH_MICROSOFT_REDIRECT_URI;
    }

    $env:AUTH_MICROSOFT_CLIENT_SECRET = az ad app credential reset `
        --id $env:AUTH_MICROSOFT_CLIENT_ID `
        --append `
        --display-name $env:SECRET_DISPLAY_NAME `
        --years 2 `
        --query "password" -o tsv;

    echo "::add-mask::$env:AUTH_MICROSOFT_CLIENT_SECRET";

    $staleCredentialIds = az ad app credential list --id $env:AUTH_MICROSOFT_CLIENT_ID --query "sort_by(@, &startDateTime)[:-3].keyId" -o tsv;

    foreach ($keyId in ($staleCredentialIds -split "`n" | Where-Object { $_ }))
    {
        az ad app credential delete `
            --id $env:AUTH_MICROSOFT_CLIENT_ID `
            --key-id $keyId;
    }

    echo "AUTH_MICROSOFT_CLIENT_ID=$env:AUTH_MICROSOFT_CLIENT_ID" >> $env:GITHUB_ENV;
    echo "AUTH_MICROSOFT_CLIENT_SECRET=$env:AUTH_MICROSOFT_CLIENT_SECRET" >> $env:GITHUB_ENV;
    echo "AUTH_MICROSOFT_TENANT_ID=$env:AUTH_MICROSOFT_TENANT_ID" >> $env:GITHUB_ENV;
```

What this does, and why it's shaped this way:

- **Idempotent app registration.** Looks the app up by display name first; creates it only if
  missing, otherwise just keeps its redirect URI/audience in sync.
- **`AUTH_MICROSOFT_SIGNIN_AUDIENCE` and `AUTH_MICROSOFT_TENANT_ID` are each literals filled in once,
  at authoring time, not computed at runtime.** Only `AzureADMyOrg` uses the real tenant GUID
  (`$env:AZURE_TENANT_ID`, the workflow's own tenant) for `AUTH_MICROSOFT_TENANT_ID` - the other
  three audiences need the literal `organizations`/`common`/`consumers` string instead, per the table
  in "Before making any change, determine" above. Since the audience is decided once per app, not per
  run, there's no reason to carry it as a workflow variable or re-derive `TenantId` from it with a
  runtime switch - just assign both as `$env:` variables once, at the top of the step, when applying
  this skill (and again in the Development script above).
- **Always reference `$env:AUTH_MICROSOFT_TENANT_ID` downstream, never `$env:AZURE_TENANT_ID`
  directly** - in the Kubernetes secret's `stringData` below and in the `$env:GITHUB_ENV` output.
  Even when the two happen to hold the same value (the `AzureADMyOrg` case), keeping every downstream
  reference pointed at `AUTH_MICROSOFT_TENANT_ID` means the app works identically regardless of which
  audience it was configured with - nothing downstream needs to know or care which literal was
  chosen.
- **`--append`, not a bare `az ad app credential reset`.** A bare reset atomically replaces every
  existing secret - any pod still running the previous deployment's env vars would find its
  `ClientSecret` invalid mid-rollout. `--append` adds a new one alongside, so the old secret keeps
  working until those pods cycle out.
- **Prune to the newest 3** (`sort_by(@, &startDateTime)[:-3]`) so the app registration doesn't
  accumulate secrets forever, while still giving roughly 3 deploys of grace before an older one
  actually stops working - comfortably outlasts a normal rolling update. Adjust the `-3` if a
  different grace period is wanted, but don't drop the pruning step entirely or the app registration
  grows an unbounded credential list.
- **`::add-mask::` immediately after generating the secret.** GitHub only auto-masks values sourced
  from `secrets.*` - this one is never stored as a GitHub secret (see below), so nothing masks it by
  default unless this line does. Keep it directly after the `credential reset` call, before anything
  else touches `$env:AUTH_MICROSOFT_CLIENT_SECRET`.
- **No `AUTH_MICROSOFT_CLIENT_SECRET` GitHub secret, ever.** Unlike the JWT keys (generated once,
  offline, stored as `{ENVIRONMENT}_AUTH_JWT_PUBLIC_KEY`/`_PRIVATE_KEY`), this workflow never
  depends on a value surviving between runs - every run produces and exports its own. Don't
  introduce one; it would just go stale the next time this step rotates the secret.

## Kubernetes

New file, `.kubernetes/auth-microsoft-secret.yaml`:

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: auth-microsoft-secret
  namespace: %KUBERNETES_NAMESPACE%
type: Opaque
stringData:
  tenant-id: %AUTH_MICROSOFT_TENANT_ID%
  client-id: %AUTH_MICROSOFT_CLIENT_ID%
  client-secret: %AUTH_MICROSOFT_CLIENT_SECRET%
```

Apply it in the `Kubernetes Deploy` step alongside `auth-jwt-secret.yaml`, before
`deployment.yaml`/`stateful-set.yaml`. Also add
`.kubernetes\auth-microsoft-secret.yaml = .kubernetes\auth-microsoft-secret.yaml` to `{name}.sln`'s
`.kubernetes` `SolutionItems` block (per AGENTS.md's Solution Structure note) - new files under
`.kubernetes/` don't show up in Visual Studio's Solution Explorer otherwise.

`.kubernetes/deployment.yaml`'s container `env`, alongside the JWT key entries:

```yaml
- name: App__Authentication__Jwt__ExternalLogins__Microsoft__TenantId
  valueFrom:
    secretKeyRef:
      name: auth-microsoft-secret
      key: tenant-id
- name: App__Authentication__Jwt__ExternalLogins__Microsoft__ClientId
  valueFrom:
    secretKeyRef:
      name: auth-microsoft-secret
      key: client-id
- name: App__Authentication__Jwt__ExternalLogins__Microsoft__ClientSecret
  valueFrom:
    secretKeyRef:
      name: auth-microsoft-secret
      key: client-secret
```

## After making the change

- Show the user every file touched, grouped by concern: appsettings, the `.docker/.env` placeholder
  keys, the workflow env var + new CI step, the new Kubernetes secret file, the `deployment.yaml`
  additions, and the `.sln` entry.
- Include the Local Development script directly in your response (not written to a file - see
  above), with a reminder to fill in their own redirect URI before running it, then add the three
  values the script assigns (`$env:AUTH_MICROSOFT_TENANT_ID`/`CLIENT_ID`/`CLIENT_SECRET`) to this
  app's own `.docker/.env` using the `App__...` key names above - not `appsettings.Development.json`.
- Restate that `offline_access` was included in `Scopes` by default, and that the client-side
  authorize request's own `scope` parameter must include it too for Microsoft to actually issue a
  `refresh_token` - don't let confirming the config change alone read as the whole fix.
- **Always include the frontend half in your reply, even though this skill only touches the
  backend.** The config change alone isn't enough to sign anyone in - the frontend has to redirect
  the user through Microsoft's own sign-in first. Per AGENTS.md's `#### Authentication` section
  (the same authorize-URL shape and PKCE explanation, don't re-derive it), give the user the
  authorize URL with this app's actual `TenantId`/`ClientId`/`RedirectUri` filled in (not left as
  placeholders, once known):
  ```
  https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/authorize
    ?client_id={ClientId}
    &response_type=code
    &redirect_uri={RedirectUri}
    &response_mode=query
    &scope=openid profile email offline_access
    &code_challenge={code_challenge}
    &code_challenge_method=S256
    &state={state}
  ```
  plus a short explanation that `code_challenge` isn't something to fill in from this app's own
  config - it's a PKCE value the frontend itself must generate a `code_verifier` for, hash
  (SHA-256, base64url-encoded) into `code_challenge` for this URL, and then send the raw
  `code_verifier` back to the login endpoint alongside the `code` Microsoft returns.
