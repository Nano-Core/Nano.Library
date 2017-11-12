using NetTopologySuite.Geometries;

namespace Nano.Controllers.Criterias.Entities
{
    /// <summary>
    /// Default Criteria Spatial.
    /// </summary>
    public class DefaultCriteriaSpatial : DefaultCriteria
    {
        /// <summary>
        /// Geometry.
        /// </summary>
        public virtual Geometry Geometry { get; set; }
    }
}