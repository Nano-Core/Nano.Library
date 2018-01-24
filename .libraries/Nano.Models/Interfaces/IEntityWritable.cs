namespace Nano.Models.Interfaces
{
    /// <summary>
    /// Entity Writable.
    /// Implementing entities are creatable, updatable and deletable.
    /// Implements <see cref="IEntityCreatable"/>, <see cref="IEntityUpdatable"/> and <see cref="IEntityDeletable"/>.
    /// </summary>
    public interface IEntityWritable : IEntityCreatable, IEntityUpdatable, IEntityDeletableSoft
    {

    }
}