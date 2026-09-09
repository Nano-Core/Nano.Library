---
name: nano-add-public-exposure
description: Expose a Nano API or Web application publicly - adds HTTPS hosting config, Kubernetes HTTPRoute (Gateway API) resources for ports 80/443, and the CI step that derives the app's public hostname from every configured Azure DNS zone. Use when the user asks to expose a Nano application publicly, add HTTPS/a public domain, or add an HTTPRoute to a Nano API or Web application.
---

# Nano add public exposure

Exposes an existing Nano API or Web application publicly — Kubernetes-internal (`ClusterIP`)
services aren't reachable from outside the cluster by default; this wires the Gateway API
routing, TLS, and DNS pieces needed to reach it at a real public hostname. Read AGENTS.md's
`##### Https` section (under `#### Hosting`) first for the config table; this skill is the
surrounding infrastructure.

**Ask whether Availability Check should be added too.** Once an app is publicly reachable,
continuous uptime monitoring (`nano-add-availability-check`) becomes possible for the first
time — it specifically requires this. Ask the user up front rather than assuming either way.

## Before making any change, determine

1. **Application type.** API or Web only — Console apps have no HTTP surface to expose. Confirm
   via `Program.cs`.
2. **Is the app already publicly exposed?** Check for `.kubernetes/httproute-80.yaml`/
   `httproute-443.yaml`. If present, say so and stop.
3. **Does the user also want Availability Check?** Ask explicitly if not already stated — see
   above. If yes, run `nano-add-availability-check` after this skill completes (it depends on
   the hostname/HTTPS wiring this skill adds).
4. **Sub-domain name.** Ask what public sub-domain this app should be reachable at (e.g. `papi`,
   `nano`) — becomes `SUB_DOMAIN_NAME`, combined with every DNS zone configured in the target
   Azure resource group at deploy time (an app can end up reachable under several zones/domains
   at once, not just one).

## appsettings.json

Base `appsettings.json`: no change — HTTP stays exposed as-is (`App:Hosting:Http`, unaffected).

`appsettings.Development.json` — HTTPS is a **local-development-only** concern; `Staging`/
`Production` TLS terminates at the gateway/cert-manager level, not via this config (AGENTS.md's
own note):

```json
"App": {
  "Hosting": {
    "Http": { "UseHttpsRedirection": true },
    "Https": {
      "Ports": [4443],
      "Certificate": {
        "Path": "/root/.dotnet/https/localhost.pfx",
        "Password": "password"
      },
      "UseHttpsRequired": true
    }
  }
}
```

Avoid port `443` here specifically — AGENTS.md notes it can trigger security warnings inside
Kubernetes; `4443` (or similar) is the established convention. A self-signed
`localhost.pfx`/password pair is needed for the certificate path to resolve locally — check
whether the project already has one (`dotnet dev-certs https` can generate one if not).

## docker-compose.yml (local Development)

Map the HTTPS port and certificate volume onto the app's own service:

```yaml
services:
  {service-name}:
    ports:
      - 4443:4443
    volumes:
      - ../:/root/.dotnet/https
```

## Kubernetes

Two new files. `service.yaml` itself needs **no change** — it keeps exposing the plain HTTP port;
the Gateway routes HTTPS traffic to it and terminates TLS itself.

`.kubernetes/httproute-80.yaml` (redirects HTTP → HTTPS):

```yaml
apiVersion: gateway.networking.k8s.io/v1
kind: HTTPRoute
metadata:
  name: %SERVICE_NAME%-route-80
  namespace: %KUBERNETES_NAMESPACE%
spec:
  parentRefs:
    - name: %GATEWAY_NAME%
      sectionName: http
  hostnames:
%ROUTE_HOST_NAMES%
  rules:
    - filters:
        - type: RequestRedirect
          requestRedirect:
            scheme: https
            statusCode: 301
```

`.kubernetes/httproute-443.yaml` (the real route to the app):

```yaml
apiVersion: gateway.networking.k8s.io/v1
kind: HTTPRoute
metadata:
  name: %SERVICE_NAME%-route-443
  namespace: %KUBERNETES_NAMESPACE%
spec:
  parentRefs:
    - name: %GATEWAY_NAME%
  hostnames:
%ROUTE_HOST_NAMES%
  rules:
    - matches:
        - path:
            type: PathPrefix
            value: /
      backendRefs:
        - name: %SERVICE_NAME%
          port: 8080
```

`%ROUTE_HOST_NAMES%` and `%GATEWAY_NAME%` are **not** static env vars — they're derived at
deploy time (see below), one hostname line per DNS zone found in the target Azure resource
group, so an app can be reachable under multiple domains without per-domain config.

## GitHub Actions

1. **Workflow env vars**:
   ```yaml
   SUB_DOMAIN_NAME: papi
   AZURE_GROUP_DNS: ${{ vars.AZURE_RESOURCE_GROUP_DNS }}
   ```
2. **Derive the hostnames and gateway**, in the `Kubernetes Deploy` step, before any manifest is
   applied:
   ```powershell
   $zoneNames = az network dns zone list -g $env:AZURE_GROUP_DNS --query "[].name" -o json | ConvertFrom-Json

   $env:ROUTE_HOST_NAMES = (
       $zoneNames | ForEach-Object {
           "  - $env:SUB_DOMAIN_NAME.$_"
       }
   ) -join "`n"

   $env:GATEWAY_NAME = kubectl get gateway -n $env:KUBERNETES_NAMESPACE -o jsonpath='{.items[0].metadata.name}'
   ```
3. Apply `httproute-80.yaml`/`httproute-443.yaml` in `Kubernetes Deploy`, same
   `Get-Content | ExpandEnvironmentVariables | kubectl apply` pattern as every other manifest.
   This assumes a `Gateway` resource already exists in the target namespace — provisioning the
   Gateway itself is a one-time, cluster-level concern outside this skill's scope; tell the user
   if `kubectl get gateway` would come back empty rather than assuming it's there.

## After making the change

- Show the user every file touched, grouped by concern (local dev, Kubernetes, CI).
- If step 3 confirmed Availability Check is also wanted, hand off to
  `nano-add-availability-check` next rather than leaving it unaddressed.
- Mention `AGENTS.md`'s `#### Http Policy Headers` (CORS, HSTS, CSP, security headers) as a
  related but separate concern worth considering for a publicly-reachable app — this skill
  doesn't configure it, only the routing/TLS/DNS layer.
