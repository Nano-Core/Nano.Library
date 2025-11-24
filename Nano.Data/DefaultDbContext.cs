using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions.Config;

namespace Nano.Data;

/// <inheritdoc />
public class DefaultDbContext : BaseDbContext<Guid>
{
    /// <inheritdoc />
    public DefaultDbContext(DbContextOptions contextOptions, IOptionsMonitor<DataOptions> dataOptions)
        : base(contextOptions, dataOptions)
    {
    }
}