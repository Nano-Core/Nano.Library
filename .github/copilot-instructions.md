# Copilot instructions — Nano Framework

This repository (and any application built on the `NanoCore`/`Nano.*` NuGet packages) uses the
**Nano framework** for API/Web/Console applications. Before making changes, check the repo root
for an `AGENTS.md` — it is the authoritative implementation reference for Nano and documents the
exact base classes, configuration, and gotchas for that specific solution. These instructions are
a short always-on summary; `AGENTS.md` takes precedence on any conflict.

## Solution shape

A Nano app named `{name}` conventionally looks like:

- `{name}/` — the app project (`Program.cs`, `Controllers/`, `Data/` for `DbContext`+`Mappings/`,
  `appsettings*.json`).
- `{name}.Models/` — a **separate sibling project** (not nested) holding entity models,
  `Criterias/` (query criteria), and `Api/` (typed API client) — only present in a "split layout";
  many smaller apps use a single-project layout with everything in `{name}/` instead.
- `.tests/Tests.{name}/` — test project.
- `.docker/`, `.kubernetes/`, `.github/workflows/build-and-deploy.yml`, root `Dockerfile` — local
  orchestration, deployment, and CI/CD.

Folder names (`Controllers/`, `Data/`, `Criterias/`, `Api/`) are convention, not a framework
requirement — Nano discovers controllers, mappings, and data providers by type via reflection, not
by location.

## Conventions to follow

- **Identity type**: default to `Guid` (`BaseEntity` = `BaseEntity<Guid>`) unless the project
  already consistently uses another `TIdentity` everywhere (entities, mappings, repository,
  controllers, API client) — it is a cross-cutting choice, never a per-entity one.
- **Entity mappings** must call `base.Configure(builder)` before any custom configuration —
  omitting it silently breaks inherited behavior (soft delete, audit, etc.).
- **Controller naming is load-bearing**: `<Entity>` + literal `s` + `Controller` (naive
  pluralization, e.g. `Country` → `CountrysController`, not "correct" English plurals) — Nano
  derives the route from the class name.
- No manual registration is needed for mappings, controllers, startup tasks, or API clients —
  Nano discovers them by type/assembly scanning. Don't add DI registration calls for these unless
  a project's existing code clearly does otherwise.
- Match existing conventions in the project (nullable-reference style, split vs single-project
  layout, which base classes are used) rather than introducing a new style for one change.
- Don't silently expand scope: adding an entity means the model/mapping/criteria/controller for
  that entity, not also touching `Program.cs`, adding NuGet packages, or running EF migrations,
  unless asked.

## Scaffolding a new entity

To scaffold a new CRUD entity (data model + EF Core mapping + query criteria + controller), follow
`.github/prompts/nano-scaffold-entity.prompt.md` in this repo (also invokable directly as
`/nano-scaffold-entity` in Copilot Chat).
