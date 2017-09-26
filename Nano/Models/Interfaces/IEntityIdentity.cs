namespace Nano.Models.Interfaces
{
    /// <summary>
    /// Entity identity inteface.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IEntityIdentity{T}.Id"/></typeparam>
    public interface IEntityIdentity<out T> : IEntity
    {
        /// <summary>
        /// Universal Unique Identifier (uuid), 
        /// Uniquely identifies the <see cref="IEntity"/>.
        /// </summary>
        T Id { get; }
    }
}