using Nano.Data.Interfaces;

namespace Nano.Data
{
    /// <inheritdoc />
    public class DefaultDbContextFactory<TProvider, TContext> : BaseDbContextFactory<TProvider, TContext>
        where TProvider : class, IDataProvider
        where TContext : DefaultDbContext
    {

    }
}