namespace Nano.Security.Exceptions;

/// <summary>
/// Unauthorized Deactivated Exception.
/// </summary>
public class UnauthorizedDeactivatedException : UnauthorizedException
{
    private const string CODE = "Deactivated";

    /// <inheritdoc />
    public UnauthorizedDeactivatedException()
        : base(UnauthorizedDeactivatedException.CODE)
    {
    }
}