using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Csp.Directives;
using Nano.App.Api.Mvc.Csp.Directives.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nano.App.Api.Mvc.Csp;

internal sealed class CspBuilder
{
    private readonly Dictionary<string, List<string>?> directives = new();

    internal void UpgradeInsecureRequests()
    {
        this.directives
            .Add("upgrade-insecure-requests", null);
    }
    internal void ReportTo(CspOptions.CspReportToOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.Endpoints.Length == 0)
        {
            return;
        }

        this.directives["report-to"] = [options.Group];
    }
    internal ICspDirectiveDefault DefaultSrc() => new CspDirectiveDefault(this.GetDirectives("default-src"));
    internal ICspDirectiveScript ScriptSrc() => new CspDirectiveScript(this.GetDirectives("script-src"));
    internal ICspDirectiveScriptAttr ScriptSrcAttr() => new CspDirectiveScriptAttr(this.GetDirectives("script-src-attr"));
    internal ICspDirectiveScriptElem ScriptSrcElem() => new CspDirectiveScriptElem(this.GetDirectives("script-src-elem"));
    internal ICspDirectiveStyle StyleSrc() => new CspDirectiveStyleBuilder(this.GetDirectives("style-src"));
    internal ICspDirectiveStyleAttr StyleSrcAttr() => new CspDirectiveStyleAttr(this.GetDirectives("style-src-attr"));
    internal ICspDirectiveStyleElem StyleSrcElem() => new CspDirectiveStyleElem(this.GetDirectives("style-src-elem"));
    internal ICspDirective ObjectSrc() => new CspDirectiveStyleElem(this.GetDirectives("object-src"));
    internal ICspDirective ImageSrc() => new CspDirectiveStyleElem(this.GetDirectives("image-src"));
    internal ICspDirective MediaSrc() => new CspDirectiveStyleElem(this.GetDirectives("media-src"));
    internal ICspDirective FramesSrc() => new CspDirectiveStyleElem(this.GetDirectives("frame-src"));
    internal ICspDirective FencedFramesSrc() => new CspDirectiveStyleElem(this.GetDirectives("fenced-frame-src"));
    internal ICspDirective FrameAncestorsSrc() => new CspDirectiveStyleElem(this.GetDirectives("frame-ancestors-src"));
    internal ICspDirective FontSrc() => new CspDirectiveStyleElem(this.GetDirectives("font-src"));
    internal ICspDirective ConnectionSrc() => new CspDirectiveStyleElem(this.GetDirectives("connection-src"));
    internal ICspDirective BaseUri() => new CspDirectiveStyleElem(this.GetDirectives("base-uri"));
    internal ICspDirective ChildrenSrc() => new CspDirectiveStyleElem(this.GetDirectives("children-src"));
    internal ICspDirective FormSrc() => new CspDirectiveStyleElem(this.GetDirectives("form-action"));
    internal ICspDirective ManifestSrc() => new CspDirectiveStyleElem(this.GetDirectives("manifest-src"));
    internal ICspDirective WorkerSrc() => new CspDirectiveStyleElem(this.GetDirectives("worker-src"));
    internal ICspDirective NavigateTo() => new CspDirectiveStyleElem(this.GetDirectives("navigate-to"));
    internal ICspDirectiveSandbox Sandbox() => new CspDirectiveSandbox(this.GetDirectives("sandbox"));
    internal ICspDirectiveTrustedType TrustedTypes() => new CspDirectiveTrustedType(this.GetDirectives("trusted-types"));

    internal string Build()
    {
        var parts = new List<string>();

        var collection = this.directives
            .Where(d => d.Value == null || (d.Value.Count > 0))
            .Select(x => x.Value == null
                ? x.Key
                : $"{x.Key} {string.Join(" ", x.Value)}");

        parts
            .AddRange(collection);

        return string.Join("; ", parts) + ";";
    }


    private List<string> GetDirectives(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (!this.directives.TryGetValue(name, out var list))
        {
            this.directives[name] = [];
        }

        return list ?? [];
    }
}