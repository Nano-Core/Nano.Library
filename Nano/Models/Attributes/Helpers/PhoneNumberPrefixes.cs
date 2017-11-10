using System;
using System.Collections.Generic;
using System.Linq;

namespace Nano.Models.Attributes.Helpers
{
    /// <summary>
    /// Phone Number Prefixes.
    /// </summary>
    internal static class PhoneNumberPrefixes
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
        internal static IDictionary<string, string> Get => PhoneNumberPrefixes.phoneNumberPrefixes;

        /// <summary>
        /// Find the phone number prefix matching the passed <see cref="string"/>.
        /// </summary>
        /// <param name="e164">The E164 <see cref="string"/>.</param>
        /// <returns>The <see cref="KeyValuePair{TKey,TValue}"/>.</returns>
        internal static KeyValuePair<string, string> Find(string e164)
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