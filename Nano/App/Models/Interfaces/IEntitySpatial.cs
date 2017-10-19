using NetTopologySuite.Geometries;

namespace Nano.App.Models.Interfaces
{
    /// <summary>
    /// Relation Spatial.
    /// Implementing <see cref="IEntity"/>'s are queryable spatially, having property of <see cref="Geometry"/> type.
    /// </summary>
    public interface IEntitySpatial : IEntity
    {
        /// <summary>
        /// Geometry.
        /// </summary>
        Geometry Geometry { get; set; }
    }
}