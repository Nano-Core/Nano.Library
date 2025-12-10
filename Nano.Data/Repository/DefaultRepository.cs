using System;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions.Config;

namespace Nano.Data.Repository;

/// <inheritdoc />
public class DefaultRepository<TContext, TIdentity> : BaseRepository<TContext, TIdentity>
    where TContext : BaseDbContext<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public DefaultRepository(IOptionsMonitor<DataOptions> options, TContext context)
        : base(options, context)
    {
    }
}