using System;
using System.Globalization;

namespace Nano.App.Exceptions;

/// <summary>
/// Represents an error where the exception message is translated according to the <see cref="CultureInfo.CurrentCulture"/>.
/// </summary>
public class TranslationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">A translated message describing the error.</param>
    public TranslationException(string message)
        : base(message)
    {
    }
}