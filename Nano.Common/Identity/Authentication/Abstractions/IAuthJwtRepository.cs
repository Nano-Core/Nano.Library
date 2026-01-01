using Nano.Common.Identity.Authentication.Models;

namespace Nano.Common.Identity.Authentication.Abstractions;

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
    void ValidateRefreshToken(string refreshToken);
}