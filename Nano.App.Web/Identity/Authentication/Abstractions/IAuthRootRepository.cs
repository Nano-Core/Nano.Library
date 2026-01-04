using System.Threading.Tasks;
using Nano.App.ApiClient.Models.Identity;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Web.Identity.Authentication.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IAuthRootRepository
{
    /// <summary>
    /// Signs in the admin user statically from configuration of username and password.
    /// </summary>
    /// <param name="logInRoot">The <see cref="LogInRoot"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    Task<AccessToken> LogInRootAsync(LogInRoot logInRoot);
}