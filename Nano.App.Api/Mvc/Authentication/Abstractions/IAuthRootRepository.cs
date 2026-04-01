using Nano.App.ApiClient.Requests.Auth.Models;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Api.Mvc.Authentication.Abstractions;

/// <summary>
/// Provides authentication for the root/admin user.
/// </summary>
public interface IAuthRootRepository
{
    /// <summary>
    /// Signs in the admin/root user using credentials from the provided <see cref="LogInRoot"/>.
    /// The credentials must match the root username and password in the configuration.
    /// </summary>
    /// <param name="logInRoot">The root login credentials.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>An <see cref="AccessToken"/> for authentication.</returns>
    Task<AccessToken> LogInRootAsync(LogInRoot logInRoot, CancellationToken cancellationToken = default);
}