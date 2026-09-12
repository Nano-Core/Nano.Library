---
name: nano-remove-logging-provider
description: Remove a Nano logging provider (Log4Net, Microsoft, NLog, or Serilog) from a Nano.Library-based application - unregisters it in Program.cs and removes the Logging configuration section from appsettings.json. Use when the user asks to remove logging, unregister the logging provider, or strip logging out of a Nano API, Web, or Console application.
---

# Nano remove logging provider

Fully removes Nano logging from an existing Nano API, Web, or Console application — the
counterpart to `nano-add-logging-provider`. If the user actually wants to *switch* to a
different provider, that's the add skill's job (it already handles replacing an existing
provider); use this skill only when the end state should be no Nano logging provider
registered at all.

## Before making any change, determine

1. **Which provider is currently registered?** Check `Program.cs` for `.AddNanoLogging<...>()`
   — the type argument tells you which provider. If none is registered, say so and stop; there's
   nothing to remove.
2. **Is the package reference this skill's to remove?** Look for a `PackageReference` to
   `Nano.Logging.<Provider>` on the application project. If found (the project uses the
   explicit/granular convention), remove it.
   - If instead the project references `NanoCore` or `Nano.All` (the "quick start" convention —
     see AGENTS.md), **leave it alone** — that package covers every Nano feature the project
     uses, not just logging, so removing it would break unrelated functionality. There's simply
     no package-level change to make in that case.

## Program.cs

Remove the `using Nano.Logging.Extensions;` and `using Nano.Logging.<Provider>;` lines, and the
`.AddNanoLogging<...>()` call inside `.ConfigureServices(...)`.

- If the lambda has no other statements left after removing the call, restore it to the
  standard blank-app placeholder shape and rename the parameter back to the discard `_`:
  `.ConfigureServices(_ => { // Add your services here. })`. Leaving an empty non-discard
  parameter or a bare empty block behind looks like an unfinished edit.
- If other real service registrations remain in the lambda, just remove the one line — don't
  touch the rest, and keep the parameter as `x`.

## appsettings.json

Remove the `Logging` section from the base `appsettings.json` entirely — it's a sibling of
`App`, added by the add-skill's `AGENTS.md`-documented shape. With no provider registered,
`LogLevel`/`LogLevelOverrides` are dead configuration nothing reads (the same class of bug as
leaving a Kubernetes probe pointed at a `HealthCheck` that was never enabled — don't leave it
behind).

## After making the change

- Show the user the modified `Program.cs` lines, the removed `appsettings.json` section, and —
  if one was removed — the `PackageReference` taken out of the `.csproj`.
- If nothing needed to change at the package level because the project uses `NanoCore`/
  `Nano.All`, say so explicitly rather than leaving it unmentioned.
- Don't touch Docker/Kubernetes/CI files — logging provider selection has no effect on any of
  those.
