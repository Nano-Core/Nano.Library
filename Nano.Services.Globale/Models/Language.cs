using System;
using System.Collections.Generic;
using System.Linq;
using Nano.Models;
using Nano.Services.Globale.Models.Collections;

namespace Nano.Services.Globale.Models
{
    /// <summary>
    /// Language.
    /// https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes#Partial_ISO_639_table
    /// </summary>
    public class Language : DefaultEntity
    {
        /// <summary>
        /// Navtive Name.
        /// </summary>
        public virtual string NativeName { get; set; }

        /// <summary>
        /// Universal Name.
        /// </summary>
        public virtual string UniversalName { get; set; }

        /// <summary>
        /// The 'ISO 639-1' code.
        /// </summary>
        public virtual string Iso639_1 { get; set; }

        /// <summary>
        /// The 'ISO 639-2B' code.
        /// </summary>
        public virtual string Iso639_2B { get; set; }

        /// <summary>
        /// The 'ISO 639-2T' code.
        /// </summary>
        public virtual string Iso639_2T { get; set; }

        /// <summary>
        /// The 'ISO 639-3' code.
        /// </summary>
        public virtual string Iso639_3 { get; set; }

        /// <summary>
        /// The 'ISO 639-6' code.
        /// </summary>
        public virtual string Iso639_6 { get; set; }

        /// <summary>
        /// The <see cref="Country"/>'s using this <see cref="Language"/>. 
        /// </summary>
        public virtual IEnumerable<Country> Countries { get; set; }

        /// <summary>
        /// Get the <see cref="Language"/> matchig the passed <paramref name="iso639_1"/> code. 
        /// </summary>
        /// <param name="iso639_1">The code for the <see cref="Language"/> to return.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="iso639_1"/> is null or empty.</exception>
        /// <returns>The <see cref="Language"/> matching the passed <paramref name="iso639_1"/> code. Returns null if no <see cref="Language"/> is found.</returns>
        public static Language Get(string iso639_1)
        {
            if (string.IsNullOrEmpty(iso639_1))
                throw new ArgumentNullException(nameof(iso639_1));

            return Languages.List.FirstOrDefault(x => x.Iso639_1 == iso639_1);
        }
    }
}