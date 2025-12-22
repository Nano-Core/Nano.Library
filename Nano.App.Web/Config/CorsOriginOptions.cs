using Nano.App.Web.Config.Enums;

namespace Nano.App.Web.Config;

/// <summary>
/// Cors Origin Options.
/// </summary>
public class CorsOriginOptions
{
    /// <summary>
    /// Embedder Policy.
    /// </summary>
    public virtual CrossOriginEmbedderPolicy? EmbedderPolicy { get; set; }

    /// <summary>
    /// Opener Policy.
    /// </summary>
    public virtual CrossOriginOpenerPolicy? OpenerPolicy { get; set; }

    /// <summary>
    /// Resource Policy.
    /// </summary>
    public virtual CrossOriginResourcePolicy? ResourcePolicy { get; set; }
}