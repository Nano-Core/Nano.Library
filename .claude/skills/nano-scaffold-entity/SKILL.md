---
name: nano-scaffold-entity
description: Scaffold a new Nano.Library entity end-to-end - data model, EF Core mapping, query criteria, and CRUD controller - following Nano framework conventions. Use when the user asks to add a new entity, resource, or CRUD endpoint to a Nano-based application (a project with an AGENTS.md describing Nano, or that references Nano.Library/NanoCore NuGet packages).
---

# Nano entity scaffold

Generates the four files Nano needs for a new CRUD-capable entity: data model, EF Core
mapping, query criteria, and controller. Read `AGENTS.md` in the target repo root first if
present — it documents the exact base classes and gotchas for that specific solution; this
skill assumes the general Nano.Library conventions and defers to a project's own AGENTS.md
on any conflict.

## Before generating anything, determine

1. **Entity name and properties.** Ask the user if not already given in the request — need
   at minimum the entity name (singular, PascalCase, e.g. `Product`) and its scalar
   properties (name + type). Don't invent business fields that weren't asked for.
2. **Project layout.** Look for a `<Something>.Models` project alongside the main app
   project (check the `.sln` or list sibling folders).
   - **Split layout** (a `.Models` project exists, e.g. `Svc.Accounts.Models`): entity
     model and query criteria go in the `.Models` project (they're part of the API client
     contract other services consume); the mapping and controller go in the main app
     project.
   - **Single-project layout** (no `.Models` project, e.g. most Nano.Lessons): all four
     files go in the one app project.
3. **Identity type.** Check the `DbContext`/`DbContextFactory` and other entities in the
   project for a `TIdentity` type parameter (e.g. `BaseEntity<int>`). If every existing
   entity just uses plain `BaseEntity` (implicit `Guid`), match that. Never introduce a
   non-`Guid` identity unless the project already uses one consistently — it's a
   cross-cutting decision (affects the entity, mapping, controller, repository calls,
   and API client), not something to add on a whim for one entity.
4. **Existing conventions.** Skim one existing entity/mapping/controller triplet in the
   project (if any exist) for property style, nullable-reference usage, and namespace
   layout, and match it.

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
  existing convention (see step 3 above).
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

## File 3 — Query criteria

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

## File 4 — Controller

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
- If the identity type isn't `Guid` (per step 3), the controller generic list needs the
  identity type too: `BaseEntityController<<Entity>, <TIdentity>, <Entity>QueryCriteria>`.
- No manual registration needed — Nano's MVC discovery picks up the controller
  automatically from the assembly.

## After generating

- Show the user the four files and where they were placed; don't silently also modify
  `Program.cs`, add NuGet packages, or run `dotnet ef migrations add` unless they ask —
  scaffolding the entity is the task, not deciding the rest of the rollout for them.
- If the project has an existing entity with the same shape you can point to as a working
  reference, mention it — it's the fastest way for the user to sanity-check the result.
