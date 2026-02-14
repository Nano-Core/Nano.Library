using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi;
using Nano.App.Api.Mvc.Documentation.Consts;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for API documentation (Swagger/OpenAPI).
/// </summary>
public class DocumentationOptions
{
    /// <summary>
    /// Name of the application or API.
    /// </summary>
    [Required]
    public virtual string Name { get; set; } = AppDefaults.DEFAULT_APP_NAME;

    /// <summary>
    /// Description of the application or API.
    /// </summary>
    public virtual string? Description { get; set; }

    /// <summary>
    /// URL or text for terms of service.
    /// </summary>
    [Url]
    public virtual string? TermsOfServiceUrl { get; set; }

    /// <summary>
    /// Contact information for the API.
    /// </summary>
    public virtual OpenApiContact? Contact { get; set; }

    /// <summary>
    /// License information for the API.
    /// </summary>
    public virtual OpenApiLicense? License { get; set; }

    /// <summary>
    /// Optional Content Security Policy nonce.
    /// </summary>
    public virtual string? CspNonce { get; set; }

    /// <summary>
    /// hide default API version if true.
    /// Default version routes will be hidden in swagger.
    /// </summary>
    [Required]
    public virtual bool HideDefaultVersion { get; set; } = true;
}