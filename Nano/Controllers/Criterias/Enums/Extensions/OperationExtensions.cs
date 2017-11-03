namespace Nano.Controllers.Criterias.Enums.Extensions
{
    /// <summary>
    /// Operation Extensions.
    /// </summary>
    public static class OperationExtensions
    {
        /// <summary>
        /// Get the parameter count supported by the <see cref="Operation"/>.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/>.</param>
        /// <returns>The parameter count.</returns>
        public static int GetParameterCount(this Operation operation)
        {
            switch (operation)
            {
                case Operation.Equal: return 1;
                case Operation.NotEqual: return 1;
                case Operation.Contains: return 1;
                case Operation.StartsWith: return 1;
                case Operation.EndsWith: return 1;
                case Operation.GreaterThan: return 1;
                case Operation.GreaterThanOrEqualTo: return 1;
                case Operation.LessThan: return 1;
                case Operation.LessThanOrEqualTo: return 1;
                case Operation.Between: return 2;
                case Operation.IsNull: return 0;
                case Operation.IsEmpty: return 0;
                case Operation.IsNullOrWhiteSpace: return 0;
                case Operation.IsNotNull: return 0;
                case Operation.IsNotEmpty: return 0;
                case Operation.IsNotNullNorWhiteSpace: return 0;
                case Operation.In: return 1;

                default:
                    return 0;
            }
        }
    }
}