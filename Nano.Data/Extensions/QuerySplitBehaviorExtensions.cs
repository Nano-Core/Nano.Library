using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Abstractions.Config.Enums;

namespace Nano.Data.Extensions;

/// <summary>
/// Provides extension methods for <see cref="QuerySplitBehavior"/> enumeration.
/// </summary>
public static class QuerySplitBehaviorExtensions
{
    /// <summary>
    /// Converts a <see cref="QuerySplitBehavior"/> value to the corresponding <see cref="QuerySplittingBehavior"/> used by Entity Framework Core.
    /// </summary>
    /// <param name="querySplitBehavior">The <see cref="QuerySplitBehavior"/> to convert.</param>
    /// <returns>The corresponding <see cref="QuerySplittingBehavior"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="querySplitBehavior"/> contains an unsupported value.</exception>
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