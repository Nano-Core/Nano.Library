using System;
using Nano.Models.Interfaces;
using NetTopologySuite.Geometries;

namespace Nano.Models;

/// <inheritdoc />
public abstract class DefaultEntitySpatial : DefaultEntitySpatial<Geometry>;

/// <inheritdoc />
public abstract class DefaultEntitySpatial<TGeometry> : DefaultEntitySpatial<TGeometry, Guid>
    where TGeometry : Geometry;

/// <inheritdoc cref="IEntitySpatial{TGeometry}"/>
public class DefaultEntitySpatial<TGeometry, TIdentity> : DefaultEntity<TIdentity>, IEntitySpatial<TGeometry>
    where TGeometry : Geometry
{
    /// <inheritdoc />
    public virtual TGeometry Geometry { get; set; }
}