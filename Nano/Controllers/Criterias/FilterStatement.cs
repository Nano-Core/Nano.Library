using System;
using System.Collections.Generic;
using System.Linq;
using Nano.Controllers.Criterias.Enums;
using Nano.Controllers.Criterias.Enums.Extensions;
using Nano.Controllers.Criterias.Interfaces;

namespace Nano.Controllers.Criterias
{
    /// <inheritdoc />
    public class FilterStatement<TPropertyType> : IFilterStatement
    {
        /// <inheritdoc />
        public virtual string Property { get; set; }

        /// <inheritdoc />
        public virtual object Value { get; set; }

        /// <inheritdoc />
        public virtual object Value2 { get; set; }

        /// <inheritdoc />
        public virtual Operation Operation { get; set; }

        /// <inheritdoc />
        public virtual OperationLogical OperationLogical { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="property">The property name.</param>
        /// <param name="operation">The <see cref="Operation"/>.</param>
        /// <param name="value">The value.</param>
        /// <param name="value2">the value2</param>
        /// <param name="operationLogical">The <see cref="Enums.OperationLogical"/>.</param>
		public FilterStatement(string property, Operation operation, TPropertyType value, TPropertyType value2, OperationLogical operationLogical = OperationLogical.And)
		{
		    if (property == null)
                throw new ArgumentNullException(nameof(property));

		    if (!typeof(TPropertyType).GetOperationTypes().Contains(operation))
		        throw new InvalidOperationException(Operation.ToString());

            this.Property = property;
		    this.Operation = operation;

            if (typeof(TPropertyType).IsArray)
			{
				if (operation != Operation.Contains && operation != Operation.In)
                    throw new ArgumentException("Only 'Operation.Contains' and 'Operation.In' support arrays as parameters.");

				var type = typeof(List<>);
                var genericType = type.MakeGenericType(typeof(TPropertyType).GetElementType());

                this.Value = value != null ? Activator.CreateInstance(genericType, value) : null;
			    this.Value2 = value2 != null ? Activator.CreateInstance(genericType, value2) : null;
            }
			else
			{
				this.Value = value;
                this.Value2 = value2;
			}

		    this.OperationLogical = operationLogical;
        }
    }
}