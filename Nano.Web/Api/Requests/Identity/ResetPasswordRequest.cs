using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity;

/// <inheritdoc />
public class ResetPasswordRequest : BaseRequestPost
{
    /// <summary>
    /// Reset Password.
    /// </summary>
    public virtual ResetPassword ResetPassword { get; set; } = new();

    /// <inheritdoc />
    public ResetPasswordRequest()
    {
        this.Action = "password/reset";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ResetPassword;
    }
}