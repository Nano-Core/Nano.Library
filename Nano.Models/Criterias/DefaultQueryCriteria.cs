using System;
using System.Collections.Generic;
using DynamicExpression;

namespace Nano.Models.Criterias
{
    /// <inheritdoc />
    public class DefaultQueryCriteria : BaseQueryCriteria
    {
        /// <summary>
        /// After At.
        /// </summary>
        public virtual DateTimeOffset? AfterAt { get; set; }

        /// <summary>
        /// Before At.
        /// </summary>
        public virtual DateTimeOffset? BeforeAt { get; set; }

        /// <inheritdoc />
        public override IList<CriteriaExpression> GetExpressions()
        {
            var expressions = base.GetExpressions();

            var expression = new CriteriaExpression();

            if (this.BeforeAt.HasValue)
                expression.LessThanOrEqual("CreatedAt", this.BeforeAt);

            if (this.AfterAt.HasValue)
                expression.GreaterThanOrEqual("CreatedAt", this.AfterAt);

            expressions
                .Add(expression);

            return expressions;
        }
    }
}