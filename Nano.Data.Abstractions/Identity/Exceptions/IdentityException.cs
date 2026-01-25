using System;

namespace Nano.Data.Abstractions.Identity.Exceptions;


/// <summary>
/// Represents an exception that is thrown for identity-related errors.
/// </summary>
public class IdentityException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public IdentityException(string message)
        : base(message)
    {
    }
}