namespace Nano.App.Exceptions;

/// <summary>
/// Bad Request Exception.
/// </summary>
public class BadRequestException : TranslationException
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message.</param>
    public BadRequestException(string message)
        : base(message)
    {
    }
}