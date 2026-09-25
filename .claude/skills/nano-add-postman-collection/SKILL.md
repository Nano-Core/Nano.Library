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
   - `BaseEntityCreatableAndDeletableController` — Read + Create (Create, Create Or Get, Create And
     Reload, Create Many) + Delete (Delete, Delete Many, Delete Query). No Edit, so no Create Or Edit.
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
   Root/Login Refresh/Logout set, plus a Login request per configured `ExternalLogins` provider
   (Microsoft/Google/Facebook, from `App:Authentication:Jwt:ExternalLogins` — see
   `nano-add-authentication-microsoft`) — see External login requests below for what each one's
   `description` needs.
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

## External login requests

A `Login {Provider}` request's body only carries what the *backend* needs (`Code`/`CodeVerifier`/
`RedirectUri` for Microsoft/Google, `AccessToken` for Facebook) — none of that can be produced by
Postman itself, since it's the *frontend* that has to redirect a real browser through the
provider's own sign-in first. Put the frontend's side of that exchange in the request's
`description`, as the actual authorize-redirect URL (or SDK note for Facebook) with placeholders,
so a tester doesn't have to go hunting for it elsewhere:

- **Source the URL template from `Nano.App.Api` README's `#### Microsoft`/`#### Google` sections**
  (under External Logins) — don't reproduce your own version of the query-string shape from
  memory, that section is the one place PKCE parameters, scopes, and (for Google)
  `access_type`/`prompt` are kept correct; copy the template from there so this collection can't
  drift from it.
- **Resolve `{ClientId}`/`{TenantId}` from this app's own config** (`appsettings.Development.json`'s
  `App:Authentication:Jwt:ExternalLogins:{Provider}`) — these are real, per-app values, not
  placeholders, so put the actual configured value in the URL.
- **`redirect_uri` also doesn't need to be a real per-app frontend URL for a test collection — use
  the same fixed local value every time**: `http://localhost/auth`. Put it directly in the
  authorize URL in the description (in place of a `{redirect_uri}` placeholder), and set
  `Auth.Microsoft.RedirectUri` (or the Google equivalent)'s **collection variable default** to this
  same value, so `Login {Provider}`'s `RedirectUri` field already matches the URL in the
  description out of the box. This value has nothing to do with the app's own hosting/CORS
  config — Microsoft/Google only compare it byte-for-byte against what the app registration's
  redirect URI allowlist contains, so `http://localhost/auth` only actually works end-to-end once
  it's added to that allowlist too (`nano-add-authentication-microsoft`'s
  `AUTH_MICROSOFT_REDIRECT_URI`/`--web-redirect-uris` is developer-supplied, not fixed by that
  skill — add `http://localhost/auth` there, or add it as an extra entry alongside a real one, so
  this collection's fixed value is actually registered and not just a placeholder that looks
  real). Document that overriding it (with a real frontend callback URL, registered to match)
  works too, same as `code_verifier`/`code_challenge` below.
- **`state` and `code_challenge`/`code_verifier` don't need to be freshly random per attempt for a
  test collection — use the same fixed values every time.** `state` is inert to everyone but the
  frontend itself (nothing server-side ever checks it), so put the literal `12345` in the URL
  instead of a `{state}` placeholder. `code_challenge` genuinely can't be an arbitrary placeholder —
  Microsoft's token endpoint checks it against `SHA256(code_verifier)` when the code is redeemed —
  but it also doesn't need to change between runs, so use this fixed, real, already-matching pair
  in every collection this skill generates, rather than computing a new one each time:
  - `code_verifier`: `Postman-DevTest-CodeVerifier-Fixed-Value-1234567890-ABCDEFGHIJK`
  - `code_challenge` (its SHA-256, base64url, no padding): `d8u9K3bpfN5jJTaQxkiQ33ippLXy-OONYvhVMFumhNY`

  Put the `code_challenge` value directly in the authorize URL in the description. Set
  `Auth.Microsoft.CodeVerifier` (or the Google equivalent)'s **collection variable default** to the
  `code_verifier` value above, so the paired `Login {Provider}` request already sends a verifier
  that matches out of the box — the tester only has to fill in `Code`/`RedirectUri` from a real
  redirect, not reconstruct a matching PKCE pair by hand. Still document that overriding the
  variable (with a freshly generated pair, and a matching `code_challenge` in the URL) works too,
  for anyone who wants a real per-attempt PKCE exchange instead of the fixed test pair.
- **For Microsoft, `TenantId` is not always the literal `organizations`** — it depends on which
  `--sign-in-audience` this specific app was set up with (check its own CI workflow's "Setup App
  Registration" step, not another app's). Per the sign-in-audience table, `AzureADMyOrg` means
  `TenantId` is the real tenant GUID (`AZURE_TENANT_ID`), `AzureADMultipleOrgs` means the literal
  `organizations`, and so on — two apps in the same solution can genuinely use different audiences,
  so don't carry a value over from one app's collection into another's without checking.
- **Facebook has no authorize-redirect URL at all** (AGENTS.md's Authentication section — it's an
  implicit flow, the client-side Login SDK obtains the access token directly). Don't fabricate one;
  instead note in the description that the frontend uses the Facebook Login SDK to obtain an
  `AccessToken` client-side and sends it straight through.

## .Prerequisites folder

Sometimes this app's own collection needs test data that only a *different* application can
create — Api.Platform's Postman collection can't test anything before a real, finalized User
exists, but Api.Platform itself has no signup of its own; a Public API's read-only reference data
(Currencies, Product Categories) may have no Create endpoint in that app at all. When this
happens, don't just note it in a description and leave the tester to go run a different
collection by hand first — add a `.Prerequisites` folder (the leading dot sorts it first in
Postman's sidebar, ahead of Auth) with the minimum requests needed to unblock this collection's
own flow.

- **Target the actual owning application directly, never a composing Public API.** A Public API
  (Api.Admin, Api.Platform) has no data of its own — it's always some internal service
  (`Svc.Accounts`, `Svc.Assets`, etc.) that actually owns the entity, per AGENTS.md's Public API vs
  internal service section. Go straight there, using that service's own base URL scheme
  (`http://{{host}}:{{port}}` — see Base URL scheme above), not through another Public API's
  composed endpoint. This also means `.Prerequisites` only ever works against `Development` (the
  same reason an internal service's own collection is Development-only) — say so in the folder's
  `description`, even when the rest of this collection runs against all three environments.
- **Create-only, not full management.** The point is unblocking this collection's own flow, not
  duplicating the owning application's own collection — one (or a short chain of) Create request(s)
  for exactly the entity/entities this collection can't create itself, nothing more. That owning
  entity's real Read/Edit/Delete surface stays in its owning application's own collection, not
  here.
- **Reuse whatever auth the owning service actually offers for this** — its own Login Root if one
  exists and its role-based authorization is enough for the generic Create actions involved (see
  AGENTS.md's Authorization policy table), or an existing anonymous endpoint if the entity in
  question genuinely has one (e.g. `signup`). Don't invent new backend capability just to make this
  folder anonymous — if the owning service's real routes require an authenticated call to create
  the prerequisite data, that's the truth to reflect, not something to route around.
- **Wire the result into the variables this collection's own requests already expect.** If Auth /
  Login expects `User.EmailAddress`/`User.Password`, the last `.Prerequisites` request's test
  script should set exactly those — not a separately-namespaced variable the rest of the collection
  never reads.
- **Place it where its own FK dependencies are actually satisfied, not always at the top.** A
  `.Prerequisites` folder with no dependency on anything else in this collection (bootstrapping the
  very first User, e.g.) is the collection's first top-level folder, ahead of Auth. One that needs
  an Id this collection's own earlier folders create (e.g. an Assets folder needs an Organization
  that only exists in another service, but that Organization itself needs this collection's own
  already-created `Tenant.Id`) can't run before that — nest a `.Prerequisites` folder *inside* the
  specific folder that needs it instead, right before that folder's own requests, so it runs at the
  point in the collection where its own inputs actually exist. Don't force every prerequisite into
  one global first folder just for consistency if doing so would make it reference a variable
  that isn't set yet.

## File output

- One file per application: `Postman_<AppName>.json` (e.g. `Postman_Svc.Accounts.json`,
  `Postman_Api.Admin.json`), in that application's **own folder** (the same directory as its
  `README.md`) - never at the solution root, never nested under a `.postman/` subfolder, never
  combined with another app's folders into one file.
- Collection `info.name` is the application name; top-level `item` entries are the app's own
  folders directly (Auth, then each entity in dependency order) - don't wrap them in an outer
  folder named after the app, since the collection itself already carries that name.
- Collection-level `auth` is `bearer` against `{{Auth.Token}}` (or `{{Auth.AdminToken}}` if the
  app's Auth folder distinguishes an admin token from a regular user token, following whatever the
  app's own login response actually returns).

## README

Add a `## Testing with Postman` section to the application's `README.md`, pointing at
`Postman_<AppName>.json` alongside it in the same folder, and listing the ordered request flow
(matching the collection's own folder order) with a one-line note per step for anything
non-obvious: which variable it sets, whether it's `AllowAnonymous`, whether it's Create-only/
read-only and why, and any business-rule guard worth knowing about before calling it. If a
`.Prerequisites` folder exists, it's step one, with a short explanation of what it bootstraps, that
it only applies to `Development`, and which variables it hands off to the rest of the flow.

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
