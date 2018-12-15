using Nano.Models.Interfaces;

namespace Nano.Models
{
    /// <inheritdoc cref="IEntityIdentity{TIdentity}"/>
    public abstract class BaseEntityIdentity<TIdentity> : BaseEntity, IEntityIdentity<TIdentity>
    {
        /// <inheritdoc />
        public virtual TIdentity Id { get; set; }

        // BUG: Fix GetHasCode (in net core 2.2)
        ///// <inheritdoc />
        //public override int GetHashCode()
        //{
        //    // ReSharper disable NonReadonlyMemberInGetHashCode
        //    // ReSharper restore NonReadonlyMemberInGetHashCode
            
        //    return this.Id.GetHashCode();
        //}

        ///// <inheritdoc />
        //public override bool Equals(object entity)
        //{
        //    if (entity is IEntityIdentity<TIdentity> identity)
        //        return this.Id.Equals(identity.Id);

        //    return object.ReferenceEquals(this, entity);
        //}
    }
}