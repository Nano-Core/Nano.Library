---
mode: agent
description: Enable Nano's built-in OpenTelemetry metrics (App:Metrics) on a Nano API or Web application - adds the config and the Kubernetes ServiceMonitor for Prometheus scraping. Use when the user asks to add metrics, Prometheus, OpenTelemetry, or a /metrics endpoint to a Nano API or Web application.
---

# Nano add metrics

Enables Nano's built-in `/metrics` endpoint (Prometheus-compatible, via OpenTelemetry) on an
existing Nano API or Web application. Read AGENTS.md's `#### Metrics (OpenTelemetry)` section
first; this skill is just the wiring.

**API/Web only** - same reasoning as Health Checks: Console apps have no HTTP pipeline, so
there's no `/metrics` to expose.

**Independent of Health Checks.** Verified directly against the registration code
(`AddNanoMetrics`/`UseNanoMetrics`) - Metrics has no dependency on `App:HealthCheck` in either
direction. Enable it on its own; don't add Health Checks "because Metrics needs it" - it doesn't.

## Before making any change, determine

1. **Application type.** Confirm API or Web via `Program.cs`. Stop for Console.
2. **Is `App:Metrics` already configured?** Check the base `appsettings.json`. If present, check
   whether `.kubernetes/service-monitor.yaml` already exists - same "don't leave it half-wired"
   concern as Health Checks, though less severe here since nothing actively breaks without the
   `ServiceMonitor` (Prometheus just won't discover the endpoint to scrape it).
3. **`azmonitoring.coreos.com/v1`, not `monitoring.coreos.com/v1`.** The K8s manifest below
   deliberately targets **Azure Managed Prometheus**'s `ServiceMonitor` CRD group - the AKS add-on
   Nano's own `Nano.App.Api` README documents this against ("these metrics can be scraped by
   Azure Managed Prometheus and visualized in Grafana dashboards") - not the community Prometheus
   Operator's CRD (`monitoring.coreos.com/v1`) that generic Kubernetes/Prometheus docs assume.
   Every app in this solution targets AKS, so this isn't a cluster-dependent uncertainty to flag -
   it's the correct, fixed `apiVersion` for this ecosystem. Don't "correct" it to
   `monitoring.coreos.com/v1` even if that's what's more commonly seen elsewhere.

## appsettings.json

Add to the base `appsettings.json`, sibling of `App:Version`/`App:Hosting`:

```json
"App": { "Metrics": { } }
```

No options - presence alone enables it. Same in every environment.

## Kubernetes

`.kubernetes/service-monitor.yaml` (new file):

```yaml
apiVersion: azmonitoring.coreos.com/v1
kind: ServiceMonitor
metadata:
  name: %SERVICE_NAME%-monitor
  namespace: %KUBERNETES_NAMESPACE%
spec:
  selector:
    matchLabels:
      app: %SERVICE_NAME%
  endpoints:
    - port: http
      path: /metrics
      interval: 1m
```

Apply it in the `Kubernetes Deploy` workflow step, same `Get-Content | ExpandEnvironmentVariables
| kubectl apply` pattern as every other manifest. Also add `.kubernetes\service-monitor.yaml =
.kubernetes\service-monitor.yaml` to `{name}.sln`'s `.kubernetes` `SolutionItems` block (see
AGENTS.md's Solution Structure note) - new files under `.kubernetes/` don't show up in Visual
Studio's Solution Explorer otherwise.

## After making the change

- Show the user every file touched.
