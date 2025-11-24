using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Deactivate User Request.
/// </summary>
public class DeactivateUserRequest<TIdentity> : BaseRequestDelete
{
    /// <summary>
    /// Id.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity Id { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public DeactivateUserRequest()
    {
        this.Action = "activate";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return null;
    }
}