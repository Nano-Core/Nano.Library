using System;
using Nano.Data;
using Nano.Models.Eventing.Interfaces;

namespace Nano.Repository;

/// <inheritdoc />
public class DefaultRepositorySpatial : BaseRepositorySpatial<BaseDbContext<Guid>, Guid>
{
    /// <inheritdoc />
    public DefaultRepositorySpatial(BaseDbContext<Guid> context, IEventing eventing)
        : base(context, eventing)
    {
    }
}