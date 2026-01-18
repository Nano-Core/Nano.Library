using System.Threading;

namespace Nano.App.StartUp;

/// <summary>
/// Startup Task Context.
/// </summary>
public sealed class StartupTaskContext
{
    private int count;

    /// <summary>
    /// Is Complete.
    /// </summary>
    public bool IsDone => this.count == 0;

    /// <summary>
    /// Increment.
    /// </summary>
    public void Increment()
    {
        Interlocked.Increment(ref this.count);
    }

    /// <summary>
    /// Decrement.
    /// </summary>
    public void Decrement()
    {
        Interlocked.Decrement(ref this.count);
    }
}