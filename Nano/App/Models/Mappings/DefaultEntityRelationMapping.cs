using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.App.Models.Interfaces;

namespace Nano.App.Models.Mappings
{
    /// <inheritdoc />
    public class DefaultEntityRelationMapping<TEntity, TRelation> : DefaultEntityMapping<TEntity>
        where TEntity : DefaultEntityRelation<TRelation>
        where TRelation : class, IEntityIdentity<Guid>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .Property("RelationId")
                .HasColumnName($"{typeof(TRelation).Name}Id");

            builder
                .Property(y => y.Summary)
                .HasMaxLength(255)
                .IsRequired();

            builder
                .HasOne(y => y.Relation)
                .WithMany()
                .IsRequired();
        }
    }
}