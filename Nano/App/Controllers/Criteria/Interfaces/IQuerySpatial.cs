using NetTopologySuite.Geometries;

namespace Nano.App.Controllers.Criteria.Interfaces
{
    /// <summary>
    /// Criteria Spatial interface.
    /// </summary>
    public interface ICriteriaSpatial : ICriteria
    {
        /// <summary>
        /// Geometry.
        /// </summary>
        Geometry Geometry { get; set; }
    }
}