using System;

namespace Nano.Data.Attributes
{
    /// <summary>
    /// Import Attribute.
    /// Types with this annotation invokes an import of data returned by the <see cref="Uri"/> during application startup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DataImportAttribute : Attribute
    {
        /// <summary>
        /// Uri.
        /// </summary>
        public virtual Uri Uri { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/>.</param>
        public DataImportAttribute(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            this.Uri = uri;
        }
    }
}