using System;
using EFCoreSecondLevelCacheInterceptor;
using Nano.Data.Abstractions.Config.Enums;

namespace Nano.Data.Extensions;

/// <summary>
/// Cache Expire Mode Extensions.
/// </summary>
public static class CacheExpireModeExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="querySplitBehavior"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static CacheExpirationMode GetCacheExpirationMode(this CacheExpireMode querySplitBehavior)
    {
        return querySplitBehavior switch
        {
            CacheExpireMode.Absolute => CacheExpirationMode.Absolute,
            CacheExpireMode.Sliding => CacheExpirationMode.Sliding,
            CacheExpireMode.NeverRemove => CacheExpirationMode.NeverRemove,
            _ => throw new ArgumentOutOfRangeException(nameof(querySplitBehavior), querySplitBehavior, null)
        };
    }
}