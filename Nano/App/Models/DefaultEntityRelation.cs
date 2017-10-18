using System;

namespace Nano.App.Models
{
    /// <inheritdoc />
    public class DefaultEntityRelation<TReference> : BaseEntityRelation<TReference, Guid>
        where TReference : BaseEntityIdentity<Guid>
    {

    }
}