using System;

namespace Nano.App.Exceptions;

/// <summary>
/// Represents a bad request error.
/// </summary>
public class BadRequestException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">A message describing the bad request.</param>
    public BadRequestException(string message)
        : base(message)
    {
    }
}