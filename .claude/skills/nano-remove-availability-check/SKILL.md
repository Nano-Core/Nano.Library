---
name: nano-remove-availability-check
description: Remove availability monitoring from a Nano API or Web application - removes the CI step that creates/maintains the Azure Application Insights ping test and alert. Use when the user asks to remove availability monitoring, an uptime check, or a ping test from a Nano application.
---

# Nano remove availability check

Removes availability monitoring from an existing Nano application — the counterpart to
`nano-add-availability-check`.

## Before making any change, determine

1. **Is Availability Check currently configured?** Check the workflow for an "Add Availability
   Check" step. If absent, say so and stop.

## GitHub Actions

Remove the "Add Availability Check" step entirely, and `AZURE_GROUP_LOGS` if nothing else in the
workflow still references it.

⚠ This does **not** delete the underlying Azure resources (the Application Insights web test and
its metric alert) — those are real Azure resources this step only creates/maintains
idempotently, it never owned their lifecycle for deletion. Removing the workflow step just stops
maintaining them going forward; the ping test keeps running (and could keep alerting) until
someone deletes it directly in Azure (`az monitor app-insights web-test delete` / removing the
alert resource). Say this explicitly rather than implying the monitoring stops the moment the
workflow step is removed.

## After making the change

- Show the user the workflow change.
- Restate the ⚠ above — the Azure-side resources need manual cleanup if the user actually wants
  the monitoring (and its alerts) to stop, not just future deploys to skip maintaining it.
- If step 1 stopped the skill early, that's the whole response.
