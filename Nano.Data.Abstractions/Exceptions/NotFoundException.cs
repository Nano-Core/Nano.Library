using System;

namespace Nano.Data.Abstractions.Exceptions;

/// <summary>
/// Represents an exception that is thrown for null errors.
/// </summary>
public class NotFoundException : NullReferenceException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NotFoundException(string message)
        : base(message)
    {
    }
}