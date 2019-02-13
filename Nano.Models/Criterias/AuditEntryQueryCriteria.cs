using System.Collections.Generic;
using DynamicExpression;

namespace Nano.Models.Criterias
{
    /// <inheritdoc />
    public class AuditEntryQueryCriteria : BaseQueryCriteria
    {
        /// <summary>
        /// Created By.
        /// </summary>
        public virtual string CreatedBy { get; set; }

        /// <summary>
        /// Create Date To.
        /// </summary>
        public virtual string CreatedDateTo { get; set; }

        /// <summary>
        /// Create Date From.
        /// </summary>
        public virtual string CreatedDateFrom { get; set; }

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

            if (this.CreatedDateTo != null)
                expression.LessThanOrEqual("CreatedDate", this.CreatedDateTo);

            if (this.CreatedDateFrom != null)
                expression.GreaterThanOrEqual("CreatedDate", this.CreatedDateFrom);

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