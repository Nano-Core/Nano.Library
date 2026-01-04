using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Authentication;

/// <summary>
/// 
/// </summary>
public interface IAuthJwtRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="generateJwtToken"></param>
    /// <returns></returns>
    AccessToken GenerateJwtToken(GenerateJwtToken generateJwtToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="refreshToken"></param>
    void ValidateTokenForRefresh(string refreshToken);
}