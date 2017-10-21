using Nano.App.Controllers.Criteria.Enums;

namespace Nano.App.Controllers.Criteria.Entities
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
        public virtual Direction Direction { get; set; } = Direction.Asc;
    }
}