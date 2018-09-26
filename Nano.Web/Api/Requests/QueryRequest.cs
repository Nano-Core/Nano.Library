using DynamicExpression.Interfaces;
using Nano.Models.Criterias;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Query Request.
    /// </summary>
    public class QueryRequest : BaseRequestJson
    {
        /// <summary>
        /// Query Criteria.
        /// </summary>
        public virtual IQueryCriteria QueryCriteria { get; set; } = new DefaultQueryCriteria();

        /// <summary>
        /// Constructor.
        /// </summary>
        public QueryRequest()
        {
            this.Action = "query";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.QueryCriteria;
        }
    }
}