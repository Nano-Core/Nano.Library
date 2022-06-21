namespace Nano.Security.Exceptions;

/// <summary>
/// Unauthorized Set Password Exception.
/// </summary>
public class UnauthorizedSetPasswordException : UnauthorizedException
{
    private const string CODE = "SetPassword";

    /// <inheritdoc />
    public UnauthorizedSetPasswordException()
        : base(UnauthorizedSetPasswordException.CODE)
    {

    }
}