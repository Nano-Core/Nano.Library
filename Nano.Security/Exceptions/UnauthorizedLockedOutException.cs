namespace Nano.Security.Exceptions;

/// <summary>
/// Unauthorized Locked Out Exception.
/// </summary>
public class UnauthorizedLockedOutException : UnauthorizedException
{
    private const string CODE = "LockedOut";

    /// <inheritdoc />
    public UnauthorizedLockedOutException()
        : base(UnauthorizedLockedOutException.CODE)
    {

    }
}