using Nano.Models;

namespace Nano.Services.Globale.Models
{
    /// <summary>
    /// City.
    /// </summary>
    public partial class City : DefaultEntity
    {
        /// <summary>
        /// Name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Zip Code.
        /// </summary>
        public virtual string ZipCode { get; set; }

        /// <summary>
        /// Country.
        /// </summary>
        public virtual Country Country { get; set; }
    }
}