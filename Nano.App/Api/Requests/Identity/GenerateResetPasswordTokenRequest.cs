using Nano.Security.Models;
using System;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class GenerateResetPasswordTokenRequest : GenerateResetPasswordTokenRequest<Guid>;

/// <inheritdoc />
public class GenerateResetPasswordTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Reset Password Token.
    /// </summary>
    public virtual GenerateResetPasswordToken<TIdentity> ResetPasswordToken { get; set; } = new();

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