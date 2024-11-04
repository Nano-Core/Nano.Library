namespace Nano.App.Api.Requests;

/// <summary>
/// Create Many Bulk Request.
/// </summary>
public class CreateManyBulkRequest : CreateManyRequest
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public CreateManyBulkRequest()
    {
        this.Action = "create/many/bulk";
    }
}