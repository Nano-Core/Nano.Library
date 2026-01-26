using System.Collections.Generic;
using DynamicExpression;
using Nano.Data.Abstractions.Models;

namespace Nano.App.Api.Controllers.Models;

/// <summary>
/// Query criteria specifically for audit entries, extending <see cref="DefaultQueryCriteria"/>.
/// </summary>
public class AuditEntryQueryCriteria : DefaultQueryCriteria
{
    /// <summary>
    /// Filter by the creator of the audit entry.
    /// </summary>
    public virtual string? CreatedBy { get; set; }

    /// <summary>
    /// Filter by the entity type name of the audited entity.
    /// </summary>
    public virtual string? EntityTypeName { get; set; }

    /// <summary>
    /// Filter by the state of the audited entity.
    /// </summary>
    public virtual string? State { get; set; }

    /// <summary>
    /// Filter by the request identifier associated with the audit entry.
    /// </summary>
    public virtual string? RequestId { get; set; }

    /// <summary>
    /// Builds the list of <see cref="CriteriaExpression"/> instances including audit-specific filters.
    /// </summary>
    /// <returns>A list of <see cref="CriteriaExpression"/> representing the query conditions.</returns>
    public override IList<CriteriaExpression> GetExpressions()
    {
        var expressions = base.GetExpressions();

        var expression = new CriteriaExpression();

        if (this.CreatedBy != null)
        {
            expression
                .Equal(nameof(AuditEntry<>.CreatedBy), this.CreatedBy);
        }

        if (this.EntityTypeName != null)
        {
            expression
                .Equal(nameof(AuditEntry<>.EntityTypeName), this.EntityTypeName);
        }

        if (this.State != null)
        {
            expression
                .Equal(nameof(AuditEntry<>.State), this.State);
        }

        if (this.RequestId != null)
        {
            expression
                .Equal(nameof(AuditEntry<>.RequestId), this.RequestId);
        }

        expressions
            .Add(expression);

        return expressions;
    }
}