---
name: nano-add-api-client
description: Scaffold the bare Api Client class a Nano application exposes to other Nano applications - the BaseApiClient/BaseIdentityApiClient subclass itself, in the owning service's {Name}.Models project. Use when a service needs a client class to exist before any custom endpoint can be built against it, or when Identity is added/removed and an existing client's base class needs to change. For adding a custom method backing a specific new endpoint, see nano-add-custom-endpoint's internal-service path instead.
---

# Nano add API client

Creates the bare typed HTTP client class a Nano application exposes to *other* applications — the
counterpart to `nano-add-api-client-configuration`, which wires an already-created client into a
*consumer*. This skill is the owning service's job, and it is boilerplate only: the class itself,
on the correct base type. It does not add custom methods — a custom method is one half of a
specific endpoint's contract (the other half being the controller action that backs it), and
scaffolding those two together is `nano-add-custom-endpoint`'s internal-service path, not
this skill's. Read AGENTS.md's `### Api Clients` section first — it documents the built-in method
groups (`.Entity`/`.Auth`/`.Audit`/`.Identity`) and the `{TargetName}.Models/Api/` location
convention in full; this skill does not repeat that, only how to apply it.

If the user's request is actually about calling this client from some other app, not creating it,
that's `nano-add-api-client-configuration`'s job instead — point them there. If the request is
"add a custom method for this new endpoint," that's `nano-add-custom-endpoint`'s
internal-service path — point them there instead of doing it here.

## Before making any change, determine

1. **Does a client class already exist for this application?** Check `{ThisApp}.Models/Api/` for
   an existing `BaseApiClient`/`BaseIdentityApiClient` subclass. If one exists and this app's
   Identity status hasn't changed, there's nothing for this skill to do — say so.
2. **Does this application have persistent Identity?** Determines the base class:
   `BaseApiClient`/`BaseApiClient<TIdentity>` (no Identity, or identity type doesn't matter to
   callers), or `BaseIdentityApiClient<TUser[,TIdentity]>` (this app has Identity — unlocks the
   `.Identity` method group for every consumer). Check this app's own `Data:Identity` config /
   `BaseEntityUser`-derived entity.
   - **If a client already exists on the plain `BaseApiClient` base and this app has Identity
     configured** (e.g. Identity was added, via `nano-add-identity`, *after* the client was first
     created): that client **must be changed** to derive from
     `BaseIdentityApiClient<TUser[,TIdentity]>` instead — otherwise none of this app's
     identity-management endpoints (sign-up, password, roles, claims, API keys) are reachable
     through it. Don't leave it on the plain base class just because "add identity" wasn't the
     request that triggered this particular change.
3. **What's the client's name?** Consumers reference it by exact class name (the `App:Apis`
   dictionary key on their side must match it exactly) — pick something unambiguous and stable;
   renaming it later breaks every consumer's config.

## Client class

`{ThisApp}.Models/Api/{ClientName}.cs`:

```csharp
// Bare pass-through — no custom methods, relies entirely on .Entity/.Auth/.Audit
public class MyApi(ApiClient apiClient) : BaseApiClient(apiClient);
```
```csharp
// Identity-backed — adds the .Identity method group for every consumer
public class MyApi(ApiClient apiClient) : BaseIdentityApiClient<MyUser>(apiClient);
```

Nothing else goes in this class as part of this skill — no custom methods, no custom request
types. Once the class exists, adding a custom method for a specific endpoint is
`nano-add-custom-endpoint`'s job, paired with the controller action it calls.

## After making the change

- Show the user the file created (or the base-class change, if this was an Identity-driven
  conversion) and confirm which project it lives in (this app's own `.Models`, not a consumer's).
- If the user's actual goal was consuming this (or another) client from a different application,
  point them at `nano-add-api-client-configuration` instead.
- If the user's actual goal was adding a custom method for a specific endpoint, point them at
  `nano-add-custom-endpoint`'s internal-service path instead — this skill only produces the
  bare class.
