using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.StartUp;

/// <summary>
/// Tracks the progress and completion status of startup tasks.
/// </summary>
public sealed class StartupTaskContext
{
    private int count;
    private bool started;
    private readonly TaskCompletionSource<object?> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>
    /// Gets a task that completes when all startup tasks have finished.
    /// </summary>
    public Task Completion
    {
        get
        {
            if (!this.started)
            {
                return Task.CompletedTask;
            }

            return this.completion.Task;
        }
    }

    /// <summary>
    /// Gets a value indicating whether all tracked startup tasks have completed.
    /// </summary>
    public bool IsDone => Volatile.Read(ref this.count) == 0 || !this.started;

    /// <summary>
    /// Call when a startup task begins.
    /// </summary>
    public void Increment()
    {
        this.started = true;

        Interlocked.Increment(ref this.count);
    }

    /// <summary>
    /// Call when a startup task completes.
    /// </summary>
    public void Decrement()
    {
        if (Interlocked.Decrement(ref this.count) == 0)
        {
            this.completion
                .TrySetResult(null);
        }
    }
}
