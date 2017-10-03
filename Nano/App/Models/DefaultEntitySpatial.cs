using Nano.App.Models.Interfaces;
using NetTopologySuite.Geometries;

namespace Nano.App.Models
{
    /// <inheritdoc cref="IEntitySpatial"/>
    public abstract class DefaultEntitySpatial : DefaultEntity, IEntitySpatial
    {
        /// <inheritdoc />
        public virtual Geometry Geometry { get; set; } // TODO: SPATIAL: Geometry is abstract.
    }
}