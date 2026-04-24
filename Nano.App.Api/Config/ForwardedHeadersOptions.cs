using Microsoft.AspNetCore.HttpOverrides;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for forwarded headers (used when behind a proxy).
/// </summary>
public class ForwardedHeadersOptions
{
    /// <summary>
    /// Defines the headers that should forwarded.
    /// Default to 'All' headers.
    /// </summary>
    public virtual ForwardedHeaders Headers { get; set; } = ForwardedHeaders.All;

    /// <summary>
    /// Specifies that forwarded headers will only be processed if the set of headers is complete for that hop.
    /// Default to <c>true</c>.
    /// </summary>
    public virtual bool RequireHeaderSymmetry { get; set; } = true;
}