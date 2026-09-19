---
name: nano-add-storage-provider
description: Add a Nano storage provider (Local or Azure) to a Nano.Library-based application - registers it in Program.cs, adds the Storage configuration section, the local docker-compose volume mount, and the Kubernetes persistent volume (plus, for Azure, the Staging/Production fileshare-provisioning CI step). Use when the user asks to add file storage, a fileshare, or a specific storage provider to a Nano API, Web, or Console application.
---

# Nano add storage provider

Wires a Nano storage provider into an existing Nano API, Web, or Console application. Read
`AGENTS.md` in the target repo root first — its `## Nano.Storage` section documents the
`Configuration` table, the provider/package table, and `IPathProvider` in full; this skill does
not repeat any of that, only how to apply it and wire the surrounding infrastructure
(docker-compose, K8s, and for Azure, CI) without breaking what's already there.

Both providers are simpler at the code level than a data or eventing provider — per AGENTS.md,
`Local` and `Azure` both represent storage already mounted into the container's filesystem and
are accessed identically through `IPathProvider`; there's no provider-specific client/SDK to
wire into the app itself. **Everything that differs between them is infrastructure** —
docker-compose is identical either way; only the Kubernetes manifests and (for Azure) the CI
provisioning step differ.

## Before making any change, determine

1. **Is this app meant to be a Public API?** Per AGENTS.md's [Controllers § Public API vs
   internal service](#public-api-vs-internal-service), a Public API composes Api Clients into
   responses and has no `IRepository` of its own — a Storage provider is *allowed* there (not a
   hard block), but it's a deviation from that lean-façade design, not the default. If this app is
   a Public API, confirm with the user that file storage genuinely belongs on this app rather than
   on an internal service reached via Api Client, before proceeding.
2. **Which provider.** `Local` or `Azure` (see AGENTS.md's provider table for package/type
   names). Ask the user if not already given.
3. **Is a storage provider already registered?** Check `Program.cs` for an existing
   `.AddNanoStorage<...>()` call — like eventing, there's one `IPathProvider` implementation
   per app, not a multi-provider case. If one exists, treat this as a replace and say so.
4. **Is a package reference even needed?** Same check as the other add-provider skills: look for
   `NanoCore`/`Nano.All` (directly, or transitively via a `.Models` project). If found, skip the
   package step. Otherwise add `<PackageReference Include="Nano.Storage.<Provider>" Version="X.Y.Z" />`
   to the **application project**, matching the version of the project's existing Nano
   application-type package. Never a `ProjectReference` to Nano.Library source.

## Program.cs

```csharp
using Nano.Storage.Extensions;
using Nano.Storage.<Provider>;
```

```csharp
x.AddNanoStorage<<Provider>Provider>();
```

`<Provider>Provider` is `LocalFileShareProvider` for `Local`, `AzureFileshareProvider` for
`Azure` (see AGENTS.md's provider table for the exact names). Same `.ConfigureServices(...)`
lambda placement and `_` → `x` rename rule as the other add-provider skills. No other C# files
are needed — no context/factory equivalent, unlike the data-provider skill.

## appsettings.json

Add the `Storage` section from AGENTS.md's `### Configuration` example to the base
`appsettings.json` (sibling of `App`). `ShareName` isn't sensitive (it's just a name, not a
credential) — it can stay set in the base file for both providers, no Development-specific
override needed for it.

Include `HealthCheck` only if `App:HealthCheck` is also enabled — AGENTS.md is explicit that
storage health checks do nothing without it (⚠ under `#### Health Checks`), so adding one
without the other is dead configuration.

## docker-compose.yml (local Development)

Identical for both providers — a bind-mounted local directory standing in for whatever the real
provider mounts in Staging/Production. Add to the app's own service in
`.docker/docker-compose.yml`:

```yaml
volumes:
  - ./bin/<share-name>:/mnt/<share-name>
```

matching `Storage:ShareName`. No separate service container needed (unlike a data or eventing
provider) — there's nothing to run, just a directory.

## Kubernetes — Local

- **`.kubernetes/storage-storageclass.yaml`** (new file):
  ```yaml
  apiVersion: storage.k8s.io/v1
  kind: StorageClass
  metadata:
    name: %SERVICE_NAME%-storage-class
  provisioner: disk.csi.azure.com
  parameters:
    storageaccounttype: Standard_LRS
    kind: Managed
  reclaimPolicy: Retain
  volumeBindingMode: WaitForFirstConsumer
  ```
- **`ReadWriteOnce`, one volume per pod, not one shared volume.** A local disk-backed volume can
  only attach to a single pod — so if the app runs more than one replica (`deployment.yaml`'s
  `kind: Deployment`, all replicas sharing one pod template), every replica referencing the same
  static PVC name would race for the same single-attach disk; only the first pod to schedule
  would mount successfully; the rest fail with a `Multi-Attach` error and never become ready. So
  `Local` storage's Deployment must be a **`StatefulSet`**, using `volumeClaimTemplates` instead
  of a single static `PersistentVolumeClaim` file — that gives each replica pod its own
  separate, uniquely-named PVC/disk automatically. (This does mean each pod's files are
  isolated from the others — a file written via one replica isn't visible from another. If the
  app instead needs one *shared* volume across replicas, that's what `Azure` storage is for,
  below — its file-share CSI mount supports concurrent multi-pod access.)
- **Replace `.kubernetes/deployment.yaml` with a new `.kubernetes/stateful-set.yaml`** — per
  AGENTS.md's Solution Structure table, the two are mutually exclusive, and `stateful-set.yaml`
  replaces `deployment.yaml` entirely rather than the two coexisting. Delete `deployment.yaml`,
  create `stateful-set.yaml` with the same content plus `kind: StatefulSet` (not `Deployment`) and
  `serviceName: %SERVICE_NAME%-stateful-headless` alongside `replicas`/`selector` (a `StatefulSet`
  field, required — see the headless service below); update the `.sln`'s `.kubernetes`
  `SolutionItems` block to reference the new filename instead of the old one. Mount the volume,
  plus the standard `tmp`
  `emptyDir` volume that backs `IPathProvider`'s temporary directory (AGENTS.md: registering a
  provider "also registers `IPathProvider` ... exposing the storage root and a temporary (`tmp`)
  directory") — include `tmp` for **both** providers, it's provider-agnostic:
  ```yaml
  volumeMounts:
  - name: %SERVICE_NAME%-volume
    mountPath: /mnt/%STORAGE_SHARE_NAME%
  - name: tmp
    mountPath: /tmp
  ```
  ```yaml
  volumes:
  - name: tmp
    emptyDir: {}
  ```
  and, as a top-level sibling of `template:` (not nested inside `template.spec`) —
  `volumeClaimTemplates` replaces the `PersistentVolumeClaim` file entirely:
  ```yaml
  volumeClaimTemplates:
  - metadata:
      name: %SERVICE_NAME%-volume
    spec:
      accessModes:
        - ReadWriteOnce
      storageClassName: %SERVICE_NAME%-storage-class
      resources:
        requests:
          storage: %STORAGE_SIZE%Gi
  ```
- **`.kubernetes/service-headless.yaml`** (new file) — a `StatefulSet` requires a governing
  headless service for pod network identity, separate from the app's normal `ClusterIP` service:
  ```yaml
  apiVersion: v1
  kind: Service
  metadata:
    name: %SERVICE_NAME%-stateful-headless
    namespace: %KUBERNETES_NAMESPACE%
  spec:
    clusterIP: None
    ports:
    - name: http
      port: 8080
    selector:
      app: %SERVICE_NAME%
  ```
- **`.kubernetes/autoscaler.yaml`** — always present on an API/Web app (per AGENTS.md's Solution
  Structure), the only app types this `StatefulSet` conversion ever applies to: change its
  `scaleTargetRef.kind` from `Deployment` to `StatefulSet` too — otherwise it silently targets a
  resource kind that no longer exists.
- **Workflow**: add `STORAGE_SIZE` (a bare number of GB, e.g. `1000` — the template above appends
  `Gi`) and `STORAGE_SHARE_NAME` env vars, and apply
  `storage-storageclass.yaml` (still needed — referenced by name from `volumeClaimTemplates`)
  and `service-headless.yaml` (same `Get-Content | ExpandEnvironmentVariables | kubectl apply`
  pattern as every other manifest) in `Kubernetes Deploy`, before `stateful-set.yaml` (which
  replaces the `deployment.yaml` apply line, per the rename above). There's no
  separate PVC file to apply — `volumeClaimTemplates` creates one per pod automatically as the
  `StatefulSet` itself is applied. Also add `.kubernetes\storage-storageclass.yaml =
  .kubernetes\storage-storageclass.yaml` and `.kubernetes\service-headless.yaml =
  .kubernetes\service-headless.yaml` to `{name}.sln`'s `.kubernetes` `SolutionItems` block (see
  AGENTS.md's Solution Structure note) — new files under `.kubernetes/` don't show up in Visual
  Studio's Solution Explorer otherwise.

## Kubernetes — Azure

This provider requires **Managed Identity** (`nano-add-azure-managed-identity` — service-account.yaml,
workload-identity annotations, the CI "Managed Identity" step that produces
`$env:IDENTITY_NAME`/`$env:IDENTITY_CLIENT_ID`/`$env:IDENTITY_PRINCIPAL_ID`) — the fileshare mount
authenticates via that identity, not a stored credential, and unlike Data's `AuthenticationType`,
Azure storage has no credentials-based fallback at all (per AGENTS.md's `Configuration` table,
`Storage` has no `AuthenticationType` setting). This isn't an adjacent, optional feature to ask
about before pulling in — it's a hard technical dependency of Azure storage itself: the
"Storage Role Permissions" step below directly references `$env:IDENTITY_PRINCIPAL_ID`, which is
simply undefined without it, so the generated workflow would fail the first time it runs. If the
project doesn't have Managed Identity yet, **apply `nano-add-azure-managed-identity` as part of this
same change** rather than stopping to ask or leaving it as a dangling prerequisite — then note in
the final summary that it was added alongside storage, so the user isn't surprised by the extra
files. It also assumes the target Azure Storage **account** already exists — provisioning the
account itself is out of this skill's scope (a real external resource to ask about, unlike
Managed Identity, which is just configuration this skill can apply itself), only the fileshare
*on* it is provisioned below.

1. **Workflow env vars**:
   ```yaml
   AZURE_GROUP_STORAGE: ${{ vars.AZURE_RESOURCE_GROUP_STORAGE }}
   AZURE_GROUP_BACKUP: ${{ vars.AZURE_RESOURCE_GROUP_BACKUP }}
   STORAGE_SIZE: 25
   STORAGE_SHARE_NAME: <share name>
   ```
2. **Fileshare provisioning** — two steps, placed after `Managed Identity` and before
   `Kubernetes Deploy`. Both are idempotent (existence-checked), safe to always include:
   ```yaml
   - name: Storage Role Permissions
     shell: pwsh
     run: |
       $env:STORAGE_ACCOUNT_ID = az storage account list -g $env:AZURE_GROUP_STORAGE --query [0].id -o tsv;

       az role assignment create `
           --assignee-object-id $env:IDENTITY_PRINCIPAL_ID `
           --assignee-principal-type ServicePrincipal `
           --role "Storage File Data SMB MI Admin" `
           --scope $env:STORAGE_ACCOUNT_ID

       if ($LastExitCode -ne 0) { throw "error"; };

   - name: Create Fileshare
     shell: pwsh
     run: |
       $env:STORAGE_ACCOUNT_NAME = az storage account list -g $env:AZURE_GROUP_STORAGE --query [0].name -o tsv;

       $env:FILE_SHARE_EXISTS = az storage share-rm exists -g $env:AZURE_GROUP_STORAGE -n $env:STORAGE_SHARE_NAME --storage-account $env:STORAGE_ACCOUNT_NAME --query exists;

       if ($env:FILE_SHARE_EXISTS -eq "false")
       {
           az storage share-rm create -g $env:AZURE_GROUP_STORAGE -n $env:STORAGE_SHARE_NAME --storage-account $env:STORAGE_ACCOUNT_NAME --access-tier TransactionOptimized --quota $env:STORAGE_SIZE;
       }
       else
       {
           az storage share-rm update -g $env:AZURE_GROUP_STORAGE -n $env:STORAGE_SHARE_NAME --storage-account $env:STORAGE_ACCOUNT_NAME --access-tier TransactionOptimized --quota $env:STORAGE_SIZE;
       }

       if ($LastExitCode -ne 0) { throw "error"; };

       $env:BACKUP_VAULT_NAME = az backup vault list -g $env:AZURE_GROUP_BACKUP --query [0].name -o tsv;

       az backup protection enable-for-azurefileshare -g $env:AZURE_GROUP_BACKUP -v $env:BACKUP_VAULT_NAME -p $env:STORAGE_ACCOUNT_NAME-fileshare-backup-policy --storage-account $env:STORAGE_ACCOUNT_NAME --azure-file-share $env:STORAGE_SHARE_NAME;

       if ($LastExitCode -ne 0) { throw "error"; };

       echo "STORAGE_ACCOUNT_NAME=$env:STORAGE_ACCOUNT_NAME" >> $env:GITHUB_ENV;
   ```
3. **`.kubernetes/storage-pv.yaml`** (new file):
   ```yaml
   apiVersion: v1
   kind: PersistentVolume
   metadata:
     name: %SERVICE_NAME%-azurefile-pv-%VOLUME_NAME_SUFFIX%
   spec:
     capacity:
       storage: %STORAGE_SIZE%Gi
     accessModes:
       - ReadWriteMany
     persistentVolumeReclaimPolicy: Retain
     storageClassName: azurefile-static
     mountOptions:
       - dir_mode=0777
       - file_mode=0777
       - uid=0
       - gid=0
     claimRef:
       name: %SERVICE_NAME%-azurefile-pvc-%VOLUME_NAME_SUFFIX%
       namespace: %KUBERNETES_NAMESPACE%
     csi:
       driver: file.csi.azure.com
       volumeHandle: %AZURE_GROUP_STORAGE%#%STORAGE_ACCOUNT_NAME%#%STORAGE_SHARE_NAME%-%VOLUME_NAME_SUFFIX%
       volumeAttributes:
         shareName: %STORAGE_SHARE_NAME%
         storageAccount: %STORAGE_ACCOUNT_NAME%
         resourceGroup: %AZURE_GROUP_STORAGE%
         clientID: %IDENTITY_CLIENT_ID%
         mountWithWorkloadIdentityToken: "true"
   ```
4. **`.kubernetes/storage-pvc.yaml`** (new file):
   ```yaml
   apiVersion: v1
   kind: PersistentVolumeClaim
   metadata:
     name: %SERVICE_NAME%-azurefile-pvc-%VOLUME_NAME_SUFFIX%
     namespace: %KUBERNETES_NAMESPACE%
   spec:
     accessModes:
       - ReadWriteMany
     storageClassName: azurefile-static
     resources:
       requests:
         storage: %STORAGE_SIZE%Gi
     volumeName: %SERVICE_NAME%-azurefile-pv-%VOLUME_NAME_SUFFIX%
   ```
5. **`%VOLUME_NAME_SUFFIX%`** — derived in the `Kubernetes Deploy` step, not a static env var:
   ```powershell
   $env:VOLUME_NAME_SUFFIX = $env:IDENTITY_CLIENT_ID.Substring(0, 5);
   ```
   placed before the `storage-pv.yaml`/`storage-pvc.yaml` apply block (which come before
   `deployment.yaml`, same order as every other manifest). The suffix keeps the PV/PVC name
   unique per identity, avoiding collisions across redeploys. Also add
   `.kubernetes\storage-pv.yaml = .kubernetes\storage-pv.yaml` and `.kubernetes\storage-pvc.yaml
   = .kubernetes\storage-pvc.yaml` to `{name}.sln`'s `.kubernetes` `SolutionItems` block (see
   AGENTS.md's Solution Structure note) — new files under `.kubernetes/` don't show up in Visual
   Studio's Solution Explorer otherwise.
6. **`.kubernetes/deployment.yaml`** — mount it (`ReadWriteMany`, so multiple replicas can share
   it, unlike `Local`), plus the same `tmp` `emptyDir` volume noted in the Local section above:
   ```yaml
   volumeMounts:
   - name: %SERVICE_NAME%-volume
     mountPath: /mnt/%STORAGE_SHARE_NAME%
   - name: tmp
     mountPath: /tmp
   ```
   ```yaml
   volumes:
   - name: %SERVICE_NAME%-volume
     persistentVolumeClaim:
       claimName: %SERVICE_NAME%-azurefile-pvc-%VOLUME_NAME_SUFFIX%
   - name: tmp
     emptyDir: {}
   ```

## After making the change

- Show the user every file touched, grouped by concern (app code, local docker-compose,
  Kubernetes, and for Azure, CI) — too many files for a flat list to be easy to sanity-check.
- If the package step was skipped (`NanoCore`/`Nano.All` already covering it), say so explicitly.
- For `Azure`, if Managed Identity wasn't already wired, say explicitly that it was added as part
  of this change (list its files alongside storage's own) — don't let it pass as an unremarked
  side effect. If the target Storage **account** doesn't exist yet, that's still an external
  prerequisite outside this skill's scope — flag it rather than silently doing only the app-code
  half of the job.
