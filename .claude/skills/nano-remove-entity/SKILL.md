---
name: nano-remove-entity
description: Remove a Nano.Library entity end-to-end - data model, EF Core mapping, query criteria, and CRUD controller - following Nano framework conventions. Use when the user asks to remove, delete, or drop a specific entity/resource from a Nano-based application, as a standalone request (not as a side effect of removing a whole Data provider or Identity - see nano-remove-data-provider/nano-remove-identity for those).
---

# Nano remove entity

Removes everything `nano-add-entity` generates for one entity — data model, EF Core mapping,
query criteria, and CRUD controller — as a standalone request. Read that skill first; this one
undoes exactly what it adds, file for file, in reverse.

**Not the same job as `nano-remove-data-provider`/`nano-remove-identity`.** Those remove an entity
only as a side effect of a bigger structural change (no Data provider left to persist it, or
Identity being torn out). This skill is for "just delete this one entity" — a genuine standalone
request that doesn't imply anything else in the app is changing.

## Before removing anything, determine

1. **Which entity, and where does it live?** Confirm the exact class name and check the project
   layout (split `.Models` project vs single-project, per `nano-add-entity`'s own layout
   rule) to know where each file actually is.
2. **What else in this repo references it?** Two distinct risks, not one:
   - **Other entities' relationships.** Search every other entity's mapping for a
     `.HasOne(...)`/`.HasMany(...)` pointing at this entity (per `nano-add-entity`'s
     both-ends-explicit mapping convention, the reference could be declared on *either* side).
     Removing the entity without also removing or reworking the other side's relationship
     configuration breaks that mapping — an `EntityTypeBuilder` call referencing a type that no
     longer exists doesn't compile. Find every one before touching anything, and confirm with the
     user how each should be resolved (drop the relationship entirely, or point it at something
     else) rather than guessing.
   - **Is this entity itself a many-to-many join entity, or does removing it orphan one?** Per
     `nano-add-entity`'s convention, a many-to-many relationship is modeled as its own join entity
     (e.g. `ProductTag`), not EF's implicit join table — so removing one side of that relationship
     (`Product` or `Tag`) leaves the join entity (`ProductTag`) with a dangling FK to a type that
     no longer exists, the same compile break as any other orphaned relationship. Remove the join
     entity too (its own File 1/File 2 pair) as part of the same change, not as an afterthought
     once the build breaks.
   - **Custom controller actions or Api Client custom requests targeting it.** Per
     `nano-add-custom-endpoint`'s internal-service path, an entity's controller may carry custom
     actions backing another application's Api Client methods, alongside its generic CRUD surface.
     Deleting the controller without accounting for those leaves that Api Client calling a route
     that no longer exists — the same class of cross-application risk `nano-remove-api-client`
     flags for a client definition, just via the generic/custom controller surface instead of a
     `BaseApiClient` subclass. List every matching request found and confirm with the user before
     proceeding.
3. **Is this entity consumed outside this repo at all?** If `{ThisApp}.Models` is published (NuGet
   or private feed) and this entity's type, query criteria, or controller-backed routes are part
   of that public surface, other applications entirely outside this repo may reference it —
   something this skill has no visibility into. Say this plainly rather than implying "nothing
   references it" just because nothing in *this* repo does.
4. **Is it `[Publish]` or `[Subscribe]`?** (AGENTS.md's `### Entity Events`) The two sides carry
   very different risk:
   - **`[Publish]`** — this app is the source of truth other applications replicate from. Removing
     it here stops every downstream `[Subscribe]`r from ever receiving another `Added`/`Modified`/
     `Deleted` event for it — their local replicas silently go stale, not error out. This is the
     same class of cross-application risk `nano-remove-api-client` flags for a client definition,
     and just as invisible: this skill has no way to see which other applications subscribe to
     this `TypeName`, in this repo or (especially) outside it. Say that plainly and confirm with
     the user before removing a `[Publish]`d entity — don't imply "nothing subscribes to this"
     just because nothing does *locally*.
   - **`[Subscribe]`** — the opposite, low-risk case: this is a local replica kept in sync from
     another application's source entity. Removing it here only stops *this* app from keeping a
     local copy; it has no effect on the publishing app's real entity or any other subscriber.
     Still worth confirming the user understands the distinction, especially if the request was
     phrased as "delete the entity" without specifying which side — but this is a much smaller
     decision than removing the `[Publish]` side.
5. **Has this entity ever actually persisted data?** Check `Migrations/` for one that created its
   table. If none exists, dropping the mapping/model is the whole job. If one does, removing the
   mapping without a corresponding migration leaves the table behind in the database, orphaned —
   ask whether the user wants a new migration generated to drop it (`dotnet ef migrations add
   <Name>`, same "only if asked, or if the project's workflow clearly expects one per entity"
   rule `nano-add-entity` uses), since this is a real, generally irreversible data-loss
   action on top of removing the code, not something to do unprompted.

## Files to delete

- `Controllers/<Entity>sController.cs` (API/Web only) — check step 2's second risk first.
- `Criterias/<Entity>QueryCriteria.cs` (API/Web only, whichever project per the layout).
- `Data/Mappings/<Entity>Mapping.cs` (main app project, always).
- `Data/<Entity>.cs` (whichever project per the layout) — check step 2's first risk first; don't
  delete this before every other entity's relationship to it has been resolved, or you'll be
  fixing the same compile break twice.

## After making the change

- Show the user every file deleted, and every other file touched to resolve step 2's
  relationship/collision risks (the other side of a relationship, or a flagged Api Client
  consumer) — not just this entity's own four files.
- Restate step 3's cross-repo caveat if `{ThisApp}.Models` is published — this skill can only
  confirm nothing *local* still depends on the entity, not that nothing anywhere does.
- Restate step 5's outcome — whether a migration was generated to actually drop the table, or
  whether that was deliberately left for the user to do separately (in which case the table
  stays behind until they do).
- If step 2 surfaced unresolved relationships or consumers the user hasn't decided how to handle,
  that's the whole response — don't delete the entity out from under a mapping or controller that
  still expects it to exist.
