using System;
using Nano.Data.Abstractions.Config.Enums;

namespace Nano.Data.Abstractions.Annotations;

/// <summary>
/// Include Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class IncludeAttribute : Attribute
{
    /// <summary>
    /// Query Split Behavior.
    /// </summary>
    public QuerySplitBehavior QuerySplitBehavior { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="querySplitBehavior">The <see cref="Config.Enums.QuerySplitBehavior"/>.</param>
    public IncludeAttribute(QuerySplitBehavior querySplitBehavior = QuerySplitBehavior.SingleQuery)
    {
        this.QuerySplitBehavior = querySplitBehavior;
    }
}