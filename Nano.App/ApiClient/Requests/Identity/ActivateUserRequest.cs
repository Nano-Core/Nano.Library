using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Activate User Request.
/// </summary>
public class ActivateUserRequest<TIdentity> : BaseRequestPost
{
    /// <summary>
    /// Id.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity Id { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public ActivateUserRequest()
    {
        this.Action = "activate";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return null;
    }
}