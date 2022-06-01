namespace Nano.Web.Api.Requests.Types
{
    /// <summary>
    /// Form Item.
    /// </summary>
    public class FormItem
    {
        /// <summary>
        /// Name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Value.
        /// </summary>
        public virtual object Value { get; set; }
    }
}