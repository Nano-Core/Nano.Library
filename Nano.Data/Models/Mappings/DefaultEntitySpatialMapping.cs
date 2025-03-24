using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models;
using NetTopologySuite.Geometries;

namespace Nano.Data.Models.Mappings;

/// <inheritdoc />
public class DefaultEntitySpatialMapping<TEntity, TGeometry> : DefaultEntityMapping<TEntity>
    where TEntity : DefaultEntitySpatial<TGeometry>
    where TGeometry : Geometry
{
    /// <inheritdoc />
    public override void Map(EntityTypeBuilder<TEntity> builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        base.Map(builder);

        builder
            .Property(x => x.Geometry)
            .HasColumnType("GEOMETRY")
            .HasSrid(4326);

        builder
            .HasIndex(x => x.Geometry)
            .IsSpatial();
    }
}