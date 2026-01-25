using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Abstractions;

internal interface IDbMigrationTask
{
    Task MigrateAndSeedAsync(CancellationToken cancellationToken = default);
}