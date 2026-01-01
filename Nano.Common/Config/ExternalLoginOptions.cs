namespace Nano.Common.Config;

/// <summary>
/// External Login Options.
/// </summary>
public class ExternalLoginOptions
{
    /// <summary>
    /// Google.
    /// </summary>
    public virtual GoogleOptions Google { get; set; }

    /// <summary>
    /// Facebook.
    /// </summary>
    public virtual FacebookOptions Facebook { get; set; }

    /// <summary>
    /// Microsoft.
    /// </summary>
    public virtual MicrosoftOptions Microsoft { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual bool IsConfigured => this.Google != null || this.Facebook != null || this.Microsoft != null;
}