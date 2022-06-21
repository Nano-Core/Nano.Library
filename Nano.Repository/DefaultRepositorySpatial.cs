using System;
using Nano.Data;

namespace Nano.Repository;

/// <inheritdoc />
public class DefaultRepositorySpatial : BaseRepositorySpatial<BaseDbContext<Guid>, Guid>
{
    /// <inheritdoc />
    public DefaultRepositorySpatial(BaseDbContext<Guid> context)
        : base(context)
    {

    }
}