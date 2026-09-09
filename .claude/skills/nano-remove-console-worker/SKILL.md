---
name: nano-remove-console-worker
description: Remove a Console Worker from a Nano Console application - deletes the BaseWorker-derived class. Use when the user asks to remove a worker, background job, or a specific task from a Nano Console application.
---

# Nano remove console worker

Removes a Console Worker from an existing Nano Console application — the counterpart to
`nano-add-console-worker`.

## Before making any change, determine

1. **Which worker?** Confirm the class name/file if the project has more than one — check
   `Workers/` (or search for `BaseWorker`/`IWorker` if not in the conventional location).
2. **Is this the last worker in the app?** Not a blocker — a Console app with zero workers still
   runs (Startup Tasks, if any, still execute), it just does nothing beyond that. Worth
   mentioning if it leaves the app with no actual job.

## Worker class

Delete the file. No config, no registration, no other references to clean up — discovery is by
type, so removing the class is the entire change.

## After making the change

- Show the user the file removed.
- If step 2 applies (last worker removed), say so explicitly — the app will still start and run
  its Startup Tasks, but do nothing further.
