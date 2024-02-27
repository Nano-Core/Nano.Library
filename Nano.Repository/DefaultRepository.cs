using System;
using Nano.Data;

namespace Nano.Repository;

/// <inheritdoc />
public class DefaultRepository : BaseRepository<BaseDbContext<Guid>, Guid>
{
    /// <inheritdoc />
    public DefaultRepository(BaseDbContext<Guid> context)
        : base(context)
    {
    }
}