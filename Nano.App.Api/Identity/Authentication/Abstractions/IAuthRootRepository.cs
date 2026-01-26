using System.Threading.Tasks;
using Nano.App.ApiClient.Models.Auth;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Api.Identity.Authentication.Abstractions;

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
    /// <returns>An <see cref="AccessToken"/> for authentication.</returns>
    Task<AccessToken> LogInRootAsync(LogInRoot logInRoot);
}