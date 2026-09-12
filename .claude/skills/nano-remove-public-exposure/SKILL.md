---
name: nano-remove-public-exposure
description: Remove public exposure from a Nano API or Web application - removes the HTTPS hosting config, Kubernetes HTTPRoute resources, and the CI hostname-derivation step. Cascades to removing Availability Check first, since that depends entirely on the app being publicly reachable. Use when the user asks to remove public exposure, take a Nano application private, or remove an HTTPRoute.
---

# Nano remove public exposure

Removes public reachability from an existing Nano API or Web application — the counterpart to
`nano-add-public-exposure`.

## Before making any change, determine

1. **Is the app currently publicly exposed?** Check for `.kubernetes/httproute-80.yaml`/
   `httproute-443.yaml`. If neither exists, say so and stop.
2. **Is Availability Check configured?** Check the workflow for an "Add Availability Check" step.
   **If so, this must be removed first, not left behind** — per `nano-add-availability-check`,
   its ping test hits `https://$SUB_DOMAIN_NAME.$zoneName/healthz`, which stops resolving the
   moment the `HTTPRoute`s are gone; leaving the check in place means it starts firing failure
   alerts for an app that was deliberately taken private, not one that's actually down. Run
   `nano-remove-availability-check` first, then continue with this skill — don't ask, this
   cascade is the expected behavior, but tell the user it happened.

## Kubernetes

Delete `.kubernetes/httproute-80.yaml` and `.kubernetes/httproute-443.yaml`. `service.yaml` is
unaffected — it never needed to change to add exposure, so it doesn't need to change to remove
it either.

## GitHub Actions

Remove the `SUB_DOMAIN_NAME`/`AZURE_GROUP_DNS` env vars (unless Availability Check's own removal
already handled `AZURE_GROUP_DNS` — don't remove it twice or assume it's still needed elsewhere
without checking), the `$env:ROUTE_HOST_NAMES`/`$env:GATEWAY_NAME` derivation step, and the
`httproute-80.yaml`/`httproute-443.yaml` apply blocks from `Kubernetes Deploy`.

## appsettings.json / docker-compose.yml

Remove the `App:Hosting:Https`/`UseHttpsRedirection` block from `appsettings.Development.json`,
and the HTTPS port mapping + certificate volume from `docker-compose.yml`. Leave the base
`appsettings.json` alone — it was never changed by the add skill (HTTP stays exposed regardless).

## After making the change

- Show the user every file touched/deleted.
- If step 2's cascade applied, restate clearly that Availability Check was removed as a
  consequence, not a separate request.
- If step 1 stopped the skill early, that's the whole response.
