---
mode: agent
description: Scaffold a custom, non-CRUD HTTP endpoint end-to-end - controller action, Request/Response DTOs, and (when the endpoint composes another Nano application) the backing Api Client method - following Nano framework and this solution's established conventions. Use when the user asks to add a one-off action, custom endpoint, or operation that doesn't fit generic CRUD/Auth/Audit/Identity to a Nano API or Web application.
---

# Nano scaffold custom endpoint

Generates a single custom HTTP action that doesn't fit the generic CRUD/Auth/Audit/Identity
surface `nano-scaffold-entity` and the built-in Api Client method groups already cover - the
controller action, its `Request`/`Response` DTOs, and, if the action needs to call another Nano
application, the Api Client call that backs it. Read AGENTS.md's `### Controllers` and
`### Api Clients` sections first; this skill does not repeat those, only how to combine them for
one custom action following this solution's own established conventions (the pattern built out
across `Api.Platform`/`Api.Admin` this session: one-liner XML doc summaries, `[ProducesResponseType]`
per status code, `Requests/`/`Responses/` folders matching the controller's own namespace).

## Before generating anything, determine

1. **Is this actually custom?** Per AGENTS.md's `## Core Principle - Built-In Before Custom`: a
   custom endpoint is only warranted if a single call can't compose the existing generic
   `.Entity`/`.Auth`/`.Audit`/`.Identity` methods (plus `[Include]`d reads) and query criteria
   can't express the filter. If the generic surface already covers this, say so and point the
   user at that instead of scaffolding something redundant - don't build a custom action just
   because it was asked for without checking first.
2. **Which kind of controller is this?** The shape of everything below depends on this, and it's
   not always obvious from the request alone:
   - **Gateway controller** (e.g. `Api.Platform`/`Api.Admin` in this solution) - the action
     composes one or more injected Api Clients (`.Entity`/`.Auth`/`.Audit`/`.Identity` calls, or
     a custom client method) into one response; it has no `IRepository` of its own.
   - **Internal service controller** - the action implements logic directly against this app's
     own `IRepository`/`IEventing`, either as a custom method on an existing entity controller
     (`BaseEntityController<...>` subclass) or a bare `BaseController` action with no entity
     backing it at all.
   If the target app/controller isn't named, or it's not clear from context which kind applies,
   **ask** - don't default to one. Getting this wrong means the wrong constructor, the wrong
   dependency, and a DTO shape aimed at the wrong layer.
3. **Endpoint shape.** HTTP verb, route segment, input shape (route/query/body parameters), and
   response shape. Ask for whatever isn't already given - don't invent fields, routes, or status
   codes that weren't asked for or that don't match an existing sibling action's pattern in the
   same controller/project.
4. **Does the backing call already exist?** For a gateway controller, check whether the Api
   Client(s) it needs are already injected in this controller (or injectable without issue) and
   whether the specific call needed is already a generic method or an existing custom method.
   - If a **new custom Api Client method** is needed and doesn't exist yet: **stop and ask the
     user whether to create it now** (that's `nano-define-api-client`'s job, in the *owning*
     service's own project - a different application than this controller likely lives in).
     Don't invoke that skill automatically, and don't scaffold this action against a method that
     doesn't exist yet as if it already does. Proceed with this skill only once the user has
     confirmed whether/how that gets created. If that new custom request ends up without a
     response, or its response doesn't resolve (by pluralized name) to the controller the action
     actually lives on - the common case for a gateway/cross-service custom endpoint, whose
     response is often a bespoke DTO or which piggy-backs on a controller unrelated to the
     response's own name - `nano-define-api-client` must set `this.Controller` explicitly in
     that request's constructor; don't assume Nano's route-inference will find the right
     controller on its own.
5. **Naming and location conventions.** Skim an existing custom action in the same controller (or
   a sibling controller in the same project) for its `Requests/`/`Responses/` folder layout,
   doc-comment style, and `[ProducesResponseType]` set, and match it - this solution already has
   an established shape for this (see `Api.Platform`/`Api.Admin`'s `AccountsController`,
   `TenantsController`, etc.), don't invent a new one.

## Request DTO (if the action takes parameters)

Location: `Requests/<Feature>/<Name>Request.cs` in the controller's own app project - **this is
a gateway-facing DTO, not an Api Client `BaseRequest`**; don't confuse the two even when an Api
Client call happens inside the same action.

```csharp
public class <Name>Request
{
    [Required]
    public virtual <Type> <Property> { get; set; } = ...;
}
```

- Only include properties the endpoint actually needs - no speculative fields.
- `[Required]` on anything that must be present; match the nullable-reference style already used
  by sibling `Request` classes in the same project.

## Response DTO (if the action returns a body)

Location: `Responses/<Feature>/<Name>Response.cs`, same project. A plain POCO (no base type
required) shaped to exactly what the client needs - not a raw pass-through of an internal entity
unless that's genuinely what the sibling conventions in this project do.

## Controller action

Add to an existing controller, or create a new one deriving from `BaseController` (gateway) or
the appropriate entity controller base (internal service, per AGENTS.md's Entity controller
hierarchy) if no suitable controller exists yet - follow `nano-scaffold-entity`'s controller-file
conventions for a brand new controller's shape/naming.

```csharp
/// <summary>
/// One-line summary of what this action does.
/// </summary>
/// <param name="request">The request.</param>
/// <param name="cancellationToken">The cancellation token.</param>
/// <returns>The response.</returns>
/// <response code="200">OK.</response>
/// <response code="400">Bad Request.</response>
/// <response code="401">Unauthorized.</response>
/// <response code="500">Error occurred.</response>
[HttpPost]
[Route("<segment>")]
[ProducesResponseType(typeof(<Name>Response), (int)HttpStatusCode.OK)]
[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
[ProducesResponseType((int)HttpStatusCode.BadRequest)]
[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
public virtual async Task<IActionResult> <Name>Async([FromBody][Required] <Name>Request request, CancellationToken cancellationToken = default)
{
    // Gateway: compose injected Api Client(s).
    // Internal service: use IRepository/IEventing directly.

    return this.Ok(response);
}
```

- Only include the `[ProducesResponseType]`s that are actually reachable by this action's logic
  - match what sibling actions in the same controller declare, don't pad the list.
- `[AllowAnonymous]` only if this runs before a JWT exists (e.g. part of a login flow) - and if
  it calls another application's endpoint in that state, that target endpoint must itself be
  `[AllowAnonymous]`; note that requirement in a comment, per the pattern used earlier this
  session for pre-auth service calls.
- **Route constant sharing** applies only when this controller is itself the *target* of another
  application's Api Client call (i.e., this is an internal service's real controller, not a
  gateway) - in that case, define the route segment as a constant in `{Name}.Models/Consts/`
  (this project's existing convention - see e.g. `Svc.Accounts.Models/Consts/`) and reference it
  from both this `[Route(...)]` and the calling side's custom request's action attribute, per
  AGENTS.md. A gateway controller with no Api Client pointed at it has no such constant to
  share.
- **Caller-context claims (tenant id, user id, etc.).** Per AGENTS.md's Authentication forwarding
  caveat: if this action (gateway or internal-service) needs a piece of the caller's identity
  further downstream, **read it here** from this app's own already-validated JWT/`HttpContext`
  and pass it explicitly as a field on the outgoing custom request - don't rely on the target
  service re-extracting the same claim from the JWT Nano forwards alongside the call. This keeps
  claim-parsing in one place and means the target doesn't need a real, matching tenant behind the
  forwarded token just to exercise the endpoint in `Development`.

## After generating

- Show the user every file touched/created, grouped by concern (Request/Response DTOs, controller
  action, and - if applicable - the Api Client method it calls) and which project each lives in.
- If step 4 surfaced a missing custom Api Client method the user hasn't decided on yet, that's
  the natural stopping point - don't scaffold the controller action against a call that doesn't
  exist, and don't guess at its shape.
- If step 1 found the generic surface already covers this, that's the whole response - explain
  what already does the job instead of generating anything.
