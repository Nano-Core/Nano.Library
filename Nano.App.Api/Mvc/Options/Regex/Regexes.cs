using System.Text.RegularExpressions;

namespace Nano.App.Api.Mvc.Options.Regex;

internal static partial class Regexes
{
    [GeneratedRegex(@"\{([^}]+)\}")]
    internal static partial System.Text.RegularExpressions.Regex CurlyBrackets();
}