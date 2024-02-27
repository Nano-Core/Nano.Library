using System;
using Microsoft.EntityFrameworkCore;

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