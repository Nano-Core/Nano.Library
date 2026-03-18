using System;
using System.Collections.Generic;
using DynamicExpression;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Enums;

namespace Nano.App.Api.Controllers.Criteria;

/// <summary>
/// Query criteria specifically for audit entries, extending <see cref="BaseQueryCriteria"/>.
/// </summary>
public class AuditEntryQueryCriteria<TIdentity> : BaseQueryCriteria
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Filter by the entity key of the audit entry.
    /// </summary>
    public virtual TIdentity? EntityKey { get; set; }

    /// <summary>
    /// Filter by the entity type name of the audited entity.
    /// </summary>
    public virtual string? EntityTypeName { get; set; }

    /// <summary>
    /// Filter by the state of the audited entity.
    /// </summary>
    public virtual AuditState? State { get; set; }

    /// <summary>
    /// Filter by the creator of the audit entry.
    /// </summary>
    public virtual string? CreatedBy { get; set; }

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

        if (!EqualityComparer<TIdentity>.Default.Equals(this.EntityKey!, default!))
        {
            expression
                .Equal(nameof(AuditEntry<>.EntityKey), this.EntityKey!);
        }

        if (this.EntityTypeName != null)
        {
            expression
                .Equal(nameof(AuditEntry<>.EntityTypeName), this.EntityTypeName);
        }

        if (this.State != null)
        {
            expression
                .Equal(nameof(AuditEntry<>.EntityState), this.State);
        }

        if (this.CreatedBy != null)
        {
            expression
                .Equal(nameof(AuditEntry<>.CreatedBy), this.CreatedBy);
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