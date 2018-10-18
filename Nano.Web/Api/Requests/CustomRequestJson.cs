namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Custom Request Json.
    /// </summary>
    public class CustomRequestJson : BaseRequestJson
    {
        /// <summary>
        /// Body.
        /// The body of the request.
        /// </summary>
        public virtual object Body { get; set; } 

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.Body;
        }
    }
}