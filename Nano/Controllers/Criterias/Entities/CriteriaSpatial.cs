using NetTopologySuite.Geometries;

namespace Nano.Controllers.Criterias.Entities
{
    /// <summary>
    /// Query Spatial.
    /// </summary>
    public class CriteriaSpatial : Criteria
    {
        /// <summary>
        /// Geometry.
        /// </summary>
        public virtual Geometry Geometry { get; set; }
    }
}