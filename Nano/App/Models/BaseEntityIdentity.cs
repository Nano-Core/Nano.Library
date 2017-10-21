using Nano.App.Models.Interfaces;

namespace Nano.App.Models
{
    /// <inheritdoc cref="BaseEntity"/>
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
            if (entity is IEntityIdentity<TIdentity> entityIdentity)
                return this.Id.ToString() == entityIdentity.Id.ToString();

            return object.ReferenceEquals(this, entity);
        }
    }
}