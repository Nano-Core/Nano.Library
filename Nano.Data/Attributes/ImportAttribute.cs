using System;

namespace Nano.Data.Attributes
{
    /// <summary>
    /// Import Attribute.
    /// Types with this annotation invokes an import of data returned by the <see cref="Uri"/> during application startup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ImportAttribute : Attribute
    {
        /// <summary>
        /// Uri.
        /// </summary>
        public virtual Uri Uri { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="url">The url.</param>
        public ImportAttribute(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            this.Uri = new Uri(url);
        }
    }
}