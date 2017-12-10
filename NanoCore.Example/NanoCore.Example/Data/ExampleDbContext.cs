using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Models.Mappings.Extensions;
using Nano.Services.Data;
using NanoCore.Example.Data.Mappings;
using NanoCore.Example.Models;

namespace NanoCore.Example.Data
{
    /// <inheritdoc />
    public class ExampleDbContext : DefaultDbContext
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
