using Nano.Controllers.Criterias.Enums;

namespace Nano.Controllers.Criterias.Interfaces
{
    /// <summary>
    /// Filter Statement interface.
    /// </summary>
    public interface IFilterStatement
    {
        /// <summary>
        /// Property.
        /// </summary>
        string Property { get; set; }

        /// <summary>
        /// Value.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Value2.
        /// </summary>
        object Value2 { get; set; }

        /// <summary>
        /// Operation.
        /// </summary>
        Operation Operation { get; set; }

        /// <summary>
        /// Operation Logical.
        /// </summary>
        OperationLogical OperationLogical { get; set; }
    }
}