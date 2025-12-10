using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Abstractions;

/// <summary>
/// Interface used with database migrations.
/// </summary>
public interface IDbMigrationTask
{
    /// <summary>
    /// Migrates and seeds data.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    Task MigrateAndSeedAsync(CancellationToken cancellationToken = default);
}