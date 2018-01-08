using Nano.Models.Interfaces;

namespace Nano.Models
{
    /// <inheritdoc cref="BaseEntity"/>
    public abstract class BaseEntityIdentity<TIdentity> : BaseEntity, IEntityIdentity<TIdentity>
    {
        /// <inheritdoc />
        public virtual TIdentity Id { get; set; }

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
            if (entity is IEntityIdentity<TIdentity> entityIdentity)
                return this.Id.Equals(entityIdentity.Id);

            return object.ReferenceEquals(this, entity);
        }
    }
}