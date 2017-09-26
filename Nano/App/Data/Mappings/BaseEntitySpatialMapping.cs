using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Interfaces;

namespace Nano.App.Data.Mappings
{
    /// <inheritdoc />
    public abstract class BaseEntitySpatialMapping<TEntity, TIdentity> : BaseEntityIdentityMapping<TEntity, TIdentity>
        where TEntity : class, IEntityIdentity<TIdentity>, IEntitySpatial
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                // BUG: FIX: Spatial Type (Geometry is abstract), maybe re-add MapGeometry() method and add new() generic contraint for compile time error.
                .Ignore(x => x.Geometry);
                //.OwnsOne(x => x.Geometry)
                //.Ignore(x => x.Factory)
                //.Ignore(x => x.UserData)
                //.Ignore(x => x.SRID)
                //.Ignore(x => x.GeometryType)
                //.Ignore(x => x.OgcGeometryType)
                //.Ignore(x => x.PrecisionModel)
                //.Ignore(x => x.Coordinate)
                //.Ignore(x => x.Coordinates)
                //.Ignore(x => x.NumPoints)
                //.Ignore(x => x.NumGeometries)
                //.Ignore(x => x.IsSimple)
                //.Ignore(x => x.IsValid)
                //.Ignore(x => x.IsEmpty)
                //.Ignore(x => x.Area)
                //.Ignore(x => x.Length)
                //.Ignore(x => x.Centroid)
                //.Ignore(x => x.InteriorPoint)
                //.Ignore(x => x.PointOnSurface)
                //.Ignore(x => x.Dimension)
                //.Ignore(x => x.Boundary)
                //.Ignore(x => x.BoundaryDimension)
                //.Ignore(x => x.Envelope)
                //.Ignore(x => x.EnvelopeInternal)
                //.Ignore(x => x.IsRectangle);
        }
    }
}