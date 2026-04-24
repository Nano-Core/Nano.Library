using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DynamicExpression.Interfaces;

namespace Nano.App.ApiClient.Requests.Models;

/// <summary>
/// Represents an update query containing criteria and property changes.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/> used to filter entities.</typeparam>
public class UpdateQuery<TCriteria>
    where TCriteria : class, IQueryCriteria, new()
{
    /// <summary>
    /// The criteria used to select entities to update.
    /// </summary>
    [Required]
    public virtual TCriteria Criteria { get; set; } = new();

    /// <summary>
    /// A dictionary of property names and their new values to update on the selected entities.
    /// </summary>
    public virtual IDictionary<string, object> PropertyUpdates { get; set; } = new Dictionary<string, object>();
}