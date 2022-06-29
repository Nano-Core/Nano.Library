using System;

namespace Nano.Security.Exceptions;

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