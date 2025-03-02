using Nano.App.Api.Requests.Attributes;

namespace Nano.App.Api.Requests.Identity;

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