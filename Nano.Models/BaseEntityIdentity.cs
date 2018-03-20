using Nano.Models.Interfaces;

namespace Nano.Models
{
    /// <inheritdoc cref="IEntityIdentity{TIdentity}"/>
    public abstract class BaseEntityIdentity<TIdentity> : BaseEntity, IEntityIdentity<TIdentity>
    {
        /// <inheritdoc />
        public virtual TIdentity Id { get; protected set; }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return this.Id.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        /// <inheritdoc />
        public override bool Equals(object entity)
        {
            if (entity is IEntityIdentity<TIdentity> identity)
                return this.Id.Equals(identity.Id);

            return object.ReferenceEquals(this, entity);
        }
    }
}