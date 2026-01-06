using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Consts;

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
    /// <returns></returns>
    RefreshToken GenerateJwtRefreshToken();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="refreshToken"></param>
    void ValidateTokenForRefresh(string refreshToken);
}