using DynamicExpression.Interfaces;
using GeoAPI.Geometries;

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
        IGeometry Geometry { get; set; }
    }
}