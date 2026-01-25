namespace Nano.App.Api.Config;

/// <summary>
/// Options for external login providers.
/// </summary>
public class ExternalLoginOptions
{
    /// <summary>
    /// Google login configuration.
    /// </summary>
    public virtual GoogleOptions? Google { get; set; }

    /// <summary>
    /// Facebook login configuration.
    /// </summary>
    public virtual FacebookOptions? Facebook { get; set; }

    /// <summary>
    /// Microsoft login configuration.
    /// </summary>
    public virtual MicrosoftOptions? Microsoft { get; set; }

    /// <summary>
    /// Indicates whether at least one external login provider is configured.
    /// </summary>
    public virtual bool IsConfigured => this.Google != null || this.Facebook != null || this.Microsoft != null;
}