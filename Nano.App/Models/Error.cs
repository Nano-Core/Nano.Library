namespace Nano.Models;

/// <summary>
/// Error.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary.
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    /// Exceptions.
    /// </summary>
    public string[] Exceptions { get; set; } = [];

    /// <summary>
    /// Is Translated.
    /// </summary>
    public bool IsTranslated { get; set; }

    /// <summary>
    /// Is Coded.
    /// </summary>
    public bool IsCoded { get; set; }
}