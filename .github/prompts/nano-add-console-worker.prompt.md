---
mode: agent
description: Add a Console Worker to a Nano Console application - a class deriving BaseWorker that runs a Console app's actual run-to-completion job. Use when the user asks to add a worker, background job, or the main task to a Nano Console application.
---

# Nano add console worker

Adds a Console Worker to an existing Nano Console application. Read AGENTS.md's
`### Console Workers` section first - it documents the lifecycle and error-handling semantics in
full; this skill is just the file shape.

## Before making any change, determine

1. **Application type.** Console Workers are a `NanoConsoleApplication`-specific concept - check
   `Program.cs`. If the app is API/Web, this isn't the right skill (background work there is a
   [Startup Task](nano-add-startup-task) instead, which has different lifecycle semantics -
   confirm which the user actually wants).
2. **Name and job.** Ask if not already given - what the worker actually does.
3. **Does it need to signal failure?** Per AGENTS.md's ⚠: unlike a startup task, a worker that
   throws does **not** abort anything - the exception is caught and logged, the worker is treated
   as complete, and every sibling worker still runs. If this worker's failure should actually
   surface (e.g. a non-zero exit code for a CronJob to alert on), that has to be handled inside
   `OnStartAsync` itself - ask whether that matters for this job before treating a plain override
   as sufficient.

## Worker class

`Workers/{Name}Worker.cs` in the application project (conventional location, not enforced -
discovered by type):

```csharp
public class MyWorker(ILogger<MyWorker> logger) : BaseWorker(logger)
{
    public override async Task OnStartAsync(CancellationToken cancellationToken = default)
    {
        // the actual work
    }

    // optional - only override if cleanup is needed; runs after every worker's OnStartAsync
    // finishes (concurrently with sibling workers' OnStopAsync), right before the app exits
    public override Task OnStopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
```

No registration needed - every non-abstract `IWorker` in the entry assembly is discovered and
registered `Scoped` automatically, same mechanism as Startup Tasks. Any other registered service
can be injected into the constructor alongside `logger`.

Remember the lifecycle this fits into: all Startup Tasks finish first, then every worker's
`OnStartAsync` runs concurrently with its siblings, then every `OnStopAsync` runs concurrently,
then the process exits on its own (`IHostApplicationLifetime.StopApplication()`) - this is what
makes a Console app a run-to-completion job rather than a long-running daemon.

## After making the change

- Show the user the file added.
- If step 3 identified a real failure-signaling need, make sure `OnStartAsync` actually handles
  it (e.g. `Environment.ExitCode = 1` before returning, or rethrowing after logging if that's the
  intended signal) - don't leave a silently-swallowed failure in a job the user said needs to
  alert on error.
