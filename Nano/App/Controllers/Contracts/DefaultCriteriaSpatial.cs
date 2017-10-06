using Nano.App.Controllers.Contracts.Interfaces;
using NetTopologySuite.Geometries;

namespace Nano.App.Controllers.Contracts
{
    /// <summary>
    /// Criteria Spatial.
    /// </summary>
    public class DefaultCriteriaSpatial : DefaultCriteria, ICriteriaSpatial
    {
        /// <summary>
        /// Geometry.
        /// </summary>
        public virtual Geometry Geometry { get; set; } // TODO: SPATIAL: Geometry is abstract.
    }
}