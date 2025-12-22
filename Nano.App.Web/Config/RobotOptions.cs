using System.ComponentModel.DataAnnotations;

namespace Nano.App.Web.Config;

/// <summary>
/// Robot Options.
/// </summary>
public class RobotOptions
{
    /// <summary>
    /// Use No Index.
    /// Instructs search engines to not index the page
    /// </summary>
    [Required]
    public virtual bool UseNoIndex { get; set; } = false;

    /// <summary>
    /// Use No Follow
    /// Instructs search engines to not follow links on the page
    /// </summary>
    [Required]
    public virtual bool UseNoFollow { get; set; } = false;

    /// <summary>
    /// Use No Snippet.
    /// Instructs search engines to not display a snippet for the page in search results
    /// </summary>
    [Required]
    public virtual bool UseNoSnippet { get; set; } = false;

    /// <summary>
    /// Use No Archive.
    /// Instructs search engines to not offer a cached version of the page in search results
    /// </summary>
    [Required]
    public virtual bool UseNoArchive { get; set; } = false;

    /// <summary>
    /// Use No ODP.
    /// Instructs search engines to not use information from the Open Directory Project for the page’s title or snippet
    /// </summary>
    [Required]
    public virtual bool UseNoOdp { get; set; } = false;

    /// <summary>
    /// Use No Translate - Instructs search engines to not offer translation of the page in search results (Google only)
    /// </summary>
    [Required]
    public virtual bool UseNoTranslate { get; set; } = false;

    /// <summary>
    /// Use No Image Index.
    /// Instructs search engines to not index images on the page (Google only)
    /// </summary>
    [Required]
    public virtual bool UseNoImageIndex { get; set; } = false;
}