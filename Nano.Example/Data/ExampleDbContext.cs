using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data;
using Nano.Data.Extensions;
using Nano.Example.Data.Mappings;
using Nano.Example.Models;

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
                .AddMapping<ExampleEntity, ExampleEntityMapping>();
        }
    }
}
