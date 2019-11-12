using DynamicExpression.Interfaces;
using NetTopologySuite.Geometries;

namespace Nano.Models.Criterias.Interfaces
{
    /// <summary>
    /// Query Criteria Spatial.
    /// </summary>
    public interface IQueryCriteriaSpatial : IQueryCriteria
    {
        /// <summary>
        /// Geometry.
        /// </summary>
        Geometry Geometry { get; set; }
    }
}