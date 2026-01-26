namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a bulk request to update many entities.
/// </summary>
public class EditManyBulkRequest : EditManyRequest
{
    /// <summary>
    /// Initializes a new instance of <see cref="EditManyBulkRequest"/>.
    /// </summary>
    public EditManyBulkRequest()
    {
        this.Action = "edit/many/bulk";
    }
}