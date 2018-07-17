using GeoAPI.Geometries;
using Nano.Models.Interfaces;

namespace Nano.Models
{
    /// <inheritdoc cref="IEntitySpatial"/>
    public abstract class DefaultEntitySpatial : DefaultEntity, IEntitySpatial
    {
        /// <inheritdoc />
        public virtual IGeometry Geometry { get; set; }
    }
}