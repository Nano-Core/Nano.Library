using System;

namespace Nano.Common.Exceptions;

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