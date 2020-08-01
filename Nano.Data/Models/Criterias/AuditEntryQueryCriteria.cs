using System.Collections.Generic;
using DynamicExpression;
using Nano.Models.Criterias;

namespace Nano.Data.Models.Criterias
{
    /// <inheritdoc />
    public class AuditEntryQueryCriteria : DefaultQueryCriteria
    {
        /// <summary>
        /// Created By.
        /// </summary>
        public virtual string CreatedBy { get; set; }

        /// <summary>
        /// Entity Type Name.
        /// </summary>
        public virtual string EntityTypeName { get; set; }

        /// <summary>
        /// State.
        /// </summary>
        public virtual string State { get; set; }

        /// <summary>
        /// Request Id.
        /// </summary>
        public virtual string RequestId { get; set; }

        /// <inheritdoc />
        public override IList<CriteriaExpression> GetExpressions()
        {
            var expressions = base.GetExpressions();

            var expression = new CriteriaExpression();

            if (this.CreatedBy != null)
                expression.Equal("CreatedBy", this.CreatedBy);

            if (this.EntityTypeName != null)
                expression.Equal("EntityTypeName", this.EntityTypeName);

            if (this.State != null)
                expression.Equal("State", this.State);

            if (this.RequestId != null)
                expression.Equal("RequestId", this.RequestId);

            expressions
                .Add(expression);

            return expressions;
        }
    }
}