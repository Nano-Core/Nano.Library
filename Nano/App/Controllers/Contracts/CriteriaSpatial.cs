using NetTopologySuite.Geometries;

namespace Nano.App.Controllers.Contracts
{
    /// <summary>
    /// Criteria Spatial.
    /// </summary>
    public class CriteriaSpatial : Criteria
    {
        /// <summary>
        /// Geometry.
        /// </summary>
        public virtual Geometry Geometry { get; set; } // TODO: SPATIAL: Geometry is abstract.
    }
}