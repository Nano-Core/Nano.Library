using System;
using Nano.Services.Globale.Models;

namespace Nano.Services.Globale.Data.Collections
{
    /// <summary>
    /// Internal class containing an <see cref="Array"/> of <see cref="Currency"/>'s. 
    /// Used by <see cref="Currency.Get(string)"/> to retrieve currency data.
    /// </summary>
    internal static class Currencies
    {
        /// <summary>
        /// An <see cref="Array"/> of <see cref="Currency"/>'s. 
        /// </summary>
        internal static Currency[] List => new[]
        {
            new Currency { Iso4217 = "DKK", NativeName = "Dansk krone", UniversalName = "Danish Krone", Symbol = "Kr.", Rate = 1.000M },
            new Currency { Iso4217 = "NOK", NativeName = "Norsk krone", UniversalName = "Norwegian Krone", Symbol = "Kr.", Rate = 1.000M },
            new Currency { Iso4217 = "SEK", NativeName = "Svensk krona", UniversalName = "Swedish Krona", Symbol = "Kr.", Rate = 1.000M },
            new Currency { Iso4217 = "EUR", NativeName = "Euro", UniversalName = "Euro", Symbol = "€", Rate = 1.000M },
            new Currency { Iso4217 = "BAM", NativeName = "Bosnia and Herzegovina convertible mark", UniversalName = "Bosnia and Herzegovina convertible mark", Symbol = "KM", Rate = 1.000M },
            new Currency { Iso4217 = "HUF", NativeName = "Hungarian forint", UniversalName = "Hungarian forint", Symbol = "Ft", Rate = 1.000M },
            new Currency { Iso4217 = "HRK", NativeName = "Croatian kuna", UniversalName = "Croatian kuna", Symbol = "kn", Rate = 1.000M },
            new Currency { Iso4217 = "ISK", NativeName = "Icelandic króna", UniversalName = "Icelandic króna", Symbol = "Kr.", Rate = 1.000M },
            new Currency { Iso4217 = "LTL", NativeName = "Lithuanian litas", UniversalName = "Lithuanian litas", Symbol = "Lt.", Rate = 1.000M },
            new Currency { Iso4217 = "LVL", NativeName = "Latvian lats", UniversalName = "Latvian lats", Symbol = "Ls.", Rate = 1.000M },
            new Currency { Iso4217 = "PLN", NativeName = "Polish złoty", UniversalName = "Polish zloty", Symbol = "zł", Rate = 1.000M },
            new Currency { Iso4217 = "RON", NativeName = "Romanian leu", UniversalName = "Romanian leu", Symbol = "lue", Rate = 1.000M },
            new Currency { Iso4217 = "RSD", NativeName = "Serbian dinar", UniversalName = "Serbian dinar", Symbol = "RSD", Rate = 1.000M },
            new Currency { Iso4217 = "UAH", NativeName = "Ukrainian hryvnia", UniversalName = "Ukrainian hryvnia", Symbol = "₴", Rate = 1.000M },
            new Currency { Iso4217 = "CHF", NativeName = "Swiss franc", UniversalName = "Swiss franc", Symbol = "CHF", Rate = 1.000M },
            new Currency { Iso4217 = "GBP", NativeName = "Pound sterling", UniversalName = "Pound sterling", Symbol = "£", Rate = 1.000M },
            new Currency { Iso4217 = "BGN", NativeName = "Bulgarian lev", UniversalName = "Bulgarian lev", Symbol = "лв", Rate = 1.000M },
            new Currency { Iso4217 = "USD", NativeName = "American Dollars", UniversalName = "American Dollars", Symbol = "$", Rate = 1.000M },
            new Currency { Iso4217 = "AUD", NativeName = "Austrailian Dollars", UniversalName = "Austrailian Dollars", Symbol = "$", Rate = 1.000M },
            new Currency { Iso4217 = "NZD", NativeName = "New Zealand Dollars", UniversalName = "New Zealand Dollars", Symbol = "$", Rate = 1.000M },
            new Currency { Iso4217 = "CAD", NativeName = "Canadian Dollars", UniversalName = "Canadian Dollars", Symbol = "$", Rate = 1.000M }
        };
    }
}