namespace Nano.Controllers.Contracts
{
    /// <summary>
    /// Pagination.
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// Number.
        /// </summary>
        public virtual int Number { get; set; } = 1;

        /// <summary>
        /// Count (Take).
        /// </summary>
        public virtual int Count { get; set; } = 25;

        /// <summary>
        /// Skip.
        /// </summary>
        internal virtual int Skip => (this.Number - 1) * this.Count;
    }
}