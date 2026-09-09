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
- No manual registration needed — Nano's MVC discovery picks up the controller
  automatically from the assembly.

## After generating

- Show the user the files generated and where they were placed (two for Console, four for
  API/Web); don't silently also modify `Program.cs`, add NuGet packages, or run
  `dotnet ef migrations add` unless they ask — scaffolding the entity is the task, not deciding
  the rest of the rollout for them.
- If the project has an existing entity with the same shape you can point to as a working
  reference, mention it — it's the fastest way for the user to sanity-check the result.
