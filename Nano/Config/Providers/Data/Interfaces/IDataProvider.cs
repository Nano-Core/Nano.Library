using Microsoft.EntityFrameworkCore;

namespace Nano.Config.Providers.Data.Interfaces
{
    /// <summary>
    /// Data Provider interface.
    /// Defines the provider used for data context in the application.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Configures the <see cref="IDataProvider"/>.
        /// </summary>
        /// <param name="builder">The <see cref="DbContextOptionsBuilder"/>.</param>
        /// <param name="options">The <see cref="DataOptions"/>.</param>
        void Configure(DbContextOptionsBuilder builder, DataOptions options);
    }
}