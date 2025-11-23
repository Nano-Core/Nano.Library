using System;
using Nano.Eventing.Abstractions;

namespace Nano.Data.Repository;

/// <inheritdoc />
public class DefaultRepository : BaseRepository<BaseDbContext<Guid>, Guid>
{
    /// <inheritdoc />
    public DefaultRepository(BaseDbContext<Guid> context, IEventing eventing)
        : base(context, eventing)
    {
    }
}