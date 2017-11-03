using Nano.Models.Enums.Extensions.Enums;

namespace Nano.Models.Enums.Extensions
{
    /// <summary>
    /// Country Extensions.
    /// </summary>
    public static class CountryExtensions
    {
        /// <summary>
        /// Gets the code of the passed <paramref name="type"/>, matching the passed <paramref name="country"/>.
        /// </summary>
        /// <param name="country">The <see cref="Country"/>.</param>
        /// <param name="type">The <see cref="CountryCodeType"/>.</param>
        /// <returns>The <see cref="string"/> code matching the passed <paramref name="country"/>.</returns>
        public static string ToCode(this Country country, CountryCodeType type = CountryCodeType.IsoAlpha2)
        {
            switch (type)
            {
                case CountryCodeType.IsoAlpha2:
                    switch (country)
                    {
                        case Country.Afghanistan: return "AF";
                        case Country.Albania: return "AL";
                        case Country.Algeria: return "DZ";
                        case Country.AmericanSamoa: return "AS";
                        case Country.Andorra: return "AD";
                        case Country.Angola: return "AO";
                        case Country.Anguilla: return "AI";
                        case Country.Antarctica: return "AQ";
                        case Country.AntiguaAndBarbuda: return "AG";
                        case Country.Argentina: return "AR";
                        case Country.Armenia: return "AM";
                        case Country.Aruba: return "AW";
                        case Country.Australia: return "AU";
                        case Country.Austria: return "AT";
                        case Country.Azerbaijan: return "AZ";
                        case Country.Bahamas: return "BS";
                        case Country.Bahrain: return "BH";
                        case Country.Bangladesh: return "BD";
                        case Country.Barbados: return "BB";
                        case Country.Belarus: return "BY";
                        case Country.Belgium: return "BE";
                        case Country.Belize: return "BZ";
                        case Country.Benin: return "BJ";
                        case Country.Bermuda: return "BM";
                        case Country.Bhutan: return "BT";
                        case Country.Bolivia: return "BO";
                        case Country.BosniaAndHerzegovina: return "BA";
                        case Country.Botswana: return "BW";
                        case Country.BouvetIsland: return "BV";
                        case Country.Brazil: return "BR";
                        case Country.BritishIndianOceanTerritory: return "IO";
                        case Country.BruneiDarussalam: return "BN";
                        case Country.Bulgaria: return "BG";
                        case Country.BurkinaFaso: return "BF";
                        case Country.Burundi: return "BI";
                        case Country.Cambodia: return "KH";
                        case Country.Cameroon: return "CM";
                        case Country.Canada: return "CA";
                        case Country.CapeVerde: return "CV";
                        case Country.CaymanIslands: return "KY";
                        case Country.CentralAfricanRepublic: return "CF";
                        case Country.Chad: return "TD";
                        case Country.Chile: return "CL";
                        case Country.China: return "CN";
                        case Country.ChristmasIsland: return "CX";
                        case Country.CocosIslands: return "CC";
                        case Country.Colombia: return "CO";
                        case Country.Comoros: return "KM";
                        case Country.Congo: return "CG";
                        case Country.TheDemocraticRepublicOfCongo: return "CD";
                        case Country.CookIslands: return "CK";
                        case Country.CostaRica: return "CR";
                        case Country.CoteDivoire: return "CI";
                        case Country.Croatia: return "HR";
                        case Country.Cuba: return "CU";
                        case Country.Cyprus: return "CY";
                        case Country.CzechRepublic: return "CZ";
                        case Country.Denmark: return "DK";
                        case Country.Djibouti: return "DJ";
                        case Country.Dominica: return "DM";
                        case Country.DominicanRepublic: return "DO";
                        case Country.EastTimor: return "TP";
                        case Country.Ecuador: return "EC";
                        case Country.Egypt: return "EG";
                        case Country.ElSalvador: return "SV";
                        case Country.EquatorialGuinea: return "GQ";
                        case Country.Eritrea: return "ER";
                        case Country.Estonia: return "EE";
                        case Country.Ethiopia: return "ET";
                        case Country.EuropeanUnion: return "EU";
                        case Country.FalklandIslAndsMalvinas: return "FK";
                        case Country.FaroeIslands: return "FO";
                        case Country.Fiji: return "FJ";
                        case Country.Finland: return "FI";
                        case Country.France: return "FR";
                        case Country.FranceMetropolitan: return "FX";
                        case Country.FrenchGuiana: return "GF";
                        case Country.FrenchPolynesia: return "PF";
                        case Country.FrenchSouthernTerritories: return "TF";
                        case Country.Gabon: return "GA";
                        case Country.Gambia: return "GM";
                        case Country.Georgia: return "GE";
                        case Country.Germany: return "DE";
                        case Country.Ghana: return "GH";
                        case Country.Gibraltar: return "GI";
                        case Country.Greece: return "GR";
                        case Country.Greenland: return "GL";
                        case Country.Grenada: return "GD";
                        case Country.Guadeloupe: return "GP";
                        case Country.Guam: return "GU";
                        case Country.Guatemala: return "GT";
                        case Country.Guinea: return "GN";
                        case Country.GuineaBissau: return "GW";
                        case Country.Guyana: return "GY";
                        case Country.Haiti: return "HT";
                        case Country.HeardIslandAndMcdonaldIslands: return "HM";
                        case Country.VaticanCityState: return "VA";
                        case Country.Honduras: return "HN";
                        case Country.HongKong: return "HK";
                        case Country.Hungary: return "HU";
                        case Country.Iceland: return "IS";
                        case Country.India: return "IN";
                        case Country.Indonesia: return "ID";
                        case Country.Iran: return "IR";
                        case Country.Iraq: return "IQ";
                        case Country.Ireland: return "IE";
                        case Country.Israel: return "IL";
                        case Country.Italy: return "IT";
                        case Country.Jamaica: return "JM";
                        case Country.Japan: return "JP";
                        case Country.Jordan: return "JO";
                        case Country.Kazakhstan: return "KZ";
                        case Country.Kenya: return "KE";
                        case Country.Kiribati: return "KI";
                        case Country.DemocraticPeoplesRepublicOfKorea: return "KP";
                        case Country.RepublicOfKorea: return "KR";
                        case Country.Kuwait: return "KW";
                        case Country.Kyrgyzstan: return "KG";
                        case Country.LaoPeoplesDemocraticRepublic: return "LA";
                        case Country.Latvia: return "LV";
                        case Country.Lebanon: return "LB";
                        case Country.Lesotho: return "LS";
                        case Country.Liberia: return "LR";
                        case Country.LibyanArabJamahiriya: return "LY";
                        case Country.Liechtenstein: return "LI";
                        case Country.Lithuania: return "LT";
                        case Country.Luxembourg: return "LU";
                        case Country.Macao: return "MO";
                        case Country.Macedonia: return "MK";
                        case Country.Madagascar: return "MG";
                        case Country.Malawi: return "MW";
                        case Country.Malaysia: return "MY";
                        case Country.Maldives: return "MV";
                        case Country.Mali: return "ML";
                        case Country.Malta: return "MT";
                        case Country.MarshallIslands: return "MH";
                        case Country.Martinique: return "MQ";
                        case Country.Mauritania: return "MR";
                        case Country.Mauritius: return "MU";
                        case Country.Mayotte: return "YT";
                        case Country.Mexico: return "MX";
                        case Country.Micronesia: return "FM";
                        case Country.Moldova: return "MD";
                        case Country.Monaco: return "MC";
                        case Country.Mongolia: return "MN";
                        case Country.Montserrat: return "MS";
                        case Country.Morocco: return "MA";
                        case Country.Mozambique: return "MZ";
                        case Country.Myanmar: return "MM";
                        case Country.Namibia: return "NA";
                        case Country.Nauru: return "NR";
                        case Country.Nepal: return "NP";
                        case Country.Netherlands: return "NL";
                        case Country.NetherlandsAntilles: return "AN";
                        case Country.NewCaledonia: return "NC";
                        case Country.NewZealand: return "NZ";
                        case Country.Nicaragua: return "NI";
                        case Country.Niger: return "NE";
                        case Country.Nigeria: return "NG";
                        case Country.Niue: return "NU";
                        case Country.NorfolkIsland: return "NF";
                        case Country.NorthernMarianaIslands: return "MP";
                        case Country.Norway: return "NO";
                        case Country.Oman: return "OM";
                        case Country.Pakistan: return "PK";
                        case Country.Palau: return "PW";
                        case Country.PalestinianTerritory: return "PS";
                        case Country.Panama: return "PA";
                        case Country.PapuaNewGuinea: return "PG";
                        case Country.Paraguay: return "PY";
                        case Country.Peru: return "PE";
                        case Country.Philippines: return "PH";
                        case Country.Pitcairn: return "PN";
                        case Country.Poland: return "PL";
                        case Country.Portugal: return "PT";
                        case Country.PuertoRico: return "PR";
                        case Country.Qatar: return "QA";
                        case Country.Reunion: return "RE";
                        case Country.Romania: return "RO";
                        case Country.RussianFederation: return "RU";
                        case Country.Rwanda: return "RW";
                        case Country.SaintHelena: return "SH";
                        case Country.SaintKittsAndNevis: return "KN";
                        case Country.SaintLucia: return "LC";
                        case Country.SaintPierreAndMiquelon: return "PM";
                        case Country.SaintVincentAndtheGrenadines: return "VC";
                        case Country.Samoa: return "WS";
                        case Country.SanMarino: return "SM";
                        case Country.SaoTomeAndPrincipe: return "ST";
                        case Country.SaudiArabia: return "SA";
                        case Country.Senegal: return "SN";
                        case Country.SerbiaAndMontenegro: return "CS";
                        case Country.Seychelles: return "SC";
                        case Country.SierraLeone: return "SL";
                        case Country.Singapore: return "SG";
                        case Country.Slovakia: return "SK";
                        case Country.Slovenia: return "SI";
                        case Country.SolomonIslands: return "SB";
                        case Country.Somalia: return "SO";
                        case Country.SouthAfrica: return "ZA";
                        case Country.SouthGeorgiaAndTheSouthSAndwichIslands: return "GS";
                        case Country.Spain: return "ES";
                        case Country.SriLanka: return "LK";
                        case Country.Sudan: return "SD";
                        case Country.Suriname: return "SR";
                        case Country.SvalbardAndJanMayen: return "SJ";
                        case Country.Swaziland: return "SZ";
                        case Country.Sweden: return "SE";
                        case Country.Switzerland: return "CH";
                        case Country.SyrianArabRepublic: return "SY";
                        case Country.Taiwan: return "TW";
                        case Country.Tajikistan: return "TJ";
                        case Country.Tanzania: return "TZ";
                        case Country.Thailand: return "TH";
                        case Country.Togo: return "TG";
                        case Country.Tokelau: return "TK";
                        case Country.Tonga: return "TO";
                        case Country.TrinidadAndTobago: return "TT";
                        case Country.Tunisia: return "TN";
                        case Country.Turkey: return "TR";
                        case Country.Turkmenistan: return "TM";
                        case Country.TurksAndCaicosIslands: return "TC";
                        case Country.Tuvalu: return "TV";
                        case Country.Uganda: return "UG";
                        case Country.Ukraine: return "UA";
                        case Country.UnitedArabEmirates: return "AE";
                        case Country.UnitedKingdom: return "UK";
                        case Country.UnitedStates: return "US";
                        case Country.UnitedStatesMinorOutlyingIslands: return "UM";
                        case Country.Uruguay: return "UY";
                        case Country.Uzbekistan: return "UZ";
                        case Country.Vanuatu: return "VU";
                        case Country.Venezuela: return "VE";
                        case Country.Vietnam: return "VN";
                        case Country.VirginIslandsBritish: return "VG";
                        case Country.VirginIslandsUs: return "VI";
                        case Country.WallisandFutuna: return "WF";
                        case Country.WesternSahara: return "EH";
                        case Country.Yemen: return "YE";
                        case Country.Yugoslavia: return "YU";
                        case Country.Zambia: return "ZM";
                        case Country.Zimbabwe: return "ZW";
                        case Country.Unknown: return string.Empty;

                        default:
                            return string.Empty;
                    }

                // TODO: Country enum: Implement ToCountry for all CountryCodeType enum types. http://www.nationsonline.org/oneworld/country_code_list.htm
                case CountryCodeType.IsoAlpha3: return string.Empty;
                case CountryCodeType.UnM49Numerical: return string.Empty;

                default:
                    return string.Empty;
            }
        }
    }
}