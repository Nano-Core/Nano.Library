using System;

namespace Nano.Data.Abstractions.Identity.Exceptions;

/// <summary>
/// Identity Exception.
/// </summary>
public class IdentityException : Exception
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message.</param>
    public IdentityException(string message)
        : base(message)
    {
    }
}