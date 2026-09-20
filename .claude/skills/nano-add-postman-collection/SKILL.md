---
name: nano-add-postman-collection
description: Generate a Postman collection for a Nano.Library-based application - one collection file per application (never a shared multi-app file), covering Auth and every controller's actual generic-action tier plus its custom endpoints, with test scripts chaining entity Ids/names between requests. Use when the user asks to create, generate, or update a Postman collection for a Nano API, Web, or internal service application.
---

# Nano add Postman collection

Generates a Postman v2.1.0 collection JSON for one Nano application. Read `AGENTS.md` in the
target repo root first — its `#### Public API vs internal service` section is the single most
important input here: it changes the base URL scheme, which environments the collection can even
be run against, and (for a Public API) whether Auth is meaningful at all.

**One collection per application, always.** Never combine multiple applications' folders into one
shared collection file — even when generating collections for several apps in the same session,
each gets its own file. A shared collection makes the URL-scheme distinction below unrepresentable
(a Public API and an internal service need different base URLs in the same collection), and makes
it impossible to swap the Postman auth token between apps without stepping on another app's tests.

## Before generating anything, determine

1. **Is this app a Public API or an internal service?** Per AGENTS.md's `Public API vs internal
   service` section — check for `.kubernetes/httproute-*.yaml` (public exposure, see
   `nano-add-public-exposure`) or `App:Hosting:Https` in `appsettings.json`. This decides the base
   URL scheme (below) and which environments the collection is even runnable against — don't
   guess from the app's name.
2. **What controllers exist, and what does each one's own base class actually expose?** Read every
   controller's declaration line — never assume full CRUD. Check which `BaseEntity*Controller` it
   inherits (see AGENTS.md's `#### Entity controller hierarchy`) and only generate the actions that
   tier actually has:
   - `BaseEntityReadOnlyController` — Read only (Index, Details, Details Many, Query, Query First,
     Query Count). No Create/Edit/Delete folder at all.
   - `BaseEntityCreatableController` — Read + Create (Create, Create Or Get, Create And Reload,
     Create Many). **No Create Or Edit** — that action needs Edit capability, which this tier
     doesn't have; don't include it just because every other tier you've seen does.
   - `BaseEntityEditableController` — Read + Edit (Edit, Edit And Reload, Edit Many, Edit Query).
     No Create, no Delete.
   - `BaseEntityDeletableController` — Read + Delete (Delete, Delete Many, Delete Query). No
     Create, no Edit.
   - `BaseEntityCreatableAndEditableController` — Read + Create + Edit, including Create Or Edit
     now that both halves exist.
   - `BaseEntityController` — full CRUD (Read + Create + Edit + Delete).
   - `BaseEntityUserController` — the full Identity surface (SignUp, password/email/phone
     set-change-confirm flows, activate/deactivate, user roles/claims, refresh tokens, plus the
     Read/Edit/Delete tier) — see `nano-add-identity`/`nano-add-authentication-jwt` for the exact
     route list, or read `BaseEntityUserController.cs` directly for the current one.
   A `[Subscribe]` entity (AGENTS.md's Entity Events) is a strong signal its controller is
   `BaseEntityCreatableController` by convention — call this out in the collection (a folder
   description noting "replica kept in sync by eventing, not normally created directly") rather
   than leaving it looking like an arbitrary restriction.
3. **Every custom (non-generic) action on each controller.** Read its `[Route(...)]`/HTTP-verb
   attributes and cite the actual route constant, not a guessed path. Note anything a tester needs
   to know before calling it: `[AllowAnonymous]`, a business-rule guard that throws
   `BadRequestException` under some condition (e.g. an archived parent record), or a side effect
   that isn't a direct write (e.g. publishing an event for async processing rather than inserting a
   row) — put this in the request's `description` field, not just in your own notes.
4. **Is `Data:Identity` configured?** (`appsettings.json`'s `App:Data:Identity`, or check
   `Program.cs` for `.AddNanoIdentity(...)`.) If it's `null`, the plain username/password Login,
   Login Refresh, and Logout actions have no user store to authenticate against and won't work —
   the Auth folder is **Login Root only**. If Identity is configured, include the full Login/Login
   Root/Login Refresh/Logout set, plus an External Login folder if `nano-add-authentication-jwt`'s
   `ExternalLogins` (Microsoft/Google/Facebook) is configured — see that skill and
   `nano-add-authentication-microsoft` for the PKCE/authorize-URL flow if Microsoft is present.
5. **Root login credentials.** Read `appsettings.Development.json`'s `Jwt.RootLogin.Username`/
   `Password` **for this specific app** — never copy another app's credentials, they're
   per-app-configured and commonly differ.
6. **Local dev port.** For an internal service, the docker-compose-mapped host port (usually
   `8080` — check `.docker/docker-compose.yml`, don't assume). For a Public API, the local HTTPS
   dev port from `Properties/launchSettings.json`'s `applicationUrl` is irrelevant here — Postman
   should hit the same `{{port_https}}` environment variable every other Public API collection
   uses (see Environments below), not a per-app literal.
7. **Entity dependency order**, so the collection can actually be run top-to-bottom: an entity with
   a required FK to another entity must come after that entity's Create in the collection, so its
   request body can reference the earlier one's chained Id variable (see Variable chaining below).
   Skip a pure join/junction entity to the end regardless of alphabetical order, since it always
   depends on both sides existing first.

## Base URL scheme

| App type | Scheme | Rationale |
|---|---|---|
| Internal service (Svc.*) | `http://{{host}}:{{port}}` | Never publicly exposed - only reachable locally via docker-compose's mapped port, or inside the cluster. `{{port}}` is a single shared environment variable the user manually points at whichever service's docker-compose they're currently running - don't invent a per-app port variable. |
| Public API (Api.*) | `https://{{host}}:{{port_https}}` | Publicly exposed via `nano-add-public-exposure`'s HTTPRoute, terminates TLS. |

**This determines which environments the collection can be run against at all.** An internal
service's collection only works against the `Development` environment (docker-compose's local
port) - `Staging`/`Production` have no meaning for it, since nothing routes to an internal service
from outside the cluster. Say this explicitly in the collection's top-level `description` (or the
README section below) rather than letting a Staging run silently produce broken URLs. A Public
API's collection is meant to run against all three.

## Environments

**Never create environment files.** Environments live in Postman itself, not the solution - once
imported they're edited and maintained there, independent of the collection's own lifecycle in
git. Instead, tell the user (once per solution - skip this if they've already confirmed the three
environments exist from an earlier run of this skill) that they need three Postman environments
named `Development`, `Staging`, and `Production`, and show this table so they can create/verify
them:

| Variable | Development | Staging | Production |
|---|---|---|---|
| `host` | `localhost` | *(their real Staging domain)* | *(their real Production domain)* |
| `port` | the internal service's docker-compose port (commonly `8080`) | *(doesn't apply - internal services aren't reachable outside the cluster)* | *(doesn't apply)* |
| `port_https` | the Public API's local dev HTTPS port (commonly `4443`) | `443` | `443` |

## Keeping the collection in sync with Postman

The collection JSON file in the solution (see File output below) is the source of truth - never
suggest editing the collection directly inside Postman, since a direct edit there would silently
diverge from the file, or get overwritten unnoticed the next time this skill regenerates it.
Writing the file never puts it into Postman by itself - always tell the user which of these two
they need to do next, since Postman has no way to notice a file changed on disk:

- **First time for this app**: import the file normally (Postman → Import → select the file). This
  creates the collection and assigns it the `_postman_id` this skill's file already carries, so
  every later regeneration lines up with the Replace case below.
- **Updating an existing collection**: re-import via **Replace**, not a fresh Import - right-click
  the collection in Postman's sidebar → Import → select the updated file. Postman matches by
  `info._postman_id` and updates the existing collection in place. A fresh Import instead would
  create a duplicate alongside the old one.

## Generic action routes

| Action | Method | Route |
|---|---|---|
| Index | GET | `index` |
| Details | GET | `{id}/details` |
| Details Many | POST | `details/many` |
| Query | POST | `query` |
| Query First | POST | `query/first` |
| Query Count | POST | `query/count` |
| Create | POST | `create` |
| Create Or Get | POST | `create/get` |
| Create Or Edit | POST | `create/edit` |
| Create And Reload | POST | `create/reload` |
| Create Many | POST | `create/many` |
| Edit | PUT | `edit` |
| Edit And Reload | PUT | `edit/reload` |
| Edit Many | PUT | `edit/many` |
| Edit Query | PUT | `edit/query` |
| Delete | DELETE | `{id}/delete` |
| Delete Many | DELETE | `delete/many` |
| Delete Query | DELETE | `delete/query` |

Read/Details/Query-type requests append `?includedepth=4` (Index also gets
`Paging.Number=1&Paging.Count=10&Order.By=CreatedAt&Order.Direction=Desc`). Query/Query First
bodies wrap the entity's own query-criteria fields in `{ "Criteria": {...}, "Paging": {...},
"Order": {...} }` (Query First omits `Paging`); Query Count and Delete Query send the criteria
object directly, unwrapped. Edit Query sends `{ "Criteria": {...}, "PropertyUpdates": {...} }`.
The entity's route segment is its controller's class name with `Controller` stripped, lowercased,
concatenated with no separator (`OrderLineItemsController` → `orderlineitems`) - matches
`[Route("[controller]")]`'s own naming, not a kebab-case guess.

## Variable chaining

For each entity, a Create (or SignUp) request's `test` script stores `{{Entity.Id}}` and one
human-readable distinguishing field (`{{Entity.Name}}`, `{{Entity.Domain}}`, etc.) as collection
variables; a Delete request's `test` script clears them back to `null`. Later entities' Create
bodies reference the earlier entity's `{{Entity.Id}}` for their FK fields, so the collection can be
run top-to-bottom without manual value substitution - order the collection's top-level folders to
match the actual FK dependency chain (see step 7 above), not alphabetically.

## File output

- One file per application: `<AppName>.json` (e.g. `Svc.Accounts.json`, `Api.Admin.json`) at the
  **solution root** - never nested under a `.postman/` subfolder, never combined with another
  app's folders into one file.
- Collection `info.name` is the application name; top-level `item` entries are the app's own
  folders directly (Auth, then each entity in dependency order) - don't wrap them in an outer
  folder named after the app, since the collection itself already carries that name.
- Collection-level `auth` is `bearer` against `{{Auth.Token}}` (or `{{Auth.AdminToken}}` if the
  app's Auth folder distinguishes an admin token from a regular user token, following whatever the
  app's own login response actually returns).

## README

Add a `## Testing with Postman` section to the application's `README.md`, pointing at
`<AppName>.json` in the solution root, and listing the ordered request flow (matching the
collection's own folder order) with a one-line note per step for anything non-obvious: which
variable it sets, whether it's `AllowAnonymous`, whether it's Create-only/read-only and why, and
any business-rule guard worth knowing about before calling it.

## After making the change

- Show the request/folder count and the top-level folder list, so the user can sanity-check
  coverage without opening the file.
- State plainly whether this app is a Public API or internal service, and which environments the
  collection can actually be run against as a result.
- Remind the user to create (or verify) the three Postman environments per the Environments
  section above, showing the variable table again - environments live in Postman itself, not a
  file this skill can check for, so only skip the reminder if the user already confirmed within
  this same conversation that all three exist.
- Tell the user to bring the file into Postman per Keeping the collection in sync above - a fresh
  **Import** if this is the first time this app's collection has been generated, or a **Replace**
  import if this run updated a collection already in Postman. Don't leave this implicit - writing
  the file changes nothing in Postman until the user does one of these two.
- If Root credentials, the local port, or Identity configuration couldn't be found (missing
  `appsettings.Development.json` section, no docker-compose file, etc.), say so explicitly rather
  than guessing or silently reusing another app's values.
- If exploring the controllers surfaced a real gap (a custom Api Client request with no matching
  controller action, a doc comment claiming behavior the code doesn't have, etc.), flag it to the
  user rather than quietly working around it in the collection.
