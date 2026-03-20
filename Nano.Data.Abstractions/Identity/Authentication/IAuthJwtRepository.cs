using System;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Authentication;

/// <summary>
/// Defines the contract for a JWT authentication repository, responsible for issuing and validating JSON Web Tokens (JWTs)
/// and refresh tokens used in authentication flows.
/// </summary>
/// <remarks>
///     Implementations of this interface are responsible for:
///     <list type="bullet">
///         <item>Generating JWT access tokens with appropriate claims, including user information and optional external provider tokens.</item>
///         <item>Generating cryptographically secure refresh tokens with configurable expiration.</item>
///         <item>Validating refresh tokens to ensure they are properly signed, issued by the expected authority, and not tampered with.</item>
///     </list>
///     The <see cref="AccessToken"/> and <see cref="RefreshToken"/> objects returned by the methods contain both the token value and its expiration information.
/// </remarks>
public interface IAuthJwtRepository
{
    /// <summary>
    /// Generates a new JWT access token for the specified user and context.
    /// </summary>
    /// <param name="generateJwtToken">
    ///     Parameters for token generation, including:
    ///     <list type="bullet">
    ///         <item>User ID, username, and email.</item>
    ///         <item>Optional external authentication provider token and refresh token.</item>
    ///         <item>Additional custom claims to include in the JWT.</item>
    ///     </list>
    /// </param>
    /// <returns>An <see cref="AccessToken"/> containing the JWT and its expiration time.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="generateJwtToken"/> is null.</exception>
    AccessToken GenerateJwtToken(GenerateJwtToken generateJwtToken);

    /// <summary>
    /// Generates a new refresh token that can be used to obtain a new access token without requiring the user to re-authenticate.
    /// </summary>
    /// <returns>A <see cref="RefreshToken"/> containing a cryptographically secure token and its expiration time.</returns>
    RefreshToken GenerateJwtRefreshToken();

    /// <summary>
    /// Validates a provided refresh token to ensure it is authentic, correctly signed, and issued by the trusted authority.
    /// </summary>
    /// <param name="refreshToken">The JWT refresh token to validate.</param>
    /// <exception cref="UnauthorizedException">Thrown if the token is invalid, malformed, or signed with an unexpected algorithm.</exception>
    void ValidateTokenForRefresh(string refreshToken);
}