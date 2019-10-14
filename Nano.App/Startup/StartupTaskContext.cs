using System.Threading;

namespace Nano.App.Startup
{
    /// <summary>
    /// Startup Task Context.
    /// </summary>
    public class StartupTaskContext
    {
        private int count;

        /// <summary>
        /// Is Complete.
        /// </summary>
        public virtual bool IsDone => this.count == 0;

        /// <summary>
        /// Increment
        /// </summary>
        public virtual void Increment()
        {
            Interlocked.Increment(ref this.count);
        }

        /// <summary>
        /// Decrement.
        /// </summary>
        public virtual void Decrement()
        {
            Interlocked.Decrement(ref this.count);
        }
    }
}