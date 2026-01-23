using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi;
using Nano.App.Api.Mvc.Documentation.Consts;

namespace Nano.App.Api.Config;

/// <summary>
/// Documentation Options.
/// </summary>
public class DocumentationOptions
{
    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    public virtual string Name { get; set; } = AppDefaults.DEFAULT_APP_NAME;

    /// <summary>
    /// Description.
    /// </summary>
    public virtual string? Description { get; set; }

    /// <summary>
    /// Terms Of Service.
    /// </summary>
    public virtual string? TermsOfService { get; set; }

    /// <summary>
    /// Contact.
    /// </summary>
    public virtual OpenApiContact? Contact { get; set; }

    /// <summary>
    /// License.
    /// </summary>
    public virtual OpenApiLicense? License { get; set; }

    /// <summary>
    /// Csp Nonce.
    /// </summary>
    public virtual string? CspNonce { get; set; }

    /// <summary>
    /// Use Default Version.
    /// </summary>
    [Required]
    public virtual bool UseDefaultVersion { get; set; } = true;
}