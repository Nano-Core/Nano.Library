using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Nano.Data.Identity;

/// <summary>
/// Custom Token Provider.
/// </summary>
/// <typeparam name="TUser"></typeparam>
public class CustomTokenProvider<TUser> : DataProtectorTokenProvider<TUser>
    where TUser : class
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataProtectionProvider"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public CustomTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<DataProtectionTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
    {
    }
}