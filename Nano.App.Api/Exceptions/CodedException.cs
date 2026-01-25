using System;

namespace Nano.App.Exceptions;

/// <summary>
/// Represents an exception with a specific code or identifier.
/// </summary>
public class CodedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodedException"/> class with a specified error message.
    /// </summary>
    /// <param name="code">A code identifying the error.</param>
    public CodedException(string code)
        : base(code)
    {
    }
}