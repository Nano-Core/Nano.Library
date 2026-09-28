---
mode: agent
description: Remove a custom BaseAuthExternalRepository<TFlow> external-login provider (scaffolded by nano-add-authentication-external-custom) from a Nano.Library-based application, without touching JWT authentication, Identity, or any other configured provider. Use when the user asks to remove a custom/third-party/in-house external login provider from a Nano API or Web application while keeping regular login working - not for removing JWT authentication entirely (nano-remove-authentication-jwt), and not for a built-in provider (nano-remove-authentication-microsoft covers Microsoft; Facebook/Google are removed by hand per AGENTS.md).
---

# Nano remove custom external authentication

Removes a custom external-login provider previously scaffolded by
`nano-add-authentication-external-custom` - the counterpart to that skill. Read it first; this one
undoes exactly what it adds. `Jwt`, the `AuthController`, Identity, and any other configured
provider are left untouched.

If the user actually wants JWT authentication removed entirely, stop and point them at
`nano-remove-authentication-jwt` instead - its own removal already takes every configured external
provider (built-in or custom) down with it; running this skill first would just be redundant.

## Before making any change, determine

1. **Which provider, specifically?** A `BaseAuthExternalRepository<TFlow>` class isn't named or
   registered anywhere central - find it by its `ProviderName` (the constructor's string argument)
   or by asking the user which class/file. If more than one custom provider exists in the app,
   remove only the one named; don't assume "the custom provider" is singular.
2. **Where does the class live, and does it use a custom flow class?** Conventionally
   `Authentication/{Provider}ExternalRepository.cs` in the main application project per
   `nano-add-authentication-external-custom`'s own convention, but confirm by locating the actual
   `BaseAuthExternalRepository<TFlow>` subclass with that `ProviderName` - it may have been moved or
   renamed since scaffolding. Check its `TFlow` generic argument: if it's `AuthCodeFlow`/`ImplicitFlow`
   there's nothing extra, but if it's a custom `BaseAuthFlow` subclass scaffolded alongside it (e.g.
   `Authentication/{Provider}Flow.cs`), that file needs deleting too - **only if nothing else still
   references it**. A custom flow class isn't necessarily this provider's exclusively; if another
   repository also uses it, leave it in place and only remove the repository class being asked
   about.
3. **Was this provider ever wired to real credentials?** Check whether an options class bound via
   `AddNanoConfigSection<TOptions>` exists for it (per the "Wiring real provider credentials later"
   note in the add-skill), and whether any Kubernetes secret / `.docker/.env` entries reference it.
   If the class was still the unmodified placeholder shell (mocked return values, no real HTTP
   calls), there's nothing beyond the class file itself to remove.
4. **Is this provider referenced by frontend sign-in code?** This skill only removes the backend
   repository class (and its config/secrets, if any) - a frontend sign-in button or redirect flow
   built against this provider isn't something this skill can find or remove.

## Removing the class

Delete the `{Provider}ExternalRepository.cs` file (and its matching `.Designer`-equivalent, if
any - none exists for this kind of class, unlike an EF migration), plus the custom flow class file
too if step 2 found one that's exclusively this provider's. No `Program.cs` change is needed
either way, since neither class was ever registered there to begin with - removal is simply
deleting the file(s).

## Removing real credentials, if step 3 found any

Only if the provider had actually been wired up past the placeholder stage:

- Remove the options class and its `AddNanoConfigSection<TOptions>(...)` call from
  `ConfigureServices(...)`, if one was added.
- Remove the matching section from every `appsettings*.json` file it was added to.
- Remove the corresponding keys from `.docker/.env`, if present.
- If a Kubernetes secret was created for this provider specifically (not shared with anything
  else), delete its `.kubernetes/*.yaml` file, remove its apply line from the `Kubernetes Deploy`
  workflow step, remove its `App__...` entries from `.kubernetes/deployment.yaml`'s container
  `env`, and remove its `.kubernetes\*.yaml = .kubernetes\*.yaml` line from `{name}.sln`'s
  `.kubernetes` `SolutionItems` block.

⚠ This does **not** delete any credential that lives outside this codebase - an API key or client
secret registered directly with the external provider's own developer console/dashboard stays
valid there until revoked by hand. Say this explicitly rather than letting "removed the config"
read as "the credential is dead."

## After making the change

- Show every file touched/deleted, including the custom flow class if one was removed.
- Confirm JWT authentication (and any other configured provider, built-in or custom) is unaffected
  - sign-in through every other path keeps working exactly as before, only this one provider
  disappears.
- State plainly which endpoint(s) this took down - the transient `/auth/login/external/{providerName}/...`
  pair, or the persistent signup/add/remove/login set, per whichever mode
  `nano-add-authentication-external-custom` had reported when this was added.
- If step 3 found real credentials wired up, restate the ⚠ above - any external-provider-side
  registration/API key isn't revoked by this skill, only this app's own reference to it.
- If step 4 found frontend sign-in code, remind the user that removing the backend alone leaves a
  sign-in button that now fails - the frontend piece needs removing separately.
