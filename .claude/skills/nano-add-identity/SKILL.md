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
2. **Is Identity already configured?** Check the base `appsettings.json` for a `Data:Identity`
   section, and the project for an entity deriving `BaseEntityUser`/`BaseEntityUser<TIdentity>`.
   If both already exist, say so and stop (or confirm with the user before adding a second user
   entity — unusual, but not something to do silently).
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
`BaseEntityUser` already provides.

- **Data model** (`Data/<Entity>.cs`, or the `.Models` project in a split layout): derive from
  `BaseEntityUser`/`BaseEntityUser<TIdentity>` instead of `BaseEntity`. Add only the scalar
  properties the user actually asked for — `BaseEntityUser` already carries the identity
  fields (username, email, phone, etc.), don't redeclare them.
- **Mapping** (`Data/Mappings/<Entity>Mapping.cs`, main app project always): derive from
  `BaseEntityUserMapping<TEntity>`/`<TEntity,TIdentity>` (namespace
  `Nano.Data.Mappings.Identity`) instead of `BaseEntityMapping<TEntity>` — it additionally
  configures the required 1:1 relationship to the underlying `IdentityUser` row and an
  `IsActive` query filter, per AGENTS.md's Data Mappings table. Same
  `base.Configure(builder)`-first rule as a normal mapping.
- **Query criteria** (API/Web only): exactly as the entity-scaffold skill's File 3 — nothing
  identity-specific here.
- **Controller** (API/Web only, `Controllers/<Entity>sController.cs`): derive from
  `BaseEntityUserController<TEntity, TCriteria>`/`<TEntity,TIdentity,TCriteria>` (namespace
  `Nano.App.Api.Controllers`) instead of `BaseEntityController<...>`, and take an additional
  constructor dependency, `IIdentityRepository`/`IIdentityRepository<TIdentity>` (namespace
  `Nano.Data.Abstractions.Identity`), required, placed **after** `eventing`:
  ```csharp
  public class UsersController(ILogger<UsersController> logger, IRepository repository, IEventing? eventing, IIdentityRepository identityRepository)
      : BaseEntityUserController<User, UserQueryCriteria>(logger, repository, eventing, identityRepository);
  ```
  Same `IEventing? eventing` check as the entity-scaffold skill's controller step: include it
  only if the project has an eventing provider registered (check `Program.cs` for
  `.AddNanoEventing<...>()`), otherwise drop the parameter and the corresponding base-constructor
  argument entirely.
  This adds the identity-management endpoints from AGENTS.md's `#### Identity user controller`
  table (sign-up, password, roles, claims, API keys, etc.) on top of standard CRUD. Endpoints
  that don't match the current configuration (e.g. API-key management when API-key auth isn't
  enabled) aren't registered at all — nothing further to do for those until that's added.

## After making the change

- Show the user every file touched.
- Remind them explicitly: this adds accounts and identity-management endpoints, but no way to
  log in yet — `nano-add-authentication-jwt` (JWT) or `nano-add-authentication-apikey` (API key, works
  standalone without JWT) are separate skills, needed before any of the `identity`-role endpoints
  are reachable by an actual caller.
- If step 1 or 2 stopped the skill early, that's the whole response — don't partially wire
  Identity while waiting on a prerequisite.
