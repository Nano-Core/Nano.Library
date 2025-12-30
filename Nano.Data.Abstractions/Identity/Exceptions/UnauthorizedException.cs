using System;

namespace Nano.Data.Abstractions.Identity.Exceptions;

/// <summary>
/// Unauthorized Exception.
/// </summary>
public class UnauthorizedException : Exception
{
    /// <inheritdoc />
    public UnauthorizedException()
    {
    }

    /// <inheritdoc />
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}