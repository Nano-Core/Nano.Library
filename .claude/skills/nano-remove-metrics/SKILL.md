---
name: nano-remove-metrics
description: Remove Nano's built-in OpenTelemetry metrics (App:Metrics) from a Nano API or Web application - removes the config and the Kubernetes ServiceMonitor. Use when the user asks to remove metrics, Prometheus, OpenTelemetry, or the /metrics endpoint from a Nano API or Web application.
---

# Nano remove metrics

Removes Nano's `/metrics` endpoint from an existing Nano API or Web application — the counterpart
to `nano-add-metrics`.

## Before making any change, determine

1. **Is `App:Metrics` currently configured?** Check the base `appsettings.json`. If absent, say
   so and stop.
2. **What depends on it?** Nothing in the app itself — Metrics has no dependents (confirmed
   against `nano-add-metrics`'s own verification that it's independent of Health Checks, and
   nothing else in the framework reads `App:Metrics`). The only external dependent is whatever
   scrapes it — if Prometheus/Grafana dashboards are actively built on this endpoint, removing it
   silently breaks that monitoring, with no error on the app side. Ask before removing if that
   seems likely, since this app has no way to know it's being scraped.

## Kubernetes

Delete `.kubernetes/service-monitor.yaml`, and remove its apply block from the `Kubernetes
Deploy` workflow step.

## appsettings.json

Remove `App:Metrics` from the base `appsettings.json`.

## After making the change

- Show the user every file touched/deleted.
- If step 2's external-scraping concern applies, restate it — removing this leaves no error
  anywhere in the app, only a monitoring dashboard that goes quiet.
- If step 1 stopped the skill early, that's the whole response.
