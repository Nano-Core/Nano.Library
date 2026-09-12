---
mode: agent
description: Remove Nano's persistent Identity store (Data:Identity) from a Nano.Library-based application - unregisters the Identity configuration and deletes the User entity/mapping/controller triplet Nano's identity actions attached to. Use when the user asks to remove user accounts, the user store, or persistent identity from a Nano API, Web, or Console application - not for removing authentication/login itself, that's nano-remove-authentication-jwt/nano-remove-authentication-apikey.
---

# Nano remove identity

Fully removes Nano's persistent Identity store from an existing Nano API, Web, or Console
application - the counterpart to `nano-add-identity`. Read that skill first - this one undoes
exactly what it adds.

## Before making any change, determine

1. **Is Identity currently configured?** Check the base `appsettings.json` for `Data:Identity`,
   and the project for an entity deriving `BaseEntityUser`/`BaseEntityUser<TIdentity>`. If neither
   exists, say so and stop.
2. **What depends on it?** Two distinct risks, different severities - check for both:
   - **Startup crash.** Search for `IIdentityRepository`/`IIdentityRepository<TIdentity>` used as
     a **required** constructor parameter anywhere in the project. This is a guaranteed hit, not
     a maybe: the identity entity's own controller (`nano-add-identity`'s own template) always
     takes it as a required parameter, so that controller crashes DI resolution the instant
     Identity is gone - the app won't start at all, same class of failure as the Data-provider
     and Eventing removal skills' crash checks.
   - **Authentication degrades, doesn't crash - but is worth flagging just as clearly.** If
     `App:Authentication:Jwt` is configured on this app (`nano-add-authentication-jwt`), removing
     Identity silently drops `AuthIdentityRepository` back to `null` (AGENTS.md's sub-repository
     table: populated only when Identity is configured) - `/auth/login`, `/auth/login/refresh`,
     and `/auth/logout` stop being registered, no exception, they just disappear. If
     `Data:Identity:ApiKey:Secret` is configured (`nano-add-authentication-apikey`), it's removed
     along with the rest of `Data:Identity` (it's a child of it) - API-key auth disappears
     entirely, including `/auth/login/apikey` if that was in use. Neither of these crashes the
     app, but both are significant behavior changes on an app that may have real callers - this
     skill does not touch `App:Authentication` itself, so if the user also wants Authentication
     removed, point them at `nano-remove-authentication-jwt`/`nano-remove-authentication-apikey`
     rather than leaving it half-configured and pointing at nothing.
   If either applies, tell the user exactly what removing Identity will do (crash vs. silent
   endpoint loss) and confirm before proceeding - don't remove out from under them without saying
   so.
3. **No package reference to remove.** Matches `nano-add-identity`: Identity was never a separate
   NuGet package, so there's nothing to remove from the `.csproj` here either.

## appsettings.json

Remove the `Data:Identity` section entirely from the base `appsettings.json`. There's no
`appsettings.Development.json` override to also clean up - per `nano-add-identity`, the whole
section lives in the base file only.

## User entity, mapping, and controller

Delete the full file set `nano-add-identity` created for whichever entity derives
`BaseEntityUser`/`BaseEntityUser<TIdentity>` (conventionally, but not always, named `User` - find
it by searching for that base class if the name isn't obvious):

- `Data/<Entity>.cs` (or the `.Models` project in a split layout).
- `Data/Mappings/<Entity>Mapping.cs`.
- `Criterias/<Entity>QueryCriteria.cs` (API/Web only - never existed for Console).
- `Controllers/<Entity>sController.cs` (API/Web only).

Don't leave the mapping behind even temporarily - `BaseEntityUserMapping<TEntity>` configures a
required relationship to the underlying `IdentityUser` row (AGENTS.md's Data Mappings table),
which stops being mapped the instant `Data:Identity` is gone; leaving the entity/mapping in place
without the config breaks EF model building at startup, not just at the controller level covered
in step 2.

## After making the change

- Show the user every file touched/deleted.
- Restate anything flagged in step 2 - the guaranteed controller crash, plus the specific
  Authentication endpoints that silently disappear if Jwt/API-key auth was configured - one more
  time here, even if the user already confirmed it.
- If the user also wants Authentication removed, say explicitly that this skill didn't touch it
  and point them at `nano-remove-authentication-jwt`/`nano-remove-authentication-apikey`.
