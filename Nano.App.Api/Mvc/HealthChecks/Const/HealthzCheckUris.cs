namespace Nano.App.Api.Mvc.HealthChecks.Const;

internal static class HealthzCheckUris
{
    internal static string Path => "/healthz";

    internal static string UiPath => $"{Path}-ui";

    internal static string ApiPath => $"{Path}-api";

    internal static string RexPath => $"{Path}-rex";

    internal static string WebHooksPath => $"{Path}-hooks";
}