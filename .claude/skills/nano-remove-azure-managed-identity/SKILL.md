---
name: nano-remove-azure-managed-identity
description: Remove Azure Managed Identity / Kubernetes Workload Identity wiring from a Nano.Library-based application - deletes service-account.yaml, the workload-identity pod annotations, and the CI "Managed Identity" step. Use when the user asks to remove Managed Identity or Workload Identity from a Nano API, Web, or Console application - checks first whether a Data or Storage provider still depends on it.
---

# Nano remove managed identity

Fully removes Azure Managed Identity / Kubernetes Workload Identity wiring from an existing Nano
API, Web, or Console application — the counterpart to `nano-add-azure-managed-identity`. Read that
skill first — this one undoes exactly what it adds.

## Before making any change, determine

1. **Is Managed Identity currently wired?** Check for `.kubernetes/service-account.yaml` and the
   `Managed Identity` workflow step. If neither exists, say so and stop.
2. **What depends on it?** Two genuinely different situations — check both, and don't treat them
   the same:
   - **A Data provider set to `AuthenticationType: Azure`** (MySql/PostgreSQL/SqlServer). This
     one has a real fallback: `Credentials`. But reverting to it isn't a config flip alone — it
     needs an actual connection string/credential the user supplies (this skill can't invent
     one), plus removing the CI `<Provider> Database Migration`/`SQL Server Create Database`
     steps' Managed-Identity-based user creation and the `Data__AuthenticationType`
     ConfigMap entry. **Ask the user which they want**: supply real credentials and revert this
     provider to `Credentials` as part of this change, or leave Managed Identity in place and
     stop here. Don't silently pick one.
   - **Azure Storage** (`AzureFileshareProvider`, `nano-add-storage-provider`'s Azure section).
     Unlike Data, there's **no credentials-based fallback for Storage** — per AGENTS.md's
     `Configuration` table, `Storage` has no `AuthenticationType` setting at all; Azure storage's
     file-share CSI mount is unconditionally Workload-Identity-authenticated
     (`mountWithWorkloadIdentityToken: "true"`). If Azure storage is in use, Managed Identity
     **cannot** be removed without breaking it outright — the only ways forward are switching to
     `Local` storage (`nano-remove-storage-provider` then `nano-add-storage-provider` with
     `Local`) or removing storage entirely first. Tell the user this plainly and stop; don't
     proceed with Managed Identity removal while Azure storage still depends on it.
   If neither applies, proceed normally.

## Kubernetes

- Delete `.kubernetes/service-account.yaml`.
- Remove the `azure.workload.identity/use: "true"` label and `serviceAccountName` field from
  `.kubernetes/deployment.yaml`/`cronjob.yaml`'s pod template.
- Remove the `service-account.yaml` apply block from the `Kubernetes Deploy` workflow step.

## GitHub Actions

Remove the `Managed Identity` step entirely. Leave `AZURE_GROUP_KUBERNETES` alone — every app's
workflow needs it independently for basic AKS deploy (`az aks get-credentials`), regardless of
Managed Identity.

⚠ This does **not** delete the underlying Azure user-assigned identity or its federated
credential in Azure itself (`az identity delete`) — that's a real Azure resource this skill
doesn't provision or own the lifecycle of. Removing the workflow step just stops maintaining it
going forward; say so explicitly rather than implying the Azure-side resource is gone too.

## After making the change

- Show the user every file touched/deleted.
- Restate whatever was flagged in step 2 — the Data provider reverted to `Credentials` (and what
  that required), or the fact that Storage blocked the whole removal — one more time here.
- If step 1 or step 2's Storage case stopped the skill early, that's the whole response.
