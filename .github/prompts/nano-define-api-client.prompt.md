---
mode: agent
description: Define or extend the Api Client surface a Nano application exposes to other Nano applications - the BaseApiClient subclass and any custom request types, in the owning service's {Name}.Models project. Use when the user asks a service to expose a new client/endpoint to callers, add a custom method to an existing Api Client, or scaffold the client shape for a Nano API or Web application other services will consume.
---

# Nano define API client

Creates or extends the typed HTTP client surface a Nano application exposes to *other*
applications - the counterpart to `nano-add-api-client`, which wires an already-defined client
into a *consumer*. This skill is the owning service's job: deciding what it exposes and how.
Read AGENTS.md's `### Api Clients` section first - it documents the built-in method groups
(`.Entity`/`.Auth`/`.Audit`/`.Identity`), the custom-request attribute shapes, and the
`{TargetName}.Models/Api/` location convention in full; this skill does not repeat that, only how
to apply it.

If the user's request is actually about calling this client from some other app, not defining it,
that's `nano-add-api-client`'s job instead - point them there.

## Before making any change, determine

1. **Does a client class already exist for this application?** Check `{ThisApp}.Models/Api/` for
   an existing `BaseApiClient`/`BaseIdentityApiClient` subclass. If the request is just "add a
   method" to an existing one, skip straight to Custom requests/methods below - there's no new
   class to create.
2. **Does this application have persistent Identity?** Determines the base class for a *new*
   client: `BaseApiClient`/`BaseApiClient<TIdentity>` (no Identity, or identity type doesn't
   matter to callers), or `BaseIdentityApiClient<TUser[,TIdentity]>` (this app has Identity -
   unlocks the `.Identity` method group for every consumer). Check this app's own `Data:Identity`
   config / `BaseEntityUser`-derived entity.
   - **If a client already exists on the plain `BaseApiClient` base and this app has Identity
     configured** (e.g. Identity was added, via `nano-add-identity`, *after* the client was first
     defined): that client **must be changed** to derive from `BaseIdentityApiClient<TUser[,TIdentity]>`
     instead - otherwise none of this app's identity-management endpoints (sign-up, password,
     roles, claims, API keys) are reachable through it. Don't leave it on the plain base class
     just because "add identity" wasn't the request that triggered this particular change.
3. **What's the client's name?** Consumers reference it by exact class name (the `App:Apis`
   dictionary key on their side must match it exactly) - pick something unambiguous and stable;
   renaming it later breaks every consumer's config.
4. **Custom endpoints needed beyond `.Entity`/`.Auth`/`.Audit`/`.Identity`?** Per AGENTS.md's
   `## Core Principle - Built-In Before Custom`: only add a custom method if a single call can't
   compose the existing generic reads/writes, or if query criteria can't express the filter.
   Confirm which endpoint(s) this app actually serves before guessing at routes - don't invent a
   contract the controller side doesn't have.

## Client class

`{ThisApp}.Models/Api/{ClientName}.cs`:

```csharp
// Bare pass-through - no custom methods, relies entirely on .Entity/.Auth/.Audit
public class MyApi(ApiClient apiClient) : BaseApiClient(apiClient);
```
```csharp
// Identity-backed - adds the .Identity method group for every consumer
public class MyApi(ApiClient apiClient) : BaseIdentityApiClient<MyUser>(apiClient);
```

Add custom methods only if step 4 applies - one method per custom request, calling
`this.InvokeAsync(request, cancellationToken)` (no response) or
`this.InvokeAsync<TRequest, TResponse>(request, cancellationToken)` (typed response). Give the
method and its doc comment the same one-liner-summary treatment as a scaffolded controller
action - name what it does and, if it exists only because the generic surface couldn't express
it, why.

## Custom requests (only if step 4 applies)

`{ThisApp}.Models/Api/Requests/{Name}Request.cs`, one action attribute
(`[GetAction]`/`[PostAction]`/etc.) naming the HTTP verb + relative route, per AGENTS.md's four
request shapes.

**Controller resolution** (per `Nano.App`'s own README): Nano infers the controller segment from
the pluralized `TResponse` type name - this is the standard convention and works fine whenever a
custom request's response genuinely *is* the target Nano entity (e.g. a custom action added to
an existing entity controller that still returns that entity). `this.Controller` must be set
explicitly in the constructor only in the two cases where that inference can't land correctly:
- **No response at all** (`InvokeAsync<TRequest>`, no `TResponse`) - there's no type to infer
  from.
- **The route doesn't align with `TResponse`'s pluralized name** - either because `TResponse` is
  a bespoke DTO/POCO rather than the entity itself, or because the action lives on a *different*
  controller than the one the response type's name would imply (a custom action piggy-backing on
  an existing controller rather than getting its own).

In this solution specifically, most custom requests so far have hit the second case - gateway
and cross-service custom endpoints tend to return bespoke response shapes, or attach to a
controller that doesn't match the response's name (see `GetTenantDomainRequest`: its response is
the `TenantDomain` entity, but the action lives on `TenantsController`, not a dedicated
`TenantDomainsController`) - so check this deliberately rather than assuming inference works.

**Define the route segment as a constant** in a `Consts` class inside `{ThisApp}.Models` and
reference it from both this request's action attribute and the corresponding controller's
`[Route(...)]` on the app side - per AGENTS.md, nothing else keeps the two sides in sync, and
Nano's own built-in requests avoid drift exactly this way. If the controller action this request
targets doesn't exist yet, say so explicitly - defining the client side of a contract the server
side doesn't implement yet leaves callers with a 404, not a working endpoint.

**Caller-context fields (tenant id, user id, etc.).** Per AGENTS.md's Authentication forwarding
caveat: if this request's target logic needs a piece of the *caller's* identity, add it as an
explicit property on the request, populated by the calling application from its own JWT - don't
design this request to assume the target controller will re-derive it from the forwarded token
instead. Note in the doc comment which claim the caller is expected to supply and why.

**Anonymous endpoints.** If this request is meant to be called before a caller has a JWT (e.g.
during another app's own login flow), note in the request's doc comment that the corresponding
controller action must be `[AllowAnonymous]` - the client side can't enforce that, only document
the expectation for whoever implements the controller action.

## After making the change

- Show the user every file touched/created, and confirm which project they live in (this app's
  own `.Models`, not a consumer's).
- List exactly what's now exposed - the client class name and every custom method added - since
  this is the contract other applications will start building against.
- If a custom request's target controller action doesn't exist yet on this app, say so
  explicitly rather than leaving an unimplemented contract unmentioned.
- If the user's actual goal was consuming this (or another) client from a different application,
  point them at `nano-add-api-client` instead - this skill only defines the surface, it doesn't
  wire anything into a caller.
