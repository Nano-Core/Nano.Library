using System;
using System.Collections.Generic;
using System.Linq;
using Nano.Models;
using Nano.Services.Globale.Models.Collections;

namespace Nano.Services.Globale.Models
{
    /// <summary>
    /// Currency.
    /// https://en.wikipedia.org/wiki/ISO_4217
    /// </summary>
    public class Currency : DefaultEntity
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
        /// The 'ISO-4217' Code.
        /// </summary>
        public virtual string Iso4217 { get; set; }

        /// <summary>
        /// The symbol to prepend / append when formatting the <see cref="Currency"/> value.
        /// </summary>
        public virtual string Symbol { get; set; }

        /// <summary>
        /// The rate index of the currency evaluation.
        /// </summary>
        public virtual decimal Rate { get; set; } = 1.0M;

        /// <summary>
        /// The <see cref="Country"/>'s using this <see cref="Currency"/>.
        /// </summary>
        public virtual IEnumerable<Country> Countries { get; set; }

        /// <summary>
        /// Get the <see cref="Currency"/> matchig the passed <paramref name="iso4217"/> code. 
        /// </summary>
        /// <param name="iso4217">The code for the <see cref="Currency"/> to return.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="iso4217"/> is null or empty.</exception>
        /// <returns>The <see cref="Currency"/> matching the passed <paramref name="iso4217"/> code. Returns null if no <see cref="Currency"/> is found.</returns>
        public static Currency Get(string iso4217)
        {
            if (string.IsNullOrEmpty(iso4217))
                throw new ArgumentNullException(nameof(iso4217));

            return Currencies.List.FirstOrDefault(x => x.Iso4217 == iso4217);
        }
    }
}