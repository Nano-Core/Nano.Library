---
mode: agent
description: Delete an Api Client's definition entirely - the BaseApiClient/BaseIdentityApiClient subclass - from the owning service's {Name}.Models project. Use when the user asks to stop exposing a client to other services entirely, or delete a client's definition from a Nano API, Web, or Console application. For removing one custom method (and its paired controller action), see nano-remove-custom-endpoint's internal-service path instead.
---

# Nano remove API client

Deletes an Api Client's definition - the `BaseApiClient`/`BaseIdentityApiClient` subclass - from
the *owning* service's `{Name}.Models` project, entirely. The counterpart to
`nano-add-api-client`. This is a different, more consequential operation than a single
consumer dropping the client: every application currently consuming this class loses it.

**This skill never touches the controller actions those custom methods called.** Deleting the
client-side definition doesn't imply deleting the server-side logic behind it - those actions
keep working, just unreachable through this particular typed client. If the user also wants a
given action removed, that's a separate, explicit decision - point them at
`nano-remove-custom-endpoint` for each one, on the owning service's app project.

If the actual goal is just "this app should stop calling that service," not "delete the client
definition entirely," that's `nano-remove-api-client-configuration`'s job instead (the *consumer*
side) - point the user there; don't delete a shared definition to satisfy one consumer's request.

If the actual goal is "remove this one custom method," not the whole class, that's
`nano-remove-custom-endpoint`'s internal-service path instead - a custom method and the
controller action it calls are one paired contract, removed together; this skill only handles
deleting the class itself.

## Before making any change, determine

1. **Is this really a full class removal?** If the request is actually about one custom method,
   redirect to `nano-remove-custom-endpoint`'s internal-service path rather than doing partial
   work here.
2. **Who else consumes this class - and say plainly that this check is necessarily incomplete.**
   Search every *locally visible* application (this repo, or other repos actually checked out and
   reachable) for an `App:Apis` entry matching this class name, or the class injected into a
   controller/worker. But if `{Name}.Models` is published (NuGet or private feed), consumers can
   exist in repos this session has no access to at all - finding zero locally is not the same as
   confirming zero exist. Present it that way to the user: "no *locally visible* consumers found,"
   not "nobody else uses this." List whatever was found, name the blind spot explicitly, and
   confirm with the user before proceeding - this is not a decision to make unilaterally, and it
   can't be made with full information either.
3. **What is actually being lost - list every custom method by name, not just "the class."** A
   bare pass-through client with nothing but `.Entity`/`.Auth`/`.Audit` usage is a low-stakes
   delete; a client with several built-out custom methods represents real, deliberate contract
   work. Enumerate them (name, and what each does per its doc comment) as part of what the user is
   confirming in step 2 - don't let a multi-method client get deleted on the same casual footing
   as an empty stub just because both are technically "one class."
4. **Route constants are conditional on whether the controller action is also going - never
   remove one on its own.** A route constant in `{Name}.Models/Consts/` is normally referenced
   from *both* this client's request and the controller action it calls (per
   `nano-add-custom-endpoint`'s route-constant-sharing convention). Since this skill leaves
   controller actions untouched by default, deleting the constant while the action still
   references it is a **guaranteed compile break in the owning service's own app project** -
   not a cross-repo risk, a self-inflicted one. Only remove a route constant here if the user has
   also confirmed the corresponding controller action is being removed in the same change (via
   `nano-remove-custom-endpoint`); otherwise leave it in place even though it looks unused
   from the client's side.

## Client class

Delete `{ThisApp}.Models/Api/{ClientName}.cs` in full, along with every request/response type
identified in step 3 that existed solely to support its custom methods (check nothing else
references them first - a shared DTO used elsewhere should stay).

Leave every route constant in place unless step 4's condition was actually met for it. Leave
every controller action untouched, always - see the note above.

## After making the change

- Show the user every file touched/deleted in this app's `.Models` project, and restate plainly
  that the controller actions those custom methods called were left untouched.
- Restate step 2's blind spot one more time - this only confirms no locally visible consumer
  remains, not that none exist anywhere.
- Restate every consuming application identified in step 2 as still needing its own cleanup -
  this skill only removes the definition; each consumer's own `App:Apis` entry and injection site
  is `nano-remove-api-client-configuration`'s job, on that consumer's side, once they've been
  told.
- If step 2 surfaced consumers the user hadn't accounted for, that may be reason enough to stop
  here and let them decide how to proceed with each one, rather than deleting out from under
  them.
