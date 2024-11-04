using System.Collections.Generic;
using DynamicExpression.Interfaces;

namespace Nano.App.Api.Requests.Models;

/// <summary>
/// Update Query.
/// </summary>
public class UpdateQuery<TCriteria>
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// Criteria.
    /// </summary>
    public virtual TCriteria Criteria { get; set; }

    /// <summary>
    /// Property Updates.
    /// </summary>
    public virtual Dictionary<string, object> PropertyUpdates { get; set; } = new();
}