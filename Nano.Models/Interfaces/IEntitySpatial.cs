using NetTopologySuite.Geometries;

namespace Nano.Models.Interfaces;

/// <summary>
/// Entity Spatial.
/// Implementing entities are queryable spatially, having a property of <see cref="Geometry"/> type.
/// </summary>
public interface IEntitySpatial : IEntitySpatial<Geometry>;

/// <summary>
/// Entity Spatial.
/// Implementing entities are queryable spatially, having a property of <see cref="Geometry"/> type.
/// </summary>
public interface IEntitySpatial<TGeometry> : IEntity
    where TGeometry : Geometry
{
    /// <summary>
    /// Geometry.
    /// </summary>
    TGeometry Geometry { get; set; }
}