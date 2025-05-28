using System;
using Microsoft.EntityFrameworkCore;
using Nano.Security;

namespace Nano.Data;

/// <inheritdoc />
public class NullDbContext : DefaultDbContext
{
    /// <inheritdoc />
    public NullDbContext(DbContextOptions<NullDbContext> dbContextOptions, DataOptions dataOptions, SecurityOptions securityOptions)
        : base(dbContextOptions, dataOptions, securityOptions)
    {
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder == null)
            throw new ArgumentNullException(nameof(optionsBuilder));

        optionsBuilder
            .UseInMemoryDatabase("nulldb");
    }
}