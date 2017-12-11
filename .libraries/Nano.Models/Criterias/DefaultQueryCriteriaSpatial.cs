using Nano.Models.Criterias.Interfaces;
using NetTopologySuite.Geometries;

namespace Nano.Models.Criterias
{
    /// <inheritdoc cref="IQueryCriteriaSpatial"/>
    public class DefaultQueryCriteriaSpatial : DefaultQueryCriteria, IQueryCriteriaSpatial
    {
        /// <inheritdoc />
        public virtual Geometry Geometry { get; set; } // BUG: SPATIAL: Geometry is abstract.
    }
}