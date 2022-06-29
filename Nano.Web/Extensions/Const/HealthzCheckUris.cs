namespace Nano.Web.Extensions.Const;

/// <summary>
/// Healthz Check Uris.
/// </summary>
internal static class HealthzCheckUris
{
    /// <summary>
    /// Path.
    /// </summary>
    internal static string Path => "/healthz";

    /// <summary>
    /// Ui Path.
    /// </summary>
    internal static string UiPath => $"{HealthzCheckUris.Path}-ui";

    /// <summary>
    /// Api Path.
    /// </summary>
    internal static string ApiPath => $"{HealthzCheckUris.Path}-api";

    /// <summary>
    /// Rex Path.
    /// </summary>
    internal static string RexPath => $"{HealthzCheckUris.Path}-rex";

    /// <summary>
    /// WebHooks Path.
    /// </summary>
    internal static string WebHooksPath => $"{HealthzCheckUris.Path}-hooks";

}