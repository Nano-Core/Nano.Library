---
name: nano-add-authentication-external-custom
description: Scaffold a custom BaseAuthExternalRepository<TFlow> external-login provider on a Nano.Library-based API/Web application, for a provider Nano has no built-in support for (not Microsoft/Google/Facebook — see nano-add-authentication-microsoft for that one, and AGENTS.md's Authentication section for why the other two stay config-only). Asks whether the provider's OAuth flow is AuthCodeFlow, ImplicitFlow (Nano's two built-in BaseAuthFlow shapes), or a custom class the user derives from BaseAuthFlow themselves (scaffolded too, if needed) — and what the provider should be called — then generates the repository class as a placeholder shell (mocked return values + TODO markers), matching the class shape used throughout Nano.Lessons' external-custom-auth examples — not a working integration against any real provider API, since this skill can't know one specific provider's token/userinfo API shape. Requires nano-add-authentication-jwt already configured (Jwt + AuthController). Use when the user asks to add a custom, third-party, or in-house external login provider to a Nano API or Web application.
---

# Nano add custom external authentication

Scaffolds a `BaseAuthExternalRepository<TFlow>` implementation on an existing Nano API/Web
application, for an external login provider Nano has no built-in support for. Read AGENTS.md's
`#### Authentication` section first, especially `##### Custom external provider` — it documents
`TFlow`, the two abstract methods, and the auto-discovery mechanism; this skill does not repeat
that, only how to apply it and what it wires up as a result.

**Prerequisite: `Jwt` and the `AuthController` must already be configured.** A repository class on
its own does nothing without something to expose it — if the app has no `Jwt` config or
`AuthController` yet, stop and point the user at `nano-add-authentication-jwt` first. That skill's
own "Custom provider" note under External Login points back here for the actual scaffold — the two
are meant to be run in that order, not duplicated against each other.

**No `Program.cs` registration.** Like every `BaseAuthExternalRepository<TFlow>` implementation,
the generated class is discovered by type — nothing to add to `.ConfigureServices(...)`.

**Output is a placeholder shell, not a working integration.** This skill cannot know a specific
provider's real token-exchange or userinfo-lookup API shape, so `AuthenticateAsync` returns
static, clearly-fake data with a `// TODO` marking where the real HTTP call belongs — the same
shape as Nano.Lessons' external-custom-auth examples. Say this plainly when done: the app will
build and the new endpoint(s) will respond, but signing in through them won't yet produce a real
user identity from any actual external service.

## Before making any change, determine

1. **Is `Jwt` (and the `AuthController`) already configured?** Check the base `appsettings.json`
   for `App:Authentication:Jwt`, and for an existing `AuthController`. If either is missing, stop
   — see the prerequisite above.
2. **What should the provider be called?** This is `ProviderName` — the constructor's string
   argument, and the literal path segment in every route this exposes
   (`/auth/login/external/{providerName}/...`, lowercased automatically at registration time
   regardless of the casing used here). Ask; don't default to the class name. Pick something that
   reads naturally in a URL (e.g. `Auth0`, `Okta`, `CompanySso`) — this is user-facing in the API
   surface, not an internal identifier.
3. **Which flow shape does the provider actually need?** Ask; don't guess. Present all three —
   don't just offer the two built-in ones and treat a custom shape as a dead end:
   - **`AuthCodeFlow`** (`Code`/`CodeVerifier`/`RedirectUri`) — the provider exchanges an
     authorization code for tokens server-side, the same shape Nano's own built-in Google/Microsoft
     providers use. Pick this if the provider does a real OAuth authorization-code redirect with
     PKCE.
   - **`ImplicitFlow`** (`AccessToken`) — the frontend obtains an access token directly (e.g. via
     the provider's own client-side SDK) and hands it straight to the backend to validate, the same
     shape Nano's built-in Facebook provider uses. Pick this if there's no server-side code
     exchange at all, just a token the frontend already has.
   - **A custom class the user derives from `BaseAuthFlow` themselves** — for when the provider's
     handshake carries different or additional data than either built-in shape (e.g. an auth code
     plus a tenant/organization identifier the provider requires alongside it). `BaseAuthFlow` is
     the abstract base both `AuthCodeFlow` and `ImplicitFlow` derive from — it's a bare marker with
     no members of its own, so deriving from it needs nothing beyond adding whatever properties the
     provider's own handshake sends. Say the base class name explicitly when presenting this option,
     since the user needs it to know what they're deriving from (or to have this skill scaffold it
     for them, see "Custom flow class" below).
   `AuthCodeFlow`/`ImplicitFlow`/a custom subclass all derive `BaseAuthFlow`. If the user picks
   custom, get the class name from them before scaffolding — see "Custom flow class" below.
4. **Does the provider support refreshing a login without the user re-authenticating?** Ask.
   This decides how `AuthenticateRefreshAsync` is scaffolded (see below) — don't default to "yes"
   just because the method has to exist.
5. **Persistent or transient login — detect it, don't ask.** Check whether `Data:Identity` is
   configured in the app's base `appsettings.json`:
   - **Not configured (transient)**: the new provider is reachable at
     `POST /auth/login/external/{providerName}/transient` (+ `/transient/refresh`), via the
     existing `AuthController`, per the `!hasIdentity && hasAuthController` gate documented in
     AGENTS.md's Authentication section. Nothing further to check.
   - **Configured (persistent)**: also check whether a `BaseEntityUserController`-derived class
     exists for the identity-backed user entity.
     - If one exists, the new provider becomes reachable at
       `POST /{userTag}s/signup/external/{providerName}` (sign up a new user via this provider) and
       `POST /{userTag}s/{id}/external-logins/add/{providerName}` /
       `POST|DELETE /{userTag}s/{id}/external-logins/remove/{providerName}` (link/unlink it on an
       existing authenticated user) — `{userTag}` being that entity's pluralized name.
     - If no such controller exists yet, say so plainly: the repository class will still compile
       and register, but nothing currently exposes a way to sign up or link a login through it —
       point the user at whatever skill scaffolds their identity-backed user controller before
       calling this done.
     - Separately, since `AuthController` also exists (checked in step 1), the provider is
       additionally reachable at `POST /auth/login/external/{providerName}` for signing an
       *existing*, already-linked user back in.
   Report the actual resulting endpoint set back to the user in "After making the change" — don't
   leave it implicit.

## Custom flow class (only if step 3 picked it)

`BaseAuthFlow` — the same base class `AuthCodeFlow`/`ImplicitFlow` derive — is a bare marker class
with no members of its own. Dispatch to the right repository/method overload happens entirely
through the actual CLR type of `TFlow` (`BaseAuthExternalRepository<TFlow>.AuthenticateAsync`'s
`flow is not TFlow typedFlow` check, and each generated endpoint's request model being generically
typed to one concrete `TFlow`), so a custom subclass needs nothing beyond deriving `BaseAuthFlow` and
adding its own properties — no fixed value or constructor argument to satisfy.

Conventionally alongside the repository class, e.g. `Authentication/{Provider}Flow.cs`:

```csharp
public class {Provider}Flow : BaseAuthFlow
{
    // TODO: add whatever properties {Provider}'s own flow actually carries - mirroring
    // AuthCodeFlow's Code/CodeVerifier/RedirectUri or ImplicitFlow's AccessToken shape,
    // each `[Required] public virtual required {Type} {Name} { get; set; }`.
}
```

Use this class (not `AuthCodeFlow`/`ImplicitFlow`) as `{TFlow}` in the repository class below.
Since this skill doesn't know what properties the provider's flow actually needs, leave the class
body as the `// TODO` above rather than guessing field names — the user fills those in once they
know exactly what the provider's handshake sends.

## The repository class

Conventionally `Authentication/{Provider}ExternalRepository.cs` in the main application project
(discovery is by type, so the location isn't enforced — match this app's existing folder
conventions if it already has an `Authentication/`-equivalent folder for anything else).

```csharp
public class {Provider}ExternalRepository() : BaseAuthExternalRepository<{TFlow}>("{ProviderName}")
{
    public override async Task<ExternalAuthenticationData> AuthenticateAsync({TFlow} flow, CancellationToken cancellationToken = default)
    {
        // TODO: call {Provider}'s own token-exchange/userinfo API using the data on `flow`
        // ({TFlow} fields — Code/CodeVerifier/RedirectUri for AuthCodeFlow, AccessToken for
        // ImplicitFlow, or whatever properties a custom flow class carries), and map its
        // response onto ExternalAuthenticationData below.
        await Task.CompletedTask;

        return new ExternalAuthenticationData
        {
            Id = "external-id",
            Username = "MyUser",
            EmailAddress = "johndoe@domain.com",
            PhoneNumber = "+4520111112",
            Name = "John Doe",
            ExternalToken = new ExternalAuthenticationToken
            {
                Name = this.ProviderName,
                Token = "token",
                RefreshToken = "refresh-token"
            }
            // TransientClaims: non-persisted claims merged into the issued JWT at login time —
            // this is the right place to map trusted data from the external provider's own
            // response (a role, a group membership) onto a claim, since it comes from
            // AuthenticateAsync's own trusted return value, not from caller input. Leave empty
            // unless the provider's response actually carries something worth asserting.
        };
    }

    public override async Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // TODO: call {Provider}'s token-refresh endpoint using `refreshToken`, and map its
        // response onto ExternalAuthenticationToken below.
        await Task.CompletedTask;

        return new ExternalAuthenticationToken
        {
            Name = this.ProviderName,
            Token = "token",
            RefreshToken = "refresh-token"
        };
    }
}
```

**If step 4 found the provider doesn't support refresh**, scaffold `AuthenticateRefreshAsync` to
throw instead of returning fake-but-plausible token data — matching `AuthExternalFacebookRepository`'s
own real, shipped behavior for exactly this case:

```csharp
    public override Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
        => throw new UnauthorizedException();
```

A placeholder that always "succeeds" is worse than one that's honestly not implemented yet —
returning mock data here would make a caller relying on refresh silently believe it works.

⚠ **`AuthenticateRefreshAsync` isn't only reachable through the transient refresh route.**
`BaseAuthIdentityRepository`'s own `/auth/login/refresh` also calls back into it, whenever the JWT
being refreshed carries an external-provider refresh-token claim from its original login — so this
method matters in **both** persistent and transient mode, not just transient. Don't assume it's
dead code just because the app is persistent.

## Wiring real provider credentials later

Not part of this skill's own output — advisory only, for when the `// TODO` above gets filled in
for real. At that point the provider will need its own credentials (an API key, a client
id/secret, a base URL) passed into the repository's constructor. Follow AGENTS.md's `### Custom
Configuration Section` pattern: a plain options class bound via `AddNanoConfigSection<TOptions>`,
injected into the repository like any other constructor dependency — not routed through
`Jwt.ExternalLogins`, which is exclusively for the three built-in providers. For Staging/Production
secret storage, this is closer to how Facebook/Google are handled than Microsoft: there's no
CLI-scriptable credential setup for an arbitrary custom provider, so ask the user how they want
the real values stored (a Kubernetes secret following this app's existing `.kubernetes/*-secret.yaml`
pattern is the common answer) rather than assuming a convention.

## After making the change

- Show the file(s) created — the repository class, and the custom flow class too if step 3 needed
  one.
- State the flow type chosen and why (matching what the user said about the provider's real OAuth
  shape) — including, if a custom class was scaffolded, that its properties are left as a `// TODO`
  for the user to fill in — and the refresh decision (implemented for real later vs. explicitly
  unsupported).
- **State the actual, resulting endpoint set from step 5** — transient's two routes, or persistent's
  full set (signup/add/remove on the user controller, plus login on `AuthController`) — not a
  generic restatement of what's theoretically possible. If step 5 found persistent mode with no
  user controller yet, repeat that gap explicitly rather than letting the response imply everything
  is reachable.
- Restate plainly that this is a placeholder: the class compiles and the endpoints respond, but
  `AuthenticateAsync`/`AuthenticateRefreshAsync` don't call the real provider yet — point at
  "Wiring real provider credentials later" above as the next step, and don't describe the feature
  as done/working end-to-end.
- If the frontend needs to redirect through the provider's own sign-in (AuthCodeFlow) or obtain a
  token via its SDK (ImplicitFlow) first, say so — that half is outside this skill and Nano
  entirely, same caveat AGENTS.md gives for the built-in providers.
