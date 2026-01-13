using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions.Config;
using Nano.Eventing.Abstractions;

namespace Nano.Data;

/// <inheritdoc />
public class DefaultDbContext : BaseDbContext<Guid>
{
    /// <inheritdoc />
    public DefaultDbContext(DbContextOptions contextOptions, IOptionsMonitor<DataOptions> dataOptions, IEventing? eventing = null)
        : base(contextOptions, dataOptions, eventing)
    {
    }
}