---
name: nano-scaffold-entity
description: Scaffold a new Nano.Library entity - data model and EF Core mapping, plus query criteria and a CRUD controller for API/Web applications - following Nano framework conventions. Use when the user asks to add a new entity, resource, or CRUD endpoint to a Nano-based application (a project with an AGENTS.md describing Nano, or that references Nano.Library/NanoCore NuGet packages).
---

# Nano entity scaffold

Generates the files Nano needs for a new entity: data model and EF Core mapping always; query
criteria and a CRUD controller too, unless the target is a Console application (Console apps
have no HTTP surface, so there's nothing for either of those to serve — see step 3 below). Read
`AGENTS.md` in the target repo root first if present — it documents the exact base classes and
gotchas for that specific solution; this skill assumes the general Nano.Library conventions and
defers to a project's own AGENTS.md on any conflict.

## Before generating anything, determine

1. **Is a Data provider already registered?** Check `Program.cs` for `.AddNanoData<TProvider,
   TContext>()` and confirm a `DbContext` exists in the project (e.g. `Data/<X>DbContext.cs`).
   An entity's mapping only takes effect once Nano's `MapEntities<TIdentity>` discovery attaches
   it to a registered context — with no Data provider, the generated files would be dead code
   with nothing to persist them. If none is registered, stop and tell the user a Data provider
   needs to be added first; don't generate the entity anyway "for later."
2. **Entity name and properties.** Ask the user if not already given in the request — need
   at minimum the entity name (singular, PascalCase, e.g. `Product`) and its scalar
   properties (name + type). Don't invent business fields that weren't asked for.
3. **Application type.** Check `Program.cs` for `NanoApiApplication`, `NanoWebApplication`, or
   `NanoConsoleApplication`.
   - **API or Web**: generate all four files below.
   - **Console**: generate only File 1 (data model) and File 2 (mapping) — skip Files 3 and 4
     entirely (query criteria and controllers are API-request concepts; a Console app has
     nothing to route them to). Confirm this with the user only if they explicitly asked for a
     controller or query criteria on a Console app — otherwise just skip silently; a Console
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
   non-`Guid` identity unless the project already uses one consistently — it's a
   cross-cutting decision (affects the entity, mapping, controller, repository calls,
   and API client), not something to add on a whim for one entity.
6. **Existing conventions.** Skim one existing entity/mapping (and controller, if
   applicable) triplet in the project (if any exist) for property style, nullable-reference
   usage, and namespace layout, and match it.
7. **Every entity gets a generic controller — full stop, independent of whatever else exists for
   it.** This is not conditional on a query criteria class already existing, and **not conditional
   on whether an Api Client happens to reference the entity** (see step 8 — that's a separate,
   later check, not the thing that decides whether a controller gets made at all). When retrofitting
   controllers onto entities that already exist: for each
   entity with no controller yet, generate the query criteria class first if one doesn't already
   exist (File 3), then the controller against it (File 4) — every entity, not just the ones an
   Api Client happens to call out. **If a controller already exists for an entity, don't recreate
   it** — move on to the next entity.
8. **Only after every entity has its generic controller (step 7): does an Api Client already
   define custom methods/requests targeting one of them?** Check `{TargetApp}.Models/Api/{ClientName}.cs`
   and its `Api/Requests/` folder (per `nano-define-api-client`) for requests whose
   `this.Controller` (explicit or inferred) points at an entity's controller. This check only adds
   *extra* custom action stubs on top of the generic controller that step 7 already guarantees
   exists — it never substitutes for it, and an entity with no matching Api Client requests still
   gets its plain generic controller from step 7, nothing less. For each matching request found,
   File 4 below scaffolds a matching controller action **stub** — signature, route, and
   attributes, body `throw new NotImplementedException();` — not the real implementation.
   Scaffolding the contract is this skill's job; implementing the actual business logic is not.

## File 1 — Data model

Location: `Data/<Entity>.cs` in the split layout's `.Models` project, or `Data/<Entity>.cs`
in the single-project layout.

```csharp
public class <Entity> : BaseEntity
{
    public string Name { get; set; } = null!;
    // ...other scalar properties as requested
}
```

- Derive from `BaseEntity` (Guid identity) or `BaseEntity<TIdentity>` — match the project's
  existing convention (see step 5 above).
- For restricted CRUD (e.g. read-only, or no delete), derive instead from
  `BaseEntityReadOnly`, `BaseEntityCreatable`, `BaseEntityUpdatable`,
  `BaseEntityCreatableAndUpdatable`, or `BaseEntityDeletable` — ask the user if the request
  implies one of these rather than full CRUD.
- **`[Subscribe]` entities are restricted CRUD by convention, not a case to ask about.** An entity
  marked `[Subscribe]` (AGENTS.md's `### Entity Events`) is a local replica kept in sync by the
  built-in `EntityEventingHandler` whenever the publishing app's source entity changes —
  update/delete normally happen through that Subscribe mechanism, not through this app's own HTTP
  surface. Use `BaseEntityCreatable` for the entity and `BaseEntityCreatableController` for its
  controller (File 4) regardless of what tier was otherwise requested — Edit/Delete shouldn't be
  exposed to callers on a subscribed entity.
- Use `required`/`= null!` per the project's existing nullable-reference style, not your own
  default.

## File 2 — Data mapping

Location: `Data/Mappings/<Entity>Mapping.cs` in the main app project (always here, even in
the split layout — mappings are EF Core-only, never part of the shared API client models).

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

- **Always call `base.Configure(builder)`** before your own configuration — omitting it
  silently breaks inherited Nano behavior (soft delete, audit, etc.). This is the single
  most common mistake when writing a mapping by hand.
- **Configure every one of the entity's own properties explicitly — including navigations and
  collections — never rely on EF's conventions to fill in what isn't written down.** An implicit,
  convention-inferred relationship is exactly the kind of mistake that's invisible until it's a
  production bug (e.g. EF silently creating a shadow FK column for a stray navigation property
  with no real relationship behind it). Being explicit is what makes a mistake visible on read,
  not what EF happens to guess correctly most of the time.
- **List properties in the same order they're declared on the entity** — one `.Property(...)`/
  `.HasOne(...)`/`.HasMany(...)` block per property, top to bottom, matching the class. This
  makes the mapping file scannable against the entity file side by side: a missing or
  out-of-place property is immediately visible, not something that only surfaces when something
  breaks at runtime.
  - **Scalar property**: `.Property(x => x.Y)`, with `.IsRequired()` / `.HasMaxLength(n)` matching
    the property's own nullability/attributes (`[MaxLength]` if present, or the property's
    non-nullable reference-type status) — not just whatever EF would infer unprompted.
  - **FK scalar + its reference navigation** (usually adjacent in the entity): one combined
    `.HasOne(x => x.Nav).WithMany(...)/.WithOne(...).HasForeignKey(x => x.FkId)` block, positioned
    where the pair sits in the property order.
  - **Inverse collection/reference navigation with no FK of its own** (the principal side of a
    relationship whose FK is declared in the *dependent* entity's own mapping): configure it
    explicitly here too, not just with a comment — `.HasMany(x => x.Children).WithOne(x => x.Parent)`
    (or `.HasOne(...).WithOne(...)` for a 1:1 principal side), with `.IsRequired()` added whenever
    the dependent's own FK property is non-nullable (matching what the dependent side's own
    `.HasForeignKey(...)` chain already declares) — **without** repeating `.HasForeignKey(...)`
    itself, that's declared once, on the dependent side. EF Core matches the two configurations by
    navigation pairing and merges them as the same relationship, so writing it from both ends is
    safe as long as they agree; it's what makes the relationship visible when scanning *this*
    entity's mapping file alone, not just the other one. **No comment needed** — the
    `.HasMany(...).WithOne(...)` call already says everything a reader needs; a comment repeating
    "FK owned/declared in the other file" for every single one of these is noise, not
    information.
  - **A navigation with no corresponding FK/relationship anywhere** (the model doesn't actually
    support what the property implies): don't let it fall through to an accidental EF-invented
    shadow relationship. Flag it to the user and ask what it should be — don't guess a
    relationship that isn't in the model. If the user says to leave the property in place without
    resolving it yet, mark it `.Ignore(x => x.Y)` explicitly (with a comment saying why) rather
    than leaving it for EF's convention to silently invent something.
- No registration step needed — Nano auto-discovers mappings via
  `ModelBuilderExtensions.MapEntities<TIdentity>` at startup.
- Add a new EF Core migration after this file exists: `dotnet ef migrations add <Name>`
  from the app project directory (only do this if the user asked you to, or if the project's
  existing workflow clearly expects a migration per entity — check `Migrations/` for
  precedent first).

## File 3 — Query criteria (API/Web only — skip for Console)

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

- Only add filter properties for fields that make sense to search/filter by — don't
  mechanically add one filter per scalar property on the entity.
- Every filter property must be `virtual` and nullable.
- Use the `CriteriaExpression` builder methods appropriate to each property's type
  (`StartsWith`/`Contains` for strings, `EqualTo`/`GreaterThan`/etc. for numerics and dates)
  — check the project's other query criteria classes for the operations actually available,
  don't guess.

## File 4 — Controller (API/Web only — skip for Console)

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
  `Country` → `CountrysController` — note this is naive `+s` pluralization, not proper
  English plural rules; Nano derives the route segment from the class name). Do not
  "correct" irregular plurals.
- Check whether the project actually registers an eventing provider (look for
  `AddNanoEventing<...>()` in `Program.cs`). If it doesn't, drop the `IEventing? eventing`
  parameter and the corresponding base-constructor argument — an unused optional eventing
  dependency is harmless, but match what sibling controllers in the same project actually do.
- If the identity type isn't `Guid` (per step 5), the controller generic list needs the
  identity type too: `BaseEntityController<<Entity>, <TIdentity>, <Entity>QueryCriteria>`.
- **`BaseEntityController<<Entity>, <Entity>QueryCriteria>` (full CRUD) is the default for every
  entity, with no exceptions other than `[Subscribe]`.** Having custom actions on the same
  controller — even ones that overlap in intent with a generic CRUD action — is not on its own a
  reason to narrow the tier. A colliding route is a defect to flag (see below), not a signal to
  remove generic capability the entity is otherwise entitled to.
- **`[Subscribe]` entities use `BaseEntityCreatableController<<Entity>, <Entity>QueryCriteria>`**,
  not `BaseEntityController` — per File 1's note, update/delete happen through the Subscribe
  mechanism, not this app's HTTP surface, so only Get/Query/Create are exposed here. This is the
  *only* case that changes the default tier.
- No manual registration needed — Nano's MVC discovery picks up the controller
  automatically from the assembly.

### Custom action stubs (per step 8 above)

For each matching Api Client request found: add one action to the controller, using the
request's own route constant (most real codebases define `public const string Route = "..."`
locally on the request class itself — reference it as `[Route(MyRequest.Route)]` rather than
retyping the literal, so the two sides can't drift) and the matching `[Http*]` verb attribute.
Match the doc-comment/`[ProducesResponseType]` conventions of whatever controllers already exist
in the project. The body is always a stub:

```csharp
public virtual Task<IActionResult> DoTheThingAsync(/* params matching the request */, CancellationToken cancellationToken = default)
    // Implement. See <ClientName>Api.DoTheThingAsync's doc comment for the full contract.
    => throw new NotImplementedException();
```

- **Don't narrow the tier to dodge a collision — flag it instead.** The default tier (above) is
  full CRUD regardless of what custom actions exist; if a custom request's route+verb is
  literally identical to a route the generic tier also exposes (e.g. a custom
  `[PostAction("create")]` alongside generic `POST .../create`), that's a genuine defect in the
  Request-side contract (one of the two routes needs to change), not a reason to remove the
  entity's generic capability. Add the custom action anyway, with a prominent comment naming
  exactly which generic route it collides with (verb + path + which AGENTS.md table row), and
  leave both in place for the user to resolve.
- **Check built-in routes too, not just the generic CRUD table** — a narrower base class can have
  its own built-in action set with its own routes (e.g. `BaseEntityUserController`'s identity
  actions, AGENTS.md's Identity user controller table: `{id}/activate`, `{id}/deactivate`, etc.).
  A custom request whose route matches one of those collides exactly the same way a generic CRUD
  route would — flag it the same way.
- **If two *custom* requests collide with each other** (same controller, same route+verb): this is
  a genuine defect in the Request-side contract, not something to silently rename or merge.
  Scaffold both stubs anyway, with a prominent comment on each naming the other action it collides
  with — flag it for the user to resolve (one of the two routes needs to change, or the two actions
  need merging into one) rather than guessing.

## After generating

- Show the user the files generated and where they were placed (two for Console, four for
  API/Web); don't silently also modify `Program.cs`, add NuGet packages, or run
  `dotnet ef migrations add` unless they ask — scaffolding the entity is the task, not deciding
  the rest of the rollout for them.
- If the project has an existing entity with the same shape you can point to as a working
  reference, mention it — it's the fastest way for the user to sanity-check the result.
