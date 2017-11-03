using Nano.Controllers.Criterias.Enums;

namespace Nano.Controllers.Criterias.Entities
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
        /// Direction.
        /// </summary>
        public virtual SortDirection Direction { get; set; } = SortDirection.Asc;
    }
}