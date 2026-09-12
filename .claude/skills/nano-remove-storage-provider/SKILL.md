---
name: nano-remove-storage-provider
description: Remove a Nano storage provider (Local or Azure) from a Nano.Library-based application - unregisters it in Program.cs and removes the Storage configuration, local docker-compose volume mount, and the Kubernetes persistent volume (plus, for Azure, the Staging/Production fileshare-provisioning CI step). Use when the user asks to remove file storage, a fileshare, or a specific storage provider from a Nano API, Web, or Console application.
---

# Nano remove storage provider

Fully removes a Nano storage provider from an existing Nano API, Web, or Console application —
the counterpart to `nano-add-storage-provider`. Read that skill first — this one undoes exactly
what it adds, file for file.

## Before making any change, determine

1. **Which provider is currently registered?** Check `Program.cs` for `.AddNanoStorage<...>()`.
   If none, say so and stop.
2. **What depends on it?** Per AGENTS.md's `## Nano.Storage` section, registering a provider also
   registers `IPathProvider`, "injectable anywhere" — search the project for it used as a
   constructor parameter. A **required** `IPathProvider` parameter fails DI resolution the moment
   the provider is gone — the app won't start at all. This is the only dependency risk here:
   unlike Eventing, there's no declarative attribute (`[Publish]`/`[Subscribe]`) tied to storage
   that would silently stop working instead — anything using it does so explicitly, in code. If a
   required injection exists, tell the user removing the provider will crash the app there and
   confirm before proceeding.
3. **Is the package reference this skill's to remove?** Same check as the other remove skills:
   leave `NanoCore`/`Nano.All` alone if present; otherwise remove the `Nano.Storage.<Provider>`
   `PackageReference` from the application project.

## Program.cs

Remove `using Nano.Storage.Extensions;`, `using Nano.Storage.<Provider>;`, and the
`.AddNanoStorage<...>()` call. Same empty-lambda cleanup as the other remove-provider skills:
restore the blank-app placeholder and `_` parameter if nothing else is left in
`.ConfigureServices(...)`.

## appsettings.json

Remove the `Storage` section entirely from the base `appsettings.json`. There's no
Development-specific override to also clean up — per `nano-add-storage-provider`, `ShareName`
isn't sensitive and stays in the base file only, for both providers.

## docker-compose.yml

Remove the `volumes` entry mapping `./bin/<share-name>:/mnt/<share-name>` from the app's own
service in `.docker/docker-compose.yml`. Unlike a data or eventing provider, storage never added
a separate service container — just this one volume line — so there's nothing else to remove
here.

## Kubernetes — Local

- Delete `.kubernetes/storage-storageclass.yaml` and `.kubernetes/service-headless.yaml`.
- If the app was converted to a `StatefulSet` for this provider (`.kubernetes/stateful-set.yaml`
  present, `serviceName: %SERVICE_NAME%-stateful-headless` set), revert it to a plain `Deployment`:
  rename the file back to `deployment.yaml`, change `kind: StatefulSet` → `kind: Deployment`,
  remove the `serviceName` field, and remove the `volumeClaimTemplates` block (there's no static
  `PersistentVolumeClaim` file to restore in its place — the volume is gone entirely, not
  replaced). `.kubernetes/autoscaler.yaml` is always present on an API/Web app — if its
  `scaleTargetRef.kind` was changed to `StatefulSet` (i.e. the app was converted per the above),
  change it back to `Deployment`.
- Remove the `volumeMounts`/`volumes` entries referencing `%SERVICE_NAME%-volume` from the
  deployment/stateful-set container spec. Also remove the `tmp` `emptyDir` volume/mount
  (`IPathProvider`'s temporary directory, per AGENTS.md) — but only if nothing else in the
  container mounts `/tmp` for an unrelated reason; check first.
- Remove the `storage-storageclass.yaml`/`service-headless.yaml` apply blocks from the
  `Kubernetes Deploy` workflow step.
- Remove the `STORAGE_SIZE`/`STORAGE_SHARE_NAME` workflow env vars, if nothing else uses them.

## Kubernetes — Azure

- Delete `.kubernetes/storage-pv.yaml` and `.kubernetes/storage-pvc.yaml`.
- Remove the `volumeMounts`/`volumes` entries referencing `%SERVICE_NAME%-volume` from
  `.kubernetes/deployment.yaml`. Also remove the `tmp` `emptyDir` volume/mount, with the same
  caveat as the Local section above — only if nothing else needs `/tmp`.
- Remove the `Storage Role Permissions` and `Create Fileshare` workflow steps, and the
  `$env:VOLUME_NAME_SUFFIX = ...` derivation step, if nothing else in the workflow still uses
  `%VOLUME_NAME_SUFFIX%`.
- Remove the `storage-pv.yaml`/`storage-pvc.yaml` apply block from `Kubernetes Deploy`.
- Remove the `STORAGE_SIZE`/`STORAGE_SHARE_NAME` workflow env vars. Only remove
  `AZURE_GROUP_STORAGE`/`AZURE_GROUP_BACKUP` if nothing else in the workflow still references
  them — Managed Identity or other Azure-backed providers may share them.

## After making the change

- Show the user every file touched/deleted, grouped by concern (app code, local docker-compose,
  and for Azure, Staging/Production CI + K8s) — same reasoning as the add skill: too many files
  for a flat list to be easy to sanity-check.
- Restate anything flagged in step 2 — a required `IPathProvider` injection that will now crash
  the app — one more time here, even if the user already confirmed it.
- If a step was skipped because the project uses `NanoCore`/`Nano.All`, say so explicitly.
