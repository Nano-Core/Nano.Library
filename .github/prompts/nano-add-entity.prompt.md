---
mode: agent
description: Scaffold a new Nano.Library entity - data model and EF Core mapping, plus query criteria and a CRUD controller for API/Web applications - following Nano framework conventions. Use when the user asks to add a new entity, resource, or CRUD endpoint to a Nano-based application (a project with an AGENTS.md describing Nano, or that references Nano.Library/NanoCore NuGet packages).
---

# Nano add entity

Generates the files Nano needs for a new entity: data model and EF Core mapping always; query
criteria and a CRUD controller too, unless the target is a Console application (Console apps
have no HTTP surface, so there's nothing for either of those to serve - see step 3 below). Read
`AGENTS.md` in the target repo root first if present - it documents the exact base classes and
gotchas for that specific solution; this skill assumes the general Nano.Library conventions and
defers to a project's own AGENTS.md on any conflict.

## Before generating anything, determine

1. **Is a Data provider already registered?** Check `Program.cs` for `.AddNanoData<TProvider,
   TContext>()` and confirm a `DbContext` exists in the project (e.g. `Data/<X>DbContext.cs`).
   An entity's mapping only takes effect once Nano's `MapEntities<TIdentity>` discovery attaches
   it to a registered context - with no Data provider, the generated files would be dead code
   with nothing to persist them. If none is registered, stop and tell the user a Data provider
   needs to be added first; don't generate the entity anyway "for later."
2. **Entity name and properties.** Ask the user if not already given in the request - need
   at minimum the entity name (singular, PascalCase, e.g. `Product`) and its scalar
   properties (name + type). Don't invent business fields that weren't asked for.
3. **Application type.** Check `Program.cs` for `NanoApiApplication`, `NanoWebApplication`, or
   `NanoConsoleApplication`.
   - **API or Web**: generate all four files below.
   - **Console**: generate only File 1 (data model) and File 2 (mapping) - skip Files 3 and 4
     entirely (query criteria and controllers are API-request concepts; a Console app has
     nothing to route them to). Confirm this with the user only if they explicitly asked for a
     controller or query criteria on a Console app - otherwise just skip silently; a Console
     app's data folder only ever needs `Data/Models/` and `Data/Mappings/`, never
     `Controllers/`/`Criterias/`.
4. **Project layout.** Look for a `<AppName>.Models` project alongside the main app project
   (check the `.sln` or list sibling folders).
   - **Split layout** (a `.Models` project exists): entity model and query criteria (if
     applicable) go in the `.Models` project (they're part of the API client contract other
     services consume); the mapping and controller (if applicable) go in the main app project.
   - **Single-project layout** (no `.Models` project): all files go in the one app project.
5. **Identity type.** Check the `DbContext`/`DbContextFactory` and other entities in the
   project for a `TIdentity` type parameter (e.g. `BaseEntity<int>`). If every existing
   entity just uses plain `BaseEntity` (implicit `Guid`), match that. Never introduce a
   non-`Guid` identity unless the project already uses one consistently - it's a
   cross-cutting decision (affects the entity, mapping, controller, repository calls,
   and API client), not something to add on a whim for one entity.
6. **Existing conventions.** Skim one existing entity/mapping (and controller, if
   applicable) triplet in the project (if any exist) for property style, nullable-reference
   usage, and namespace layout, and match it.
7. **Every entity gets a generic controller - full stop, independent of whatever else exists for
   it.** This is not conditional on a query criteria class already existing, and **not conditional
   on whether an Api Client happens to reference the entity** - that's a separate concern this
   skill doesn't judge. When retrofitting controllers onto entities that already exist: for each
   entity with no controller yet, generate the query criteria class first if one doesn't already
   exist (File 3), then the controller against it (File 4) - every entity, not just the ones an
   Api Client happens to call out. **If a controller already exists for an entity, don't recreate
   it** - move on to the next entity.

   This skill scaffolds the generic substrate only - it doesn't search for or reason about custom
   Api Client requests that might target this entity's controller. Whether a pre-existing custom
   request already points here (only possible when retrofitting a controller onto an entity that
   already existed) - and, if so, how its stub action gets scaffolded, named, and checked for
   route collisions - is `nano-add-custom-endpoint`'s job, including creating the controller
   itself inline if this skill hasn't been run yet. That skill already owns
   controller-resolution, route-constant conventions, and collision/override handling;
   duplicating any of that judgment here would just be two places for the same rule to drift
   apart.
8. **Is this entity `[Publish]`d, `[Subscribe]`d, or neither?** (AGENTS.md's `### Entity Events`)
   Ask if it isn't already clear from the request - this changes both the CRUD tier (File 1/File
   4) and, for `[Subscribe]`, the entity's actual shape:
   - **`[Publish]`**: a normal entity, full CRUD by default, additionally marked `[Publish](...)`
     with the property paths other applications are allowed to replicate. Only add paths the
     request actually asked to expose - don't publish every scalar property by default.
   - **`[Subscribe]`**: this entity's shape is **not** something to invent from user-provided
     field names - it must mirror the actual publisher. Locate the source entity's `[Publish]`
     attribute (if it's locally visible, e.g. another app in this same workspace) and derive:
     the class name (must match the publisher's `TypeName` - its published type's simple class
     name), and the properties (the **leaf segment of each publish path**, flattened/denormalized,
     not a structural copy of the source's navigation shape - see AGENTS.md's Subscribing
     example). If the publisher isn't locally visible, ask the user for the exact `TypeName` and
     leaf property names/types instead of guessing a shape that has to match byte-for-byte for
     the eventing handler to find it. See File 1 below for the resulting CRUD-tier restriction.
   - **Neither**: a normal entity, full CRUD by default, no Entity Events involvement.

## File 1 - Data model

Location: `Data/<Entity>.cs` in the split layout's `.Models` project, or `Data/<Entity>.cs`
in the single-project layout.

```csharp
public class <Entity> : BaseEntity
{
    public string Name { get; set; } = null!;
    // ...other scalar properties as requested
}
```

- Derive from `BaseEntity` (Guid identity) or `BaseEntity<TIdentity>` - match the project's
  existing convention (see step 5 above).
- For restricted CRUD (e.g. read-only, or no delete), derive instead from
  `BaseEntityReadOnly`, `BaseEntityCreatable`, `BaseEntityUpdatable`,
  `BaseEntityCreatableAndUpdatable`, `BaseEntityCreatableAndDeletable`, or `BaseEntityDeletable` -
  ask the user if the request implies one of these rather than full CRUD.
- **`[Subscribe]` entities are restricted CRUD by convention, not a case to ask about.** Per step
  8, a `[Subscribe]` entity is a local replica kept in sync by the built-in
  `EntityEventingHandler` whenever the publishing app's source entity changes - update/delete
  normally happen through that Subscribe mechanism, not through this app's own HTTP surface. Use
  `BaseEntityCreatable` for the entity and `BaseEntityCreatableController` for its controller
  (File 4) regardless of what tier was otherwise requested - Edit/Delete shouldn't be exposed to
  callers on a subscribed entity. The entity's shape itself (class name, properties) comes from
  step 8's publisher lookup, not from this skill's normal "ask the user for scalar properties"
  step 2 - don't invent field names for a `[Subscribe]` entity.
- **`[Publish]` entities** get the attribute per step 8, with no change to CRUD tier or shape -
  they're scaffolded exactly like any other entity, just additionally marked for replication.
- Use `required`/`= null!` per the project's existing nullable-reference style, not your own
  default.
- **Every `string` property gets an explicit `[MaxLength(n)]`** - never leave a string column
  unbounded. Pick `n` from what the property actually represents, not one number applied
  mechanically: a `Name` is usually fine at `128`, an email address or URL wants more (`256`), a
  short code/abbreviation wants less (`32`/`16`), free-form notes/descriptions may need more still
  - use `128` as the reasonable default when nothing about the property's own meaning suggests a
  different size, not as a rule to apply without thinking. This annotation and File 2's
  `.HasMaxLength(n)` must agree - see File 2's note on keeping both in sync.
- **`bool` and `enum` properties get an explicit default**, via `[DefaultValue(...)]` on the
  property, matching whatever the property is initialized to in the class (`= true`/
  `= MyEnum.SomeMember`) - don't leave a `bool`/`enum` property's default implicit (C#'s own
  `false`/first-member-value default) without stating it, since that's exactly the kind of
  intent that's invisible on read until it's a production surprise. File 2's `.HasDefaultValue(...)`
  must match the same value.
- **Date/time properties are always named `...At`** - `CreatedAt`, `StartsAt`, `EndsAt`, `CancelledAt`,
  `ShreddedAt`, never `...Date`, `...Timestamp`, or a `...Utc` suffix (a `DateTime` holding UTC is still just
  `...At`). Name it after the event: past tense for something that happened (`ClosedAt`), a plain "starts/ends"
  form for a boundary (`StartsAt`, `EndsAt`). This carries through everywhere the property appears: the mapping,
  query criteria filters, requests, responses and their doc comments.
- **Derive, don't store, a value that is a pure function of other columns and the clock** (e.g. a lifecycle
  status computed from `StartsAt`/`EndsAt`). Expose it as a getter-only property marked `[NotMapped]` (and
  `.Ignore(...)` in the mapping), and have the query criteria translate a filter on it into conditions on the
  underlying columns. A stored status column for this goes stale, and a database generated column can't
  reference the current time.

## File 2 - Data mapping

Location: `Data/Mappings/<Entity>Mapping.cs` in the main app project (always here, even in
the split layout - mappings are EF Core-only, never part of the shared API client models).

```csharp
public class <Entity>Mapping : BaseEntityMapping<<Entity>>
{
    public override void Configure(EntityTypeBuilder<<Entity>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Configure(builder);

        builder
            .Property(x => x.Name);
    }
}
```

- **Always call `base.Configure(builder)`** before your own configuration - omitting it
  silently breaks inherited Nano behavior (soft delete, audit, etc.). This is the single
  most common mistake when writing a mapping by hand.
- **Configure every one of the entity's own properties explicitly - including navigations and
  collections - never rely on EF's conventions to fill in what isn't written down.** An implicit,
  convention-inferred relationship is exactly the kind of mistake that's invisible until it's a
  production bug (e.g. EF silently creating a shadow FK column for a stray navigation property
  with no real relationship behind it). Being explicit is what makes a mistake visible on read,
  not what EF happens to guess correctly most of the time.
- **List properties in the same order they're declared on the entity** - one `.Property(...)`/
  `.HasOne(...)`/`.HasMany(...)` block per property, top to bottom, matching the class. This
  makes the mapping file scannable against the entity file side by side: a missing or
  out-of-place property is immediately visible, not something that only surfaces when something
  breaks at runtime.
  - **Scalar property**: `.Property(x => x.Y)`, with `.IsRequired()` matching the property's own
    nullability. For a `string`, always add `.HasMaxLength(n)` matching File 1's `[MaxLength(n)]`
    exactly - the two must agree; a mismatch means the database allows more than the API's own
    model validation does, or vice versa. For a `bool`/`enum` property, add
    `.HasDefaultValue(...)` matching File 1's `[DefaultValue(...)]` and the property's own C#
    default - explicit at the database level too, not just in the model.
  - **FK scalar + its reference navigation** (usually adjacent in the entity): one combined
    `.HasOne(x => x.Nav).WithMany(...)/.WithOne(...).HasForeignKey(x => x.FkId)` block, positioned
    where the pair sits in the property order, **plus an explicit `.OnDelete(DeleteBehavior.…)`**
    on the same chain - never leave delete behavior to EF's own inferred default (`Cascade` for a
    required/non-nullable FK, `ClientSetNull` for an optional one). State it explicitly even when
    the desired behavior happens to match what EF would infer anyway: the point is that a reader
    (or a future edit) sees the intended behavior written down, not that it produces a different
    result today. Pick the value deliberately per relationship - `Cascade` when the dependent
    genuinely can't exist without the parent (most owned child rows), `Restrict` when deleting the
    parent while dependents still exist should be a hard error instead of silently taking them
    with it, `SetNull` only for a genuinely optional reference. Don't default to `Cascade`
    everywhere just because it's often EF's own inferred behavior for required FKs.
  - **Inverse collection/reference navigation with no FK of its own** (the principal side of a
    relationship whose FK is declared in the *dependent* entity's own mapping): configure it
    explicitly here too, not just with a comment - `.HasMany(x => x.Children).WithOne(x => x.Parent)`
    (or `.HasOne(...).WithOne(...)` for a 1:1 principal side), with `.IsRequired()` added whenever
    the dependent's own FK property is non-nullable (matching what the dependent side's own
    `.HasForeignKey(...)` chain already declares) - **without** repeating `.HasForeignKey(...)`
    itself, that's declared once, on the dependent side. **Repeat the same `.OnDelete(...)` call
    from the dependent side's chain here too** - declaring delete behavior from both ends,
    matching, is what makes it visible when scanning *this* entity's mapping file alone, the same
    reasoning as declaring the relationship itself from both ends. EF Core matches the two
    configurations by navigation pairing and merges them as the same relationship, so writing it
    from both ends is safe as long as they agree. **No comment needed** for the relationship/FK
    itself - the `.HasMany(...).WithOne(...)` call already says everything a reader needs; a
    comment repeating "FK owned/declared in the other file" for every single one of these is
    noise, not information. The `.OnDelete(...)` restatement is the one thing actually worth
    duplicating, not narrating.
  - **A navigation with no corresponding FK/relationship anywhere** (the model doesn't actually
    support what the property implies): don't let it fall through to an accidental EF-invented
    shadow relationship. Flag it to the user and ask what it should be - don't guess a
    relationship that isn't in the model. If the user says to leave the property in place without
    resolving it yet, mark it `.Ignore(x => x.Y)` explicitly (with a comment saying why) rather
    than leaving it for EF's convention to silently invent something.
  - **A unique constraint** (a property, or a combination of properties, that must be unique):
    `.HasIndex(x => x.Y).IsUnique()` (or `.HasIndex(x => new { x.A, x.B }).IsUnique()` for a
    composite key) - explicit, never left for a `[Key]`-adjacent attribute or assumed convention
    to imply. Ask the user which properties (if any) need this if it isn't already obvious from
    the request (e.g. "domain must be unique across all tenants").
  - **Many-to-many relationships are modeled as an explicit join entity, never
    `.HasMany(...).WithMany(...)`.** EF Core's implicit many-to-many (a hidden join table with no
    entity of its own) means there's no place to hang extra columns later (an assignment
    timestamp, a role on the relationship, etc.) without a breaking schema change, and no explicit
    mapping file to read the relationship's actual shape from. Model it as its own entity (e.g.
    `Product`/`Tag` → a real `ProductTag` entity with `ProductId`/`TagId` FKs) with its own File
    1/File 2 pair - a normal one-to-many-to-one shape from each side, not a special case - even
    when the join entity currently has no columns beyond the two FKs. A join entity with nothing but
    its foreign keys is only ever added and removed, never edited (editing one would just re-point
    it), so it derives from `BaseEntityCreatableAndDeletable` in File 1 and gets
    `BaseEntityCreatableAndDeletableController` in File 4.
- **Index every property the entity is queried or sorted by** - one `HasIndex(...)` in the mapping for each
  property File 3's query criteria filters on, and each property an ordering (`Order.By`) or a keyword
  search reaches, declared at the end of `Configure`, after the properties and relationships. Foreign keys
  already get an index from EF's conventions, so don't repeat those unless a composite covers them better;
  don't index a low-selectivity flag (a plain `bool` or status on its own). When a list is always filtered
  by one property and sorted by another (a foreign key, newest first), use one composite index in that
  order (`HasIndex(x => new { x.ParentId, x.CreatedAt })`), which replaces a plain single-column index on
  the same leading property. An index only helps a text search that anchors at the start of the value,
  which is why File 3 uses `StartsWith`, not `Contains`. If it isn't clear from the request how the entity
  is listed, searched, and sorted, ask before guessing which indexes it needs.
- No registration step needed - Nano auto-discovers mappings via
  `ModelBuilderExtensions.MapEntities<TIdentity>` at startup.
- Add a new EF Core migration after this file exists: `dotnet ef migrations add <Name>`
  from the app project directory (only do this if the user asked you to, or if the project's
  existing workflow clearly expects a migration per entity - check `Migrations/` for
  precedent first).

## File 3 - Query criteria (API/Web only - skip for Console)

Location: `Criterias/<Entity>QueryCriteria.cs` in the split layout's `.Models` project, or
`Criterias/<Entity>QueryCriteria.cs` in the single-project layout.

```csharp
public class <Entity>QueryCriteria : BaseQueryCriteria
{
    public virtual string? Name { get; set; }

    public override IList<CriteriaExpression> GetExpressions()
    {
        var expressions = base.GetExpressions();

        var expression = expressions.FirstOrDefault() ?? new CriteriaExpression();

        if (!string.IsNullOrEmpty(this.Name))
        {
            expression
                .StartsWith("Name", this.Name);
        }

        expressions
            .Add(expression);

        return expressions;
    }
}
```

- Only add filter properties for fields that make sense to search/filter by - don't
  mechanically add one filter per scalar property on the entity.
- Every filter property must be `virtual` and nullable.
- **Text search uses `StartsWith`, never `Contains`** - a `Contains` (leading wildcard) can't use an index
  and scans the whole table as it grows; `StartsWith` can. Whatever a criteria property filters on gets a
  matching `HasIndex` in File 2's mapping.
- Use the `CriteriaExpression` builder methods appropriate to each property's type
  (`StartsWith` for strings, `Equal`/`GreaterThan`/etc. for numerics and dates)
  - check the project's other query criteria classes for the operations actually available,
  don't guess.

## File 4 - Controller (API/Web only - skip for Console)

Location: `Controllers/<Entity>sController.cs` in the main app project.

```csharp
public class <Entity>sController(ILogger<<Entity>sController> logger, IRepository repository, IEventing? eventing)
    : BaseEntityController<<Entity>, <Entity>QueryCriteria>(logger, repository, eventing)
{
    // Custom actions, if any
}
```

- **Naming is load-bearing, not cosmetic**: the class name must be the entity name with a
  literal `s` appended, then `Controller` (e.g. `Product` → `ProductsController`,
  `Country` → `CountrysController` - note this is naive `+s` pluralization, not proper
  English plural rules; Nano derives the route segment from the class name). Do not
  "correct" irregular plurals.
- Check whether the project actually registers an eventing provider (look for
  `AddNanoEventing<...>()` in `Program.cs`). If it doesn't, drop the `IEventing? eventing`
  parameter and the corresponding base-constructor argument - an unused optional eventing
  dependency is harmless, but match what sibling controllers in the same project actually do.
- If the identity type isn't `Guid` (per step 5), the controller generic list needs the
  identity type too: `BaseEntityController<<Entity>, <TIdentity>, <Entity>QueryCriteria>`.
- **`BaseEntityController<<Entity>, <Entity>QueryCriteria>` (full CRUD) is the default for every
  entity, with no exceptions other than `[Subscribe]` and a foreign-keys-only join entity (below).**
  Having custom actions on the same
  controller - even ones that overlap in intent with a generic CRUD action - is not on its own a
  reason to narrow the tier. A colliding route is a defect to flag (see below), not a signal to
  remove generic capability the entity is otherwise entitled to.
- **`[Subscribe]` entities use `BaseEntityCreatableController<<Entity>, <Entity>QueryCriteria>`**,
  not `BaseEntityController` - per File 1's note, update/delete happen through the Subscribe
  mechanism, not this app's HTTP surface, so only Get/Query/Create are exposed here.
- **A join entity with only foreign keys uses
  `BaseEntityCreatableAndDeletableController<<Entity>, <Entity>QueryCriteria>`** (Get/Query/Create/Delete,
  no Edit) - see File 2's many-to-many note. Any rule that must hold on add or remove (e.g. the parent is
  immutable once in use) is then an override of the create/delete actions only, with no edit path left
  open around it. These two are the only cases that change the default tier.
- No manual registration needed - Nano's MVC discovery picks up the controller
  automatically from the assembly.

## Postman collection (API/Web only - skip for Console)

If the application already has a Postman collection (`Postman_<AppName>.json` in the application's own
folder, next to its `README.md`), add the new entity's folder to it, following the
`nano-add-postman-collection` skill's conventions: only the generic actions the controller's actual base
class exposes (see File 4), placed in FK-dependency order, with Id/name chaining test scripts. Edit only the
new folder; don't regenerate or reorder the rest of the file, and remind the user to **Replace**-import it in
Postman.

**If no collection exists, do nothing Postman-related** - don't create one. The entity is picked up when the
user later generates the collection with `nano-add-postman-collection`.

## After generating

- Show the user the files generated and where they were placed (two for Console, four for
  API/Web); don't silently also modify `Program.cs`, add NuGet packages, or run
  `dotnet ef migrations add` unless they ask - scaffolding the entity is the task, not deciding
  the rest of the rollout for them.
- If the project has an existing entity with the same shape you can point to as a working
  reference, mention it - it's the fastest way for the user to sanity-check the result.
