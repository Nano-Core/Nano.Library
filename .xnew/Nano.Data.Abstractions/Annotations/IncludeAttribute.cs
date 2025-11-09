using System;
using Nano.Data;

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
    public QuerySplitBehavior QuerySplittingBehavior { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="querySplittingBehavior">The <see cref="QuerySplittingBehavior"/>.</param>
    public IncludeAttribute(QuerySplitBehavior querySplittingBehavior = QuerySplitBehavior.SingleQuery)
    {
        this.QuerySplittingBehavior = querySplittingBehavior;
    }
}