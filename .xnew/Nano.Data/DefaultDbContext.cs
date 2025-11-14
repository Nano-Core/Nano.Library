using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Abstractions.Config;

namespace Nano.Data;

/// <inheritdoc />
public class DefaultDbContext : BaseDbContext<Guid>
{
    /// <inheritdoc />
    public DefaultDbContext(DbContextOptions contextOptions, DataOptions dataOptions)
        : base(contextOptions, dataOptions)
    {
    }
}