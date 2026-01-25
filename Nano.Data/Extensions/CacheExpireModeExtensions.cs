using System;
using EFCoreSecondLevelCacheInterceptor;
using Nano.Data.Abstractions.Config.Enums;

namespace Nano.Data.Extensions;

internal static class CacheExpireModeExtensions
{
    internal static CacheExpirationMode GetCacheExpirationMode(this CacheExpireMode querySplitBehavior)
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