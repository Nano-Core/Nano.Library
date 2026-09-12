---
name: nano-add-identity
description: Configure Nano's persistent Identity store (Data:Identity) on a Nano.Library-based application that already has a Data provider - adds the Identity configuration section and the User entity/mapping/controller triplet Nano's identity actions attach to. Use when the user asks to add user accounts, a user store, sign-up, or persistent identity to a Nano API, Web, or Console application - not when they ask for login/JWT/authentication itself, that's a separate concern.
---

# Nano add identity

Configures Nano's persistent user/role/claim store on an existing Nano API, Web, or Console
application. Read `AGENTS.md`'s `## Nano.Data` → `#### Identity` section first — it documents the
full `Configuration` table and the auto-created roles in detail; this skill does not repeat that,
only how to apply it and add the `User` entity Nano's identity actions attach to.

**Identity is a separate concern from Authentication.** `Data:Identity` (this skill) is the
persistent *store* for users/roles/claims; `App:Authentication` (JWT/API key login) is a
different, independently-configurable section — AGENTS.md's own `#### Authentication` documents
JWT working standalone with no Identity at all ("transient" auth). This skill adds accounts and
identity-management endpoints (sign-up, password, roles, claims, API keys); it does not add any
way to log in. If the user actually wants login/JWT, that's a different skill.

## Before making any change, determine

1. **Is a Data provider already registered?** Check `Program.cs` for `.AddNanoData<TProvider,
   TContext>()`. Identity is layered onto the existing `DbContext`, not a separate package or
   provider — with none registered, stop and tell the user a Data provider needs to be added
   first (see `nano-add-data-provider`).
2. **Does an entity with the intended name already exist?** (conventionally `User`, but whatever
   the user actually names it) Three cases, not two:
   - **Already derives `BaseEntityUser`/`BaseEntityUser<TIdentity>`** — Identity is already wired
     to it. Say so and stop (or confirm before adding a second user entity — unusual, but not
     something to do silently).
   - **Already exists, but derives plain `BaseEntity`/`BaseEntity<TIdentity>`** (or one of the
     narrower capability bases) — this app already has its own reason for this entity to exist,
     independent of Identity. **Don't create a second, conflicting class.** Convert the existing
     one in place instead: change its base class to `BaseEntityUser`/`BaseEntityUser<TIdentity>`,
     and carry every existing custom property and any existing controller's custom actions over
     unchanged — this is an addition to what's there, not a replacement. First check its existing
     properties against what `BaseEntityUser` already provides (username, email address, phone
     number, etc.) — if a name collides, **stop and ask the user** how to resolve it (rename the
     existing property, or drop it in favor of the built-in one); don't silently pick for them.
   - **Doesn't exist at all** — create fresh, as below.
3. **No package reference needed.** Unlike the other add-provider skills, Identity isn't a
   separate NuGet package — `BaseEntityUser`, `BaseDbContext<TIdentity>`, `IIdentityRepository`,
   and `BaseEntityUserController` all ship as part of `Nano.Data`/`Nano.App.Api` themselves, so
   whichever Data provider package is already referenced already carries them. Nothing to add
   here.
4. **Entity identity type.** If entities already exist in the project, match their `TIdentity`
   (see the entity-scaffold skill's identity-type step) — `BaseEntityUser<TIdentity>` and
   `IIdentityRepository<TIdentity>` must agree with it.
5. **Application type.** Check `Program.cs` for `NanoApiApplication`, `NanoWebApplication`, or
   `NanoConsoleApplication` — same split as the entity-scaffold skill: API/Web get the full
   entity/mapping/criteria/controller set below; Console gets only the entity and mapping (no
   HTTP surface to route identity actions through), unless the user explicitly wants to drive
   identity from repository code in a worker.

## appsettings.json

Add the `Data:Identity` section from AGENTS.md's `#### Identity` example to the base
`appsettings.json`, as a sibling of `ConnectionString` under `Data`. Unlike `ConnectionString`,
the whole section lives in the base file — nothing in it is an environment-specific secret, so
there's no `appsettings.Development.json` split to make.

- Default `UseAudit` to `"None"` (the framework default) unless the user asks for identity
  models to be audited.
- **Leave `ApiKey` out entirely** (don't set even a `null` placeholder) — it's meaningful only
  once API-key authentication is added on top, which is Authentication's job, not this skill's;
  adding it here with nothing consuming it yet is dead config.

## User entity, mapping, and controller

This is the entity-scaffold skill's file set, with identity-specific base classes in place of
the plain ones — read that skill first for the file-location/project-layout rules (split
`.Models` project vs. single-project), which apply unchanged here. Ask the user for the entity
name if not given (conventionally `User`) and any additional properties beyond what
`BaseEntityUser` already provides — unless step 2 already found an existing plain entity to
convert, in which case its existing properties carry over as-is; don't ask for them again.

- **Data model** (`Data/<Entity>.cs`, or the `.Models` project in a split layout): derive from
  `BaseEntityUser`/`BaseEntityUser<TIdentity>` instead of `BaseEntity`. **If converting an
  existing entity** (step 2), this is the *only* change to the class itself — just the base type;
  every existing property and method stays. **If creating fresh**, add only the scalar properties
  the user actually asked for — `BaseEntityUser` already carries the identity fields (username,
  email, phone, etc.), don't redeclare them.
- **Mapping** (`Data/Mappings/<Entity>Mapping.cs`, main app project always): derive from
  `BaseEntityUserMapping<TEntity>`/`<TEntity,TIdentity>` (namespace
  `Nano.Data.Mappings.Identity`) instead of `BaseEntityMapping<TEntity>` — it additionally
  configures the required 1:1 relationship to the underlying `IdentityUser` row and an
  `IsActive` query filter, per AGENTS.md's Data Mappings table. **If converting an existing
  entity** (step 2), convert its existing mapping file the same way — just the base class;
  keep every custom `Configure(...)` statement already in it, still calling
  `base.Configure(builder)` first. **If creating fresh**, same `base.Configure(builder)`-first
  rule as a normal mapping.
- **Query criteria** (API/Web only): exactly as the entity-scaffold skill's File 3 — nothing
  identity-specific here.
- **Controller** (API/Web only, `Controllers/<Entity>sController.cs`): derive from
  `BaseEntityUserController<TEntity, TCriteria>`/`<TEntity,TIdentity,TCriteria>` (namespace
  `Nano.App.Api.Controllers`) instead of `BaseEntityController<...>`, and take an additional
  constructor dependency, `IIdentityRepository`/`IIdentityRepository<TIdentity>` (namespace
  `Nano.Data.Abstractions.Identity`), required, placed **after** `eventing`. **If this entity
  already had a controller with custom actions**, keep every one of them — this change is the
  base class and the added constructor dependency, nothing else.

  ⚠ **`BaseEntityUserController` does *not* use the single-optional-`IEventing?`-parameter
  pattern** the plain entity controllers (and this skill's own description above) might suggest.
  It exposes **two distinct constructor overloads** instead — one with no eventing parameter at
  all, one with a **required, non-nullable** `IEventing eventing`:
  ```csharp
  // No eventing provider registered in this project
  public class UsersController(ILogger<UsersController> logger, IRepository repository, IIdentityRepository identityRepository)
      : BaseEntityUserController<User, UserQueryCriteria>(logger, repository, identityRepository);

  // An eventing provider IS registered - eventing is required here, not IEventing?
  public class UsersController(ILogger<UsersController> logger, IRepository repository, IEventing eventing, IIdentityRepository identityRepository)
      : BaseEntityUserController<User, UserQueryCriteria>(logger, repository, eventing, identityRepository);
  ```
  Check `Program.cs` for `.AddNanoEventing<...>()` and pick the matching overload — don't write
  `IEventing? eventing` here and pass it positionally into the `eventing` slot: that's a nullable
  reference passed to a non-nullable parameter, which is a compile **error** (not just a warning)
  under this solution's `TreatWarningsAsErrors`. If no provider is registered, omit the parameter
  entirely (first overload) rather than passing `null`.

  This adds the identity-management endpoints from AGENTS.md's `#### Identity user controller`
  table (sign-up, password, roles, claims, API keys, etc.) on top of standard CRUD. Endpoints
  that don't match the current configuration (e.g. API-key management when API-key auth isn't
  enabled) aren't registered at all — nothing further to do for those until that's added.

## Api Client side

If this application exposes an Api Client for other apps to consume (`nano-define-api-client`),
and it was previously a plain `BaseApiClient`/`BaseApiClient<TIdentity>`, **it must now be
changed to derive from `BaseIdentityApiClient<TUser[,TIdentity]>`** (`TUser` = this `User` entity)
— that's what unlocks the `.Identity` method group (sign-up, password, roles, claims, API keys,
etc.) for every consumer of this client. A client left on the plain base class after Identity is
added has no way to expose any of the endpoints this skill just enabled. Check for an existing
client class in `{ThisApp}.Models/Api/` and update its base class as part of this change — don't
leave that as a follow-up the user has to remember separately.

## After making the change

- Show the user every file touched.
- Remind them explicitly: this adds accounts and identity-management endpoints, but no way to
  log in yet — `nano-add-authentication-jwt` (JWT) or `nano-add-authentication-apikey` (API key, works
  standalone without JWT) are separate skills, needed before any of the `identity`-role endpoints
  are reachable by an actual caller.
- If step 1 or 2 stopped the skill early, that's the whole response — don't partially wire
  Identity while waiting on a prerequisite.
