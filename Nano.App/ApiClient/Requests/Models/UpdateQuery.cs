using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DynamicExpression.Interfaces;

namespace Nano.App.ApiClient.Requests.Models;

/// <summary>
/// Update Query.
/// </summary>
public class UpdateQuery<TCriteria>
    where TCriteria : class, IQueryCriteria, new()
{
    /// <summary>
    /// Criteria.
    /// </summary>
    [Required]
    public virtual TCriteria Criteria { get; set; } = null!;

    /// <summary>
    /// Property Updates.
    /// </summary>
    public virtual Dictionary<string, object> PropertyUpdates { get; set; } = new();
}