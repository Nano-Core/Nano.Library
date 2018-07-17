using GeoAPI.Geometries;
using Nano.Models.Criterias.Interfaces;

namespace Nano.Models.Criterias
{
    /// <inheritdoc cref="IQueryCriteriaSpatial"/>
    public class DefaultQueryCriteriaSpatial : DefaultQueryCriteria, IQueryCriteriaSpatial
    {
        /// <inheritdoc />
        public virtual IGeometry Geometry { get; set; }
    }
}