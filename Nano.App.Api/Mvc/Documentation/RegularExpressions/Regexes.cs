using System.Text.RegularExpressions;

namespace Nano.App.Api.Mvc.Documentation.RegularExpressions;

internal static partial class Regexes
{
    [GeneratedRegex(@"\{([^}]+)\}", RegexOptions.Compiled)]
    internal static partial Regex CurlyBrackets();

    [GeneratedRegex(@"<script([^>]*)>", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    internal static partial Regex ScriptTags();

    [GeneratedRegex(@"<style([^>]*)>", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    internal static partial Regex StyleTags();
}