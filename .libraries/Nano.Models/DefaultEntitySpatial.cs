using Nano.Models.Interfaces;
using NetTopologySuite.Geometries;

namespace Nano.Models
{
    /// <inheritdoc cref="IEntitySpatial"/>
    public abstract class DefaultEntitySpatial : DefaultEntity, IEntitySpatial
    {
        /// <inheritdoc />
        public virtual Geometry Geometry { get; set; } // BUG: SPATIAL: Geometry is abstract.
    }
}