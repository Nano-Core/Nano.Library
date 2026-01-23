using System.Collections.Generic;
using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveScriptAttr(List<string> values)
    : BaseCspBuilder(values), ICspDirectiveScriptAttr;