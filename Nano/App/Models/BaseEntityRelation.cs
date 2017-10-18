using Nano.App.Models.Interfaces;

namespace Nano.App.Models
{
    /// <inheritdoc />
    public abstract class BaseEntityRelation<TRelation, TIdentity> : BaseEntityIdentity<TIdentity>
        where TRelation : IEntityIdentity<TIdentity>
    {
        /// <summary>
        /// Entity.
        /// </summary>
        public virtual TRelation Entity { get; set; }
    }
}