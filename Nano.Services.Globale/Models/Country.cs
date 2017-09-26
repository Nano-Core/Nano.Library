using System;
using System.Collections.Generic;
using System.Linq;
using Nano.Models;
using Nano.Services.Globale.Models.Collections;

namespace Nano.Services.Globale.Models
{
    /// <summary>
    /// Country.
    /// http://www.nationsonline.org/oneworld/country_code_list.htm
    /// </summary>
    public class Country : DefaultEntity
    {
        /// <summary>
        /// Native Name.
        /// </summary>
        public virtual string NativeName { get; set; }

        /// <summary>
        /// Universal Name.
        /// </summary>
        public virtual string UniversalName { get; set; }

        /// <summary>
        /// The 'ISO ALPHA-2' Code.
        /// </summary>
        public virtual string IsoAlpha2 { get; set; }

        /// <summary>
        /// The 'ISO UN M49' Code.
        /// </summary>
        public virtual string IsoNumericUnM49 { get; set; }

        /// <summary>
        /// The prefix used for phone numbers.
        /// </summary>
        public virtual string PhoneNumberPrefix { get; set; }

        /// <summary>
        /// Is Metric System.
        /// </summary>
        public virtual bool IsMetricSystem { get; set; } = true;

        /// <summary>
        /// Is Imperial System.
        /// </summary>
        public virtual bool IsImperialSystem { get; set; } = false;

        /// <summary>
        /// The <see cref="Currency"/>.
        /// </summary>
        public virtual Currency Currency { get; set; }

        /// <summary>
        /// Language.
        /// </summary>
        public virtual Language Language { get; set; }

        /// <summary>
        /// The <see cref="City"/>'s assoicated with the <see cref="Country"/>.
        /// </summary>
        public virtual IEnumerable<City> Cities { get; set; }

        /// <summary>
        /// Get the <see cref="Country"/> matchig the passed <paramref name="isoAlpha2"/> code. 
        /// </summary>
        /// <param name="isoAlpha2">The code for the <see cref="Country"/> to return.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="isoAlpha2"/> is null or empty.</exception>
        /// <returns>The <see cref="Country"/> matching the passed <paramref name="isoAlpha2"/> code. Returns null if no <see cref="Country"/> is found.</returns>
        public static Country Get(string isoAlpha2)
        {
            if (string.IsNullOrEmpty(isoAlpha2))
                throw new ArgumentNullException(nameof(isoAlpha2));

            return Countries.List.FirstOrDefault(x => x.IsoAlpha2 == isoAlpha2);
        }
    }
}