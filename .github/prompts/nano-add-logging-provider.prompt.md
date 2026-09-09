---
mode: agent
description: Add a Nano logging provider (Log4Net, Microsoft, NLog, or Serilog) to a Nano.Library-based application - registers the provider in Program.cs and adds the Logging configuration section to appsettings.json. Use when the user asks to add logging, set up a specific logging provider, or switch the logging provider in a Nano API, Web, or Console application.
---

# Nano add logging provider

Wires a Nano logging provider into an existing Nano API, Web, or Console application. Read
`AGENTS.md` in the target repo root first - it documents `Nano.Logging`'s registration
one-liner, the four providers' package names, and the exact `Logging` config shape/defaults
under its `## Nano.Logging` section; this skill does not repeat any of that, only how to apply
it correctly to an existing project without breaking what's already there.

## Before making any change, determine

1. **Which provider.** One of `Log4Net`, `Microsoft`, `NLog`, `Serilog` (see AGENTS.md's
   provider table for the package/type names). Ask the user if not already given.
2. **Is a provider already registered?** Nano supports exactly one logging provider at a
   time - check `Program.cs` for an existing `.AddNanoLogging<...>()` call. If one exists for
   a *different* provider, tell the user this will replace it (remove the old `using`,
   provider call, and reference) rather than silently adding a second one. If it's already the
   *same* provider, say so and stop - nothing to do.
3. **Is a package reference even needed?** Check whether the provider's type already resolves
   without adding anything: look for a `PackageReference` to `NanoCore` or `Nano.All` (they're
   identical, see AGENTS.md) on the application project itself, or on a `.Models` project it
   reaches via `ProjectReference` (AGENTS.md's "quick start" convention - either package pulls
   in every Nano package, including every logging provider, transitively). If found, **no
   package change is needed at all** - skip straight to Program.cs.
   - Otherwise, the project uses the explicit/granular convention: add
     `<PackageReference Include="Nano.Logging.<Provider>" Version="X.Y.Z" />` to the
     **application project's** `.csproj` (never a `.Models` project - per AGENTS.md, providers
     belong on the app project, `Nano.App` is the only Nano package `.Models` needs), using the
     **exact same version** as the project's existing `Nano.App.Api`/`Nano.App.Web`/
     `Nano.App.Console` reference. Don't invent or guess a version.
   - Never add a `ProjectReference` to Nano.Library source - always a NuGet `PackageReference`,
     even if the rest of the project currently references Nano.Library from source. Some
     internal Nano.Library development repos do that for their own convenience but explicitly
     document it as something to replace with NuGet packages before deployment - it's not the
     convention to extend into a new reference.

## Program.cs

Add the `using`s and registration call AGENTS.md's `### Registration` section shows, inside the
**existing** `.ConfigureServices(...)` lambda - don't create a second `.ConfigureServices` call
if one already exists.

- If the existing lambda parameter is the discard placeholder `_` (e.g. `.ConfigureServices(_
  => { // Add your services here. })`, the standard blank-app boilerplate), rename it to `x` and
  remove the placeholder comment - `x` is the Nano convention once the lambda holds a real
  registration.
- If other real service registrations already exist in the lambda, just add the
  `AddNanoLogging<...>()` call alongside them; don't touch unrelated lines.
- Works identically for `NanoApiApplication`, `NanoWebApplication`, and `NanoConsoleApplication`
  - the `.ConfigureServices(...)` call and `AddNanoLogging<...>()` registration are the same
  across all three app types (`NanoWebApplication` extends `NanoApiApplication`).

## appsettings.json

Add the `Logging` section from AGENTS.md's `### Configuration` example to the base
`appsettings.json` only (sibling of `App`, not nested inside it) - no environment-overlay file
needs it.

- If a `Logging` section already exists (e.g. from a previously-registered different provider),
  leave its `LogLevel`/`LogLevelOverrides` values as-is - they're provider-agnostic - and only
  touch `Program.cs` and the package/project reference.

## After making the change

- Show the user the modified `Program.cs` lines, the `appsettings.json` addition, and - if one
  was needed - the `PackageReference` added to the `.csproj`. If none was needed (NanoCore/
  Nano.All already covers it), say so explicitly rather than leaving it unmentioned.
- If this replaced a different provider, explicitly list what was removed (old `using`,
  provider call, and reference if one was added for it) alongside what was added, so the user
  can sanity-check the swap.
- Don't add any package beyond the logging provider itself, and don't touch Docker/Kubernetes/CI
  files - logging provider selection has no effect on any of those.
