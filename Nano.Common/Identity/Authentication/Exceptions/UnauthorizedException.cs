using System;

namespace Nano.Common.Identity.Authentication.Exceptions;

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