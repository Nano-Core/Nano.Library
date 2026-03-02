using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Abstractions;

/// <summary>
/// Task for registering migration fo the database in the application.
/// </summary>
internal interface IDbMigrationTask
{
    /// <summary>
    /// Migrates and seeds the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the registration operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous registration operation.</returns>
    Task MigrateAndSeedAsync(CancellationToken cancellationToken = default);
}