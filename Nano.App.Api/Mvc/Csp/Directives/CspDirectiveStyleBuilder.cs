using System.Collections.Generic;
using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveStyleBuilder(List<string> values)
    : BaseCspBuilder(values), ICspDirectiveStyle
{
    public void UnsafeHashes() => this.values.Add("'unsafe-hashes'");
    public void RequireTrustedType() => this.values.Add("'require-trusted-types-for style'");
    public void RequireSri() => this.values.Add("'require-sri-for style'");
}