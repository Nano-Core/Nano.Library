using System;
using Nano.Models.Interfaces;
using NetTopologySuite.Geometries;

namespace Nano.Models;

/// <inheritdoc />
public abstract class DefaultEntitySpatial : DefaultEntitySpatial<Guid>
{

}

/// <inheritdoc cref="IEntitySpatial"/>
public class DefaultEntitySpatial<TIdentity> : DefaultEntity<TIdentity>, IEntitySpatial
{
    /// <inheritdoc />
    public virtual Geometry Geometry { get; set; }
}