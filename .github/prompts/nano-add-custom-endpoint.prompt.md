---
mode: agent
description: Scaffold a custom, non-CRUD HTTP endpoint end-to-end - two genuinely different shapes depending on the controller. A Public API endpoint composes existing Api Client calls into one response. An internal-service endpoint implements real logic against this app's own IRepository/IEventing, and - since it's a contract another application will call - also scaffolds the paired Api Client custom request/method that calls it, in the same change. Use when the user asks to add a one-off action, custom endpoint, or operation that doesn't fit generic CRUD/Auth/Audit/Identity to a Nano API or Web application.
---

# Nano add custom endpoint

Generates a single custom HTTP action that doesn't fit the generic CRUD/Auth/Audit/Identity
surface `nano-add-entity` and the built-in Api Client method groups already cover. Read
AGENTS.md's `### Controllers` (including its `#### Public API vs internal service` note) and
`### Api Clients` sections first; this skill does not repeat those, only how to combine them
following this solution's own established conventions (one-liner XML doc summaries,
`[ProducesResponseType]` per status code, `Requests/`/`Responses/` folders matching the
controller's own namespace).

**Two genuinely different jobs, not one skill with an optional extra step.** A Public API endpoint
composes calls that already exist elsewhere; an internal-service endpoint *is* a new piece of
contract another application will call, so scaffolding it also means scaffolding the client-side
half of that same contract - one coherent task, not two skills chained together. **Step 2** below
determines which applies; read only the matching path once it's decided.

⚠ **Terminology**: this skill calls the customer/end-user-facing role "**Public API**" (e.g.
`Api.Platform`/`Api.Admin` in this solution - this solution's own project template for one is
`nanocore-api-public`), never "gateway." "Gateway" in this codebase means the Kubernetes Gateway
API resource (`nano-add-public-exposure`'s `HTTPRoute`/`Gateway`) or the network-edge/cert-manager
TLS layer in front of a cluster - an unrelated, infrastructure-level concept. Don't reuse that word
for this application-level role, in code, comments, or conversation with the user.

---

## Step 1 - Confirm a custom endpoint is actually needed

Per AGENTS.md's `## Core Principle - Built-In Before Custom`: a custom endpoint is only warranted
once the generic `.Entity` surface + `[Include]`-driven eager loading + query criteria is confirmed
insufficient - not just "less convenient." Walk through this before scaffolding anything:

- **Can the desired response be expressed as the target entity plus some of its navigation
  properties, at some depth?** If yes, this is very likely not a custom endpoint at all: tag the
  needed navigations `[Include]` on the entity (in the owning service's `.Models` project) and have
  the caller pass `includeDepth` through the generic `.Entity` call - `0` for a bare entity, higher
  to reach further navigations. See AGENTS.md's Include Annotation section for the exact mechanics.
- **Two real limits of that mechanism, either of which can still justify going custom even when the
  shape looks nav-expressible:**
  - **No selective `$expand`.** `includeDepth` only dials recursion depth - it can't pick which
    tagged navigations come back at that depth. If the response needs entity+NavA at depth 2 but
    never NavB even though NavB sits at the same depth, `[Include]` can't express that distinction;
    a custom endpoint can.
  - **`[Include]` is global to the entity, not scoped to one caller.** Tagging a navigation makes
    it eager-loadable for *every* consumer of that entity's generic endpoints - other internal
    services, other Public APIs - not just the one that prompted the change. If a navigation
    genuinely shouldn't be reachable by every other consumer (sensitive data, a payload-size
    concern for high-traffic internal callers), that's a real reason to keep a narrowly-scoped
    custom endpoint instead of tagging it.
- **Still custom regardless of `[Include]`:** computed/aggregated fields that don't exist on the
  entity, responses composed from more than one unrelated entity graph, or actual business logic
  beyond read/write. A representative case: an action that has to validate something (e.g. an
  email domain against a set of allowed domains) and then perform a multi-entity write as one
  atomic operation, where the write can't happen at all until the validation passes - neither step
  is expressible as a single generic `.Entity` call, and splitting them into two separate generic
  calls from the caller would let the write happen without the validation ever running. This is
  the right call for a custom endpoint, not a sign to keep looking for a generic-composition way
  around it.
- **The same generic-composition workaround needed at 2+ call sites is itself a signal to stop
  composing and build the real custom endpoint.** A union across two entity types (e.g. "roles
  owned by this tenant, plus roles reachable via its subscription plan") done as two generic calls
  glued together in one Public API action is fine the first time; the same two calls duplicated
  again in a second and third action is a sign the composition belongs on the *target* service as
  a real custom endpoint instead - one round trip, one place the logic lives, instead of the same
  non-trivial join reimplemented at every caller. Don't wait for a fourth duplicate before
  promoting it.
- **Before scaffolding a single-item lookup, check whether a sibling list/aggregate endpoint
  already makes it redundant.** If a "get all roles for this tenant" endpoint already returns every
  role with `RolePermissions` (or whatever the caller needs) populated, a separate "get one role"
  endpoint often isn't pulling its weight - the caller can fetch or already has the list and pick
  the one entry it wants. This isn't a hard rule (a list that's expensive to fetch, or a route that
  needs to 404 on a specific id rather than filter client-side, can still justify keeping both),
  but don't scaffold the single-item version reflexively just because a list version exists; ask
  whether it earns its own endpoint.
- **Is the actual need "the generic write plus an invariant that must always hold," not a new
  route at all?** If what's missing is validation before a create/edit, a linked-entity side-effect
  after one, or a guard before a delete *or a create* (e.g. rejecting a new child row once a
  sibling entity's existence makes the parent immutable) - and it should apply no matter which
  caller hits the generic route, not just one Public API that remembers to compose it - that's a
  case for **overriding the generic CRUD action** on the owning entity's own controller, not adding
  a separate custom endpoint. See **Internal service path § Overriding a generic CRUD action
  instead of a new endpoint** below before scaffolding a new route for this.
- **Before designing a custom action (or a composition) around a delete or an update, check what
  the database relationship already does for you.** A required (non-optional) EF Core relationship
  with no explicit `.OnDelete(...)` override defaults to `Cascade` - deleting the parent already
  removes the dependent row(s) at the database level, so an explicit second delete call for that
  child is redundant, not just harmless. This does **not** apply if the parent is soft-deletable
  (`IEntitySoftDeletable`): a soft delete is a plain `UPDATE` setting `IsDeleted`, not a real SQL
  `DELETE`, so no FK cascade fires and any dependent cleanup has to stay explicit. The reverse
  gotcha applies to edits: the generic `Edit`/`EditAndGet` actions only copy scalar properties onto
  the tracked entity (`SetValues`-style) - reassigning a collection navigation and calling generic
  Edit does **not** add/remove the underlying rows, so an update that needs to reconcile a child
  collection still needs its own explicit add/remove calls (composed at the Public API, or inside
  an overridden action - see below). Check both directions before adding calls a real cascade
  already makes unnecessary, or assuming a collection reassignment does something it doesn't.

If the generic surface (with appropriate `[Include]` tagging and `includeDepth`) already covers
this, say so and point the user at that instead of scaffolding something redundant - don't build a
custom action just because it was asked for without checking first. Note that adding a new
`[Include]` tag to an already-consumed entity is itself a change to that entity's existing generic
surface, not a free side-effect - say so rather than tagging it silently.

## Step 2 - Public API or internal service?

Not always obvious from the request alone - ask if unclear, don't default to one. Getting this
wrong means the wrong constructor, the wrong dependency, and a DTO shape aimed at the wrong layer:

- **Public API controller** (e.g. `Api.Platform`/`Api.Admin` in this solution) - the action
  composes one or more injected Api Clients (`.Entity`/`.Auth`/`.Audit`/`.Identity` calls, or an
  existing custom client method) into one response; it has no `IRepository` of its own. Go to
  **Public API path** below.
- **Internal service controller** - the action implements logic directly against this app's own
  `IRepository`/`IEventing`, either as a custom method on an existing entity controller
  (`BaseEntityController<...>` subclass) or a bare `BaseController` action with no entity backing
  it at all. Go to **Internal service path** below.

## Step 3 - Pin down shape and conventions

- **Endpoint shape.** HTTP verb, route segment, input shape (parameters), and response shape. Ask
  for whatever isn't already given - don't invent fields, routes, or status codes that weren't
  asked for or that don't match an existing sibling action's pattern in the same
  controller/project.
- **Naming and location conventions.** Skim an existing custom action in the same controller (or a
  sibling controller in the same project) for its `Requests/`/`Responses/` folder layout,
  doc-comment style, and `[ProducesResponseType]` set, and match it - this solution already has an
  established shape for this, don't invent a new one.

---

## Shared DTO conventions

Both paths below build request/response DTOs the same way - read this once, apply it wherever a
DTO comes up in either path:

- **Only include properties the endpoint actually needs** - no speculative fields, and (for a
  request) only what the *caller* should be able to set, never fields that represent
  internal/server-assigned state (an entity's `Id`, audit timestamps, computed status, etc.), even
  if the controller action happens to build an entity from the request afterward.
- **Match validation attributes to what the underlying entity/write actually needs, not just
  `[Required]`.** `[MaxLength]` on a string that maps to a length-constrained column, `[Range]` on
  a bounded number, etc. - mirror whatever the entity's own mapping/property already enforces, so a
  bad request is rejected by model binding before it ever reaches an Api Client call or a repository
  write, instead of surfacing as a downstream 400/500.
- **Check for an existing sibling DTO with the same shape before defining a new one.** A response
  that just repeats `Id`/`Name`/`Description`/`CreatedBy`/`PermissionKeys` from `Role` probably
  already has a `RoleResponse` (or equivalent) somewhere in the same project - reuse it rather than
  defining a near-duplicate. This applies across paths too: if an internal-service change makes a
  Public API's existing bespoke response DTO redundant (e.g. an indirection layer it existed to
  route around gets removed), that's a real signal to delete the bespoke DTO and switch the caller
  to the sibling one, not to keep both.
- **Default to returning the entity (or collection of entities) directly - reach for `[Include]` to
  stitch together whatever the response needs before reaching for a custom Response DTO.** A custom
  endpoint's job is usually a query or a write too specific for generic `.Entity`, not a shape too
  specific for the entity itself - the response is still an ordinary `Role`/`IEnumerable<Role>` with
  the right navigations eager-loaded. Return that type directly -
  `[ProducesResponseType(typeof(Role[]), ...)]`, `this.Ok(roles)` - not wrapped in a `<Name>Response`
  that just repeats the same properties. Building a custom Response DTO is valid, but treat it as
  the *last resort*: reach for it when the shape genuinely can't come from the entity plus
  `[Include]` (computed/aggregated fields not stored anywhere - e.g. "is this plan referenced by any
  Subscription," derived at read time rather than persisted - or a flattened projection across more
  than one unrelated entity graph) - not by default, and not just because it's the response of a
  custom action. A Public API that wants its *own* shaped DTO still maps the raw entity into one on
  its own side; that's not a reason for the target service's endpoint to invent one first.
- **If the response leans on nested navigations, every level of that chain needs `[Include]`, not
  just the top one.** Per AGENTS.md's Response Serialization section, a navigation only appears in
  the JSON response when it's both loaded (via `includeDepth`) *and* tagged `[Include]` on the
  property itself - and this applies independently at every level of the graph. A response built by
  walking `Role → SubscriptionPlanRoles → Role → RolePermissions` needs `[Include]` on each of those
  navigation properties, not just the first one; skipping a middle link means that step silently
  comes back empty even though the ends are tagged correctly. Trace the exact path the response
  constructor/mapping actually walks and confirm every property on it is tagged before assuming
  `[Include]` "already covers this."

---

## Public API path

The controller composes calls that already exist elsewhere - this path never defines a new Api
Client method of its own.

### Does the backing call already exist?

Check whether the Api Client(s) this action needs are already injected in this controller (or
injectable without issue) and whether the specific call needed is already a generic method or an
existing custom method - including a custom method on the *target* service's own controller that
already computes the exact union/aggregate this action needs (e.g. an existing "get all roles
available to a tenant" internal-service action), rather than re-deriving the same result here via
several generic calls.

If a **new custom Api Client method** is needed and doesn't exist yet: **stop and ask the user
whether to create it now**. That method's controller action lives on the *target* service - a
different application than this Public API. If the target's Api Client class doesn't exist at all
yet, that's `nano-add-api-client`'s job, on the target's own project; if the whole custom-endpoint
contract (controller action + client method) doesn't exist yet, that's *this skill's own Internal
service path*, run against the target application, not this one. Don't invoke either automatically,
and don't scaffold this Public API action against a method that doesn't exist yet as if it already
does - proceed here only once the user has confirmed whether/how that gets created elsewhere.

**Don't wrap a plain generic call - or a plain generic *composition* - in a new custom Api Client
method.** If the backing call is already `.Entity`/`.Auth`/`.Audit`/`.Identity` with no shaping or
extra logic of its own, call it directly from this controller action - don't add a method to the
target's Api Client class that does nothing but forward to the generic method. This isn't limited
to a single call: a get-then-create/edit/delete pattern (look something up, then mutate based on
what came back) is still just generic composition, not custom logic, and reads perfectly fine as
2-3 calls directly in the controller action - it doesn't earn a wrapper method just because it's
more than one line. A custom Api Client method should only exist when it's paired with a
controller action doing something the generic surface genuinely can't (the Internal service path
below, e.g. cross-entity validation gating a write) - a bare composition of generic calls, wrapped
or not, just hides what's actually happening for no benefit.

**Don't add a redundant existence pre-check in front of a built-in `.Identity` action.** Activate/
Deactivate/etc. already handle a nonexistent id themselves (404/no-op) - a `QueryFirstAsync` purely
to confirm the id exists before calling one is duplicated work the service already does. Only look
something up first if the action needs data the built-in call doesn't already return, or needs to
enforce an authorization/scoping check the built-in call doesn't perform itself - and if you skip
the lookup specifically to avoid that redundancy, say plainly in a comment whether that also drops
a scoping check (e.g. tenant ownership) the lookup used to provide, so it's a visible trade-off
rather than a silent one.

### Request DTO (if the action takes parameters)

Location: `Requests/<Feature>/<Name>Request.cs` in the Public API's own app project - **this is a
Public-API-facing DTO, not an Api Client `BaseRequest`**; don't confuse the two even though an Api
Client call happens inside the same action.

`<Feature>` is always the **route name of the controller that hosts the action** (its class name without
`Controller`, e.g. a controller named `FooBarsController` uses `FooBars`), for both `Requests/` and
`Responses/`. One folder per controller; never group DTOs by another axis (a shared `Common/` folder, or a
folder named after a sub-entity the controller also serves). A DTO used by several controllers lives in the
folder of the controller that returns it, or is duplicated per controller if each needs its own shape. A
request/response in a folder that doesn't match its controller is a defect: move it and fix the namespace
and `using`s.

```csharp
public class <Name>Request
{
    [Required]
    public virtual <Type> <Property> { get; set; } = ...;
}
```

`[Required]` on anything that must be present; match the nullable-reference style already used by
sibling `Request` classes in the same project. See **Shared DTO conventions** above for what else
belongs on this class.

### Response DTO (if the action returns a body)

Location: `Responses/<Feature>/<Name>Response.cs`, same project. A plain POCO (no base type
required) shaped to exactly what the caller needs - not a raw pass-through of an internal entity
unless that's genuinely what the sibling conventions in this project do. See **Shared DTO
conventions** above for the entity-vs-DTO default and the nested-`[Include]` requirement.

### Controller action

Add to an existing Public API controller, or create a new one deriving from `BaseController` if no
suitable controller exists yet:

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
    // Compose injected Api Client(s).

    return this.Ok(response);
}
```

- Only include the `[ProducesResponseType]`s that are actually reachable by this action's logic -
  match what sibling actions in the same controller declare, don't pad the list.
- **Only use the `...AndGet` Api Client calls when the returned entity is actually used.**
  `.Entity.CreateAndGetAsync`/`EditAndGetAsync` (with `CreateAndGetRequest`/`EditAndGetRequest`) make the
  target service reload the entity with its includes, which is wasted work when the action discards the
  result (a bare `return this.Ok();`, a join-row insert, a fire-and-forget update). Use plain
  `.Entity.CreateAsync`/`EditAsync` (`CreateRequest`/`EditRequest`) there, and reserve the `...AndGet`
  variants for when the result feeds the response. The same goes for `CreateOrGetAsync` when the existing
  row's content isn't needed.
- **Don't materialize a collection you don't need to.** Leave a LINQ query/`IEnumerable` as is when it's only
  enumerated once (mapped into a response, iterated, passed on). Materialize only when it's genuinely
  required: enumerated more than once, used as a value that must be stable (a `Contains` set, a captured
  closure), or assigned to a persisted collection property. Then prefer `.ToArray()` (the most minimal
  collection - also fine for an `ICollection<T>` navigation being persisted) and use `.ToList()` only when a
  `List<T>` is truly needed (`Add`/`AddRange`, an API that takes a `List<T>`). Likewise no `?? []` on a
  collection an Api Client call returns.
- `[AllowAnonymous]` only if this runs before a JWT exists (e.g. part of a login flow) - and if it
  calls another application's endpoint in that state, that target endpoint must itself be
  `[AllowAnonymous]`; note that requirement in a comment.
- **Caller-context claims (tenant id, user id, etc.).** Per AGENTS.md's Authentication forwarding
  caveat: if this action needs a piece of the caller's identity further downstream, **read it
  here** from this app's own already-validated JWT/`HttpContext` and pass it explicitly as a field
  on the outgoing custom request - don't rely on the target service re-extracting the same claim
  from the JWT Nano forwards alongside the call.

---

## Internal service path

This action **is** a new piece of contract another application will call - scaffolding it means
scaffolding both halves together: the controller action, and the paired Api Client custom
request/method that lets other applications actually call it.

### Does this app's own Api Client class exist yet?

Check `{ThisApp}.Models/Api/` for an existing `BaseApiClient`/`BaseIdentityApiClient` subclass. If
none exists, create the bare class inline as part of this same change - it's boilerplate with no
decision to make (see `nano-add-api-client`'s "Client class" shape), not a reason to stop and
chain into a separate skill.

### Shared body model

If the action takes parameters, define the payload **once**, as a plain model class in
`{ThisApp}.Models` (conventionally `Api/Requests/Models/<Name>.cs`) - this is the class **both**
the controller's `[FromBody]` parameter **and** the Api Client request's `[Body]` property bind
to, not two separate DTOs kept in sync by hand:

```csharp
public class <Name>
{
    [Required]
    public virtual <Type> <Property> { get; set; } = ...;
}
```

See **Shared DTO conventions** above for what belongs on this class. If the action returns a body,
prefer returning the target entity/collection directly (same section) - a
`Responses/<Feature>/<Name>Response.cs` POCO is the last resort, for when the shape genuinely can't
come from the entity itself.

### Api Client request and method

`{ThisApp}.Models/Api/Requests/{Name}Request.cs`, one action attribute
(`[GetAction]`/`[PostAction]`/etc.) naming the HTTP verb + relative route, wrapping the shared body
model from above:

```csharp
[PostAction(MyActionRoutes.MY_ACTION)]
public class MyActionRequest : BaseRequest
{
    [Body]
    public virtual MyAction Model { get; set; } = null!;

    public MyActionRequest()
    {
        this.Controller = "MyEntities";
    }
}
```

**Controller resolution** (per `Nano.App`'s own README): Nano infers the controller segment from
the pluralized `TResponse` type name - this works fine whenever a custom request's response
genuinely *is* the target Nano entity (e.g. a custom action added to an existing entity
controller that still returns that entity). Set `this.Controller` explicitly in the constructor
only in the two cases where inference can't land correctly:
- **No response at all** (`InvokeAsync<TRequest>`, no `TResponse`) - there's no type to infer
  from.
- **The route doesn't align with `TResponse`'s pluralized name** - either because `TResponse` is
  a bespoke DTO/POCO rather than the entity itself, or because the action lives on a *different*
  controller than the one the response type's name would imply.

Check this deliberately rather than assuming inference works - e.g. a request whose `TResponse`
is `MyFile` but whose action actually lives on `MyEntitiesController` (a file attached to an
entity, not a controller of its own) needs `this.Controller = "MyEntities";` set explicitly, the
same shape as the example above.

**Define the route segment as a constant** in a `Consts` class inside `{ThisApp}.Models` and
reference it from both this request's action attribute and the controller action's `[Route(...)]`
below - per AGENTS.md, nothing else keeps the two sides in sync, and Nano's own built-in requests
avoid drift exactly this way.

Add the corresponding method to this app's own Api Client class:

```csharp
public virtual Task MyActionAsync(MyAction model, CancellationToken cancellationToken = default)
{
    return this.InvokeAsync(new MyActionRequest
    {
        Model = model
    }, cancellationToken);
}
```

One method per custom request, calling `this.InvokeAsync(request, cancellationToken)` (no
response) or `this.InvokeAsync<TRequest, TResponse>(request, cancellationToken)` (typed response).
Give the method and its doc comment the same one-liner-summary treatment as the controller action
- name what it does and, if it exists only because the generic surface couldn't express it, why.

**If the caller needs to tell "not found" apart from "found but empty," keep the method's return
type nullable and let a 404 come back as `null` - don't `?? []` it away.** Per AGENTS.md's Api
Clients gotchas, a non-success response never throws for a plain 404 - it returns `null` for
`TResponse`, which for a collection response means `null` (not-found) is already distinguishable
from an empty collection (found, nothing to return) with no extra plumbing. Have the controller
action return `this.NotFound()` for the not-found case explicitly, and resist collapsing the
client method's `null` into `[]` "for convenience" - that throws away the exact distinction the
caller needs (e.g. a tenant that doesn't exist vs. a real tenant with no roles yet).

**The method's parameter is the shared body model itself, not its properties spread out as
separate scalar parameters.** `MyActionAsync(MyAction model, ...)` above, not
`MyActionAsync(string propertyOne, int propertyTwo, ...)` - the whole point of the shared body
model (previous section) is that it *is* the contract's shape; re-exploding it into scalar
parameters here just to reconstruct the same object one line later is pointless indirection, and
it makes the client method's signature drift from the model instead of just being it. Only
parameters that sit *outside* the body model itself (e.g. an `[FromQuery]`/route-bound id like
`tenantId`, which the request's own `[Query]`/`[Route]` property carries separately from `[Body]`)
belong as their own parameter alongside the model.

**Caller-context fields (tenant id, user id, etc.).** Per AGENTS.md's Authentication forwarding
caveat: if this request's target logic needs a piece of the *caller's* identity, add it as an
explicit property on the shared body model, populated by the calling application from its own
JWT - don't design this request to assume this controller will re-derive it from the forwarded
token instead. Note in the doc comment which claim the caller is expected to supply and why.

**Anonymous endpoints.** If this request is meant to be called before a caller has a JWT (e.g.
during another app's own login flow), note in the request's doc comment that the controller
action must be `[AllowAnonymous]`, and add that attribute below - the client side can't enforce
it, only document the expectation.

### Controller action

```csharp
/// <summary>
/// One-line summary of what this action does.
/// </summary>
/// <param name="model">The <see cref="MyAction"/>.</param>
/// <param name="cancellationToken">The cancellation token.</param>
/// <returns>The response.</returns>
/// <response code="200">OK.</response>
/// <response code="400">Bad Request.</response>
/// <response code="401">Unauthorized.</response>
/// <response code="500">Error occurred.</response>
[HttpPost]
[Route(MyActionRoutes.MY_ACTION)]
[ProducesResponseType((int)HttpStatusCode.OK)]
[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
[ProducesResponseType((int)HttpStatusCode.BadRequest)]
[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
public virtual async Task<IActionResult> MyActionAsync([FromBody][Required] MyAction model, CancellationToken cancellationToken = default)
{
    // Use IRepository/IEventing directly.

    return this.Ok();
}
```

- Add to an existing entity controller, or create a new one (`BaseController`, or the appropriate
  entity controller base per AGENTS.md's Entity controller hierarchy) if none exists yet - follow
  `nano-add-entity`'s controller-file conventions for a brand new controller's shape/naming
  (correct base tier, naming, eventing-parameter handling). This includes the retrofit case: an
  entity that already exists but has no generic controller yet still gets its full generic
  controller as part of creating it here - this action doesn't replace or narrow that entitlement.
- **Use `this.Repository`/`this.Eventing`, not the raw primary-constructor parameter, inside the
  action body.** On an entity controller (`BaseEntityController<...>` and friends), the primary
  constructor's `repository`/`eventing` parameters are already passed to the base constructor:
  referencing the same parameter again inside a method captures it a second time and is a compile
  error (CS9107 - "captured into the state of the enclosing type and its value is also passed to
  the base constructor"). The base class exposes `this.Repository`/`this.Eventing` properties for
  exactly this reason - use those instead.
- Only include the `[ProducesResponseType]`s actually reachable by this action's logic.
- Same collection rule as the Public API path: don't `.ToList()`/`?? []` a repository or Api Client result
  you only enumerate once; materialize only when required, preferring `.ToArray()` and `.ToList()` only when
  a `List<T>` is truly needed (e.g. `AddRange`).
- **Persist every change to a loaded entity with an explicit `this.Repository.UpdateAsync(entity, ...)`.** Don't
  assign a property on an entity read through the repository and rely on it being tracked and written by a later
  `AddAsync`/`DeleteAsync` save. Nano's `DbContext` overrides `Update` (it hydrates the entity graph so audit
  captures the original vs new values), so an update that skips that path is not equivalent, and an explicit call
  keeps the write visible on read. The price is a separate save per call instead of one transaction, so order the
  calls where a unique index or foreign key needs it (e.g. close the old row before adding the row that replaces
  it, delete before restoring a predecessor), and only rely on a single save when that atomicity genuinely matters.
- **Throw, don't bare-return, for error responses that will cross an Api Client boundary.** A
  plain `this.BadRequest()`/`this.NotFound()` `IActionResult` has no `ProblemDetails` body. Per
  AGENTS.md's Api Clients Gotchas and Error Handling sections: a `404` always surfaces as `null` to
  the caller regardless of body, so bare `this.NotFound()` is fine - but any other non-2xx with no
  parseable `ProblemDetails` body becomes an `ApiClientException` the calling Public API's own
  middleware doesn't recognize, and it falls back to a **generic 500** for whoever called the
  Public API, silently losing the real 400. Throw `new BadRequestException("<short reason>")` (`Nano.App.Exceptions`) (always with a short, human-readable message, never the parameterless form)
  instead - the centralized exception-handling middleware turns it into a proper `ProblemDetails`
  400 that an Api Client parses as `ProblemDetailsException` (any status), which the calling
  Public API's own middleware then correctly re-surfaces with the same status code. Same idea for a
  not-found case that specifically needs a message/code rather than a bare 404:
  `Nano.Data.Abstractions.Exceptions.NotFoundException`.
- **Don't narrow an entity's controller tier to dodge a route collision with this action - flag it
  instead.** The controller's tier (full CRUD by default, per `nano-add-entity`) doesn't change
  because a custom action sits alongside it. If this action's route+verb is identical to a route
  the generic tier also exposes, that's a genuine defect in the request-side contract (one of the
  two routes needs to change) - add the action anyway, with a prominent comment naming exactly
  which generic route it collides with (verb + path + which AGENTS.md table row), and leave both
  in place for the user to resolve.
- **Check built-in routes on narrower base classes too, not just the generic CRUD table** - e.g.
  `BaseEntityUserController`'s identity actions (AGENTS.md's Identity user controller table:
  `{id}/activate`, `{id}/deactivate`, etc.). An action whose route matches one of those collides
  exactly the same way a generic CRUD route would - flag it the same way.
- **The one exception: a deliberate override, not a collision.** Every generic CRUD action is
  `virtual` specifically so a subclass can extend it. If what's actually needed is "do what the
  base action already does, plus a little extra" - e.g. create the entity, then also publish a
  custom event - the correct approach is to **override the base method** (call the base
  implementation, or reproduce its persistence step, then add the extra behavior) on the *same*
  route, not scaffold a separate custom action that happens to reuse it. An override isn't a
  collision at all - same method, same route, extended behavior - so there's nothing to flag.
  Reach for this only when the operation genuinely *is* the base behavior plus a bit more; if it's
  doing something meaningfully different at that route, that's a real collision per the rule
  above, not an override candidate. This stays the exception, not the default - most custom
  actions should still avoid the base routes entirely; don't reach for an override as a shortcut
  to reuse a route a distinct operation shouldn't share. See the fuller treatment of this pattern
  right below - it's common enough to deserve its own walkthrough, not just a one-line exception.

#### Overriding a generic CRUD action instead of a new endpoint

The case above generalizes into a real alternative to scaffolding a new custom action: whenever
the actual requirement is "the same generic write, plus an invariant that must hold no matter which
caller/route triggers it" - validation before a create/edit, a linked-entity side-effect after one,
a reference-count guard before a delete *or before a create* (e.g. a plan whose Roles must stop
being addable, not just removable, once any Subscription references it) - override the relevant
`BaseEntityController<...>` method(s) directly rather than adding a parallel custom action next to
them. This enforces the rule as a property of the *entity's own controller*, so it holds for every
consumer, not just the one Public API that remembered to compose it.

- **Cover every generic write variant the invariant must survive, not just the one your current
  caller happens to use.** `BaseEntityController`'s full CRUD route table has several single-entity
  variants per verb: `CreateAsync`/`CreateAndGetAsync`/`CreateOrGetAsync` for create,
  `EditAsync`/`EditAndGetAsync` for edit, `DeleteAsync`/`DeleteManyAsync` for delete (plus bulk/
  query-based variants - decide per case whether those are reachable/relevant enough to matter).
  If the invariant genuinely must always hold, override all of the single-entity variants a caller
  could plausibly reach; overriding only the one your current Public API calls leaves the same gap
  a new custom endpoint would have needed to close anyway, just via a different route.
- **A new entity's `Id` is already assigned client-side, before persistence.** `BaseEntity`'s
  constructor sets `this.Id = Guid.NewGuid()` - so inside a `CreateAsync`/`CreateAndGetAsync`/
  `CreateOrGetAsync` override, `entity.Id` is already the real, final id immediately, even before
  calling `base.CreateAsync(...)`. Use it directly for any follow-up work (e.g. creating a linking
  row) instead of trying to extract an id back out of the base call's `IActionResult`.
- **The override's signature is fixed by the base method - there's no room to thread extra
  caller-context through it.** `EditAsync(TEntity entity, CancellationToken)`/
  `DeleteAsync(Guid id, CancellationToken)` can't gain an extra `tenantId` parameter the way a
  bespoke custom action could. If per this solution's convention a downstream service doesn't
  parse the caller's JWT itself (tenant/caller scoping is resolved once at the Public API and
  passed down explicitly - see AGENTS.md's Authentication forwarding caveat), then a
  generic-action override can only enforce invariants derivable from the entity/data itself
  (permission-subset validation, reference-count guards, linking rows) - it can't perform
  tenant-ownership authorization. The Public API still needs its own ownership pre-check (e.g. a
  scoped `QueryFirst`) before calling the generic write; the override and the Public API check are
  complementary, not either-or.
- **A reference-count/existence guard can gate a create just as validly as a delete.** Don't assume
  this pattern only protects against removing something still in use - "reject adding a child row
  once a sibling entity's existence makes the parent immutable" is the same shape of check
  (`CountAsync`/`QueryCountAsync` against the related entity, `throw new BadRequestException("<short reason>")` if it's
  non-zero), just applied to `CreateAsync`/`CreateAndGetAsync`/`CreateOrGetAsync` instead of
  `DeleteAsync`. If an entity has any notion of "locked" or "immutable" derived from another
  entity's existence, check both directions before assuming only deletes need guarding.
- **Reconciling a collection navigation is still an explicit step inside the override.** The same
  "generic Edit only copies scalars" gotcha from **Step 1** applies here unchanged - an
  `EditAsync`/`EditAndGetAsync` override that needs to add/remove related rows (not just update
  scalar properties) does so via its own `this.Repository.AddAsync`/`DeleteAsync` calls before or
  after calling `base.EditAsync(...)`, the same as a Public-API-side composition would have needed
  to. Overriding moves *where* this logic lives, not whether it's still needed.
- **Duplicate the validation across each overridden variant rather than extracting a shared private
  helper**, if that's this project's established preference for controllers (confirm against
  existing sibling controllers/AGENTS.md conventions before assuming) - expect the same block
  repeated in `CreateAsync`/`CreateAndGetAsync`/`CreateOrGetAsync` etc. rather than factored out.
- Still throw `BadRequestException`/`NotFoundException` for invariant violations inside these
  overrides, per the bullet above - the same Api Client propagation gotcha applies whether the
  error comes from a bespoke custom action or an overridden generic one.
- **If this action's route collides with another custom action's route** (same controller, same
  route+verb): a genuine defect in the request-side contract, not something to silently rename or
  merge. Scaffold both anyway, with a prominent comment on each naming the other action it
  collides with - flag it for the user to resolve rather than guessing.
- **Caller-context claims** - mirror of the request-side note above: read the caller's claims
  from this app's own JWT/`HttpContext` if this action needs them for something *further*
  downstream (e.g. calling yet another service) - this note is about what the *caller* already
  supplied explicitly on the request, which is the normal case for an internal-service action's
  own use of caller context.

---

## Postman collection

If the application already has a Postman collection, every endpoint this skill adds must also land in it:

1. Look for `Postman_<AppName>.json` in the application's own folder (next to its `README.md`).
2. **If it exists**, add the new endpoint's request to it - inside the folder for its controller, in the
   position matching the controller's own action order - with the correct verb, route (the same route
   constant the controller uses), request body/query, and a `description` noting anything a tester needs
   to know (`[AllowAnonymous]`, business-rule guards, side effects). Follow the conventions of the
   `nano-add-postman-collection` skill (base URL scheme, variable chaining, `bearer` auth). Edit only the
   new request; don't regenerate or reorder the rest of the file.
3. **If it doesn't exist, do nothing Postman-related** - don't create a collection. The endpoint is picked up
   when the user later generates the collection with `nano-add-postman-collection`.
4. If the file was edited, remind the user to **Replace**-import it in Postman - writing the file changes
   nothing in Postman by itself.

---
## After generating

- Show the user every file touched/created, grouped by concern (Request/Response DTOs, controller
  action, and - for the internal-service path - the shared body model, the Api Client request,
  and the Api Client method) and which project each lives in.
- **Internal service path**: state plainly that this scaffolds the contract, not the business
  logic - the controller action's body is a stub unless the user asked for the real
  implementation too.
- **Public API path**: if a missing custom Api Client method was surfaced and the user hasn't
  decided on it yet, that's the natural stopping point - don't scaffold the controller action
  against a call that doesn't exist, and don't guess at its shape.
- If step 1 found the generic surface already covers this, that's the whole response - explain
  what already does the job instead of generating anything.
