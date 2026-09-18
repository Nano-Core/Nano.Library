using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;
/// <summary>
/// Wire-level request body for refreshing an access token - used for HTTP model binding and the api
/// client only, not passed to <see cref="IAuthIdentityRepository{TIdentity}.LogInRefreshAsync"/> directly.
/// The expired or soon-to-expire access token itself is not part of this model - it is read from the
/// request's own Authorization header, not the body, so there is exactly one source of truth for which
/// session is being refreshed.
/// </summary>
public class LogInRefresh
{
    /// <summary>
    /// The refresh token used to issue a new access token.
    /// </summary>
    [Required]
    public virtual required string RefreshToken { get; set; }
}