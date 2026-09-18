---
mode: agent
description: Configure Nano's built-in Microsoft external login provider (App:Authentication:Jwt:ExternalLogins:Microsoft) on a Nano.Library-based API/Web application — adds the config, and (for Staging/Production) a self-provisioning, self-rotating Azure AD app registration wired into the GitHub Actions workflow plus a Kubernetes secret. Use when the user asks to add "Sign in with Microsoft", Entra ID, or Azure AD external login to a Nano API or Web application — requires nano-add-authentication-jwt already configured (Jwt.ExternalLogins lives under that same config), and does not apply to Google/Facebook, which have no CLI-scriptable credential setup and stay manually configured (see AGENTS.md's Authentication section).
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
3. **Development only, or does this need to actually work in Staging/Production?** If the user only
   wants to test locally, do the appsettings step below and stop - skip the CI/Kubernetes sections
   entirely rather than wiring up infrastructure nobody asked for yet.
4. **Is a redirect URI known?** Needed for the Azure AD app registration either way (manual for
   Development, scripted for Staging/Production). Ask if the request doesn't name a client - don't
   guess a port/path.
5. **Which accounts should be able to sign in?** Ask - don't assume. This is the app registration's
   `--sign-in-audience`, and it also changes what `TenantId` must hold at runtime, since
   `AuthExternalMicrosoftRepository` interpolates it straight into the token endpoint URL
   (`https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/token`):

   | Who can sign in | `--sign-in-audience` | Runtime `TenantId` |
   | --- | --- | --- |
   | Only users in your own tenant (default, most common) | `AzureADMyOrg` | the real tenant GUID |
   | Users in any Azure AD/Entra org | `AzureADMultipleOrgs` | literal `organizations` |
   | Any org + personal Microsoft accounts | `AzureADandPersonalMicrosoftAccount` | literal `common` |
   | Personal Microsoft accounts only | `PersonalMicrosoftAccount` | literal `consumers` |

   Default to `AzureADMyOrg` if the user has no specific need - it's the least-privilege choice and
   what `Nano.Lessons/Api.Auth.External.Microsoft` and `Nano.Templates/Api.Admin` both use. Whatever
   is chosen, remind the user that the client-side code that starts the sign-in (MSAL.js or
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
    "Scopes": [ "openid", "profile", "email" ]
  }
}
```

`appsettings.Development.json` - same shape, still `null`. **Do not hardcode real values here**,
unlike the shared JWT Development key pair - a Microsoft app registration is tied to a real Azure
tenant, not a throwaway pair everyone in the codebase can share. The developer fills these in
locally themselves, after creating their own Entra ID app registration (Azure Portal → Microsoft
Entra ID → App registrations → New registration → choose the sign-in audience decided above → Web
redirect URI matching whatever client will call this → Certificates & secrets → new client secret,
copied immediately since it's shown once → note the Application (client) ID). No Graph API
permission is needed beyond the default - `openid`/`profile`/`email` only affect what lands in the
`id_token`, not access to any resource.

For `TenantId`, use the table above: the real Directory (tenant) ID from the app registration only
if it's `AzureADMyOrg`; otherwise the literal `organizations`/`common`/`consumers` string, which is
the same for every developer regardless of which tenant they created the registration in.

`appsettings.Staging.json`/`appsettings.Production.json` - **nothing**. Per the Kubernetes section
below, `TenantId`/`ClientId`/`ClientSecret` are injected as environment variables from a Kubernetes
secret, the same way `Jwt.PublicKey`/`PrivateKey` are - not present in any config file for those
environments.

## Staging/Production - self-provisioning, self-rotating CI step

This is the part that's actually scriptable, unlike Facebook/Google. Add two workflow-level env vars:

```yaml
env:
  AUTH_MICROSOFT_REDIRECT_URI: ${{ vars.AUTH_MICROSOFT_REDIRECT_URI }}
  AUTH_MICROSOFT_SIGN_IN_AUDIENCE: ${{ vars.AUTH_MICROSOFT_SIGN_IN_AUDIENCE }}
```

`vars`, not `secrets` - neither is sensitive. Ask the user for `AUTH_MICROSOFT_REDIRECT_URI`'s value
(the real deployed client's callback URL) rather than defaulting to a placeholder, and set
`AUTH_MICROSOFT_SIGN_IN_AUDIENCE` to whichever `--sign-in-audience` value was decided in step 5 above
(e.g. `AzureADMyOrg`).

Add a **"Setup App Registration"** step, after `Build & Push Image` and before `Kubernetes Deploy`
(needs `AZURE_TENANT_ID`/an authenticated `az` session from `Azure Login`, and its output feeds the
Kubernetes step):

```yaml
- name: Setup App Registration
  shell: pwsh
  run: |
    $env:APP_DISPLAY_NAME = $env:SERVICE_NAME + "-app";
    $env:SECRET_DISPLAY_NAME = $env:APP_DISPLAY_NAME + "-secret-" + (Get-Date -Format "yyyyMMddHHmmss");
    $env:AUTH_MICROSOFT_CLIENT_ID = az ad app list --display-name $env:APP_DISPLAY_NAME --query "[0].appId" -o tsv;

    if (-not $env:AUTH_MICROSOFT_CLIENT_ID)
    {
        az ad app create `
            --display-name $env:APP_DISPLAY_NAME `
            --sign-in-audience $env:AUTH_MICROSOFT_SIGN_IN_AUDIENCE `
            --web-redirect-uris $env:AUTH_MICROSOFT_REDIRECT_URI;

        $env:AUTH_MICROSOFT_CLIENT_ID = az ad app list --display-name $env:APP_DISPLAY_NAME --query "[0].appId" -o tsv;
    }
    else
    {
        az ad app update `
            --id $env:AUTH_MICROSOFT_CLIENT_ID `
            --sign-in-audience $env:AUTH_MICROSOFT_SIGN_IN_AUDIENCE `
            --web-redirect-uris $env:AUTH_MICROSOFT_REDIRECT_URI;
    }

    $env:AUTH_MICROSOFT_TENANT_ID = switch ($env:AUTH_MICROSOFT_SIGN_IN_AUDIENCE)
    {
        "AzureADMyOrg" { $env:AZURE_TENANT_ID }
        "AzureADMultipleOrgs" { "organizations" }
        "AzureADandPersonalMicrosoftAccount" { "common" }
        "PersonalMicrosoftAccount" { "consumers" }
    };

    $env:AUTH_MICROSOFT_CLIENT_SECRET = az ad app credential reset `
        --id $env:AUTH_MICROSOFT_CLIENT_ID `
        --append `
        --display-name $env:SECRET_DISPLAY_NAME `
        --years 1 `
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
- **`AUTH_MICROSOFT_TENANT_ID` is derived from the audience, not reused from `$env:AZURE_TENANT_ID`
  directly.** Only `AzureADMyOrg` uses the real tenant GUID at runtime - the other three audiences
  need the literal `organizations`/`common`/`consumers` string instead, per the table in "Before
  making any change, determine" above. If the app is `AzureADMyOrg`, this still resolves to
  `$env:AZURE_TENANT_ID` (the workflow's own tenant, used for `az login`), so nothing changes for
  the common case.
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

## Reference implementation

`Nano.Lessons/Api.Auth.External.Microsoft` is this exact setup end-to-end (transient login, no
Identity) - its `.github/workflows/build-and-deploy.yml`, `.kubernetes/auth-microsoft-secret.yaml`,
and `.kubernetes/deployment.yaml` are the working, tested version of everything above. When in
doubt about exact formatting or step ordering, diff against that lesson rather than guessing.

One difference: the lesson (and `Nano.Templates/Api.Admin`) hardcode `AzureADMyOrg` directly rather
than reading `$env:AUTH_MICROSOFT_SIGN_IN_AUDIENCE`, and their Kubernetes secret still reads
`%AZURE_TENANT_ID%` rather than `%AUTH_MICROSOFT_TENANT_ID%` - both are intentionally left as the
simpler, single-tenant-only version, since neither needs broader sign-in. Don't "fix" them to match
this skill unless asked; treat this skill's parameterized version as what to scaffold for a *new*
app whose audience was actually asked about in step 5.

## After making the change

- Show the user every file touched, grouped by concern: appsettings per environment, and - if
  Staging/Production was in scope - the workflow env var + new step, the new Kubernetes secret file,
  the `deployment.yaml` additions, and the `.sln` entry.
- If step 3 found this was Development-only, that's the whole response - don't wire up the CI/
  Kubernetes half unasked.
- Remind the user to create their own Entra ID app registration for Development (the manual steps
  above) before testing locally - this skill can't do that part for them, only Staging/Production is
  scriptable.
- If they also want Facebook or Google, say plainly that this skill doesn't cover those - configure
  `Jwt.ExternalLogins.Facebook`/`.Google` by hand per AGENTS.md, and ask how they want the secret
  stored for Staging/Production rather than assuming this skill's Microsoft-specific pattern applies.
