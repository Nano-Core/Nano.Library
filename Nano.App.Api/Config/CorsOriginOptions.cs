using Nano.App.Api.Config.Enums;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring CORS origins.
/// </summary>
public class CorsOriginOptions
{
    /// <summary>
    /// Cross-Origin Embedder Policy.
    /// </summary>
    public virtual CrossOriginEmbedderPolicy? EmbedderPolicy { get; set; }

    /// <summary>
    /// Cross-Origin Opener Policy.
    /// </summary>
    public virtual CrossOriginOpenerPolicy? OpenerPolicy { get; set; }

    /// <summary>
    /// Cross-Origin Resource Policy.
    /// </summary>
    public virtual CrossOriginResourcePolicy? ResourcePolicy { get; set; }
}