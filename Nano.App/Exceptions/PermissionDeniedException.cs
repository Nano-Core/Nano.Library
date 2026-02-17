using System;

namespace Nano.App.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a user does not have the required permissions to perform an action.
/// </summary>
public class PermissionDeniedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionDeniedException"/> class.
    /// </summary>
    public PermissionDeniedException()
        : this("")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionDeniedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PermissionDeniedException(string message)
        : base(message)
    {
    }
}