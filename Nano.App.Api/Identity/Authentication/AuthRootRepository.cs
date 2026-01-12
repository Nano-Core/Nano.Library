using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nano.App.Api.Identity.Authentication.Abstractions;
using Nano.App.ApiClient.Models.Identity;
using Nano.App.Config;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Consts;

namespace Nano.App.Api.Identity.Authentication;

/// <inheritdoc />
public class AuthRootRepository : IAuthRootRepository
{
    private readonly LogInRootOptions options;
    private readonly IAuthJwtRepository authJwtRepository;

    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="authJwtRepository"></param>
    public AuthRootRepository(LogInRootOptions options, IAuthJwtRepository authJwtRepository)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.authJwtRepository = authJwtRepository ?? throw new ArgumentNullException(nameof(authJwtRepository));
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInRootAsync(LogInRoot logInRoot)
    {
        if (logInRoot == null)
        {
            throw new ArgumentNullException(nameof(logInRoot));
        }

        await Task.CompletedTask;

        if (logInRoot.Username != this.options.Username)
        {
            var identityError = new IdentityErrorDescriber().InvalidUserName(logInRoot.Username);

            throw new AggregateException(identityError.Description);
        }

        if (logInRoot.Password != this.options.Password)
        {
            var identityError = new IdentityErrorDescriber().PasswordMismatch();

            throw new AggregateException(identityError.Description);
        }

        var accessToken = this.authJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                UserId = null,
                UserEmail = null,
                UserName = this.options.Username,
                Claims =
                [
                    new Claim(ClaimTypes.Role, BuiltInUserRoles.ADMINISTRATOR)
                ]
            });

        return accessToken;
    }
}