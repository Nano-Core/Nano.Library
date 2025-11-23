using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Nano.Data.Identity.DataProtection;

/// <summary>
/// Custom Data Protector Token Provider.
/// </summary>
/// <typeparam name="TUser"></typeparam>
public class CustomDataProtectorTokenProvider<TUser> : DataProtectorTokenProvider<TUser>
    where TUser : class
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataProtectionProvider"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public CustomDataProtectorTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<DataProtectionTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
    {
    }
}