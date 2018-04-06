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
        public override CriteriaExpression GetExpression<TEntity>()
        {
            var filter = base.GetExpression<TEntity>();

            if (this.CreatedBy != null)
                filter.Equal("CreatedBy", this.CreatedBy);

            if (this.CreatedDateTo != null)
                filter.LessThanOrEqual("CreatedDate", this.CreatedDateTo);

            if (this.CreatedDateFrom != null)
                filter.GreaterThanOrEqual("CreatedDate", this.CreatedDateFrom);

            if (this.EntityTypeName != null)
                filter.Equal("EntityTypeName", this.EntityTypeName);

            if (this.State != null)
                filter.Equal("State", this.State);

            if (this.RequestId != null)
                filter.Equal("RequestId", this.RequestId);

            return filter;
        }
    }
}