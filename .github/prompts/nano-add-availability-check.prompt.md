---
mode: agent
description: Add continuous uptime monitoring for a publicly-exposed Nano API or Web application - creates an Azure Application Insights availability (ping) test against the app's /healthz endpoint across every DNS zone, plus a metric alert rule. Requires the app already be publicly exposed. Use when the user asks to add availability monitoring, an uptime check, or a ping test to a Nano application.
---

# Nano add availability check

Adds continuous availability monitoring for an existing, publicly-exposed Nano API or Web
application, via an Azure Application Insights ping test against `/healthz` plus a metric alert.
This is CI/infrastructure only - nothing in the application itself changes.

## Before making any change, determine

1. **Is the app publicly exposed?** Check for `.kubernetes/httproute-80.yaml`/`httproute-443.yaml`
   (`nano-add-public-exposure`). **Required** - the ping test hits a real public HTTPS URL; there
   is nothing for it to test otherwise. If not present, stop and point the user at
   `nano-add-public-exposure` first - don't wire a check against a URL that doesn't resolve.
2. **Is Health Checks enabled?** The test targets `/healthz` specifically, matching on the
   `"status":"unhealthy"` string to detect failure - check for `App:HealthCheck`
   (`nano-add-health-checks`). If absent, `/healthz` doesn't exist at all; stop and point the
   user at that skill too.
3. **Is Availability Check already configured?** Check the workflow for an "Add Availability
   Check" step. If present, say so and stop.
4. **`SUB_DOMAIN_NAME` must already be set** (from `nano-add-public-exposure`) - this step reuses
   it, doesn't define it. Confirm it's there rather than assuming.

## GitHub Actions

1. **Workflow env var**:
   ```yaml
   AZURE_GROUP_LOGS: ${{ vars.AZURE_RESOURCE_GROUP_LOGS }}
   ```
2. **"Add Availability Check" step**, placed at the end of the pipeline (after deployment).
   Idempotent - creates the web test and alert only if they don't already exist for each DNS
   zone, safe to run every deploy:
   ```yaml
   - name: Add Availability Check
     shell: pwsh
     run: |
       $env:AZURE_LOCATION = az monitor log-analytics workspace list -g $env:AZURE_GROUP_LOGS --query [0].location -o tsv;
       $env:APPLICATION_INSIGHT_ID = az monitor app-insights component show -g $env:AZURE_GROUP_LOGS --query [0].id -o tsv;
       $env:HIDDEN_LINK = 'hidden-link:' + $env:APPLICATION_INSIGHT_ID + '=Resource';

       $zoneNames = az network dns zone list -g $env:AZURE_GROUP_DNS --query "[].name" -o json | ConvertFrom-Json

       foreach ($zoneName in $zoneNames)
       {
           $env:WEB_TEST_NAME = $env:SERVICE_NAME + '-availability-' + $env:ASPNETCORE_ENVIRONMENT.ToLower() + '-' + $env:SUB_DOMAIN_NAME  + '-' + ($zoneName.TrimEnd('.') -replace '\.', '-')

           az monitor app-insights web-test show -g $env:AZURE_GROUP_LOGS -n $env:WEB_TEST_NAME --query id -o tsv 2>$null

           if ($LastExitCode -ne 0)
           {
               az monitor app-insights web-test create `
                   -n $env:WEB_TEST_NAME `
                   --defined-web-test-name $env:WEB_TEST_NAME `
                   -g $env:AZURE_GROUP_LOGS `
                   -l $env:AZURE_LOCATION `
                   --kind ping `
                   --web-test-kind standard `
                   --frequency 300 `
                   --enabled true `
                   --retry-enabled true `
                   --ssl-check true `
                   --ssl-lifetime-check 30 `
                   --http-verb GET `
                   --request-url https://$env:SUB_DOMAIN_NAME.$zoneName/healthz `
                   --expected-status-code 200 `
                   --content-validation content-match='"status":"unhealthy"' ignore-case=true pass-if-text-found=false `
                   --tags $env:HIDDEN_LINK `
                   --locations Id='us-ca-sjc-azr' `
                   --locations Id='us-va-ash-azr' `
                   --locations Id='emea-gb-db3-azr' `
                   --locations Id='emea-nl-ams-azr' `
                   --locations Id='apac-hk-hkn-azr';

               if ($LastExitCode -ne 0)
               {
                   throw "error";
               }
           }

           $env:WEB_TEST_ALERT_NAME = $env:WEB_TEST_NAME + "-alert";

           az resource show -g $env:AZURE_GROUP_LOGS -n $env:WEB_TEST_ALERT_NAME --query id -o tsv 2>$null;

           if ($LastExitCode -ne 0)
           {
               $env:WEB_TEST_ID = az monitor app-insights web-test show -g $env:AZURE_GROUP_LOGS -n $env:WEB_TEST_NAME --query id -o tsv;
               $env:ACTION_GROUP_ID = az monitor action-group list -g $env:AZURE_GROUP_LOGS --query [0].id -o tsv;

               $alertRuleProperties = @{
                   severity            = 1
                   enabled              = $true
                   scopes               = @($env:WEB_TEST_ID, $env:APPLICATION_INSIGHT_ID)
                   evaluationFrequency  = "PT1M"
                   windowSize           = "PT5M"
                   criteria             = @{
                       "odata.type"        = "Microsoft.Azure.Monitor.WebtestLocationAvailabilityCriteria"
                       webTestId           = $env:WEB_TEST_ID
                       componentId         = $env:APPLICATION_INSIGHT_ID
                       failedLocationCount = 2
                   }
                   actions = @(
                       @{ actionGroupId = $env:ACTION_GROUP_ID }
                   )
               }

               $json = $alertRuleProperties | ConvertTo-Json -Depth 10
               [System.IO.File]::WriteAllText("$PWD/alert.json", $json, [System.Text.UTF8Encoding]::new($false))

               az resource create `
                   -g $env:AZURE_GROUP_LOGS `
                   -n $env:WEB_TEST_ALERT_NAME `
                   -l global `
                   --resource-type "Microsoft.Insights/metricAlerts" `
                   -p '@alert.json';

               if ($LastExitCode -ne 0)
               {
                   throw "error";
               }
           }
       }
   ```

This creates one ping test **per DNS zone** the app is reachable under (matching
`nano-add-public-exposure`'s multi-zone hostname derivation), each pinged from 5 global Azure
locations every 5 minutes, alerting when at least 2 locations report failure within a 5-minute
window. `az monitor log-analytics workspace list`/`az monitor app-insights component show`/
`az monitor action-group list` all assume a Log Analytics workspace, Application Insights
component, and action group already exist in `AZURE_GROUP_LOGS` - one-time, cluster/subscription-
level prerequisites outside this skill's scope; tell the user if any of those would come back
empty rather than assuming they're provisioned.

## After making the change

- Show the user the workflow changes.
- If step 1 or step 2 stopped the skill early, that's the whole response - don't wire a check
  against a URL or endpoint that doesn't exist yet.
- Mention that the actual web test/alert resources are created on the **next deploy run**, not
  by editing the workflow file alone.
