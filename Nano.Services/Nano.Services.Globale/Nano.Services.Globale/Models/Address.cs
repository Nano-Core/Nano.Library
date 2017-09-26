using System.Text.RegularExpressions;
using Nano.Models;

namespace Nano.Services.Globale.Models
{
    /// <summary>
    /// Address.
    /// </summary>
    public partial class Address : DefaultEntitySpatial
    {
        /// <summary>
        /// Address String.
        /// </summary>
        public virtual string AddressString { get; set; }

        /// <summary>
        /// City.
        /// </summary>
        public virtual City City { get; set; }

        /// <summary>
        /// Time Zone.
        /// </summary>
        public virtual TimeZone TimeZone { get; set; }

        /// <summary>
        /// Details.
        /// </summary>
        public virtual AddressDetails Details { get; set; }

        /// <summary>
        /// Address Details.
        /// </summary>
        public class AddressDetails
        {
            /// <summary>
            /// Street Name.
            /// </summary>
            public virtual string StreetName { get; set; }

            /// <summary>
            /// Place Name.
            /// </summary>
            public virtual string PlaceName { get; set; }

            /// <summary>
            /// House Number.
            /// </summary>
            public virtual string HouseNumber { get; set; }

            /// <summary>
            /// Floor Number.
            /// </summary>
            public virtual string FloorNumber { get; set; }

            /// <summary>
            /// Apartment Number.
            /// </summary>
            public virtual string ApartmentNumber { get; set; }

            /// <summary>
            /// Condominium Number.
            /// </summary>
            public virtual string CondominiumNumber { get; set; }

            /// <summary>
            /// Area Name.
            /// </summary>
            public virtual string AreaName { get; set; }

            /// <summary>
            /// Comment.
            /// </summary>
            public virtual string Comment { get; set; }

            /// <summary>
            /// Extract the house letter from <see cref="AddressDetails.HouseNumber"/>.
            /// </summary>
            /// <param name="houseNumber">The house number (raw).</param>
            /// <returns>Returns any letter associated with the <see cref="AddressDetails.HouseNumber"/>.</returns>
            public static string GetHouseLetter(string houseNumber)
            {
                return string.IsNullOrEmpty(houseNumber)
                    ? null
                    : new Regex("(.*)\\d+", RegexOptions.IgnoreCase).Replace(houseNumber, string.Empty);
            }

            /// <summary>
            /// Extract the house number from <see cref="AddressDetails.HouseNumber"/>.
            /// </summary>
            /// <param name="houseNumber">The house number (raw).</param>
            /// <returns>Returns the number part associated with the <see cref="AddressDetails.HouseNumber"/>.</returns>
            public static string GetHouseNumber(string houseNumber)
            {
                return string.IsNullOrWhiteSpace(houseNumber)
                    ? null
                    : new Regex("\\D+(.*)", RegexOptions.IgnoreCase).Replace(houseNumber, "");
            }
        }
    }
}