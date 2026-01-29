using System.ComponentModel.DataAnnotations;
using DynamicExpression.Entities;
using DynamicExpression.Interfaces;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to index entities using a query.
/// </summary>
[PostAction(ActionRoutes.INDEX)]
public class IndexRequest : BaseRequest
{
    /// <summary>
    /// The query defining entities to index.
    /// </summary>
    [Required]
    [Body]
    public virtual IQuery Query { get; set; } = new Query();

    /// <summary>
    /// Optional depth for including related entities.
    /// </summary>
    [Query]
    public virtual int? IncludeDepth { get; set; }
}