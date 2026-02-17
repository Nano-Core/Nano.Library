using System;

namespace Nano.Data.Abstractions.Identity.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a user is unauthorized to perform an action.
/// </summary>
public class UnauthorizedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
    /// </summary>
    public UnauthorizedException()
        : this("")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}