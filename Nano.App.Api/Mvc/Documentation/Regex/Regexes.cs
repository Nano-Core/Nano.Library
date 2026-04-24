using System.Text.RegularExpressions;

namespace Nano.App.Api.Mvc.Documentation.Regex;

internal static partial class Regexes
{
    [GeneratedRegex(@"\{([^}]+)\}")]
    internal static partial System.Text.RegularExpressions.Regex CurlyBrackets();

    [GeneratedRegex("<(script|style)([^>]*)>")]
    internal static partial System.Text.RegularExpressions.Regex ScriptAndStyles();
}