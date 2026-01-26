using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Request to generate a reset password token.
/// </summary>
public class GenerateResetPasswordTokenRequest : BaseRequestPost
{
    /// <summary>
    /// The reset password token information.
    /// </summary>
    public virtual GenerateResetPasswordToken ResetPasswordToken { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="GenerateResetPasswordTokenRequest"/>.
    /// Sets the action to "password/reset/token".
    /// </summary>
    public GenerateResetPasswordTokenRequest()
    {
        this.Action = "password/reset/token";
    }

    /// <summary>
    /// Gets the request body containing the reset password token.
    /// </summary>
    public override object GetBody()
    {
        return this.ResetPasswordToken;
    }
}