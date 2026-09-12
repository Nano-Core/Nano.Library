---
mode: agent
description: Delete an Api Client surface (or a custom method on it) that a Nano application exposes to other applications - the BaseApiClient subclass and its custom request/response types, from the owning service's {Name}.Models project. Use when the user asks to stop exposing an endpoint/client to other services, remove a custom method from an Api Client, or delete a client's definition entirely from a Nano API, Web, or Console application.
---

# Nano undefine API client

Deletes an Api Client's definition - the `BaseApiClient` subclass and/or its custom request
types - from the *owning* service's `{Name}.Models` project. The counterpart to
`nano-define-api-client`. This is a different, more consequential operation than a single
consumer dropping the client: every application currently consuming this class loses it.

If the actual goal is just "this app should stop calling that service," not "delete the client
definition entirely," that's `nano-remove-api-client`'s job instead (the *consumer* side) - point
the user there; don't delete a shared definition to satisfy one consumer's request.

## Before making any change, determine

1. **Is this a full class removal, or just one custom method?** "Remove the `GetByEmailAsync`
   method from `MyApi`" is much narrower than "delete `MyApi` entirely" - confirm scope before
   touching anything.
2. **Who else consumes this class?** Search every application that references this
   `{Name}.Models` project (`ProjectReference` in a monorepo, or every consumer of the published
   NuGet if it's cross-repo) for an `App:Apis` entry matching this class name, or the class
   injected into a controller/worker. **Every one of those breaks** - either a compile error (if
   the class itself is deleted) or a dead/misconfigured `App:Apis` entry (if just a method
   consumers called is removed). List every affected consumer and confirm with the user before
   proceeding - this is not a decision to make unilaterally on the owning service's behalf.
3. **Custom request/response types** - if a custom method is being removed and its request/
   response types (`{Name}Request.cs` under `Api/Requests/`, any dedicated response POCO) exist
   solely to support it, they come out too. Check nothing else references them first.

## Client class and custom methods

If removing the whole class: delete `{ThisApp}.Models/Api/{ClientName}.cs` and every
request/response type that existed solely to support it.

If removing one custom method: delete just that method from the class, plus its dedicated
request/response types (per step 3) - leave the rest of the class, and any generic
`.Entity`/`.Auth`/`.Audit`/`.Identity` usage, untouched.

## Route constants

If a `Consts` class constant (per `nano-define-api-client`'s "define the route segment as a
constant" step) existed solely for the removed request's route, remove it too - otherwise it's
a dangling reference to a route nothing serves anymore.

## After making the change

- Show the user every file touched/deleted in this app's `.Models` project.
- Restate every consuming application identified in step 2 as still needing its own cleanup -
  this skill only removes the definition; each consumer's own `App:Apis` entry and injection site
  is `nano-remove-api-client`'s job, on that consumer's side, once they've been told.
- If step 2 surfaced consumers the user hadn't accounted for, that may be reason enough to stop
  here and let them decide how to proceed with each one, rather than deleting out from under
  them.
