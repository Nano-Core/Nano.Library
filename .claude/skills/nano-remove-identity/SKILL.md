---
name: nano-remove-identity
description: Remove Nano's persistent Identity store (Data:Identity) from a Nano.Library-based application - unregisters the Identity configuration and deletes the User entity/mapping/controller triplet Nano's identity actions attached to. Use when the user asks to remove user accounts, the user store, or persistent identity from a Nano API, Web, or Console application - not for removing authentication/login itself, that's nano-remove-authentication-jwt/nano-remove-authentication-apikey.
---

# Nano remove identity

Fully removes Nano's persistent Identity store from an existing Nano API, Web, or Console
application — the counterpart to `nano-add-identity`. Read that skill first — this one undoes
exactly what it adds.

## Before making any change, determine

1. **Is Identity currently configured?** Check the base `appsettings.json` for `Data:Identity`,
   and the project for an entity deriving `BaseEntityUser`/`BaseEntityUser<TIdentity>`. If neither
   exists, say so and stop.
2. **What depends on it?** Two distinct risks, different severities — check for both:
   - **Startup crash.** Search for `IIdentityRepository`/`IIdentityRepository<TIdentity>` used as
     a **required** constructor parameter anywhere in the project. This is a guaranteed hit, not
     a maybe: the identity entity's own controller (`nano-add-identity`'s own template) always
     takes it as a required parameter, so that controller crashes DI resolution the instant
     Identity is gone — the app won't start at all, same class of failure as the Data-provider
     and Eventing removal skills' crash checks.
   - **Authentication degrades, doesn't crash — but is worth flagging just as clearly.** If
     `App:Authentication:Jwt` is configured on this app (`nano-add-authentication-jwt`), removing
     Identity silently drops `AuthIdentityRepository` back to `null` (AGENTS.md's sub-repository
     table: populated only when Identity is configured) — `/auth/login`, `/auth/login/refresh`,
     and `/auth/logout` stop being registered, no exception, they just disappear. If
     `Data:Identity:ApiKey:Secret` is configured (`nano-add-authentication-apikey`), it's removed
     along with the rest of `Data:Identity` (it's a child of it) — API-key auth disappears
     entirely, including `/auth/login/apikey` if that was in use. Neither of these crashes the
     app, but both are significant behavior changes on an app that may have real callers — this
     skill does not touch `App:Authentication` itself, so if the user also wants Authentication
     removed, point them at `nano-remove-authentication-jwt`/`nano-remove-authentication-apikey`
     rather than leaving it half-configured and pointing at nothing.
   If either applies, tell the user exactly what removing Identity will do (crash vs. silent
   endpoint loss) and confirm before proceeding — don't remove out from under them without saying
   so.
3. **Does this app's own Api Client derive from `BaseIdentityApiClient<TUser[,TIdentity]>`?**
   Check `{ThisApp}.Models/Api/` for a client using the identity-backed base class with this
   app's `User` entity as `TUser`. If so, it must be reverted to the plain
   `BaseApiClient`/`BaseApiClient<TIdentity>` base as part of this same change, not left for
   later — either as a **guaranteed compile break** (if step 5 below deletes the entity, `TUser`
   stops existing) or as a client that compiles but lies about what the target app actually
   serves (if step 5 converts the entity back to a plain one instead — the `.Identity` method
   group it still exposes has nothing left to call either way).
4. **Does the entity carry anything beyond what `nano-add-identity` itself would have
   generated?** This determines whether step 5 below deletes the entity outright or converts it
   back to a plain one — check before touching any file:
   - Custom scalar properties on the entity beyond what `BaseEntityUser` provides.
   - Custom controller actions beyond the standard CRUD + identity-management set
     `nano-add-identity` added.
   - Any other code in the project that depends on this entity for a reason that has nothing to
     do with Identity (e.g. it's referenced by other entities' navigations, or it's genuinely this
     app's core business entity and Identity was layered onto it after the fact, per
     `nano-add-identity`'s own "convert an existing plain entity" case).
   If none of these apply, the entity is pure boilerplate from `nano-add-identity` with nothing
   else depending on it — full deletion (step 5's first option) is safe. If any apply, **don't
   default to deleting it** — ask the user whether they want full deletion anyway (only correct
   if the entity truly has no remaining purpose once Identity is gone) or an in-place conversion
   back to a plain entity that keeps everything custom intact.
5. **No package reference to remove.** Matches `nano-add-identity`: Identity was never a separate
   NuGet package, so there's nothing to remove from the `.csproj` here either.

## appsettings.json

Remove the `Data:Identity` section entirely from the base `appsettings.json`. There's no
`appsettings.Development.json` override to also clean up — per `nano-add-identity`, the whole
section lives in the base file only.

## User entity, mapping, and controller

Find the entity by searching for whichever one derives `BaseEntityUser`/`BaseEntityUser<TIdentity>`
(conventionally, but not always, named `User`). What happens to it depends on step 4's answer:

**Pure boilerplate (no custom content, or the user chose full deletion anyway):** delete the
whole file set:
- `Data/<Entity>.cs` (or the `.Models` project in a split layout).
- `Data/Mappings/<Entity>Mapping.cs`.
- `Criterias/<Entity>QueryCriteria.cs` (API/Web only — never existed for Console).
- `Controllers/<Entity>sController.cs` (API/Web only).

**Has custom content the user wants kept:** convert in place instead of deleting — the mirror
image of `nano-add-identity`'s "convert an existing plain entity" case:
- **Data model**: change the base class from `BaseEntityUser`/`BaseEntityUser<TIdentity>` back to
  `BaseEntity`/`BaseEntity<TIdentity>` — nothing else about the class changes; every custom
  property stays.
- **Mapping**: change the base class from `BaseEntityUserMapping<TEntity>`/`<TEntity,TIdentity>`
  back to `BaseEntityMapping<TEntity>`/`<TEntity,TIdentity>` — this is what actually matters here,
  not optional cleanup: `BaseEntityUserMapping<TEntity>` configures a required relationship to the
  underlying `IdentityUser` row (AGENTS.md's Data Mappings table), which stops being mapped the
  instant `Data:Identity` is gone. Leaving the old base class in place breaks EF model building at
  startup, not just the controller-level crash covered in step 2 — converting the mapping is not
  something to skip even when keeping the entity.
- **Query criteria**: untouched — nothing identity-specific lives here either way.
- **Controller**: change the base class from `BaseEntityUserController<...>` back to
  `BaseEntityController<...>` (or whichever narrower capability base fits), drop the
  `IIdentityRepository` constructor parameter, and remove only the identity-management actions
  `nano-add-identity` added — keep every other custom action as-is.

Either way, don't leave a `BaseEntityUserMapping`/`BaseEntityUserController` behind pointed at a
`Data:Identity` section that no longer exists — that's the one part of this that's never safe to
defer, in either branch.

## Api Client side

If step 3 found a client on `BaseIdentityApiClient<TUser[,TIdentity]>`, **revert it to
`BaseApiClient`/`BaseApiClient<TIdentity>`** now, as part of this same change, regardless of
which branch the section above took: the `.Identity` method group it exposed has nothing left to
call either way — the identity-management actions are gone from the controller whether the
entity itself was deleted or just converted back to a plain one. If the entity was deleted
outright, this is also a guaranteed compile break (`TUser` stops existing), not just a dangling
capability. Don't leave this as a follow-up for the user to remember separately.

## After making the change

- Show the user every file touched/deleted/converted, including any Api Client reverted to its
  plain base class, and say explicitly which branch step 4 took (full deletion vs. converted back
  to a plain entity) and why.
- Restate anything flagged in step 2 — the guaranteed controller crash, plus the specific
  Authentication endpoints that silently disappear if Jwt/API-key auth was configured — one more
  time here, even if the user already confirmed it.
- If the user also wants Authentication removed, say explicitly that this skill didn't touch it
  and point them at `nano-remove-authentication-jwt`/`nano-remove-authentication-apikey`.
