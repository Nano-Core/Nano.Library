using System;
using Microsoft.EntityFrameworkCore;

namespace Nano.Models.Attributes;

/// <summary>
/// Include Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class IncludeAttribute : Attribute
{
    /// <summary>
    /// Query Splitting Behavior.
    /// </summary>
    public QuerySplittingBehavior QuerySplittingBehavior { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="querySplittingBehavior">The <see cref="QuerySplittingBehavior"/>.</param>
    public IncludeAttribute(QuerySplittingBehavior querySplittingBehavior = QuerySplittingBehavior.SingleQuery)
    {
        this.QuerySplittingBehavior = querySplittingBehavior;
    }
}