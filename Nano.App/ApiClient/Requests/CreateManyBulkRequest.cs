namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a bulk request to create many entities.
/// </summary>
public class CreateManyBulkRequest : CreateManyRequest
{
    /// <summary>
    /// Initializes a new instance of <see cref="CreateManyBulkRequest"/>.
    /// </summary>
    public CreateManyBulkRequest()
    {
        this.Action = "create/many/bulk";
    }
}