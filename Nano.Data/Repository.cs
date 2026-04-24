using System;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions.Config;

namespace Nano.Data;

/// <inheritdoc />
public class Repository<TContext, TIdentity> : BaseRepository<TContext, TIdentity>
    where TContext : BaseDbContext<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public Repository(IOptionsMonitor<DataOptions> options, TContext context)
        : base(options, context)
    {
    }
}