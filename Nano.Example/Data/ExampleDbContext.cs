using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data;
using Nano.Data.Extensions;
using Nano.Example.Models;
using Nano.Example.Models.Mappings;

namespace Nano.Example.Data
{
    /// <inheritdoc />
    public class ExampleDbContext : BaseDbContext
    {
        /// <inheritdoc />
        public ExampleDbContext(DbContextOptions options)
            : base(options)
        {
            
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);

            modelBuilder
                .AddMapping<ExampleEntity, ExampleEntityMapping>()
                .AddMapping<ExampleEntityRelation, ExampleEntityRelationMapping>();
        }
    }
}
