using NetTopologySuite.Geometries;

namespace Nano.App.Controllers.Criteria.Interfaces
{
    /// <summary>
    /// Query Spatial interface.
    /// </summary>
    public interface IQuerySpatial : IQuery
    {
        /// <summary>
        /// Geometry.
        /// </summary>
        Geometry Geometry { get; set; }
    }
}