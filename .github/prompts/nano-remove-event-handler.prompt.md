---
mode: agent
description: Remove an event handler from a Nano application - deletes the BaseEventHandler<TEvent>-derived class. Use when the user asks to remove an event handler, subscriber, or consumer for Nano's Publish/Subscribe eventing from a Nano API, Web, or Console application.
---

# Nano remove event handler

Removes an event handler from an existing Nano application - the counterpart to
`nano-add-event-handler`.

## Before making any change, determine

1. **Which handler?** Confirm the class name/file if the project has more than one - check
   `{App}/Eventing/` (or search for `BaseEventHandler<` if not in the conventional location).
2. **Is the event's contract class local or shared?** Local (defined directly in this app) or shared (in
   a `{App}.Events` sibling project - see `nano-add-event-handler`). This decides what else, if anything,
   needs cleaning up beyond the handler itself.
3. **If shared: does this app still publish that event anywhere?** Removing the handler doesn't remove
   the event - this app (or the handler's removal notwithstanding) might still call `PublishAsync` for it
   elsewhere. Check before touching the contract class or its project.
4. **If shared and nothing in this app still publishes or subscribes to it: is it the only event type left
   in `{App}.Events`?** If so, the whole project is now dead weight in this solution.

## Removing the handler

Delete the handler class. No config, no registration, no other references to clean up - discovery is by
type, so removing the class is the entire change for the handler itself.

## Removing the contract (only if steps 3–4 say so)

- **Local event, no longer published:** delete the contract class alongside the handler.
- **Shared event, no longer published or subscribed to anywhere in this app, and it was the only event
  type in `{App}.Events`:** offer to remove the whole `{App}.Events` project - delete it, remove it from
  the `.sln`, remove the `ProjectReference` from `{App}`, and remove its "Publish NuGet" step from
  `.github/workflows/build-and-deploy.yml`. Confirm with the user first - this is a published package, and
  another solution may already depend on the last-published version even if nothing in *this* solution
  does anymore.

## After making the change

- Show the user the file(s) removed.
- If the contract or the `{App}.Events` project was removed too, say so explicitly and why.
