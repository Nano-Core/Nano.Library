namespace Nano.Models.Interfaces
{
    /// <summary>
    /// Entity Writable.
    /// Implementing <see cref="IEntity"/>'s are creatable, updatable and deletable.
    /// Implements <see cref="IEntityCreatable"/>, <see cref="IEntityUpdatable"/> and <see cref="IEntityDeletable"/>.
    /// </summary>
    public interface IEntityWritable : IEntityCreatable, IEntityUpdatable, IEntityDeletable
    {

    }
}