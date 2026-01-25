using System.Threading;

namespace Nano.App.StartUp;

/// <summary>
/// Tracks the progress and completion status of startup tasks.
/// </summary>
public sealed class StartupTaskContext
{
    private int count;

    /// <summary>
    /// Gets a value indicating whether all tracked startup tasks have completed.
    /// </summary>
    public bool IsDone => this.count == 0;

    /// <summary>
    /// Increments the count of active startup tasks.
        /// Call this when a new startup task begins.
    /// </summary>
    public void Increment()
    {
        Interlocked.Increment(ref this.count);
    }

    /// <summary>
    /// Decrements the count of active startup tasks.
    /// Call this when a startup task completes.
    /// </summary>
    public void Decrement()
    {
        Interlocked.Decrement(ref this.count);
    }
}