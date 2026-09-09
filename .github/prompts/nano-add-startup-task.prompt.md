---
mode: agent
description: Add a Startup Task to a Nano application - a class deriving BaseStartupTask that runs one-time initialization before the app accepts traffic (API/Web) or workers start (Console). Use when the user asks to add cache warm-up, a startup check, or one-time initialization to a Nano API, Web, or Console application.
---

# Nano add startup task

Adds a Startup Task to an existing Nano API, Web, or Console application. Read AGENTS.md's
`### Start-Up Tasks` section first - it documents execution/readiness semantics in full; this
skill is just the file shape. Not the same mechanism as Nano's built-in data-provider migration
task - this is for your own one-time initialization work.

## Before making any change, determine

1. **Name and job.** Ask if not already given - what needs to happen once before the app is
   considered ready (cache warm-up, an external dependency check, etc.).
2. **Must it be allowed to fail the whole app?** Per AGENTS.md: if `OnStartAsync` throws, **the
   exception propagates and the application fails to start** - a startup task cannot fail
   silently, unlike a Console Worker. If the user actually wants best-effort/non-fatal behavior
   instead, a [Console Worker](nano-add-console-worker) (Console apps only) or a plain
   `IHostedService` might be the better fit - confirm before assuming a hard failure is wanted.
3. **Does `OnStopAsync` need to do real cleanup?** Read the timing note below before relying on
   it for anything tied to actual application shutdown.

## Startup task class

`Startup/{Name}StartupTask.cs` in the application project (conventional location, not enforced -
discovered by type):

```csharp
public class MyStartupTask(ILogger<MyStartupTask> logger) : BaseStartupTask(logger)
{
    public override async Task OnStartAsync(CancellationToken cancellationToken = default)
    {
        // one-time init - cache warm-up, external dependency check, etc.
    }

    // optional - only override if needed; see the timing note below
    public override async Task OnStopAsync(CancellationToken cancellationToken = default)
    {
        // cleanup for what OnStartAsync acquired - runs right after OnStartAsync completes,
        // NOT at real application shutdown
    }
}
```

No registration needed - every non-abstract `IStartupTask` in the entry assembly is discovered
and registered `Scoped` automatically. Any other registered service, including scoped ones, can
be injected into the constructor.

⚠ **`OnStopAsync` is not "runs at application shutdown."** It fires immediately after every
task's `OnStartAsync` completes, as a completion/cleanup hook - not tied to real shutdown timing
(the host's real shutdown sequence may invoke it again, but that's incidental, not its purpose).
Only override it for cleanup that belongs right after this task's own startup work.

**Execution**: all registered tasks' `OnStartAsync` run **concurrently** (`Task.WhenAll`), in one
shared scope, before the app accepts requests (API/Web) or any Console Worker starts. If [Health
Checks](nano-add-health-checks) are enabled, the app isn't reported ready until every task's
`OnStartAsync` **and** `OnStopAsync` have completed - this readiness gate applies automatically,
nothing further to wire for it.

## After making the change

- Show the user the file added.
- Restate step 2's consequence plainly: an unhandled exception here takes the whole app down at
  startup - make sure that's the behavior actually wanted for this specific task before finishing.
