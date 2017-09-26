using System;
using Microsoft.EntityFrameworkCore;
using Nano.App.Data;
using Nano.App.Data.Extensions;
using Nano.Services.Example.Data.Mappings;
using Nano.Services.Example.Models;

namespace Nano.Services.Example.Data
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
