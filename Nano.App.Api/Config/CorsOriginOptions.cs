using Nano.App.Api.Config.Enums;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring CORS origins.
/// </summary>
public class CorsOriginOptions
{
    /// <summary>
    /// The HTTP Cross-Origin-Embedder-Policy (COEP) response header configures the current document's policy for loading and embedding cross-origin resources.
    /// </summary>
    public virtual CrossOriginEmbedderPolicy? EmbedderPolicy { get; set; }

    /// <summary>
    /// The HTTP Cross-Origin-Opener-Policy (COOP) response header allows a website to control whether a new top-level document, opened using Window.open() or
    /// by navigating to a new page, is opened in the same browsing context group (BCG) or in a new browsing context group.
    /// </summary>
    public virtual CrossOriginOpenerPolicy? OpenerPolicy { get; set; }

    /// <summary>
    /// The HTTP Cross-Origin-Resource-Policy response header (CORP) indicates that the browser should block no-cors cross-origin or cross-site requests to the given resource.
    /// </summary>
    public virtual CrossOriginResourcePolicy? ResourcePolicy { get; set; }
}