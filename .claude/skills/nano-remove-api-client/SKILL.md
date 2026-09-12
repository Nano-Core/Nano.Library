---
name: nano-remove-api-client
description: Stop consuming a Nano Api Client from this application - removes the App:Apis configuration entry and the client's injection site, without touching the client's definition in the owning service. Use when the user asks to remove a call to another Nano service/API, stop consuming an internal service, or drop an API client from this app's gateway composition in a Nano API, Web, or Console application.
---

# Nano remove API client

Removes this application's *consumption* of a Nano Api Client — the `App:Apis` config entry and
the injection site — without touching the client's definition in the owning service's `.Models`
project. The counterpart to `nano-add-api-client`. Read that skill first — this one undoes
exactly what it adds, and nothing more.

If the actual goal is to delete the client's definition entirely (so *no* application can consume
it anymore), that's `nano-undefine-api-client`'s job instead, on the owning service's side — this
skill never deletes a `.Models` project's client class, since that class may still be consumed by
other applications this skill has no visibility into.

## Before making any change, determine

1. **Is the `App:Apis` entry for this client actually present in this app?** Check the base
   `appsettings.json` for `App:Apis:{ClientClassName}`. If none, say so and stop.
2. **What depends on it in this app?** Search for the client class used as a constructor
   parameter (controller or worker) in *this* app only. This isn't a startup-crash risk — the
   client itself has no required-service semantics beyond normal C# compilation — but removing
   the config while something still injects the class simply **won't compile** (or, if the class
   still resolves some other way, silently stops working). Find every injection site in this app
   first.

## appsettings.json (this app)

Remove the `App:Apis:{ClientClassName}` section from the base `appsettings.json`, and its
`LogInRoot`/other overrides from `appsettings.Development.json` and any Staging/Production secret
wiring (Kubernetes secret reference, GitHub secret), if present.

## Injection sites (this app)

Remove the constructor parameter and field from every controller/worker in this app that took
this client, per step 2 — this is a compile-breaking change if left in place after the config is
gone.

## After making the change

- Show the user every file touched in this app.
- Note explicitly that the client's own definition in the owning service's `.Models` project was
  **not** touched — other applications may still consume it. If the user's actual intent was to
  delete the definition entirely, point them at `nano-undefine-api-client` next.
- If step 1 or 2 stopped the skill early (no entry present, or unresolved injection sites), that's
  the whole response — don't leave broken constructor parameters behind.
