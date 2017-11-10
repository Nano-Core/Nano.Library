using NetTopologySuite.Geometries;

namespace Nano.Controllers.Criterias.Interfaces
{
    /// <summary>
    /// Criteria Spatial interface.
    /// </summary>
    public interface ICriteriaSpatial : ICriteria
    {
        /// <summary>
        /// Geometry.
        /// </summary>
        Geometry Geometry { get; set; } // BUG: SPATIAL: Geometry is abstract.
    }
}