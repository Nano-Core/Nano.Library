using Nano.App.ApiClient.Models.Identity;
using Nano.App.Config;
using Nano.App.Web.Identity.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Exceptions;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication;

namespace Nano.App.Web.Identity.Authentication;

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

        if (this.options.Username == null)
        {
            throw new NullReferenceException(nameof(this.options.Username));
        }

        if (this.options.Password == null)
        {
            throw new NullReferenceException(nameof(this.options.Password));
        }

        if (logInRoot.Username != this.options.Username || logInRoot.Password != this.options.Password)
        {
            throw new UnauthorizedException($"The user: {logInRoot.Username} failed with incorrect username or password.");
        }

        var tokenData = new GenerateJwtToken
        {
            UserId = null,
            UserName = this.options.Username,
            Claims =
            [
                new Claim(ClaimTypes.Role, BuiltInUserRoles.ADMINISTRATOR)
            ]
        };

        var accessToken = this.authJwtRepository
            .GenerateJwtToken(tokenData);

        return accessToken;
    }
}