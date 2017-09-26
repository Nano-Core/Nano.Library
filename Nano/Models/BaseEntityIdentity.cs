using Nano.Models.Interfaces;

namespace Nano.Models
{
    /// <inheritdoc cref="BaseEntity"/>
    public abstract class BaseEntityIdentity<T> : BaseEntity, IEntityIdentity<T>
    {
        /// <inheritdoc />
        public virtual T Id { get; protected set; } = default;

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
            if (entity is IEntityIdentity<T> entityIdentity)
                return this.Id.ToString() == entityIdentity.Id.ToString();

            return object.ReferenceEquals(this, entity);
        }
    }
}