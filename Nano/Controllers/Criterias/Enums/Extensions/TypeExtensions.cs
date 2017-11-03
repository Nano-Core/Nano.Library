using System;
using System.Collections.Generic;
using System.Linq;

namespace Nano.Controllers.Criterias.Enums.Extensions
{
    /// <summary>
    /// Type Extensions.
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly Dictionary<OperationType, HashSet<Type>> operationTypes = new Dictionary<OperationType, HashSet<Type>>
        {
            { OperationType.Text, new HashSet<Type> { typeof(string), typeof(char) } },
            { OperationType.Number, new HashSet<Type> { typeof(int), typeof(uint), typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
            { OperationType.Boolean, new HashSet<Type> { typeof(bool) } },
            { OperationType.Date, new HashSet<Type> { typeof(DateTime), typeof(DateTimeOffset) } },
            { OperationType.Nullable, new HashSet<Type> { typeof(Nullable<>) } }
        };

        /// <summary>
        /// Gets the <see cref="Operation"/> supported by the <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <returns>The <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<Operation> GetOperationTypes(this Type type)
        {
            var operations = new List<Operation>();
            var typeName = type.IsArray ? type.GetElementType().Name : type.Name;
            var operationType = TypeExtensions.operationTypes.FirstOrDefault(i => i.Value.Any(v => v.Name == typeName)).Key;

            switch (operationType)
            {
                case OperationType.Default:
                    operations.AddRange(new[] { Operation.Equal, Operation.NotEqual });
                    break;

                case OperationType.Text:
                    operations.AddRange(new[] { Operation.Contains, Operation.EndsWith, Operation.Equal, Operation.IsEmpty, Operation.IsNotEmpty, Operation.IsNotNull, Operation.IsNotNullNorWhiteSpace, Operation.IsNull, Operation.IsNullOrWhiteSpace, Operation.NotEqual, Operation.StartsWith });
                    break;

                case OperationType.Number:
                    operations.AddRange(new[] { Operation.Between, Operation.Equal, Operation.GreaterThan, Operation.GreaterThanOrEqualTo, Operation.LessThan, Operation.LessThanOrEqualTo, Operation.NotEqual });
                    break;

                case OperationType.Boolean:
                    operations.AddRange(new[] { Operation.Equal, Operation.NotEqual });
                    break;

                case OperationType.Date:
                    operations.AddRange(new[] { Operation.Between, Operation.Equal, Operation.GreaterThan, Operation.GreaterThanOrEqualTo, Operation.LessThan, Operation.LessThanOrEqualTo, Operation.NotEqual });
                    break;
                case OperationType.Nullable:
                    operations.AddRange(new[] { Operation.IsNull, Operation.IsNotNull });
                    operations.AddRange(Nullable.GetUnderlyingType(type).GetOperationTypes());
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (type.IsArray)
                operations.Add(Operation.In);

            return operations;
        }
    }
}