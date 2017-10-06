using Nano.App.Controllers.Contracts.Enums;

namespace Nano.App.Controllers.Contracts
{
    /// <summary>
    /// Ordering.
    /// </summary>
    public class Ordering
    {
        /// <summary>
        /// By.
        /// </summary>
        public virtual string By { get; set; } = "Id";

        /// <summary>
        /// SortDirection.
        /// </summary>
        public Direction Direction { get; set; } = Direction.Asc;
    }
}