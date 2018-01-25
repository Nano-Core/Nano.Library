namespace Nano.Models.Interfaces
{
    /// <summary>
    /// Entity identity inteface.
    /// Implementing <see cref="IEntity"/>'s having identity property.
    /// </summary>
    /// <typeparam name="TIdentity">The type of <see cref="IEntityIdentity{T}.Id"/>.</typeparam>
    public interface IEntityIdentity<TIdentity> : IEntity
    {
        /// <summary>
        /// Id.
        /// Uniquely identifies the <see cref="IEntity"/>.
        /// </summary>
        TIdentity Id { get; set; }
    }
}