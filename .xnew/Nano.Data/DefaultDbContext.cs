using System;
using Microsoft.EntityFrameworkCore;
using Nano.Security;

namespace Nano.Data;

/// <inheritdoc />
public class DefaultDbContext : BaseDbContext<Guid>
{
    /// <inheritdoc />
    public DefaultDbContext(DbContextOptions contextOptions, DataOptions dataOptions, IdentityOptions securityOptions)
        : base(contextOptions, dataOptions, securityOptions)
    {
    }
}