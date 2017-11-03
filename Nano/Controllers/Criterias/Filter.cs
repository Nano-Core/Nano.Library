using System;
using System.Collections.Generic;
using Nano.Controllers.Criterias.Enums;
using Nano.Controllers.Criterias.Interfaces;

namespace Nano.Controllers.Criterias
{
    /// <summary>
    /// Filter.
    /// </summary>
    public class Filter
	{
        /// <summary>
        /// Statements
        /// </summary>
        internal virtual List<IFilterStatement> Statements { get; } = new List<IFilterStatement>();

	    /// <summary>
	    /// Add <see cref="Operation.Equal"/> filter.
	    /// </summary>
	    /// <typeparam name="TPropertyType">The type of the property.</typeparam>
	    /// <param name="property">The property name.</param>
	    /// <param name="value">The value.</param>
	    /// <param name="logical">The <see cref="OperationLogical"/>.</param>
	    public virtual void Equal<TPropertyType>(string property, TPropertyType value, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By(property, Operation.Equal, value, default, logical);
	    }

        /// <summary>
        /// Add <see cref="Operation.NotEqual"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="value">The value.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void NotEqual<TPropertyType>(string property, TPropertyType value, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By(property, Operation.NotEqual, value, default, logical);
	    }

        /// <summary>
        /// Add <see cref="Operation.Contains"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="value">The value.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void Contains<TPropertyType>(string property, TPropertyType value, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By(property, Operation.Contains, value, default, logical);
	    }

        /// <summary>
        /// Add <see cref="Operation.StartsWith"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="value">The value.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void StartsWith<TPropertyType>(string property, TPropertyType value, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By(property, Operation.StartsWith, value, default, logical);
	    }

        /// <summary>
        /// Add <see cref="Operation.EndsWith"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="value">The value.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void EndsWith<TPropertyType>(string property, TPropertyType value, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By(property, Operation.EndsWith, value, default, logical);
	    }

        /// <summary>
        /// Add <see cref="Operation.GreaterThan"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="value">The value.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void GreaterThan<TPropertyType>(string property, TPropertyType value, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By(property, Operation.GreaterThan, value, default, logical);
	    }

        /// <summary>
        /// Add <see cref="Operation.GreaterThanOrEqualTo"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="value">The value.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void GreaterThanOrEqualTo<TPropertyType>(string property, TPropertyType value, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By(property, Operation.GreaterThanOrEqualTo, value, default, logical);
	    }

        /// <summary>
        /// Add <see cref="Operation.LessThan"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="value">The value.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void LessThan<TPropertyType>(string property, TPropertyType value, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By(property, Operation.LessThan, value, default, logical);
	    }

        /// <summary>
        /// Add <see cref="Operation.LessThanOrEqualTo"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="value">The value.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void LessThanOrEqualTo<TPropertyType>(string property, TPropertyType value, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By(property, Operation.LessThanOrEqualTo, value, default, logical);
	    }

        /// <summary>
        /// Add <see cref="Operation.Between"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="value">The value.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void Between<TPropertyType>(string property, TPropertyType value, TPropertyType value2, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By(property, Operation.Between, value, value2, logical);
	    }

	    /// <summary>
	    /// Add <see cref="Operation.IsNull"/> filter.
	    /// </summary>
	    /// <typeparam name="TPropertyType">The type of the property.</typeparam>
	    /// <param name="property">The property name.</param>
	    /// <param name="logical">The <see cref="OperationLogical"/>.</param>
	    public virtual void IsNull<TPropertyType>(string property, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By<TPropertyType>(property, Operation.IsNull, default, default, logical);
	    }

        /// <summary>
        /// Add <see cref="Operation.IsEmpty"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void IsEmpty<TPropertyType>(string property, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By<TPropertyType>(property, Operation.IsEmpty, default, default, logical);
	    }
       
        /// <summary>
        /// Add <see cref="Operation.IsNullOrWhiteSpace"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void IsNullOrWhiteSpace<TPropertyType>(string property, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By<TPropertyType>(property, Operation.IsNullOrWhiteSpace, default, default, logical);
	    }
        
        /// <summary>
        /// Add <see cref="Operation.IsNotNull"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void IsNotNull<TPropertyType>(string property, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By<TPropertyType>(property, Operation.IsNotNull, default, default, logical);
	    }
        
        /// <summary>
        /// Add <see cref="Operation.IsNotEmpty"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void IsNotEmpty<TPropertyType>(string property, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By<TPropertyType>(property, Operation.IsNotEmpty, default, default, logical);
	    }
        
        /// <summary>
        /// Add <see cref="Operation.IsNotNullNorWhiteSpace"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void IsNotNullNorWhiteSpace<TPropertyType>(string property, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By<TPropertyType>(property, Operation.IsNotNullNorWhiteSpace, default, default, logical);
	    }

        /// <summary>
        /// Add <see cref="Operation.In"/> filter.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="value">The value.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
        public virtual void In<TPropertyType>(string property, TPropertyType value, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        this.By(property, Operation.In, value, default, logical);
	    }

        /// <summary>
        /// Adds a new <see cref="FilterStatement{TPropertyType}" /> to the <see cref="Filter" />.
        /// </summary>
        /// <typeparam name="TPropertyType"></typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="operation">The <see cref="Operation"/>.</param>
        /// <param name="value">The value.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="logical">The <see cref="OperationLogical"/>.</param>
	    public virtual void By<TPropertyType>(string property, Operation operation, TPropertyType value, TPropertyType value2 = default, OperationLogical logical = OperationLogical.And)
	    {
	        if (property == null)
	            throw new ArgumentNullException(nameof(property));

	        var statement = new FilterStatement<TPropertyType>(property, operation, value, value2, logical);

	        this.Statements.Add(statement);
	    }
    }
}
