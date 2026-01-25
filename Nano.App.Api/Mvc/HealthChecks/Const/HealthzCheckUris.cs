namespace Nano.App.Api.Mvc.HealthChecks.Const;

internal static class HealthzCheckUris
{
    internal static string Path => "/healthz";

    internal static string UiPath => $"{HealthzCheckUris.Path}-ui";

    internal static string ApiPath => $"{HealthzCheckUris.Path}-api";

    internal static string RexPath => $"{HealthzCheckUris.Path}-rex";

    internal static string WebHooksPath => $"{HealthzCheckUris.Path}-hooks";
}