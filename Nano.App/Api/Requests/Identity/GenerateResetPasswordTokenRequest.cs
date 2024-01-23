using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class GenerateResetPasswordTokenRequest : BaseRequestPost
{
    /// <summary>
    /// Reset Password Token.
    /// </summary>
    public virtual GenerateResetPasswordToken ResetPasswordToken { get; set; } = new();

    /// <inheritdoc />
    public GenerateResetPasswordTokenRequest()
    {
        this.Action = "password/reset/token";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ResetPasswordToken;
    }
}