---
name: nano-add-azure-managed-identity
description: Wire Azure Managed Identity federated to Kubernetes Workload Identity onto a Nano.Library-based application's Kubernetes deployment - adds service-account.yaml, the workload-identity pod annotations, and the CI "Managed Identity" step that provisions and federates the identity. Use when the user asks to add Managed Identity, Workload Identity, or passwordless Azure resource access to a Nano API, Web, or Console application - typically a prerequisite before setting a Data or Storage provider to AuthenticationType: Azure.
---

# Nano add managed identity

Wires Azure Managed Identity, federated to Kubernetes Workload Identity, onto an existing Nano
API, Web, or Console application's Kubernetes deployment. This is infrastructure-only — there's
no `App:`/`Data:` config section it owns itself; providers consume the identity it establishes
through their own `AuthenticationType: Azure` setting. `nano-add-data-provider` (MySql/
PostgreSQL/SqlServer) and `nano-add-storage-provider` (Azure section) both already assume this is
wired before their Staging/Production sections apply — this skill is what makes that true.

## Before making any change, determine

1. **Is Managed Identity already wired?** Check for `.kubernetes/service-account.yaml` and a
   `Managed Identity` step in the `Kubernetes Deploy` workflow. If present, say so and stop.
2. **What's it for?** Ask if not already given — usually about to back a Data provider
   (`AuthenticationType: Azure`) or Azure Storage. This skill only establishes the identity
   itself; flipping a provider's `AuthenticationType` is that provider's own skill's job, not
   this one's — don't do it here even if the reason is already known.
3. **Application type.** No difference in wiring between API, Web, or Console — whichever of
   `deployment.yaml`/`cronjob.yaml` the app has gets the same annotations.
4. **Does the workflow already have `AZURE_GROUP_KUBERNETES`?** Every app's workflow needs it
   already for basic AKS deploy (`az aks get-credentials`), so it's virtually always already
   present — confirm rather than assume, but there's normally nothing to add for it here.

## Kubernetes

1. **`.kubernetes/service-account.yaml`** (new file):
   ```yaml
   apiVersion: v1
   kind: ServiceAccount
   metadata:
     name: %SERVICE_NAME%-service-account
     namespace: %KUBERNETES_NAMESPACE%
     annotations:
       azure.workload.identity/client-id: %IDENTITY_CLIENT_ID%
   ```
2. **`.kubernetes/deployment.yaml`/`cronjob.yaml`**: add the workload-identity label to the pod
   template's metadata and reference the service account in the pod spec:
   ```yaml
   template:
     metadata:
       labels:
         azure.workload.identity/use: "true"
     spec:
       serviceAccountName: %SERVICE_NAME%-service-account
   ```

## GitHub Actions

Add the `Managed Identity` step, placed after the AKS-credentials step (`az aks get-credentials`)
and before `Kubernetes Deploy`. It's idempotent — creates the identity only if it doesn't already
exist, and (re)creates the federated credential every run regardless:

```yaml
- name: Managed Identity
  shell: pwsh
  run: |
    $env:IDENTITY_NAME = $env:SERVICE_NAME + "-identity";
    $env:IDENTITY_PRINCIPAL_ID = az identity show -g $env:AZURE_GROUP_KUBERNETES -n $env:IDENTITY_NAME --query principalId -o tsv;
    $env:KUBERNETES_ISSUER_URL = az aks list -g $env:AZURE_GROUP_KUBERNETES --query [0].['oidcIssuerProfile.issuerUrl'] -o tsv;

    if (-not $env:IDENTITY_PRINCIPAL_ID)
    {
        az identity create `
            -g $env:AZURE_GROUP_KUBERNETES `
            -n $env:IDENTITY_NAME;

        if ($LastExitCode -ne 0)
        {
            throw "error";
        };

        $env:IDENTITY_PRINCIPAL_ID = az identity show -g $env:AZURE_GROUP_KUBERNETES -n $env:IDENTITY_NAME --query principalId -o tsv;
    }

    $env:IDENTITY_CLIENT_ID = az identity show -g $env:AZURE_GROUP_KUBERNETES -n $env:IDENTITY_NAME --query clientId -o tsv;

    az identity federated-credential create `
        --name $env:SERVICE_NAME-credentials `
        --resource-group $env:AZURE_GROUP_KUBERNETES `
        --identity-name $env:IDENTITY_NAME `
        --issuer $env:KUBERNETES_ISSUER_URL `
        --subject "system:serviceaccount:${env:KUBERNETES_NAMESPACE}:${env:SERVICE_NAME}-service-account" `
        --audience api://AzureADTokenExchange;

    if ($LastExitCode -ne 0)
    {
        throw "error";
    };

    echo "IDENTITY_NAME=$env:IDENTITY_NAME" >> $env:GITHUB_ENV;
    echo "IDENTITY_CLIENT_ID=$env:IDENTITY_CLIENT_ID" >> $env:GITHUB_ENV;
    echo "IDENTITY_PRINCIPAL_ID=$env:IDENTITY_PRINCIPAL_ID" >> $env:GITHUB_ENV;
```

Apply `service-account.yaml` in the `Kubernetes Deploy` step, before `deployment.yaml`/
`cronjob.yaml` — same `Get-Content | ExpandEnvironmentVariables | kubectl apply` pattern as every
other manifest.

## After making the change

- Show the user every file touched.
- Remind them this only establishes the identity — nothing consumes it yet. Point them at
  `nano-add-data-provider` (set `AuthenticationType: Azure`) or `nano-add-storage-provider` (its
  Azure section) as the actual next step for whatever prompted this.
- If step 1 stopped the skill early, that's the whole response.
