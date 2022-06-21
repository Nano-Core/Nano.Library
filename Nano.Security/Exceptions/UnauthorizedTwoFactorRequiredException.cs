namespace Nano.Security.Exceptions;

/// <summary>
/// Unauthorized Two-Factor Required Exception.
/// </summary>
public class UnauthorizedTwoFactorRequiredException : UnauthorizedException
{
    private const string CODE = "TwoFactorAuthenticationRequired";

    /// <inheritdoc />
    public UnauthorizedTwoFactorRequiredException()
        : base(UnauthorizedTwoFactorRequiredException.CODE)
    {

    }
}