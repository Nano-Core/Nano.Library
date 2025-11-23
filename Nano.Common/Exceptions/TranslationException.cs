using System;

namespace Nano.Common.Exceptions;

/// <summary>
/// Translation Exception.
/// </summary>
public class TranslationException : Exception
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message.</param>
    public TranslationException(string message)
        : base(message)
    {
    }
}