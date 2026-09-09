---
mode: agent
description: Remove a Nano Api Client from an application - deletes the BaseApiClient subclass and its App:Apis configuration entry. Use when the user asks to remove a call to another Nano service/API, remove an API client, or stop consuming an internal service from a Nano API, Web, or Console application.
---

# Nano remove API client

Removes a Nano Api Client from an existing application - the counterpart to
`nano-add-api-client`. Read that skill first - this one undoes exactly what it adds.

## Before making any change, determine

1. **Which client, and where is it defined?** Confirm the class name and whether it lives in this
   app's own project or a referenced `{TargetName}.Models` project (owning-service convention -
   removing it from a shared `.Models` project affects every consumer of that project, not just
   this app; confirm that's actually intended before deleting a shared file).
2. **What depends on it?** Search for the client class used as a constructor parameter (controller
   or worker). Unlike most Nano dependencies, this isn't a startup-crash risk - the client itself
   has no required-service semantics beyond normal C# compilation - but removing the class while
   something still references it simply **won't compile**. Find every consumer first; either this
   skill also removes those usages (ask the user), or stop and let them decide what replaces the
   call.
3. **Is the `App:Apis` entry safe to remove alone?** If the class itself is defined in a shared
   `.Models` project used by other apps too, and only *this* app's config/injection should go
   away, don't delete the class - just remove this app's `App:Apis` entry and its injection site.

## appsettings.json

Remove the `App:Apis:{ClientClassName}` section from the base `appsettings.json`, and its
`LogInRoot`/other overrides from `appsettings.Development.json` and any Staging/Production
secret wiring, if present.

## Client class and custom requests

If nothing else consumes the class (confirmed in step 2) and it isn't shared with other apps:
delete `{TargetName}.Models/Api/{ClientName}.cs` and any request/response types under
`Api/Requests/`/`Api/Responses/` that exist solely to support this client.

## Injection sites

Remove the constructor parameter and field from every controller/worker that took this client,
per step 2 - this is a compile-breaking change if left in place after the class is gone.

## After making the change

- Show the user every file touched/deleted.
- If step 1 or 2 stopped the skill early (shared `.Models` project, or unresolved consumers),
  that's the whole response - don't delete a shared file or leave broken constructor parameters
  behind.
