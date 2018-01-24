using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data;
using Nano.Data.Models.Mappings.Extensions;
using Nano.Services.Data;
using NanoCore.Example.Data.Models.Mappings;
using NanoCore.Example.Models;

namespace NanoCore.Example.Data
{
    /// <inheritdoc />
    public class ExampleDbContext : DefaultDbContext
    {
        /// <inheritdoc />
        public ExampleDbContext(DbContextOptions contextOptions, DataOptions dataOptions)
            : base(contextOptions, dataOptions)
        {

        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.OnModelCreating(builder);

            builder
                .AddMapping<ExampleEntity, ExampleEntityMapping>()
                .AddMapping<ExampleEntitySpatial, ExampleEntitySpatialMapping>()
                .AddMapping<ExampleEntityTypes, ExampleEntityTypesMapping>();
        }
    }
}
