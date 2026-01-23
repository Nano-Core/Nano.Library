using System;
using Nano.App.Api.Config;

namespace Nano.App.Api.Mvc.Csp.Extensions;

internal static class CspBuilderExtensions
{
    internal static CspBuilder UseUpgradeInsecureRequests(this CspBuilder builder, CspOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        if (options.UpgradeInsecureRequests)
        {
            builder
                .UpgradeInsecureRequests();
        }

        return builder;
    }
  
    internal static CspBuilder UseCspReportTo(this CspBuilder builder, CspOptions.CspReportToOptions? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        if (cspDirective.Endpoints.Length > 0)
        {
            builder
                .ReportTo(cspDirective);
        }

        return builder;
    }

    internal static CspBuilder UseCspDefault(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var defaultSrc = builder
            .DefaultSrc();

        if (cspDirective.IsNone)
        {
            defaultSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                defaultSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                defaultSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspStyle(this CspBuilder builder, CspOptions.CspDirectiveStyles? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var styleSrc = builder
            .StyleSrc();

        if (cspDirective.IsNone)
        {
            styleSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                styleSrc
                    .Self();
            }

            if (cspDirective.IsUnsafeInline)
            {
                styleSrc
                    .UnsafeInline();
            }

            if (cspDirective.IsUnsafeHashes)
            {
                styleSrc
                    .UnsafeHashes();
            }

            if (cspDirective.Sources.Length > 0)
            {
                styleSrc
                    .Sources(cspDirective.Sources);
            }

            if (cspDirective.Nonces.Length > 0)
            {
                styleSrc
                    .Nonces(cspDirective.Nonces);
            }

            if (cspDirective.Hashes.Length > 0)
            {
                styleSrc
                    .Hashes(cspDirective.Hashes);
            }

            if (cspDirective.RequireTrustedTypes)
            {
                styleSrc
                    .RequireTrustedType();
            }

            if (cspDirective.RequireSri)
            {
                styleSrc
                    .RequireSri();
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspStyleAttr(this CspBuilder builder, CspOptions.CspDirectiveElement? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var styleSrcAttr = builder
            .StyleSrcAttr();

        if (cspDirective.IsNone)
        {
            styleSrcAttr
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                styleSrcAttr
                    .Self();
            }

            if (cspDirective.IsUnsafeInline)
            {
                styleSrcAttr
                    .UnsafeInline();
            }

            if (cspDirective.Sources.Length > 0)
            {
                styleSrcAttr
                    .Sources(cspDirective.Sources);
            }

            if (cspDirective.Nonces.Length > 0)
            {
                styleSrcAttr
                    .Nonces(cspDirective.Nonces);
            }

            if (cspDirective.Hashes.Length > 0)
            {
                styleSrcAttr
                    .Hashes(cspDirective.Hashes);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspStyleElem(this CspBuilder builder, CspOptions.CspDirectiveElement? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var styleSrcAttr = builder
            .StyleSrcElem();

        if (cspDirective.IsNone)
        {
            styleSrcAttr
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                styleSrcAttr
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                styleSrcAttr
                    .Sources(cspDirective.Sources);
            }

            if (cspDirective.Nonces.Length > 0)
            {
                styleSrcAttr
                    .Nonces(cspDirective.Nonces);
            }

            if (cspDirective.Hashes.Length > 0)
            {
                styleSrcAttr
                    .Hashes(cspDirective.Hashes);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspScript(this CspBuilder builder, CspOptions.CspDirectiveScripts? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var scriptSrc = builder
            .ScriptSrc();

        if (cspDirective.IsNone)
        {
            scriptSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                scriptSrc
                    .Self();
            }

            if (cspDirective.IsUnsafeEval)
            {
                scriptSrc
                    .UnsafeEval();
            }

            if (cspDirective.IsUnsafeHashes)
            {
                scriptSrc
                    .UnsafeHashes();
            }

            if (cspDirective.IsUnsafeInline)
            {
                scriptSrc
                    .UnsafeInline();
            }

            if (cspDirective.IsUnsafeWasmEval)
            {
                scriptSrc
                    .UnsafeWasmEval();
            }

            if (cspDirective.StrictDynamic)
            {
                scriptSrc
                    .StrictDynamic();
            }

            if (cspDirective.UnsafeHashedAttributes)
            {
                scriptSrc
                    .UnsafeHashedAttributes();
            }

            if (cspDirective.UnsafeAllowRedirects)
            {
                scriptSrc
                    .UnsafeAllowRedirects();
            }

            if (cspDirective.ReportSample)
            {
                scriptSrc
                    .ReportSample();
            }

            if (cspDirective.Sources.Length > 0)
            {
                scriptSrc
                    .Sources(cspDirective.Sources);
            }

            if (cspDirective.Nonces.Length > 0)
            {
                scriptSrc
                    .Nonces(cspDirective.Nonces);
            }

            if (cspDirective.Hashes.Length > 0)
            {
                scriptSrc
                    .Hashes(cspDirective.Hashes);
            }

            if (cspDirective.RequireTrustedTypes)
            {
                scriptSrc
                    .RequireTrustedType();
            }

            if (cspDirective.RequireSri)
            {
                scriptSrc
                    .RequireSri();
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspScriptAttr(this CspBuilder builder, CspOptions.CspDirectiveElement? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var scriptSrcAttr = builder
            .ScriptSrcAttr();

        if (cspDirective.IsNone)
        {
            scriptSrcAttr
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                scriptSrcAttr
                    .Self();
            }

            if (cspDirective.IsUnsafeInline)
            {
                scriptSrcAttr
                    .UnsafeInline();
            }

            if (cspDirective.Sources.Length > 0)
            {
                scriptSrcAttr
                    .Sources(cspDirective.Sources);
            }

            if (cspDirective.Nonces.Length > 0)
            {
                scriptSrcAttr
                    .Nonces(cspDirective.Nonces);
            }

            if (cspDirective.Hashes.Length > 0)
            {
                scriptSrcAttr
                    .Hashes(cspDirective.Hashes);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspScriptElem(this CspBuilder builder, CspOptions.CspDirectiveElement? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var scriptSrcElem = builder
            .ScriptSrcElem();

        if (cspDirective.IsNone)
        {
            scriptSrcElem
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                scriptSrcElem
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                scriptSrcElem
                    .Sources(cspDirective.Sources);
            }

            if (cspDirective.Nonces.Length > 0)
            {
                scriptSrcElem
                    .Nonces(cspDirective.Nonces);
            }

            if (cspDirective.Hashes.Length > 0)
            {
                scriptSrcElem
                    .Hashes(cspDirective.Hashes);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspObject(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var objectSrc = builder
            .ObjectSrc();

        if (cspDirective.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                objectSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspImage(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var objectSrc = builder
            .ImageSrc();

        if (cspDirective.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                objectSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspMedia(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var objectSrc = builder
            .MediaSrc();

        if (cspDirective.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                objectSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspFrame(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var objectSrc = builder
            .FramesSrc();

        if (cspDirective.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                objectSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspFencedFrame(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var objectSrc = builder
            .FencedFramesSrc();

        if (cspDirective.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                objectSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspFrameAncestor(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var objectSrc = builder
            .FrameAncestorsSrc();

        if (cspDirective.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                objectSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspFont(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var objectSrc = builder
            .FontSrc();

        if (cspDirective.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                objectSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspConnection(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var objectSrc = builder
            .ConnectionSrc();

        if (cspDirective.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                objectSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspBaseUri(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var objectSrc = builder
            .BaseUri();

        if (cspDirective.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                objectSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspChildren(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var objectSrc = builder
            .ChildrenSrc();

        if (cspDirective.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                objectSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspForm(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var objectSrc = builder
            .FormSrc();

        if (cspDirective.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                objectSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspManifest(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var objectSrc = builder
            .ManifestSrc();

        if (cspDirective.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                objectSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspWorker(this CspBuilder builder, CspOptions.CspDirective? cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var objectSrc = builder
            .WorkerSrc();

        if (cspDirective.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (cspDirective.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (cspDirective.Sources.Length > 0)
            {
                objectSrc
                    .Sources(cspDirective.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseTrustedTypes(this CspBuilder builder, CspOptions.CspDirectiveTrustedTypes? cspDirective)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        var trustedTypes = builder
            .TrustedTypes();

        if (cspDirective.IsNone)
        {
            trustedTypes
                .None();
        }
        else
        {
            if (cspDirective.AllowDuplicates)
            {
                trustedTypes
                    .AllowDuplicates();
            }

            if (cspDirective.Policies.Length > 0)
            {
                trustedTypes
                    .TrustedTypes(cspDirective.Policies);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspSandbox(this CspBuilder builder, CspOptions.CspDirectiveSandbox? cspDirectiveSandbox = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirectiveSandbox == null)
        {
            return builder;
        }

        var sandbox = builder
            .Sandbox();

        if (cspDirectiveSandbox.AllowDownloads)
        {
            sandbox
                .AllowDownloads();
        }

        if (cspDirectiveSandbox.AllowForms)
        {
            sandbox
                .AllowForms();
        }

        if (cspDirectiveSandbox.AllowModals)
        {
            sandbox
                .AllowModals();
        }

        if (cspDirectiveSandbox.AllowOrientationLock)
        {
            sandbox
                .AllowOrientationLock();
        }

        if (cspDirectiveSandbox.AllowPointerLock)
        {
            sandbox
                .AllowPointerLock();
        }

        if (cspDirectiveSandbox.AllowPopups)
        {
            sandbox
                .AllowPopups();
        }

        if (cspDirectiveSandbox.AllowPopupsToEscapeSandbox)
        {
            sandbox
                .AllowPopupsToEscapeSandbox();
        }

        if (cspDirectiveSandbox.AllowPresentation)
        {
            sandbox
                .AllowPresentation();
        }

        if (cspDirectiveSandbox.AllowSameOrigin)
        {
            sandbox
                .AllowSameOrigin();
        }

        if (cspDirectiveSandbox.AllowScripts)
        {
            sandbox
                .AllowScripts();
        }

        if (cspDirectiveSandbox.AllowStorageAccessByUserActivation)
        {
            sandbox
                .AllowStorageAccessByUserActivation();
        }

        if (cspDirectiveSandbox.AllowTopNavigation)
        {
            sandbox
                .AllowTopNavigation();
        }

        if (cspDirectiveSandbox.AllowTopNavigationByUserActivation)
        {
            sandbox
                .AllowTopNavigationByUserActivation();
        }

        if (cspDirectiveSandbox.AllowTopNavigationToCustomProtocols)
        {
            sandbox
                .AllowTopNavigationToCustomProtocols();
        }

        return builder;
    }
}