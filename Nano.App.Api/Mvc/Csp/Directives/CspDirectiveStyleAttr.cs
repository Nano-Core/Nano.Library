using System.Collections.Generic;
using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveStyleAttr(List<string> values)
    : BaseCspBuilder(values), ICspDirectiveStyleAttr;