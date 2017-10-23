using NetTopologySuite.Geometries;

namespace Nano.App.Controllers.Criteria.Entities
{
    /// <summary>
    /// Query Spatial.
    /// </summary>
    public class CriteriaSpatial : Criteria
    {
        /// <summary>
        /// Geometry.
        /// </summary>
        public virtual Geometry Geometry { get; set; } // TODO: SPATIAL: Geometry is abstract.
    }
}