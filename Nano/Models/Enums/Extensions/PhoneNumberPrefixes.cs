using System;
using System.Collections.Generic;
using System.Linq;

namespace Nano.Models.Enums.Extensions
{
    // TODO: Implement country, language timezone and (currecny) collection data, including timezone polygons.
    ///// <summary>
    ///// Internal class containing an <see cref="Array"/> of <see cref="Country"/>'s. 
    ///// Used by <see cref="Country.Get(string)"/> to retrieve country data.
    ///// </summary>
    //internal static class Countries
    //{
    //    /// <summary>
    //    /// An <see cref="Array"/> of <see cref="Country"/>'s. 
    //    /// </summary>
    //    internal static Country[] List => new[]
    //    {
    //        new Country { IsoAlpha2 = "AW", NativeName = "Aruba", UniversalName = "Aruba", PhoneNumberPrefix = "+297", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AX", NativeName = "Åland Islands", UniversalName = "Åland Islands", PhoneNumberPrefix = "+358", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AZ", NativeName = "Azerbaijan", UniversalName = "Azerbaijan", PhoneNumberPrefix = "+994", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BA", NativeName = "Bosnia and Herzegovina", UniversalName = "Bosnia and Herzegovina", PhoneNumberPrefix = "+387", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BB", NativeName = "Barbados", UniversalName = "Barbados", PhoneNumberPrefix = "+1246", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BD", NativeName = "Bangladesh", UniversalName = "Bangladesh", PhoneNumberPrefix = "+880", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BF", NativeName = "Burkina Faso", UniversalName = "Burkina Faso", PhoneNumberPrefix = "+226", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BH", NativeName = "Bahrain", UniversalName = "Bahrain", PhoneNumberPrefix = "+973", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BI", NativeName = "Burundi", UniversalName = "Burundi", PhoneNumberPrefix = "+257", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BJ", NativeName = "Benin", UniversalName = "Benin", PhoneNumberPrefix = "+229", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BL", NativeName = "Saint Barthélemy", UniversalName = "Saint Barthélemy", PhoneNumberPrefix = "+590", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BM", NativeName = "Bermuda", UniversalName = "Bermuda", PhoneNumberPrefix = "+1441", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BN", NativeName = "Brunei Darussalam", UniversalName = "Brunei Darussalam", PhoneNumberPrefix = "+673", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BO", NativeName = "Bolivia", UniversalName = "Bolivia", PhoneNumberPrefix = "+591", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BR", NativeName = "Brazil", UniversalName = "Brazil", PhoneNumberPrefix = "+55", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BS", NativeName = "Bahamas", UniversalName = "Bahamas", PhoneNumberPrefix = "+1242", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BT", NativeName = "Bhutan", UniversalName = "Bhutan", PhoneNumberPrefix = "+975", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BW", NativeName = "Botswana", UniversalName = "Botswana", PhoneNumberPrefix = "+267", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BY", NativeName = "Belarus", UniversalName = "Belarus", PhoneNumberPrefix = "+375", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BZ", NativeName = "Belize", UniversalName = "Belize", PhoneNumberPrefix = "+501", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CA", NativeName = "Canada", UniversalName = "Canada", PhoneNumberPrefix = "+1", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CC", NativeName = "Cocos (Keeling) Islands", UniversalName = "Cocos (Keeling) Islands", PhoneNumberPrefix = "+61", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CD", NativeName = "Congo, Democratic Republic of the", UniversalName = "Congo, Democratic Republic of the", PhoneNumberPrefix = "+243", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CF", NativeName = "Central African Republic", UniversalName = "Central African Republic", PhoneNumberPrefix = "+236", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CG", NativeName = "Congo", UniversalName = "Congo", PhoneNumberPrefix = "+242", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CH", NativeName = "Switzerland", UniversalName = "Switzerland", PhoneNumberPrefix = "+41", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CI", NativeName = "Côte d`Ivoire", UniversalName = "Côte d`Ivoire", PhoneNumberPrefix = "+225", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CK", NativeName = "Cook Islands", UniversalName = "Cook Islands", PhoneNumberPrefix = "+682", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CL", NativeName = "Chile", UniversalName = "Chile", PhoneNumberPrefix = "+56", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CM", NativeName = "Cameroon", UniversalName = "Cameroon", PhoneNumberPrefix = "+237", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CN", NativeName = "China", UniversalName = "China", PhoneNumberPrefix = "+86", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CO", NativeName = "Colombia", UniversalName = "Colombia", PhoneNumberPrefix = "+57", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CR", NativeName = "Costa Rica", UniversalName = "Costa Rica", PhoneNumberPrefix = "+506", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CU", NativeName = "Cuba", UniversalName = "Cuba", PhoneNumberPrefix = "+53", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CV", NativeName = "Cape Verde", UniversalName = "Cape Verde", PhoneNumberPrefix = "+238", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CX", NativeName = "Christmas Island", UniversalName = "Christmas Island", PhoneNumberPrefix = "+61", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "DJ", NativeName = "Djibouti", UniversalName = "Djibouti", PhoneNumberPrefix = "+253", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "DM", NativeName = "Dominica", UniversalName = "Dominica", PhoneNumberPrefix = "+1767", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "DO", NativeName = "Dominican Republic", UniversalName = "Dominican Republic", PhoneNumberPrefix = "+1809", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "DZ", NativeName = "Algeria", UniversalName = "Algeria", PhoneNumberPrefix = "+213", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "EC", NativeName = "Ecuador", UniversalName = "Ecuador", PhoneNumberPrefix = "+593", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "EG", NativeName = "Egypt", UniversalName = "Egypt", PhoneNumberPrefix = "+20", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "EH", NativeName = "Western Sahara", UniversalName = "Western Sahara", PhoneNumberPrefix = "+212", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "ER", NativeName = "Eritrea", UniversalName = "Eritrea", PhoneNumberPrefix = "+291", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "ET", NativeName = "Ethiopia", UniversalName = "Ethiopia", PhoneNumberPrefix = "+251", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "FJ", NativeName = "Fiji", UniversalName = "Fiji", PhoneNumberPrefix = "+679", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "FK", NativeName = "Falkland Islands (Malvinas)", UniversalName = "Falkland Islands (Malvinas)", PhoneNumberPrefix = "+500", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "FM", NativeName = "Micronesia, Federated States of", UniversalName = "Micronesia, Federated States of", PhoneNumberPrefix = "+691", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "FO", NativeName = "Faroe Islands", UniversalName = "Faroe Islands", PhoneNumberPrefix = "+298", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GA", NativeName = "Gabon", UniversalName = "Gabon", PhoneNumberPrefix = "+241", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GD", NativeName = "Grenada", UniversalName = "Grenada", PhoneNumberPrefix = "+1473", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GE", NativeName = "Georgia", UniversalName = "Georgia", PhoneNumberPrefix = "+995", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GF", NativeName = "French Guiana", UniversalName = "French Guiana", PhoneNumberPrefix = "+594", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GG", NativeName = "Guernsey", UniversalName = "Guernsey", PhoneNumberPrefix = "+44", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GH", NativeName = "Ghana", UniversalName = "Ghana", PhoneNumberPrefix = "+233", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GI", NativeName = "Gibraltar", UniversalName = "Gibraltar", PhoneNumberPrefix = "+350", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GL", NativeName = "Greenland", UniversalName = "Greenland", PhoneNumberPrefix = "+299", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GM", NativeName = "Gambia", UniversalName = "Gambia", PhoneNumberPrefix = "+220", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GN", NativeName = "Guinea", UniversalName = "Guinea", PhoneNumberPrefix = "+224", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GP", NativeName = "Guadeloupe", UniversalName = "Guadeloupe", PhoneNumberPrefix = "+590", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GQ", NativeName = "Equatorial Guinea", UniversalName = "Equatorial Guinea", PhoneNumberPrefix = "+240", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GS", NativeName = "South Georgia and the South Sandwich Islands", UniversalName = "South Georgia and the South Sandwich Islands", PhoneNumberPrefix = "+500", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GT", NativeName = "Guatemala", UniversalName = "Guatemala", PhoneNumberPrefix = "+502", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GU", NativeName = "Guam", UniversalName = "Guam", PhoneNumberPrefix = "+1671", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GW", NativeName = "Guinea-Bissau", UniversalName = "Guinea-Bissau", PhoneNumberPrefix = "+245", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GY", NativeName = "Guyana", UniversalName = "Guyana", PhoneNumberPrefix = "+592", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "HK", NativeName = "Hong Kong", UniversalName = "Hong Kong", PhoneNumberPrefix = "+852", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "HN", NativeName = "Honduras", UniversalName = "Honduras", PhoneNumberPrefix = "+504", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "HR", NativeName = "Croatia", UniversalName = "Croatia", PhoneNumberPrefix = "+385", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "HT", NativeName = "Haiti", UniversalName = "Haiti", PhoneNumberPrefix = "+509", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "ID", NativeName = "Indonesia", UniversalName = "Indonesia", PhoneNumberPrefix = "+62", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "IL", NativeName = "Israel", UniversalName = "Israel", PhoneNumberPrefix = "+972", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "IM", NativeName = "Isle of Man", UniversalName = "Isle of Man", PhoneNumberPrefix = "+44", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "IN", NativeName = "India", UniversalName = "India", PhoneNumberPrefix = "+91", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "IO", NativeName = "British Indian Ocean Territory", UniversalName = "British Indian Ocean Territory", PhoneNumberPrefix = "+246", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "IQ", NativeName = "Iraq", UniversalName = "Iraq", PhoneNumberPrefix = "+964", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "IR", NativeName = "Iran, Islamic Republic of", UniversalName = "Iran, Islamic Republic of", PhoneNumberPrefix = "+98", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "IS", NativeName = "Iceland", UniversalName = "Iceland", PhoneNumberPrefix = "+354", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "JE", NativeName = "Jersey", UniversalName = "Jersey", PhoneNumberPrefix = "+44", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "JM", NativeName = "Jamaica", UniversalName = "Jamaica", PhoneNumberPrefix = "+1876", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "JO", NativeName = "Jordan", UniversalName = "Jordan", PhoneNumberPrefix = "+962", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "JP", NativeName = "Japan", UniversalName = "Japan", PhoneNumberPrefix = "+81", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "KE", NativeName = "Kenya", UniversalName = "Kenya", PhoneNumberPrefix = "+254", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "KG", NativeName = "Kyrgyzstan", UniversalName = "Kyrgyzstan", PhoneNumberPrefix = "+996", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "KH", NativeName = "Cambodia", UniversalName = "Cambodia", PhoneNumberPrefix = "+855", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "KI", NativeName = "Kiribati", UniversalName = "Kiribati", PhoneNumberPrefix = "+686", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "KM", NativeName = "Comoros", UniversalName = "Comoros", PhoneNumberPrefix = "+269", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "KN", NativeName = "Saint Kitts and Nevis", UniversalName = "Saint Kitts and Nevis", PhoneNumberPrefix = "+1869", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "KP", NativeName = "Korea, Democratic People`s Republic of", UniversalName = "Korea, Democratic People`s Republic of", PhoneNumberPrefix = "+850", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "KR", NativeName = "Korea, Republic of", UniversalName = "Korea, Republic of", PhoneNumberPrefix = "+82", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "KW", NativeName = "Kuwait", UniversalName = "Kuwait", PhoneNumberPrefix = "+965", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "KY", NativeName = "Cayman Islands", UniversalName = "Cayman Islands", PhoneNumberPrefix = "+1345", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "KZ", NativeName = "Kazakhstan", UniversalName = "Kazakhstan", PhoneNumberPrefix = "+76", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "LA", NativeName = "Lao People`s Democratic Republic", UniversalName = "Lao People`s Democratic Republic", PhoneNumberPrefix = "+856", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "LB", NativeName = "Lebanon", UniversalName = "Lebanon", PhoneNumberPrefix = "+961", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "LC", NativeName = "Saint Lucia", UniversalName = "Saint Lucia", PhoneNumberPrefix = "+1758", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "LI", NativeName = "Liechtenstein", UniversalName = "Liechtenstein", PhoneNumberPrefix = "+423", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "LK", NativeName = "Sri Lanka", UniversalName = "Sri Lanka", PhoneNumberPrefix = "+94", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "LR", NativeName = "Liberia", UniversalName = "Liberia", PhoneNumberPrefix = "+231", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "LS", NativeName = "Lesotho", UniversalName = "Lesotho", PhoneNumberPrefix = "+266", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "LY", NativeName = "Libyan Arab Jamahiriya", UniversalName = "Libyan Arab Jamahiriya", PhoneNumberPrefix = "+218", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MA", NativeName = "Morocco", UniversalName = "Morocco", PhoneNumberPrefix = "+212", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MC", NativeName = "Monaco", UniversalName = "Monaco", PhoneNumberPrefix = "+377", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MD", NativeName = "Moldova", UniversalName = "Moldova", PhoneNumberPrefix = "+373", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "ME", NativeName = "Montenegro", UniversalName = "Montenegro", PhoneNumberPrefix = "+382", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MF", NativeName = "Saint Martin (French part)", UniversalName = "Saint Martin (French part)", PhoneNumberPrefix = "+1599", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MG", NativeName = "Madagascar", UniversalName = "Madagascar", PhoneNumberPrefix = "+261", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MH", NativeName = "Marshall Islands", UniversalName = "Marshall Islands", PhoneNumberPrefix = "+692", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MK", NativeName = "Macedonia, the former Yugoslav Republic of", UniversalName = "Macedonia, the former Yugoslav Republic of", PhoneNumberPrefix = "+389", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "ML", NativeName = "Mali", UniversalName = "Mali", PhoneNumberPrefix = "+223", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MM", NativeName = "Myanmar", UniversalName = "Myanmar", PhoneNumberPrefix = "+95", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MN", NativeName = "Mongolia", UniversalName = "Mongolia", PhoneNumberPrefix = "+976", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MO", NativeName = "Macao", UniversalName = "Macao", PhoneNumberPrefix = "+853", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MP", NativeName = "Northern Mariana Islands", UniversalName = "Northern Mariana Islands", PhoneNumberPrefix = "+1670", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MQ", NativeName = "Martinique", UniversalName = "Martinique", PhoneNumberPrefix = "+596", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MR", NativeName = "Mauritania", UniversalName = "Mauritania", PhoneNumberPrefix = "+222", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MS", NativeName = "Montserrat", UniversalName = "Montserrat", PhoneNumberPrefix = "+1664", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MU", NativeName = "Mauritius", UniversalName = "Mauritius", PhoneNumberPrefix = "+230", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MV", NativeName = "Maldives", UniversalName = "Maldives", PhoneNumberPrefix = "+960", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MW", NativeName = "Malawi", UniversalName = "Malawi", PhoneNumberPrefix = "+265", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MX", NativeName = "Mexico", UniversalName = "Mexico", PhoneNumberPrefix = "+52", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MY", NativeName = "Malaysia", UniversalName = "Malaysia", PhoneNumberPrefix = "+60", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MZ", NativeName = "Mozambique", UniversalName = "Mozambique", PhoneNumberPrefix = "+258", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "NA", NativeName = "Namibia", UniversalName = "Namibia", PhoneNumberPrefix = "+264", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "NC", NativeName = "New Caledonia", UniversalName = "New Caledonia", PhoneNumberPrefix = "+687", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "NE", NativeName = "Niger", UniversalName = "Niger", PhoneNumberPrefix = "+227", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "NF", NativeName = "Norfolk Island", UniversalName = "Norfolk Island", PhoneNumberPrefix = "+672", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "NG", NativeName = "Nigeria", UniversalName = "Nigeria", PhoneNumberPrefix = "+234", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "NI", NativeName = "Nicaragua", UniversalName = "Nicaragua", PhoneNumberPrefix = "+505", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "BG", NativeName = "Bulgaria", UniversalName = "Bulgaria", PhoneNumberPrefix = "+359", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "CY", NativeName = "Cyprus", UniversalName = "Cyprus", PhoneNumberPrefix = "+357", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "DE", NativeName = "Germany", UniversalName = "Germany", PhoneNumberPrefix = "+49", IsMetricSystem = true, Currency = Currency.Get("EUR"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "EE", NativeName = "Estonia", UniversalName = "Estonia", PhoneNumberPrefix = "+372", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "GR", NativeName = "Greece", UniversalName = "Greece", PhoneNumberPrefix = "+30", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "HU", NativeName = "Hungary", UniversalName = "Hungary", PhoneNumberPrefix = "+36", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "FI", NativeName = "Finland", UniversalName = "Finland", PhoneNumberPrefix = "+358", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "FR", NativeName = "France", UniversalName = "France", PhoneNumberPrefix = "+33", IsMetricSystem = true, Currency = Currency.Get("EUR"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "LT", NativeName = "Lithuania", UniversalName = "Lithuania", PhoneNumberPrefix = "+370", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AD", NativeName = "Andorra", UniversalName = "Andorra", PhoneNumberPrefix = "+376", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AE", NativeName = "United Arab Emirates", UniversalName = "United Arab Emirates", PhoneNumberPrefix = "+971", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AF", NativeName = "Afghanistan", UniversalName = "Afghanistan", PhoneNumberPrefix = "+93", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AG", NativeName = "Antigua and Barbuda", UniversalName = "Antigua and Barbuda", PhoneNumberPrefix = "+1268", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AI", NativeName = "Anguilla", UniversalName = "Anguilla", PhoneNumberPrefix = "+1264", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AL", NativeName = "Albania", UniversalName = "Albania", PhoneNumberPrefix = "+355", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AM", NativeName = "Armenia", UniversalName = "Armenia", PhoneNumberPrefix = "+374", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AN", NativeName = "Netherlands Antilles", UniversalName = "Netherlands Antilles", PhoneNumberPrefix = "+599", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AO", NativeName = "Angola", UniversalName = "Angola", PhoneNumberPrefix = "+244", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AQ", NativeName = "Antarctica", UniversalName = "Antarctica", PhoneNumberPrefix = "+672", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AR", NativeName = "Argentina", UniversalName = "Argentina", PhoneNumberPrefix = "+54", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "AS", NativeName = "American Samoa", UniversalName = "American Samoa", PhoneNumberPrefix = "+1684", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "LV", NativeName = "Latvia", UniversalName = "Latvia", PhoneNumberPrefix = "+371", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "MT", NativeName = "Malta", UniversalName = "Malta", PhoneNumberPrefix = "+356", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "NP", NativeName = "Nepal", UniversalName = "Nepal", PhoneNumberPrefix = "+977", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "NR", NativeName = "Nauru", UniversalName = "Nauru", PhoneNumberPrefix = "+674", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "NU", NativeName = "Niue", UniversalName = "Niue", PhoneNumberPrefix = "+683", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "OM", NativeName = "Oman", UniversalName = "Oman", PhoneNumberPrefix = "+968", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "PA", NativeName = "Panama", UniversalName = "Panama", PhoneNumberPrefix = "+507", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "PE", NativeName = "Peru", UniversalName = "Peru", PhoneNumberPrefix = "+51", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "PF", NativeName = "French Polynesia", UniversalName = "French Polynesia", PhoneNumberPrefix = "+594", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "PG", NativeName = "Papua New Guinea", UniversalName = "Papua New Guinea", PhoneNumberPrefix = "+675", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "PH", NativeName = "Philippines", UniversalName = "Philippines", PhoneNumberPrefix = "+63", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "PK", NativeName = "Pakistan", UniversalName = "Pakistan", PhoneNumberPrefix = "+92", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "PM", NativeName = "Saint Pierre and Miquelon", UniversalName = "Saint Pierre and Miquelon", PhoneNumberPrefix = "+508", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "PN", NativeName = "Pitcairn", UniversalName = "Pitcairn", PhoneNumberPrefix = "+870", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "PR", NativeName = "Puerto Rico", UniversalName = "Puerto Rico", PhoneNumberPrefix = "+1787", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "PS", NativeName = "Palestinian Territory, Occupied", UniversalName = "Palestinian Territory, Occupied", PhoneNumberPrefix = "+970", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "PW", NativeName = "Palau", UniversalName = "Palau", PhoneNumberPrefix = "+680", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "PY", NativeName = "Paraguay", UniversalName = "Paraguay", PhoneNumberPrefix = "+595", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "QA", NativeName = "Qatar", UniversalName = "Qatar", PhoneNumberPrefix = "+974", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "RE", NativeName = "Réunion", UniversalName = "Réunion", PhoneNumberPrefix = "+262", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "RS", NativeName = "Serbia", UniversalName = "Serbia", PhoneNumberPrefix = "+381", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "RU", NativeName = "Russian Federation", UniversalName = "Russian Federation", PhoneNumberPrefix = "+7", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "RW", NativeName = "Rwanda", UniversalName = "Rwanda", PhoneNumberPrefix = "+250", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SA", NativeName = "Saudi Arabia", UniversalName = "Saudi Arabia", PhoneNumberPrefix = "+966", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SB", NativeName = "Solomon Islands", UniversalName = "Solomon Islands", PhoneNumberPrefix = "+677", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SC", NativeName = "Seychelles", UniversalName = "Seychelles", PhoneNumberPrefix = "+248", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SD", NativeName = "Sudan", UniversalName = "Sudan", PhoneNumberPrefix = "+249", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SG", NativeName = "Singapore", UniversalName = "Singapore", PhoneNumberPrefix = "+65", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SH", NativeName = "Saint Helena", UniversalName = "Saint Helena", PhoneNumberPrefix = "+290", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SJ", NativeName = "Svalbard and Jan Mayen", UniversalName = "Svalbard and Jan Mayen", PhoneNumberPrefix = "+47", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SL", NativeName = "Sierra Leone", UniversalName = "Sierra Leone", PhoneNumberPrefix = "+232", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SM", NativeName = "San Marino", UniversalName = "San Marino", PhoneNumberPrefix = "+378", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SN", NativeName = "Senegal", UniversalName = "Senegal", PhoneNumberPrefix = "+221", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SO", NativeName = "Somalia", UniversalName = "Somalia", PhoneNumberPrefix = "+252", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SR", NativeName = "Suriname", UniversalName = "Suriname", PhoneNumberPrefix = "+597", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "ST", NativeName = "Sao Tome and Principe", UniversalName = "Sao Tome and Principe", PhoneNumberPrefix = "+239", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SV", NativeName = "El Salvador", UniversalName = "El Salvador", PhoneNumberPrefix = "+503", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SY", NativeName = "Syrian Arab Republic", UniversalName = "Syrian Arab Republic", PhoneNumberPrefix = "+963", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SZ", NativeName = "Swaziland", UniversalName = "Swaziland", PhoneNumberPrefix = "+268", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TC", NativeName = "Turks and Caicos Islands", UniversalName = "Turks and Caicos Islands", PhoneNumberPrefix = "+1649", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TD", NativeName = "Chad", UniversalName = "Chad", PhoneNumberPrefix = "+235", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TF", NativeName = "French Southern Territories", UniversalName = "French Southern Territories", PhoneNumberPrefix = "+596", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TG", NativeName = "Togo", UniversalName = "Togo", PhoneNumberPrefix = "+228", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TH", NativeName = "Thailand", UniversalName = "Thailand", PhoneNumberPrefix = "+66", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TJ", NativeName = "Tajikistan", UniversalName = "Tajikistan", PhoneNumberPrefix = "+992", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TK", NativeName = "Tokelau", UniversalName = "Tokelau", PhoneNumberPrefix = "+690", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TL", NativeName = "Timor-Leste", UniversalName = "Timor-Leste", PhoneNumberPrefix = "+670", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TM", NativeName = "Turkmenistan", UniversalName = "Turkmenistan", PhoneNumberPrefix = "+993", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TN", NativeName = "Tunisia", UniversalName = "Tunisia", PhoneNumberPrefix = "+216", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TO", NativeName = "Tonga", UniversalName = "Tonga", PhoneNumberPrefix = "+676", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TR", NativeName = "Turkey", UniversalName = "Turkey", PhoneNumberPrefix = "+90", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TT", NativeName = "Trinidad and Tobago", UniversalName = "Trinidad and Tobago", PhoneNumberPrefix = "+1868", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TV", NativeName = "Tuvalu", UniversalName = "Tuvalu", PhoneNumberPrefix = "+688", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TW", NativeName = "Taiwan, Province of China", UniversalName = "Taiwan, Province of China", PhoneNumberPrefix = "+886", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "TZ", NativeName = "Tanzania, United Republic of", UniversalName = "Tanzania, United Republic of", PhoneNumberPrefix = "+255", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "UA", NativeName = "Ukraine", UniversalName = "Ukraine", PhoneNumberPrefix = "+380", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "UG", NativeName = "Uganda", UniversalName = "Uganda", PhoneNumberPrefix = "+256", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "UY", NativeName = "Uruguay", UniversalName = "Uruguay", PhoneNumberPrefix = "+598", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "UZ", NativeName = "Uzbekistan", UniversalName = "Uzbekistan", PhoneNumberPrefix = "+998", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "VA", NativeName = "Holy See (Vatican City State)", UniversalName = "Holy See (Vatican City State)", PhoneNumberPrefix = "+379", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "VC", NativeName = "Saint Vincent and the Grenadines", UniversalName = "Saint Vincent and the Grenadines", PhoneNumberPrefix = "+1784", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "VE", NativeName = "Venezuela", UniversalName = "Venezuela", PhoneNumberPrefix = "+58", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "VG", NativeName = "Virgin Islands, British", UniversalName = "Virgin Islands, British", PhoneNumberPrefix = "+1284", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "VI", NativeName = "Virgin Islands, U.S.", UniversalName = "Virgin Islands, U.S.", PhoneNumberPrefix = "+1340", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "VN", NativeName = "Viet Nam", UniversalName = "Viet Nam", PhoneNumberPrefix = "+84", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "VU", NativeName = "Vanuatu", UniversalName = "Vanuatu", PhoneNumberPrefix = "+678", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "WF", NativeName = "Wallis and Futuna", UniversalName = "Wallis and Futuna", PhoneNumberPrefix = "+681", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "WS", NativeName = "Samoa", UniversalName = "Samoa", PhoneNumberPrefix = "+685", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "YE", NativeName = "Yemen", UniversalName = "Yemen", PhoneNumberPrefix = "+967", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "YT", NativeName = "Mayotte", UniversalName = "Mayotte", PhoneNumberPrefix = "+262", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "ZA", NativeName = "South Africa", UniversalName = "South Africa", PhoneNumberPrefix = "+27", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "ZM", NativeName = "Zambia", UniversalName = "Zambia", PhoneNumberPrefix = "+260", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "ZW", NativeName = "Zimbabwe", UniversalName = "Zimbabwe", PhoneNumberPrefix = "+263", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "SI", NativeName = "Slovenia", UniversalName = "Slovenia", PhoneNumberPrefix = "+386", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("ab") },
    //        new Country { IsoAlpha2 = "DK", NativeName = "Danmark", UniversalName = "Denmark", PhoneNumberPrefix = "+45", IsMetricSystem = true, Currency = Currency.Get("EUR"), Language = Language.Get("da") },
    //        new Country { IsoAlpha2 = "NL", NativeName = "Netherlands", UniversalName = "Netherlands", PhoneNumberPrefix = "+31", IsMetricSystem = true, Currency = Currency.Get("EUR"), Language = Language.Get("nl") },
    //        new Country { IsoAlpha2 = "NZ", NativeName = "New Zealand", UniversalName = "New Zealand", PhoneNumberPrefix = "+64", IsMetricSystem = true, Currency = Currency.Get("NZD"), Language = Language.Get("en") },
    //        new Country { IsoAlpha2 = "GB", NativeName = "United Kingdom", UniversalName = "United Kingdom", PhoneNumberPrefix = "+44", IsMetricSystem = false, Currency = Currency.Get("GBP"), Language = Language.Get("en") },
    //        new Country { IsoAlpha2 = "IE", NativeName = "Ireland", UniversalName = "Ireland", PhoneNumberPrefix = "+353", IsMetricSystem = true, Currency = Currency.Get("GBP"), Language = Language.Get("en") },
    //        new Country { IsoAlpha2 = "UM", NativeName = "United States Minor Outlying Islands", UniversalName = "United States Minor Outlying Islands", PhoneNumberPrefix = "+1", IsMetricSystem = false, Currency = Currency.Get("USD"), Language = Language.Get("en") },
    //        new Country { IsoAlpha2 = "US", NativeName = "United States", UniversalName = "United States", PhoneNumberPrefix = "+1", IsMetricSystem = false, Currency = Currency.Get("USD"), Language = Language.Get("en") },
    //        new Country { IsoAlpha2 = "SK", NativeName = "Slovakia", UniversalName = "Slovakia", PhoneNumberPrefix = "+421", IsMetricSystem = true, Currency = Currency.Get("EUR"), Language = Language.Get("en") },
    //        new Country { IsoAlpha2 = "LU", NativeName = "Luxembourg", UniversalName = "Luxembourg", PhoneNumberPrefix = "+352", IsMetricSystem = true, Currency = Currency.Get("EUR"), Language = Language.Get("fr") },
    //        new Country { IsoAlpha2 = "ES", NativeName = "Spain", UniversalName = "Spain", PhoneNumberPrefix = "+34", IsMetricSystem = true, Currency = Currency.Get("EUR"), Language = Language.Get("fr") },
    //        new Country { IsoAlpha2 = "BE", NativeName = "Belgium", UniversalName = "Belgium", PhoneNumberPrefix = "+32", IsMetricSystem = true, Currency = Currency.Get("EUR"), Language = Language.Get("fr") },
    //        new Country { IsoAlpha2 = "AT", NativeName = "Austria", UniversalName = "Austria", PhoneNumberPrefix = "+43", IsMetricSystem = true, Currency = Currency.Get("EUR"), Language = Language.Get("de") },
    //        new Country { IsoAlpha2 = "CZ", NativeName = "Czech Republic", UniversalName = "Czech Republic", PhoneNumberPrefix = "+420", IsMetricSystem = true, Currency = Currency.Get("EUR"), Language = Language.Get("de") },
    //        new Country { IsoAlpha2 = "AU", NativeName = "Australia", UniversalName = "Australia", PhoneNumberPrefix = "+61", IsMetricSystem = true, Currency = Currency.Get("DKK"), Language = Language.Get("de") },
    //        new Country { IsoAlpha2 = "IT", NativeName = "Italy", UniversalName = "Italy", PhoneNumberPrefix = "+39", IsMetricSystem = true, Currency = Currency.Get("EUR"), Language = Language.Get("it") },
    //        new Country { IsoAlpha2 = "NO", NativeName = "Norway", UniversalName = "Norway", PhoneNumberPrefix = "+47", IsMetricSystem = true, Currency = Currency.Get("NOK"), Language = Language.Get("nb") },
    //        new Country { IsoAlpha2 = "PL", NativeName = "Poland", UniversalName = "Poland", PhoneNumberPrefix = "+48", IsMetricSystem = true, Currency = Currency.Get("PLN"), Language = Language.Get("pl") },
    //        new Country { IsoAlpha2 = "PT", NativeName = "Portugal", UniversalName = "Portugal", PhoneNumberPrefix = "+351", IsMetricSystem = true, Currency = Currency.Get("EUR"), Language = Language.Get("pt-br") },
    //        new Country { IsoAlpha2 = "SE", NativeName = "Sweden", UniversalName = "Sweden", PhoneNumberPrefix = "+46", IsMetricSystem = true, Currency = Currency.Get("SEK"), Language = Language.Get("sk") },
    //        new Country { IsoAlpha2 = "RO", NativeName = "Romania", UniversalName = "Romania", PhoneNumberPrefix = "+40", IsMetricSystem = true, Currency = Currency.Get("RON"), Language = Language.Get("sv") }
    //    };
    //}
    ///// <summary>
    ///// Internal class containing an <see cref="Array"/> of <see cref="Language"/>'s. 
    ///// Used by <see cref="Language.Get(string)"/> to retrieve language data.
    ///// </summary>
    //internal static class Languages
    //{
    //    /// <summary>
    //    /// An <see cref="Array"/> of <see cref="Language"/>'s. 
    //    /// </summary>
    //    internal static Language[] List => new[]
    //    {
    //        new Language { Iso639_1 = "ab", NativeName = "аҧсуа", UniversalName = "Abkhaz" },
    //        new Language { Iso639_1 = "aa", NativeName = "Afaraf", UniversalName = "Afar" },
    //        new Language { Iso639_1 = "af", NativeName = "Afrikaans", UniversalName = "Afrikaans" },
    //        new Language { Iso639_1 = "ak", NativeName = "Akan", UniversalName = "Akan" },
    //        new Language { Iso639_1 = "sq", NativeName = "Shqip", UniversalName = "Albanian" },
    //        new Language { Iso639_1 = "am", NativeName = "አማርኛ", UniversalName = "Amharic" },
    //        new Language { Iso639_1 = "ar", NativeName = "العربية", UniversalName = "Arabic" },
    //        new Language { Iso639_1 = "an", NativeName = "Aragonés", UniversalName = "Aragonese" },
    //        new Language { Iso639_1 = "hy", NativeName = "Հայերեն", UniversalName = "Armenian" },
    //        new Language { Iso639_1 = "as", NativeName = "অসমীয়া", UniversalName = "Assamese" },
    //        new Language { Iso639_1 = "av", NativeName = "авар мацӀ, магӀарул мацӀ", UniversalName = "Avaric" },
    //        new Language { Iso639_1 = "ae", NativeName = "avesta", UniversalName = "Avestan" },
    //        new Language { Iso639_1 = "ay", NativeName = "aymar aru", UniversalName = "Aymara" },
    //        new Language { Iso639_1 = "az", NativeName = "azərbaycan dili", UniversalName = "Azerbaijani" },
    //        new Language { Iso639_1 = "bm", NativeName = "bamanankan", UniversalName = "Bambara" },
    //        new Language { Iso639_1 = "ba", NativeName = "башҡорт теле", UniversalName = "Bashkir" },
    //        new Language { Iso639_1 = "eu", NativeName = "euskara, euskera", UniversalName = "Basque" },
    //        new Language { Iso639_1 = "be", NativeName = "Беларуская", UniversalName = "Belarusian" },
    //        new Language { Iso639_1 = "bn", NativeName = "বাংলা", UniversalName = "Bengali" },
    //        new Language { Iso639_1 = "bh", NativeName = "भोजपुरी", UniversalName = "Bihari" },
    //        new Language { Iso639_1 = "bi", NativeName = "Bislama", UniversalName = "Bislama" },
    //        new Language { Iso639_1 = "bs", NativeName = "bosanski jezik", UniversalName = "Bosnian" },
    //        new Language { Iso639_1 = "br", NativeName = "brezhoneg", UniversalName = "Breton" },
    //        new Language { Iso639_1 = "bg", NativeName = "български език", UniversalName = "Bulgarian" },
    //        new Language { Iso639_1 = "my", NativeName = "ဗမာစာ", UniversalName = "Burmese" },
    //        new Language { Iso639_1 = "ca", NativeName = "Català", UniversalName = "Catalan; Valencian" },
    //        new Language { Iso639_1 = "ch", NativeName = "Chamoru", UniversalName = "Chamorro" },
    //        new Language { Iso639_1 = "ce", NativeName = "нохчийн мотт", UniversalName = "Chechen" },
    //        new Language { Iso639_1 = "ny", NativeName = "chiCheŵa, chinyanja", UniversalName = "Chichewa; Chewa; Nyanja" },
    //        new Language { Iso639_1 = "zh-CHT", NativeName = "中文 (Zhōngwén), 漢語", UniversalName = "Chinese, Traditional" },
    //        new Language { Iso639_1 = "cv", NativeName = "чӑваш чӗлхи", UniversalName = "Chuvash" },
    //        new Language { Iso639_1 = "kw", NativeName = "Kernewek", UniversalName = "Cornish" },
    //        new Language { Iso639_1 = "co", NativeName = "corsu, lingua corsa", UniversalName = "Corsican" },
    //        new Language { Iso639_1 = "cr", NativeName = "ᓀᐦᐃᔭᐍᐏᐣ", UniversalName = "Cree" },
    //        new Language { Iso639_1 = "hr", NativeName = "hrvatski", UniversalName = "Croatian" },
    //        new Language { Iso639_1 = "cs", NativeName = "česky, čeština", UniversalName = "Czech" },
    //        new Language { Iso639_1 = "da", NativeName = "dansk", UniversalName = "Danish" },
    //        new Language { Iso639_1 = "dv", NativeName = "ދިވެހި", UniversalName = "Divehi; Dhivehi; Maldivian;" },
    //        new Language { Iso639_1 = "nl", NativeName = "Nederlands, Vlaams", UniversalName = "Dutch" },
    //        new Language { Iso639_1 = "dz", NativeName = "རྫོང་ཁ", UniversalName = "Dzongkha" },
    //        new Language { Iso639_1 = "en", NativeName = "English", UniversalName = "English" },
    //        new Language { Iso639_1 = "eo", NativeName = "Esperanto", UniversalName = "Esperanto" },
    //        new Language { Iso639_1 = "et", NativeName = "eesti, eesti keel", UniversalName = "Estonian" },
    //        new Language { Iso639_1 = "ee", NativeName = "Eʋegbe", UniversalName = "Ewe" },
    //        new Language { Iso639_1 = "fo", NativeName = "føroyskt", UniversalName = "Faroese" },
    //        new Language { Iso639_1 = "fj", NativeName = "vosa Vakaviti", UniversalName = "Fijian" },
    //        new Language { Iso639_1 = "fi", NativeName = "suomi, suomen kieli", UniversalName = "Finnish" },
    //        new Language { Iso639_1 = "fr", NativeName = "français, langue française", UniversalName = "French" },
    //        new Language { Iso639_1 = "ff", NativeName = "Fulfulde, Pulaar, Pular", UniversalName = "Fula; Fulah; Pulaar; Pular" },
    //        new Language { Iso639_1 = "gl", NativeName = "Galego", UniversalName = "Galician" },
    //        new Language { Iso639_1 = "ka", NativeName = "ქართული", UniversalName = "Georgian" },
    //        new Language { Iso639_1 = "de", NativeName = "Deutsch", UniversalName = "German" },
    //        new Language { Iso639_1 = "el", NativeName = "Ελληνικά", UniversalName = "Greek, Modern" },
    //        new Language { Iso639_1 = "gn", NativeName = "Avañe'ẽ", UniversalName = "Guaraní" },
    //        new Language { Iso639_1 = "gu", NativeName = "ગુજરાતી", UniversalName = "Gujarati" },
    //        new Language { Iso639_1 = "ht", NativeName = "Kreyòl ayisyen", UniversalName = "Haitian; Haitian Creole" },
    //        new Language { Iso639_1 = "ha", NativeName = "Hausa, هَوُسَ", UniversalName = "Hausa" },
    //        new Language { Iso639_1 = "he", NativeName = "עברית", UniversalName = "Hebrew (modern)" },
    //        new Language { Iso639_1 = "hz", NativeName = "Otjiherero", UniversalName = "Herero" },
    //        new Language { Iso639_1 = "hi", NativeName = "हिन्दी, हिंदी", UniversalName = "Hindi" },
    //        new Language { Iso639_1 = "ho", NativeName = "Hiri Motu", UniversalName = "Hiri Motu" },
    //        new Language { Iso639_1 = "hu", NativeName = "Magyar", UniversalName = "Hungarian" },
    //        new Language { Iso639_1 = "ia", NativeName = "Interlingua", UniversalName = "Interlingua" },
    //        new Language { Iso639_1 = "id", NativeName = "Bahasa Indonesia", UniversalName = "Indonesian" },
    //        new Language { Iso639_1 = "ie", NativeName = "Originally called Occidental; then Interlingue after WWII", UniversalName = "Interlingue" },
    //        new Language { Iso639_1 = "ga", NativeName = "Gaeilge", UniversalName = "Irish" },
    //        new Language { Iso639_1 = "ig", NativeName = "Asụsụ Igbo", UniversalName = "Igbo" },
    //        new Language { Iso639_1 = "ik", NativeName = "Iñupiaq, Iñupiatun", UniversalName = "Inupiaq" },
    //        new Language { Iso639_1 = "io", NativeName = "Ido", UniversalName = "Ido" },
    //        new Language { Iso639_1 = "is", NativeName = "Íslenska", UniversalName = "Icelandic" },
    //        new Language { Iso639_1 = "it", NativeName = "Italiano", UniversalName = "Italian" },
    //        new Language { Iso639_1 = "iu", NativeName = "ᐃᓄᒃᑎᑐᑦ", UniversalName = "Inuktitut" },
    //        new Language { Iso639_1 = "ja", NativeName = "日本語 (にほんご)", UniversalName = "Japanese" },
    //        new Language { Iso639_1 = "jv", NativeName = "basa Jawa", UniversalName = "Javanese" },
    //        new Language { Iso639_1 = "kl", NativeName = "kalaallisut, kalaallit oqaasii", UniversalName = "Kalaallisut, Greenlandic" },
    //        new Language { Iso639_1 = "kn", NativeName = "ಕನ್ನಡ", UniversalName = "Kannada" },
    //        new Language { Iso639_1 = "kr", NativeName = "Kanuri", UniversalName = "Kanuri" },
    //        new Language { Iso639_1 = "ks", NativeName = "कश्मीरी, كشميري‎", UniversalName = "Kashmiri" },
    //        new Language { Iso639_1 = "kk", NativeName = "Қазақ тілі", UniversalName = "Kazakh" },
    //        new Language { Iso639_1 = "km", NativeName = "ភាសាខ្មែរ", UniversalName = "Khmer" },
    //        new Language { Iso639_1 = "ki", NativeName = "Gĩkũyũ", UniversalName = "Kikuyu, Gikuyu" },
    //        new Language { Iso639_1 = "rw", NativeName = "Ikinyarwanda", UniversalName = "Kinyarwanda" },
    //        new Language { Iso639_1 = "ky", NativeName = "кыргыз тили", UniversalName = "Kirghiz, Kyrgyz" },
    //        new Language { Iso639_1 = "kv", NativeName = "коми кыв", UniversalName = "Komi" },
    //        new Language { Iso639_1 = "kg", NativeName = "KiKongo", UniversalName = "Kongo" },
    //        new Language { Iso639_1 = "ko", NativeName = "한국어 (韓國語), 조선어 (朝鮮語)", UniversalName = "Korean" },
    //        new Language { Iso639_1 = "ku", NativeName = "Kurdî, كوردی‎", UniversalName = "Kurdish" },
    //        new Language { Iso639_1 = "kj", NativeName = "Kuanyama", UniversalName = "Kwanyama, Kuanyama" },
    //        new Language { Iso639_1 = "la", NativeName = "latine, lingua latina", UniversalName = "Latin" },
    //        new Language { Iso639_1 = "lb", NativeName = "Lëtzebuergesch", UniversalName = "Luxembourgish, Letzeburgesch" },
    //        new Language { Iso639_1 = "lg", NativeName = "Luganda", UniversalName = "Luganda" },
    //        new Language { Iso639_1 = "li", NativeName = "Limburgs", UniversalName = "Limburgish, Limburgan, Limburger" },
    //        new Language { Iso639_1 = "ln", NativeName = "Lingála", UniversalName = "Lingala" },
    //        new Language { Iso639_1 = "lo", NativeName = "ພາສາລາວ", UniversalName = "Lao" },
    //        new Language { Iso639_1 = "lt", NativeName = "lietuvių kalba", UniversalName = "Lithuanian" },
    //        new Language { Iso639_1 = "lu", NativeName = "Luba-Katanga", UniversalName = "Luba-Katanga" },
    //        new Language { Iso639_1 = "lv", NativeName = "latviešu valoda", UniversalName = "Latvian" },
    //        new Language { Iso639_1 = "gv", NativeName = "Gaelg, Gailck", UniversalName = "Manx" },
    //        new Language { Iso639_1 = "mk", NativeName = "македонски јазик", UniversalName = "Macedonian" },
    //        new Language { Iso639_1 = "mg", NativeName = "Malagasy fiteny", UniversalName = "Malagasy" },
    //        new Language { Iso639_1 = "ms", NativeName = "bahasa Melayu, بهاس ملايو‎", UniversalName = "Malay" },
    //        new Language { Iso639_1 = "ml", NativeName = "മലയാളം", UniversalName = "Malayalam" },
    //        new Language { Iso639_1 = "mt", NativeName = "Malti", UniversalName = "Maltese" },
    //        new Language { Iso639_1 = "mi", NativeName = "te reo Māori", UniversalName = "Maori" },
    //        new Language { Iso639_1 = "mr", NativeName = "मराठी", UniversalName = "Marathi (Mara?hi)" },
    //        new Language { Iso639_1 = "mh", NativeName = "Kajin M̧ajeļ", UniversalName = "Marshallese" },
    //        new Language { Iso639_1 = "mn", NativeName = "монгол", UniversalName = "Mongolian" },
    //        new Language { Iso639_1 = "na", NativeName = "Ekakairũ Naoero", UniversalName = "Nauru" },
    //        new Language { Iso639_1 = "nv", NativeName = "Diné bizaad, Dinékʼehǰí", UniversalName = "Navajo, Navaho" },
    //        new Language { Iso639_1 = "nb", NativeName = "Norsk bokmål", UniversalName = "Norwegian Bokmål" },
    //        new Language { Iso639_1 = "nd", NativeName = "isiNdebele", UniversalName = "North Ndebele" },
    //        new Language { Iso639_1 = "ne", NativeName = "नेपाली", UniversalName = "Nepali" },
    //        new Language { Iso639_1 = "ng", NativeName = "Owambo", UniversalName = "Ndonga" },
    //        new Language { Iso639_1 = "nn", NativeName = "Norsk nynorsk", UniversalName = "Norwegian Nynorsk" },
    //        new Language { Iso639_1 = "no", NativeName = "Norsk", UniversalName = "Norwegian" },
    //        new Language { Iso639_1 = "ii", NativeName = "ꆈꌠ꒿ Nuosuhxop", UniversalName = "Nuosu" },
    //        new Language { Iso639_1 = "nr", NativeName = "isiNdebele", UniversalName = "South Ndebele" },
    //        new Language { Iso639_1 = "oc", NativeName = "Occitan", UniversalName = "Occitan" },
    //        new Language { Iso639_1 = "oj", NativeName = "ᐊᓂᔑᓈᐯᒧᐎᓐ", UniversalName = "Ojibwe, Ojibwa" },
    //        new Language { Iso639_1 = "cu", NativeName = "ѩзыкъ словѣньскъ", UniversalName = "Old Church Slavonic, Church Slavic, Church Slavonic, Old Bulgarian, Old Slavonic" },
    //        new Language { Iso639_1 = "om", NativeName = "Afaan Oromoo", UniversalName = "Oromo" },
    //        new Language { Iso639_1 = "or", NativeName = "ଓଡ଼ିଆ", UniversalName = "Oriya" },
    //        new Language { Iso639_1 = "os", NativeName = "ирон æвзаг", UniversalName = "Ossetian, Ossetic" },
    //        new Language { Iso639_1 = "pa", NativeName = "ਪੰਜਾਬੀ, پنجابی‎", UniversalName = "Panjabi, Punjabi" },
    //        new Language { Iso639_1 = "pi", NativeName = "पाऴि", UniversalName = "Pali" },
    //        new Language { Iso639_1 = "fa", NativeName = "فارسی", UniversalName = "Persian" },
    //        new Language { Iso639_1 = "pl", NativeName = "polski", UniversalName = "Polish" },
    //        new Language { Iso639_1 = "ps", NativeName = "پښتو", UniversalName = "Pashto, Pushto" },
    //        new Language { Iso639_1 = "pt-br", NativeName = "Português Brazil", UniversalName = "Portuguese Brazil" },
    //        new Language { Iso639_1 = "qu", NativeName = "Runa Simi, Kichwa", UniversalName = "Quechua" },
    //        new Language { Iso639_1 = "rm", NativeName = "rumantsch grischun", UniversalName = "Romansh" },
    //        new Language { Iso639_1 = "rn", NativeName = "Ikirundi", UniversalName = "Kirundi" },
    //        new Language { Iso639_1 = "ro", NativeName = "română", UniversalName = "Romanian, Moldavian, Moldovan" },
    //        new Language { Iso639_1 = "ru", NativeName = "русский язык", UniversalName = "Russian" },
    //        new Language { Iso639_1 = "sa", NativeName = "संस्कृतम्", UniversalName = "Sanskrit (Sa?sk?ta)" },
    //        new Language { Iso639_1 = "sc", NativeName = "sardu", UniversalName = "Sardinian" },
    //        new Language { Iso639_1 = "sd", NativeName = "सिन्धी, سنڌي، سندھی‎", UniversalName = "Sindhi" },
    //        new Language { Iso639_1 = "se", NativeName = "Davvisámegiella", UniversalName = "Northern Sami" },
    //        new Language { Iso639_1 = "sm", NativeName = "gagana fa'a Samoa", UniversalName = "Samoan" },
    //        new Language { Iso639_1 = "sg", NativeName = "yângâ tî sängö", UniversalName = "Sango" },
    //        new Language { Iso639_1 = "sr", NativeName = "српски језик", UniversalName = "Serbian" },
    //        new Language { Iso639_1 = "gd", NativeName = "Gàidhlig", UniversalName = "Scottish Gaelic; Gaelic" },
    //        new Language { Iso639_1 = "sn", NativeName = "chiShona", UniversalName = "Shona" },
    //        new Language { Iso639_1 = "si", NativeName = "සිංහල", UniversalName = "Sinhala, Sinhalese" },
    //        new Language { Iso639_1 = "sk", NativeName = "slovenčina", UniversalName = "Slovak" },
    //        new Language { Iso639_1 = "sl", NativeName = "slovenščina", UniversalName = "Slovene" },
    //        new Language { Iso639_1 = "so", NativeName = "Soomaaliga, af Soomaali", UniversalName = "Somali" },
    //        new Language { Iso639_1 = "st", NativeName = "Sesotho", UniversalName = "Southern Sotho" },
    //        new Language { Iso639_1 = "es", NativeName = "Español", UniversalName = "Spanish" },
    //        new Language { Iso639_1 = "su", NativeName = "Basa Sunda", UniversalName = "Sundanese" },
    //        new Language { Iso639_1 = "sw", NativeName = "Kiswahili", UniversalName = "Swahili" },
    //        new Language { Iso639_1 = "ss", NativeName = "SiSwati", UniversalName = "Swati" },
    //        new Language { Iso639_1 = "sv", NativeName = "svenska", UniversalName = "Swedish" },
    //        new Language { Iso639_1 = "ta", NativeName = "தமிழ்", UniversalName = "Tamil" },
    //        new Language { Iso639_1 = "te", NativeName = "తెలుగు", UniversalName = "Telugu" },
    //        new Language { Iso639_1 = "tg", NativeName = "тоҷикӣ, toğikī, تاجیکی‎", UniversalName = "Tajik" },
    //        new Language { Iso639_1 = "th", NativeName = "ไทย", UniversalName = "Thai" },
    //        new Language { Iso639_1 = "ti", NativeName = "ትግርኛ", UniversalName = "Tigrinya" },
    //        new Language { Iso639_1 = "bo", NativeName = "བོད་ཡིག", UniversalName = "Tibetan Standard, Tibetan, Central" },
    //        new Language { Iso639_1 = "tk", NativeName = "Türkmen, Түркмен", UniversalName = "Turkmen" },
    //        new Language { Iso639_1 = "tl", NativeName = "Wikang Tagalog, ᜏᜒᜃᜅ᜔ ᜆᜄᜎᜓᜄ᜔", UniversalName = "Tagalog" },
    //        new Language { Iso639_1 = "tn", NativeName = "Setswana", UniversalName = "Tswana" },
    //        new Language { Iso639_1 = "to", NativeName = "faka Tonga", UniversalName = "Tonga (Tonga Islands)" },
    //        new Language { Iso639_1 = "tr", NativeName = "Türkçe", UniversalName = "Turkish" },
    //        new Language { Iso639_1 = "ts", NativeName = "Xitsonga", UniversalName = "Tsonga" },
    //        new Language { Iso639_1 = "tt", NativeName = "татарча, tatarça, تاتارچا‎", UniversalName = "Tatar" },
    //        new Language { Iso639_1 = "tw", NativeName = "Twi", UniversalName = "Twi" },
    //        new Language { Iso639_1 = "ty", NativeName = "Reo Tahiti", UniversalName = "Tahitian" },
    //        new Language { Iso639_1 = "ug", NativeName = "Uyƣurqə, ئۇيغۇرچە‎", UniversalName = "Uighur, Uyghur" },
    //        new Language { Iso639_1 = "uk", NativeName = "українська", UniversalName = "Ukrainian" },
    //        new Language { Iso639_1 = "ur", NativeName = "اردو", UniversalName = "Urdu" },
    //        new Language { Iso639_1 = "uz", NativeName = "O'zbek, Ўзбек, أۇزبېك‎", UniversalName = "Uzbek" },
    //        new Language { Iso639_1 = "ve", NativeName = "Tshivenḓa", UniversalName = "Venda" },
    //        new Language { Iso639_1 = "vi", NativeName = "Tiếng Việt", UniversalName = "Vietnamese" },
    //        new Language { Iso639_1 = "vo", NativeName = "Volapük", UniversalName = "Volapük" },
    //        new Language { Iso639_1 = "wa", NativeName = "Walon", UniversalName = "Walloon" },
    //        new Language { Iso639_1 = "cy", NativeName = "Cymraeg", UniversalName = "Welsh" },
    //        new Language { Iso639_1 = "wo", NativeName = "Wollof", UniversalName = "Wolof" },
    //        new Language { Iso639_1 = "fy", NativeName = "Frysk", UniversalName = "Western Frisian" },
    //        new Language { Iso639_1 = "xh", NativeName = "isiXhosa", UniversalName = "Xhosa" },
    //        new Language { Iso639_1 = "yi", NativeName = "ייִדיש", UniversalName = "Yiddish" },
    //        new Language { Iso639_1 = "yo", NativeName = "Yorùbá", UniversalName = "Yoruba" },
    //        new Language { Iso639_1 = "za", NativeName = "Saɯ cueŋƅ, Saw cuengh", UniversalName = "Zhuang, Chuang" },
    //        new Language { Iso639_1 = "zu", NativeName = "isiZulu", UniversalName = "Zulu" },
    //        new Language { Iso639_1 = "zh-CHS", NativeName = "中文 (Zhōngwén), 汉语", UniversalName = "Chinese, Simplified" },
    //        new Language { Iso639_1 = "pt", NativeName = "Português", UniversalName = "Portuguese" }
    //    };
    //}

    ///// <summary>
    ///// Internal class containing an <see cref="Array"/> of <see cref="Currency"/>'s. 
    ///// Used by <see cref="Currency.Get(string)"/> to retrieve currency data.
    ///// </summary>
    //internal static class Currencies
    //{
    //    /// <summary>
    //    /// An <see cref="Array"/> of <see cref="Currency"/>'s. 
    //    /// </summary>
    //    internal static Currency[] List => new[]
    //    {
    //        new Currency { Iso4217 = "DKK", NativeName = "Dansk krone", UniversalName = "Danish Krone", Symbol = "Kr.", Rate = 1.000M },
    //        new Currency { Iso4217 = "NOK", NativeName = "Norsk krone", UniversalName = "Norwegian Krone", Symbol = "Kr.", Rate = 1.000M },
    //        new Currency { Iso4217 = "SEK", NativeName = "Svensk krona", UniversalName = "Swedish Krona", Symbol = "Kr.", Rate = 1.000M },
    //        new Currency { Iso4217 = "EUR", NativeName = "Euro", UniversalName = "Euro", Symbol = "€", Rate = 1.000M },
    //        new Currency { Iso4217 = "BAM", NativeName = "Bosnia and Herzegovina convertible mark", UniversalName = "Bosnia and Herzegovina convertible mark", Symbol = "KM", Rate = 1.000M },
    //        new Currency { Iso4217 = "HUF", NativeName = "Hungarian forint", UniversalName = "Hungarian forint", Symbol = "Ft", Rate = 1.000M },
    //        new Currency { Iso4217 = "HRK", NativeName = "Croatian kuna", UniversalName = "Croatian kuna", Symbol = "kn", Rate = 1.000M },
    //        new Currency { Iso4217 = "ISK", NativeName = "Icelandic króna", UniversalName = "Icelandic króna", Symbol = "Kr.", Rate = 1.000M },
    //        new Currency { Iso4217 = "LTL", NativeName = "Lithuanian litas", UniversalName = "Lithuanian litas", Symbol = "Lt.", Rate = 1.000M },
    //        new Currency { Iso4217 = "LVL", NativeName = "Latvian lats", UniversalName = "Latvian lats", Symbol = "Ls.", Rate = 1.000M },
    //        new Currency { Iso4217 = "PLN", NativeName = "Polish złoty", UniversalName = "Polish zloty", Symbol = "zł", Rate = 1.000M },
    //        new Currency { Iso4217 = "RON", NativeName = "Romanian leu", UniversalName = "Romanian leu", Symbol = "lue", Rate = 1.000M },
    //        new Currency { Iso4217 = "RSD", NativeName = "Serbian dinar", UniversalName = "Serbian dinar", Symbol = "RSD", Rate = 1.000M },
    //        new Currency { Iso4217 = "UAH", NativeName = "Ukrainian hryvnia", UniversalName = "Ukrainian hryvnia", Symbol = "₴", Rate = 1.000M },
    //        new Currency { Iso4217 = "CHF", NativeName = "Swiss franc", UniversalName = "Swiss franc", Symbol = "CHF", Rate = 1.000M },
    //        new Currency { Iso4217 = "GBP", NativeName = "Pound sterling", UniversalName = "Pound sterling", Symbol = "£", Rate = 1.000M },
    //        new Currency { Iso4217 = "BGN", NativeName = "Bulgarian lev", UniversalName = "Bulgarian lev", Symbol = "лв", Rate = 1.000M },
    //        new Currency { Iso4217 = "USD", NativeName = "American Dollars", UniversalName = "American Dollars", Symbol = "$", Rate = 1.000M },
    //        new Currency { Iso4217 = "AUD", NativeName = "Austrailian Dollars", UniversalName = "Austrailian Dollars", Symbol = "$", Rate = 1.000M },
    //        new Currency { Iso4217 = "NZD", NativeName = "New Zealand Dollars", UniversalName = "New Zealand Dollars", Symbol = "$", Rate = 1.000M },
    //        new Currency { Iso4217 = "CAD", NativeName = "Canadian Dollars", UniversalName = "Canadian Dollars", Symbol = "$", Rate = 1.000M }
    //    };
    //}


    ///// <summary>
    ///// Time Zone.
    ///// https://en.wikipedia.org/wiki/Tz_database
    ///// </summary>
    //public class TimeZone : DefaultEntity
    //{
    //    /// <summary>
    //    /// Required.
    //    /// The 'Olson' name.
    //    /// </summary>
    //    public virtual string OlsonName { get; set; }

    //    /// <summary>
    //    /// Required.
    //    /// The 'Microsoft' name.
    //    /// </summary>
    //    public virtual string MicrosoftName { get; set; }
    //}


    /// <summary>
    /// Phone Number Prefixes.
    /// </summary>
    public static class PhoneNumberPrefixes
    {
        private static readonly IDictionary<string, string> phoneNumberPrefixes = new Dictionary<string, string>
        {
            {"AT", "+43"},
            {"BE", "+32"},
            {"BG", "+359"},
            {"CY", "+357"},
            {"CZ", "+420"},
            {"DE", "+49"},
            {"DK", "+45"},
            {"EE", "+372"},
            {"ES", "+34"},
            {"FI", "+358"},
            {"FR", "+33"},
            {"GB", "+44"},
            {"GR", "+30"},
            {"HU", "+36"},
            {"IE", "+353"},
            {"IT", "+39"},
            {"LT", "+370"},
            {"LU", "+352"},
            {"LV", "+371"},
            {"MT", "+356"},
            {"NL", "+31"},
            {"PL", "+48"},
            {"PT", "+351"},
            {"RO", "+40"},
            {"SE", "+46"},
            {"SI", "+386"},
            {"SK", "+421"},
            {"AD", "+376"},
            {"AE", "+971"},
            {"AF", "+93"},
            {"AG", "+1268"},
            {"AI", "+1264"},
            {"AL", "+355"},
            {"AM", "+374"},
            {"AN", "+599"},
            {"AO", "+244"},
            {"AR", "+54"},
            {"AS", "+1684"},
            {"AU", "+61"},
            {"AW", "+297"},
            {"AZ", "+994"},
            {"BA", "+387"},
            {"BB", "+1246"},
            {"BD", "+880"},
            {"BF", "+226"},
            {"BH", "+973"},
            {"BI", "+257"},
            {"BJ", "+229"},
            {"BL", "+590"},
            {"BM", "+1441"},
            {"BN", "+673"},
            {"BO", "+591"},
            {"BR", "+55"},
            {"BS", "+1242"},
            {"BT", "+975"},
            {"BW", "+267"},
            {"BY", "+375"},
            {"BZ", "+501"},
            {"CD", "+243"},
            {"CF", "+236"},
            {"CG", "+242"},
            {"CH", "+41"},
            {"CI", "+225"},
            {"CK", "+682"},
            {"CL", "+56"},
            {"CM", "+237"},
            {"CN", "+86"},
            {"CO", "+57"},
            {"CR", "+506"},
            {"CU", "+53"},
            {"CV", "+238"},
            {"DJ", "+253"},
            {"DM", "+1767"},
            {"DO", "+1809"},
            {"DZ", "+213"},
            {"EC", "+593"},
            {"EG", "+20"},
            {"ER", "+291"},
            {"ET", "+251"},
            {"FJ", "+679"},
            {"FK", "+500"},
            {"FM", "+691"},
            {"FO", "+298"},
            {"GA", "+241"},
            {"GD", "+1473"},
            {"GE", "+995"},
            {"GH", "+233"},
            {"GI", "+350"},
            {"GL", "+299"},
            {"GM", "+220"},
            {"GN", "+224"},
            {"GQ", "+240"},
            {"GT", "+502"},
            {"GU", "+1671"},
            {"GW", "+245"},
            {"GY", "+592"},
            {"HK", "+852"},
            {"HN", "+504"},
            {"HR", "+385"},
            {"HT", "+509"},
            {"ID", "+62"},
            {"IL", "+972"},
            {"IN", "+91"},
            {"IO", "+246"},
            {"IQ", "+964"},
            {"IR", "+98"},
            {"IS", "+354"},
            {"JM", "+1876"},
            {"JO", "+962"},
            {"JP", "+81"},
            {"KE", "+254"},
            {"KG", "+996"},
            {"KH", "+855"},
            {"KI", "+686"},
            {"KM", "+269"},
            {"KN", "+1869"},
            {"KP", "+850"},
            {"KR", "+82"},
            {"KW", "+965"},
            {"KY", "+1345"},
            {"KZ", "+76"},
            {"LA", "+856"},
            {"LB", "+961"},
            {"LC", "+1758"},
            {"LI", "+423"},
            {"LK", "+94"},
            {"LR", "+231"},
            {"LS", "+266"},
            {"LY", "+218"},
            {"MA", "+212"},
            {"MC", "+377"},
            {"MD", "+373"},
            {"ME", "+382"},
            {"MF", "+1599"},
            {"MG", "+261"},
            {"MH", "+692"},
            {"MK", "+389"},
            {"ML", "+223"},
            {"MM", "+95"},
            {"MN", "+976"},
            {"MO", "+853"},
            {"MP", "+1670"},
            {"MQ", "+596"},
            {"MR", "+222"},
            {"MS", "+1664"},
            {"MU", "+230"},
            {"MV", "+960"},
            {"MW", "+265"},
            {"MX", "+52"},
            {"MY", "+60"},
            {"MZ", "+258"},
            {"NA", "+264"},
            {"NC", "+687"},
            {"NE", "+227"},
            {"NF", "+672"},
            {"NG", "+234"},
            {"NI", "+505"},
            {"NO", "+47"},
            {"NP", "+977"},
            {"NR", "+674"},
            {"NU", "+683"},
            {"NZ", "+64"},
            {"OM", "+968"},
            {"PA", "+507"},
            {"PE", "+51"},
            {"PF", "+594"},
            {"PG", "+675"},
            {"PH", "+63"},
            {"PK", "+92"},
            {"PM", "+508"},
            {"PN", "+870"},
            {"PR", "+1787"},
            {"PS", "+970"},
            {"PW", "+680"},
            {"PY", "+595"},
            {"QA", "+974"},
            {"RS", "+381"},
            {"RU", "+7"},
            {"RW", "+250"},
            {"SA", "+966"},
            {"SB", "+677"},
            {"SC", "+248"},
            {"SD", "+249"},
            {"SG", "+65"},
            {"SH", "+290"},
            {"SL", "+232"},
            {"SM", "+378"},
            {"SN", "+221"},
            {"SO", "+252"},
            {"SR", "+597"},
            {"ST", "+239"},
            {"SV", "+503"},
            {"SY", "+963"},
            {"SZ", "+268"},
            {"TC", "+1649"},
            {"TD", "+235"},
            {"TG", "+228"},
            {"TH", "+66"},
            {"TJ", "+992"},
            {"TK", "+690"},
            {"TL", "+670"},
            {"TM", "+993"},
            {"TN", "+216"},
            {"TO", "+676"},
            {"TR", "+90"},
            {"TT", "+1868"},
            {"TV", "+688"},
            {"TW", "+886"},
            {"TZ", "+255"},
            {"UA", "+380"},
            {"UG", "+256"},
            {"UY", "+598"},
            {"UZ", "+998"},
            {"VA", "+379"},
            {"VC", "+1784"},
            {"VE", "+58"},
            {"VG", "+1284"},
            {"VI", "+1340"},
            {"VN", "+84"},
            {"VU", "+678"},
            {"WF", "+681"},
            {"WS", "+685"},
            {"YE", "+967"},
            {"YT", "+262"},
            {"ZA", "+27"},
            {"ZM", "+260"},
            {"ZW", "+263"}
        };
        private static readonly IList<KeyValuePair<string, string>> phoneNumberPrefixesNorthAmerica = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("US", "+1201"), // Jersey City
            new KeyValuePair<string, string>("US", "+1202"), // Washington
            new KeyValuePair<string, string>("US", "+1203"), // Bridgeport
            new KeyValuePair<string, string>("US", "+1205"), // Birmingham
            new KeyValuePair<string, string>("US", "+1206"), // Seattle
            new KeyValuePair<string, string>("US", "+1207"), // Portland
            new KeyValuePair<string, string>("US", "+1208"), // Boise
            new KeyValuePair<string, string>("US", "+1209"), // Stockton
            new KeyValuePair<string, string>("US", "+1209"), // Stockton
            new KeyValuePair<string, string>("US", "+1210"), // San Antonio
            new KeyValuePair<string, string>("US", "+1212"), // New York City
            new KeyValuePair<string, string>("US", "+1213"), // Los Angeles
            new KeyValuePair<string, string>("US", "+1214"), // Dallas
            new KeyValuePair<string, string>("US", "+1215"), // Philadelphia
            new KeyValuePair<string, string>("US", "+1216"), // Cleveland
            new KeyValuePair<string, string>("US", "+1217"), // Champaign
            new KeyValuePair<string, string>("US", "+1218"), // Duluth
            new KeyValuePair<string, string>("US", "+1219"), // Gary
            new KeyValuePair<string, string>("US", "+1224"), // Waukegan
            new KeyValuePair<string, string>("US", "+1225"), // Baton Rouge
            new KeyValuePair<string, string>("US", "+1227"), // Silver Spring
            new KeyValuePair<string, string>("US", "+1228"), // Biloxi
            new KeyValuePair<string, string>("US", "+1229"), // Albany
            new KeyValuePair<string, string>("US", "+1231"), // Grant
            new KeyValuePair<string, string>("US", "+1234"), // Akron
            new KeyValuePair<string, string>("US", "+1239"), // Cape Coral
            new KeyValuePair<string, string>("US", "+1240"), // Bethesda
            new KeyValuePair<string, string>("US", "+1248"), // Pontiac
            new KeyValuePair<string, string>("US", "+1251"), // Mobile
            new KeyValuePair<string, string>("US", "+1252"), // Rocky Mount
            new KeyValuePair<string, string>("US", "+1253"), // Tacoma
            new KeyValuePair<string, string>("US", "+1254"), // Hamilton
            new KeyValuePair<string, string>("US", "+1256"), // Huntsville
            new KeyValuePair<string, string>("US", "+1260"), // Fort Wayne
            new KeyValuePair<string, string>("US", "+1262"), // Kenosha
            new KeyValuePair<string, string>("US", "+1267"), // Philadelphia
            new KeyValuePair<string, string>("US", "+1269"), // Otsego
            new KeyValuePair<string, string>("US", "+1270"), // Owensboro
            new KeyValuePair<string, string>("US", "+1274"), // Green Bay
            new KeyValuePair<string, string>("US", "+1276"), // Danville
            new KeyValuePair<string, string>("US", "+1278"), // Ann Arbor
            new KeyValuePair<string, string>("US", "+1281"), // Houston
            new KeyValuePair<string, string>("US", "+1283"), // Cincinatti
            new KeyValuePair<string, string>("US", "+1301"), // Silver Spring
            new KeyValuePair<string, string>("US", "+1302"), // Wilmington
            new KeyValuePair<string, string>("US", "+1303"), // Denver
            new KeyValuePair<string, string>("US", "+1304"), // Charleston
            new KeyValuePair<string, string>("US", "+1305"), // Miami
            new KeyValuePair<string, string>("US", "+1307"), // Laramie
            new KeyValuePair<string, string>("US", "+1308"), // Kearney
            new KeyValuePair<string, string>("US", "+1309"), // Rock Island
            new KeyValuePair<string, string>("US", "+1310"), // Los Angeles
            new KeyValuePair<string, string>("US", "+1312"), // Chicago
            new KeyValuePair<string, string>("US", "+1313"), // Detroit
            new KeyValuePair<string, string>("US", "+1314"), // St. Louis
            new KeyValuePair<string, string>("US", "+1315"), // Syracuse
            new KeyValuePair<string, string>("US", "+1316"), // Wichita
            new KeyValuePair<string, string>("US", "+1317"), // Indianapolis
            new KeyValuePair<string, string>("US", "+1318"), // Shreveport
            new KeyValuePair<string, string>("US", "+1319"), // Cedar Rapids
            new KeyValuePair<string, string>("US", "+1320"), // Alexandria
            new KeyValuePair<string, string>("US", "+1321"), // Orlando
            new KeyValuePair<string, string>("US", "+1323"), // Los Angeles
            new KeyValuePair<string, string>("US", "+1325"), // Abilene
            new KeyValuePair<string, string>("US", "+1330"), // Akron
            new KeyValuePair<string, string>("US", "+1331"), // Aurora
            new KeyValuePair<string, string>("US", "+1334"), // Montgomery
            new KeyValuePair<string, string>("US", "+1336"), // Greensboro
            new KeyValuePair<string, string>("US", "+1337"), // Lafayette
            new KeyValuePair<string, string>("US", "+1339"), // Lynn
            new KeyValuePair<string, string>("US", "+1341"), // Oakland
            new KeyValuePair<string, string>("US", "+1347"), // Brooklyn
            new KeyValuePair<string, string>("US", "+1351"), // Lowell
            new KeyValuePair<string, string>("US", "+1352"), // Gainesville
            new KeyValuePair<string, string>("US", "+1360"), // Bellingham
            new KeyValuePair<string, string>("US", "+1361"), // Corpus Christi
            new KeyValuePair<string, string>("US", "+1364"), // Owensboro
            new KeyValuePair<string, string>("US", "+1369"), // Santa Rosa
            new KeyValuePair<string, string>("US", "+1380"), // Columbus
            new KeyValuePair<string, string>("US", "+1385"), // Salt Lake City
            new KeyValuePair<string, string>("US", "+1386"), // Daytona Beach
            new KeyValuePair<string, string>("US", "+1401"), // Providence
            new KeyValuePair<string, string>("US", "+1402"), // Omaha
            new KeyValuePair<string, string>("US", "+1404"), // Atlanta
            new KeyValuePair<string, string>("US", "+1405"), // Oklahoma City
            new KeyValuePair<string, string>("US", "+1406"), // Billings
            new KeyValuePair<string, string>("US", "+1407"), // Orlando
            new KeyValuePair<string, string>("US", "+1408"), // San Jose
            new KeyValuePair<string, string>("US", "+1409"), // Galveston
            new KeyValuePair<string, string>("US", "+1410"), // Baltimore
            new KeyValuePair<string, string>("US", "+1412"), // Pittsburgh
            new KeyValuePair<string, string>("US", "+1413"), // Chicopee
            new KeyValuePair<string, string>("US", "+1414"), // Milwaukee
            new KeyValuePair<string, string>("US", "+1415"), // San Francisco
            new KeyValuePair<string, string>("US", "+1417"), // Springfield
            new KeyValuePair<string, string>("US", "+1419"), // Toledo
            new KeyValuePair<string, string>("US", "+1423"), // Chattanooga
            new KeyValuePair<string, string>("US", "+1424"), // Santa Monica
            new KeyValuePair<string, string>("US", "+1425"), // Bellevue
            new KeyValuePair<string, string>("US", "+1430"), // Tyler
            new KeyValuePair<string, string>("US", "+1432"), // Odessa
            new KeyValuePair<string, string>("US", "+1434"), // Lynchburg
            new KeyValuePair<string, string>("US", "+1435"), // St. George
            new KeyValuePair<string, string>("US", "+1440"), // Cleveland
            new KeyValuePair<string, string>("US", "+1442"), // Escondido
            new KeyValuePair<string, string>("US", "+1443"), // Baltimore
            new KeyValuePair<string, string>("US", "+1445"), // Philadelphia
            new KeyValuePair<string, string>("US", "+1447"), // Champaign
            new KeyValuePair<string, string>("US", "+1458"), // Eugene
            new KeyValuePair<string, string>("US", "+1464"), // Cicero
            new KeyValuePair<string, string>("US", "+1469"), // Dallas
            new KeyValuePair<string, string>("US", "+1470"), // Atlanta
            new KeyValuePair<string, string>("US", "+1475"), // Bridgeport
            new KeyValuePair<string, string>("US", "+1478"), // Macon
            new KeyValuePair<string, string>("US", "+1479"), // Fort Smith
            new KeyValuePair<string, string>("US", "+1480"), // Phoenix
            new KeyValuePair<string, string>("US", "+1484"), // Bethlehem
            new KeyValuePair<string, string>("US", "+1501"), // Little Rock
            new KeyValuePair<string, string>("US", "+1502"), // Louisville
            new KeyValuePair<string, string>("US", "+1503"), // Portland
            new KeyValuePair<string, string>("US", "+1504"), // New Orleans
            new KeyValuePair<string, string>("US", "+1505"), // Albuquerque
            new KeyValuePair<string, string>("US", "+1507"), // Mankato
            new KeyValuePair<string, string>("US", "+1508"), // Worcester
            new KeyValuePair<string, string>("US", "+1509"), // Spokane
            new KeyValuePair<string, string>("US", "+1510"), // Oakland
            new KeyValuePair<string, string>("US", "+1512"), // Austin
            new KeyValuePair<string, string>("US", "+1513"), // Cincinnati
            new KeyValuePair<string, string>("US", "+1515"), // Iowa City
            new KeyValuePair<string, string>("US", "+1516"), // Hempstead
            new KeyValuePair<string, string>("US", "+1517"), // Lansing
            new KeyValuePair<string, string>("US", "+1518"), // Albany
            new KeyValuePair<string, string>("US", "+1520"), // Tucson
            new KeyValuePair<string, string>("US", "+1530"), // Redding
            new KeyValuePair<string, string>("US", "+1531"), // Omaha
            new KeyValuePair<string, string>("US", "+1534"), // Eau Claire
            new KeyValuePair<string, string>("US", "+1540"), // Roanoke
            new KeyValuePair<string, string>("US", "+1541"), // Eugene
            new KeyValuePair<string, string>("US", "+1551"), // Jersey City
            new KeyValuePair<string, string>("US", "+1557"), // St. Louis
            new KeyValuePair<string, string>("US", "+1559"), // Fresno
            new KeyValuePair<string, string>("US", "+1561"), // West Palm Beach
            new KeyValuePair<string, string>("US", "+1562"), // Long Beach
            new KeyValuePair<string, string>("US", "+1563"), // Davenport
            new KeyValuePair<string, string>("US", "+1564"), // Seattle
            new KeyValuePair<string, string>("US", "+1567"), // Toledo
            new KeyValuePair<string, string>("US", "+1570"), // Scranton
            new KeyValuePair<string, string>("US", "+1571"), // Arlington
            new KeyValuePair<string, string>("US", "+1573"), // Columbia
            new KeyValuePair<string, string>("US", "+1574"), // South Bend
            new KeyValuePair<string, string>("US", "+1575"), // Las Cruces
            new KeyValuePair<string, string>("US", "+1580"), // Lawton
            new KeyValuePair<string, string>("US", "+1585"), // Rochester
            new KeyValuePair<string, string>("US", "+1586"), // Warren
            new KeyValuePair<string, string>("US", "+1601"), // Jackson
            new KeyValuePair<string, string>("US", "+1602"), // Phoenix
            new KeyValuePair<string, string>("US", "+1603"), // Manchester
            new KeyValuePair<string, string>("US", "+1605"), // Sioux Falls
            new KeyValuePair<string, string>("US", "+1606"), // Ashland
            new KeyValuePair<string, string>("US", "+1607"), // Elmira
            new KeyValuePair<string, string>("US", "+1608"), // Madison
            new KeyValuePair<string, string>("US", "+1609"), // Atlantic City
            new KeyValuePair<string, string>("US", "+1610"), // Bethlehem
            new KeyValuePair<string, string>("US", "+1612"), // Minneapolis
            new KeyValuePair<string, string>("US", "+1614"), // Columbus
            new KeyValuePair<string, string>("US", "+1615"), // Nashville
            new KeyValuePair<string, string>("US", "+1616"), // Grand Rapids
            new KeyValuePair<string, string>("US", "+1617"), // Boston
            new KeyValuePair<string, string>("US", "+1618"), // Alton
            new KeyValuePair<string, string>("US", "+1619"), // San Diego
            new KeyValuePair<string, string>("US", "+1620"), // Dodge City
            new KeyValuePair<string, string>("US", "+1623"), // Phoenix
            new KeyValuePair<string, string>("US", "+1626"), // Pomona
            new KeyValuePair<string, string>("US", "+1627"), // Santa Rosa
            new KeyValuePair<string, string>("US", "+1628"), // San Francisco
            new KeyValuePair<string, string>("US", "+1630"), // Naperville
            new KeyValuePair<string, string>("US", "+1631"), // Brentwood
            new KeyValuePair<string, string>("US", "+1636"), // St. Charles
            new KeyValuePair<string, string>("US", "+1641"), // Mason City
            new KeyValuePair<string, string>("US", "+1646"), // New York City
            new KeyValuePair<string, string>("US", "+1650"), // Daly City
            new KeyValuePair<string, string>("US", "+1651"), // St. Paul
            new KeyValuePair<string, string>("US", "+1657"), // Anaheim
            new KeyValuePair<string, string>("US", "+1659"), // Birmingham
            new KeyValuePair<string, string>("US", "+1660"), // Marshall
            new KeyValuePair<string, string>("US", "+1661"), // Santa Clarita
            new KeyValuePair<string, string>("US", "+1662"), // Starkville
            new KeyValuePair<string, string>("US", "+1667"), // Baltimore
            new KeyValuePair<string, string>("US", "+1669"), // San Jose
            new KeyValuePair<string, string>("US", "+1678"), // Atlanta
            new KeyValuePair<string, string>("US", "+1679"), // Detroit
            new KeyValuePair<string, string>("US", "+1681"), // Charleston
            new KeyValuePair<string, string>("US", "+1682"), // Fort Worth
            new KeyValuePair<string, string>("US", "+1689"), // Orlando
            new KeyValuePair<string, string>("US", "+1701"), // Fargo
            new KeyValuePair<string, string>("US", "+1702"), // Las Vegas
            new KeyValuePair<string, string>("US", "+1703"), // Arlington
            new KeyValuePair<string, string>("US", "+1704"), // Charlotte
            new KeyValuePair<string, string>("US", "+1706"), // Augusta
            new KeyValuePair<string, string>("US", "+1707"), // Santa Rosa
            new KeyValuePair<string, string>("US", "+1708"), // Cicero
            new KeyValuePair<string, string>("US", "+1712"), // Sioux City
            new KeyValuePair<string, string>("US", "+1713"), // Houston
            new KeyValuePair<string, string>("US", "+1714"), // Anaheim
            new KeyValuePair<string, string>("US", "+1715"), // Eau Claire
            new KeyValuePair<string, string>("US", "+1716"), // Niagara Falls
            new KeyValuePair<string, string>("US", "+1717"), // Lancaster
            new KeyValuePair<string, string>("US", "+1718"), // Brooklyn
            new KeyValuePair<string, string>("US", "+1719"), // Pueblo
            new KeyValuePair<string, string>("US", "+1720"), // Denver
            new KeyValuePair<string, string>("US", "+1724"), // New Castle
            new KeyValuePair<string, string>("US", "+1727"), // St. Petersburg
            new KeyValuePair<string, string>("US", "+1730"), // Alton
            new KeyValuePair<string, string>("US", "+1731"), // Jackson
            new KeyValuePair<string, string>("US", "+1732"), // Edison
            new KeyValuePair<string, string>("US", "+1734"), // Ann Arbor
            new KeyValuePair<string, string>("US", "+1737"), // Austin
            new KeyValuePair<string, string>("US", "+1740"), // Lancaster
            new KeyValuePair<string, string>("US", "+1747"), // Burbank
            new KeyValuePair<string, string>("US", "+1752"), // Anaheim
            new KeyValuePair<string, string>("US", "+1754"), // Fort Lauderdale
            new KeyValuePair<string, string>("US", "+1757"), // Virginia Beach
            new KeyValuePair<string, string>("US", "+1760"), // Escondido
            new KeyValuePair<string, string>("US", "+1762"), // Augusta
            new KeyValuePair<string, string>("US", "+1763"), // Plymouth
            new KeyValuePair<string, string>("US", "+1764"), // Daly City
            new KeyValuePair<string, string>("US", "+1765"), // Lafayette
            new KeyValuePair<string, string>("US", "+1769"), // Jackson
            new KeyValuePair<string, string>("US", "+1770"), // Atlanta
            new KeyValuePair<string, string>("US", "+1772"), // Port St Lucie
            new KeyValuePair<string, string>("US", "+1773"), // Chicago
            new KeyValuePair<string, string>("US", "+1774"), // Worcester
            new KeyValuePair<string, string>("US", "+1775"), // Reno
            new KeyValuePair<string, string>("US", "+1779"), // Rockford
            new KeyValuePair<string, string>("US", "+1781"), // Lynn
            new KeyValuePair<string, string>("US", "+1785"), // Topeka
            new KeyValuePair<string, string>("US", "+1786"), // Miami
            new KeyValuePair<string, string>("US", "+1801"), // Salt Lake City
            new KeyValuePair<string, string>("US", "+1802"), // Brattleboro
            new KeyValuePair<string, string>("US", "+1803"), // Columbia
            new KeyValuePair<string, string>("US", "+1804"), // Richmond
            new KeyValuePair<string, string>("US", "+1805"), // Santa Barbara
            new KeyValuePair<string, string>("US", "+1806"), // Lubbock
            new KeyValuePair<string, string>("US", "+1808"), // Honolulu
            new KeyValuePair<string, string>("US", "+1810"), // Flint
            new KeyValuePair<string, string>("US", "+1812"), // Evansville
            new KeyValuePair<string, string>("US", "+1813"), // Tampa
            new KeyValuePair<string, string>("US", "+1814"), // Erie
            new KeyValuePair<string, string>("US", "+1815"), // Rockford
            new KeyValuePair<string, string>("US", "+1816"), // Kansas City
            new KeyValuePair<string, string>("US", "+1817"), // Fort Worth
            new KeyValuePair<string, string>("US", "+1818"), // Burbank
            new KeyValuePair<string, string>("US", "+1828"), // Asheville
            new KeyValuePair<string, string>("US", "+1830"), // Medina
            new KeyValuePair<string, string>("US", "+1831"), // Salinas
            new KeyValuePair<string, string>("US", "+1832"), // Houston
            new KeyValuePair<string, string>("US", "+1835"), // Bethlehem
            new KeyValuePair<string, string>("US", "+1843"), // Charleston
            new KeyValuePair<string, string>("US", "+1845"), // Kingston
            new KeyValuePair<string, string>("US", "+1847"), // Waukegan
            new KeyValuePair<string, string>("US", "+1848"), // Edison
            new KeyValuePair<string, string>("US", "+1850"), // Tallahassee
            new KeyValuePair<string, string>("US", "+1856"), // Camden
            new KeyValuePair<string, string>("US", "+1857"), // Boston
            new KeyValuePair<string, string>("US", "+1858"), // San Diego
            new KeyValuePair<string, string>("US", "+1859"), // Lexington
            new KeyValuePair<string, string>("US", "+1860"), // Hartford
            new KeyValuePair<string, string>("US", "+1862"), // Newark
            new KeyValuePair<string, string>("US", "+1863"), // Lakeland
            new KeyValuePair<string, string>("US", "+1864"), // Greenville
            new KeyValuePair<string, string>("US", "+1865"), // Knoxville
            new KeyValuePair<string, string>("US", "+1870"), // Jonesboro
            new KeyValuePair<string, string>("US", "+1872"), // Chicago
            new KeyValuePair<string, string>("US", "+1878"), // Pittsburgh
            new KeyValuePair<string, string>("US", "+1901"), // Memphis
            new KeyValuePair<string, string>("US", "+1903"), // Tyler
            new KeyValuePair<string, string>("US", "+1904"), // Jacksonville
            new KeyValuePair<string, string>("US", "+1906"), // Sault Ste Marie
            new KeyValuePair<string, string>("US", "+1907"), // Anchorage
            new KeyValuePair<string, string>("US", "+1908"), // Elizabeth
            new KeyValuePair<string, string>("US", "+1909"), // Anaheim
            new KeyValuePair<string, string>("US", "+1910"), // Fayetteville
            new KeyValuePair<string, string>("US", "+1912"), // Savannah
            new KeyValuePair<string, string>("US", "+1913"), // Kansas City
            new KeyValuePair<string, string>("US", "+1914"), // Yonkers
            new KeyValuePair<string, string>("US", "+1915"), // El Paso
            new KeyValuePair<string, string>("US", "+1916"), // Sacramento
            new KeyValuePair<string, string>("US", "+1917"), // New York City
            new KeyValuePair<string, string>("US", "+1918"), // Tulsa
            new KeyValuePair<string, string>("US", "+1919"), // Raleigh
            new KeyValuePair<string, string>("US", "+1920"), // Green Bay
            new KeyValuePair<string, string>("US", "+1925"), // Concord
            new KeyValuePair<string, string>("US", "+1927"), // Orlando
            new KeyValuePair<string, string>("US", "+1928"), // Yuma
            new KeyValuePair<string, string>("US", "+1931"), // Clarksville
            new KeyValuePair<string, string>("US", "+1935"), // San Diego
            new KeyValuePair<string, string>("US", "+1936"), // Huntsville
            new KeyValuePair<string, string>("US", "+1937"), // Dayton
            new KeyValuePair<string, string>("US", "+1938"), // Huntsville
            new KeyValuePair<string, string>("US", "+1940"), // Denton
            new KeyValuePair<string, string>("US", "+1941"), // Sarasota
            new KeyValuePair<string, string>("US", "+1947"), // Troy
            new KeyValuePair<string, string>("US", "+1949"), // Irvine
            new KeyValuePair<string, string>("US", "+1951"), // Riverside
            new KeyValuePair<string, string>("US", "+1952"), // Bloomington
            new KeyValuePair<string, string>("US", "+1954"), // Fort Lauderdale
            new KeyValuePair<string, string>("US", "+1956"), // Laredo
            new KeyValuePair<string, string>("US", "+1957"), // Albuquerque
            new KeyValuePair<string, string>("US", "+1959"), // Hartford
            new KeyValuePair<string, string>("US", "+1970"), // Grand Junction
            new KeyValuePair<string, string>("US", "+1971"), // Portland
            new KeyValuePair<string, string>("US", "+1972"), // Dallas
            new KeyValuePair<string, string>("US", "+1973"), // Newark
            new KeyValuePair<string, string>("US", "+1975"), // Kansas City
            new KeyValuePair<string, string>("US", "+1978"), // Lowell
            new KeyValuePair<string, string>("US", "+1979"), // Bryan
            new KeyValuePair<string, string>("US", "+1980"), // Charlotte
            new KeyValuePair<string, string>("US", "+1984"), // Raleigh
            new KeyValuePair<string, string>("US", "+1985"), // Hammond
            new KeyValuePair<string, string>("US", "+1989"), // Saginaw
            new KeyValuePair<string, string>("CA", "+1403"), // Calgary
            new KeyValuePair<string, string>("CA", "+1587"), // Calgary
            new KeyValuePair<string, string>("CA", "+1780"), // Edmonton
            new KeyValuePair<string, string>("CA", "+1819"), // Gatineau
            new KeyValuePair<string, string>("CA", "+1902"), // Halifax
            new KeyValuePair<string, string>("CA", "+1519"), // London
            new KeyValuePair<string, string>("CA", "+1226"), // London
            new KeyValuePair<string, string>("CA", "+1905"), // Mississauga
            new KeyValuePair<string, string>("CA", "+1289"), // Mississauga
            new KeyValuePair<string, string>("CA", "+1514"), // Montreal
            new KeyValuePair<string, string>("CA", "+1438"), // Montreal
            new KeyValuePair<string, string>("CA", "+1613"), // Ottawa
            new KeyValuePair<string, string>("CA", "+1343"), // Ottawa
            new KeyValuePair<string, string>("CA", "+1581"), // Quebec City
            new KeyValuePair<string, string>("CA", "+1418"), // Quebec City
            new KeyValuePair<string, string>("CA", "+1306"), // Saskatoon
            new KeyValuePair<string, string>("CA", "+1705"), // Sault Ste. Marie
            new KeyValuePair<string, string>("CA", "+1249"), // Sault Ste. Marie
            new KeyValuePair<string, string>("CA", "+1600"), // Specialized Telecom Services
            new KeyValuePair<string, string>("CA", "+1506"), // St. John
            new KeyValuePair<string, string>("CA", "+1709"), // St. John"s
            new KeyValuePair<string, string>("CA", "+1450"), // Terrebone
            new KeyValuePair<string, string>("CA", "+1579"), // Terrebone
            new KeyValuePair<string, string>("CA", "+1807"), // Thunber Bay
            new KeyValuePair<string, string>("CA", "+1647"), // Toronto
            new KeyValuePair<string, string>("CA", "+1416"), // Toronto
            new KeyValuePair<string, string>("CA", "+1236"), // Vancouver
            new KeyValuePair<string, string>("CA", "+1778"), // Vancouver
            new KeyValuePair<string, string>("CA", "+1604"), // Vancouver
            new KeyValuePair<string, string>("CA", "+1250"), // Victoria
            new KeyValuePair<string, string>("CA", "+1204"), // Winnipeg
            new KeyValuePair<string, string>("CA", "+1867") // Yellowknife
        };

        /// <summary>
        /// Get the phone number prefix collection.
        /// </summary>
        public static IDictionary<string, string> Get => PhoneNumberPrefixes.phoneNumberPrefixes;

        /// <summary>
        /// Find the phone number prefix matching the passed <see cref="string"/>.
        /// </summary>
        /// <param name="e164">The E164 <see cref="string"/>.</param>
        /// <returns>The <see cref="KeyValuePair{TKey,TValue}"/>.</returns>
        public static KeyValuePair<string, string> Find(string e164)
        {
            if (e164 == null)
                throw new ArgumentNullException(nameof(e164));

            if (e164.StartsWith("+1"))
            {
                var northAmericanPrefix = PhoneNumberPrefixes.phoneNumberPrefixesNorthAmerica.FirstOrDefault(x => e164.StartsWith(x.Value));
                if (northAmericanPrefix.Value != null)
                    return northAmericanPrefix;
            }

            return PhoneNumberPrefixes.Get.FirstOrDefault(x => e164.StartsWith(x.Value));
        }
    }
}