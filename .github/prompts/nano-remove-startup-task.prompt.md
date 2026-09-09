---
mode: agent
description: Remove a Startup Task from a Nano application - deletes the BaseStartupTask-derived class. Use when the user asks to remove a startup task, cache warm-up, or one-time initialization from a Nano API, Web, or Console application.
---

# Nano remove startup task

Removes a Startup Task from an existing Nano API, Web, or Console application - the counterpart
to `nano-add-startup-task`.

## Before making any change, determine

1. **Which task?** Confirm the class name/file if the project has more than one - check
   `Startup/` (or search for `BaseStartupTask`/`IStartupTask` if not in the conventional
   location).
2. **Behavior change to flag, not a crash risk.** Nothing else in the app takes a required
   dependency on a startup task's existence, so removal never breaks compilation or DI. The one
   real effect: if [Health Checks](nano-add-health-checks) are enabled, the app's readiness gate
   no longer waits on whatever this task was checking/warming - readiness becomes available
   sooner, and whatever the task guaranteed (a warm cache, a verified dependency) is no longer
   guaranteed before traffic is accepted. Tell the user this plainly if it seems load-bearing.

## Startup task class

Delete the file. No config, no registration, no other references to clean up - discovery is by
type, so removing the class is the entire change.

## After making the change

- Show the user the file removed.
- Restate step 2 if the removed task looked like it was guarding something meaningful (an
  external dependency check, a required warm-up) rather than being purely cosmetic.
