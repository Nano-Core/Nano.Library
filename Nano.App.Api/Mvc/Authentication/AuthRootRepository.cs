using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.App.ApiClient.Requests.Auth.Models;
using Nano.App.Config;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Exceptions;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc />
public class AuthRootRepository : IAuthRootRepository
{
    private readonly LogInRootOptions options;
    private readonly IAuthJwtRepository authJwtRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthRootRepository"/> class.
    /// </summary>
    /// <param name="options">The <see cref="LogInRootOptions"/> configuration.</param>
    /// <param name="authJwtRepository">The JWT authentication repository used for issuing tokens.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> or <paramref name="authJwtRepository"/> is null.</exception>
    public AuthRootRepository(LogInRootOptions options, IAuthJwtRepository authJwtRepository)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.authJwtRepository = authJwtRepository ?? throw new ArgumentNullException(nameof(authJwtRepository));
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInRootAsync(LogInRoot logInRoot)
    {
        ArgumentNullException.ThrowIfNull(logInRoot);

        await Task.CompletedTask;

        if (logInRoot.Username != this.options.Username || logInRoot.Password != this.options.Password)
        {
            throw new UnauthorizedException();
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