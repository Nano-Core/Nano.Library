namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Update Many Bulk Request.
/// </summary>
public class EditManyBulkRequest : EditManyRequest
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public EditManyBulkRequest()
    {
        this.Action = "edit/many/bulk";
    }
}