using System.ComponentModel.DataAnnotations;
using DynamicExpression.Entities;
using DynamicExpression.Interfaces;
using NetTopologySuite.Geometries;

namespace Nano.Models.Criterias;

/// <summary>
/// Spatial Query.
/// </summary>
public class SpatialQuery<TCriteria> : Query<TCriteria>
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// Geometry.
    /// </summary>
    [Required]
    public virtual Geometry Geometry { get; set; }
}