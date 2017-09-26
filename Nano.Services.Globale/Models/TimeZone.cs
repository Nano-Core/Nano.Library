using Nano.Models;

namespace Nano.Services.Globale.Models
{
    /// <summary>
    /// Time Zone.
    /// https://en.wikipedia.org/wiki/Tz_database
    /// </summary>
    public class TimeZone : DefaultEntity
    {
        /// <summary>
        /// Required.
        /// The 'Olson' name.
        /// </summary>
        public virtual string OlsonName { get; set; }

        /// <summary>
        /// Required.
        /// The 'Microsoft' name.
        /// </summary>
        public virtual string MicrosoftName { get; set; }
    }
}