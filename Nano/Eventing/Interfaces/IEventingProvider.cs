namespace Nano.Eventing.Interfaces
{
    /// <summary>
    /// Eventing Provider interface.
    /// Defines the provider used for eventing in the application.
    /// </summary>
    public interface IEventingProvider
    {
        /// <summary>
        /// Configures the <see cref="IEventing"/>.
        /// </summary>
        /// <returns>The <see cref="IEventing"/>.</returns>
        IEventing Configure();
    }
}