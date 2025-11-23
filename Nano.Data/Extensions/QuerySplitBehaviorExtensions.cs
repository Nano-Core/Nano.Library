using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Abstractions.Config.Enums;

namespace Nano.Data.Extensions;

/// <summary>
/// Query Split Behavior Extensions.
/// </summary>
public static class QuerySplitBehaviorExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="querySplitBehavior"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static QuerySplittingBehavior GetQuerySplittingBehavior(this QuerySplitBehavior querySplitBehavior)
    {
        return querySplitBehavior switch
        {
            QuerySplitBehavior.SingleQuery => QuerySplittingBehavior.SingleQuery,
            QuerySplitBehavior.SplitQuery => QuerySplittingBehavior.SplitQuery,
            _ => throw new ArgumentOutOfRangeException(nameof(querySplitBehavior), querySplitBehavior, null)
        };
    }
}