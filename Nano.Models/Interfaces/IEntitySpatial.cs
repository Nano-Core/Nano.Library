using GeoAPI.Geometries;

namespace Nano.Models.Interfaces
{
    /// <summary>
    /// Relation Spatial.
    /// Implementing entities are queryable spatially, having a property of <see cref="Geometry"/> type.
    /// </summary>
    public interface IEntitySpatial : IEntity
    {
        /// <summary>
        /// Geometry.
        /// </summary>
        IGeometry Geometry { get; set; }
    }
}