using System;
using Nano.Data.Abstractions.Config.Enums;

namespace Nano.Data.Abstractions.Annotations;

/// <summary>
/// Indicates that a navigation property should be automatically included in queries.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class IncludeAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the query splitting behavior for this property.
    /// Defaults to <see cref="QuerySplitBehavior.SingleQuery"/>.
    /// </summary>
    public QuerySplitBehavior QuerySplitBehavior { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncludeAttribute"/> class.
    /// </summary>
    /// <param name="querySplitBehavior">The <see cref="QuerySplitBehavior"/> to use for this property. Defaults to <see cref="QuerySplitBehavior.SingleQuery"/>.</param>
    public IncludeAttribute(QuerySplitBehavior querySplitBehavior = QuerySplitBehavior.SingleQuery)
    {
        this.QuerySplitBehavior = querySplitBehavior;
    }
}