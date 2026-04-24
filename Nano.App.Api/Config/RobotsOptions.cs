using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring robots meta tags.
/// </summary>
public class RobotsOptions
{
    /// <summary>
    /// Instructs search engines to not index the page.
    /// </summary>
    [Required]
    public virtual bool UseNoIndex { get; set; } = false;

    /// <summary>
    /// Instructs search engines to not follow links on the page.
    /// </summary>
    [Required]
    public virtual bool UseNoFollow { get; set; } = false;

    /// <summary>
    /// Instructs search engines to not display a snippet for the page in search results.
    /// </summary>
    [Required]
    public virtual bool UseNoSnippet { get; set; } = false;

    /// <summary>
    /// Instructs search engines to not offer a cached version of the page.
    /// </summary>
    [Required]
    public virtual bool UseNoArchive { get; set; } = false;

    /// <summary>
    /// Instructs search engines to not use Open Directory Project info for title/snippet.
    /// </summary>
    [Required]
    public virtual bool UseNoOdp { get; set; } = false;

    /// <summary>
    /// Instructs search engines to not offer translation of the page (Google only).
    /// </summary>
    [Required]
    public virtual bool UseNoTranslate { get; set; } = false;

    /// <summary>
    /// Instructs search engines to not index images on the page (Google only).
    /// </summary>
    [Required]
    public virtual bool UseNoImageIndex { get; set; } = false;
}