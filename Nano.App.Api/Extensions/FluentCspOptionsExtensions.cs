using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Nano.App.Api.Config;

namespace Nano.App.Api.Extensions;

// TODO: Csp Directives
// navigate-to,
// prefetch-src,
// require-trusted-types-for,
// trusted-types,
// require-sri-for,
// report-to,
// script-src-attr,
// script-src-elem,
// style-src-attr,
// style-src-elem,
// Sandbox (allow-downloads-without-user-activation, allow-storage-access-by-user-activation, allow-top-navigation-by-user-activation)
// report only?

internal static class FluentCspOptionsExtensions
{
    internal static CspBuilder UseCspReportUris(this CspBuilder builder, string[] reportUris)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (!reportUris.Any())
        {
            return builder;
        }

        foreach (var reportUri in reportUris)
        {
            builder
                .AddReportUri()
                .To(reportUri);
        }

        return builder;
    }

    internal static CspBuilder UseCspDefaults(this CspBuilder builder, CspOptions.CspDirective cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .DefaultSources(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspStyles(this CspBuilder builder, CspOptions.CspDirectiveStyles cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .StyleSources(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.IsUnsafeInline)
        //            {
        //                x.UnsafeInline();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspScripts(this CspBuilder builder, CspOptions.CspDirectiveScripts cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .ScriptSources(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.IsUnsafeEval)
        //            {
        //                x.UnsafeEval();
        //            }

        //            if (cspDirective.IsUnsafeInline)
        //            {
        //                x.UnsafeInline();
        //            }

        //            if (cspDirective.StrictDynamic)
        //            {
        //                x.StrictDynamic();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspObjects(this CspBuilder builder, CspOptions.CspDirective cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .ObjectSources(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspImages(this CspBuilder builder, CspOptions.CspDirective cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .ImageSources(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspMedia(this CspBuilder builder, CspOptions.CspDirective cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .MediaSources(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspFrames(this CspBuilder builder, CspOptions.CspDirective cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .FrameSources(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspFrameAncestors(this CspBuilder builder, CspOptions.CspDirective cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .FrameAncestors(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspFonts(this CspBuilder builder, CspOptions.CspDirective cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .FontSources(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspConnections(this CspBuilder builder, CspOptions.CspDirective cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .ConnectSources(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspBaseUris(this CspBuilder builder, CspOptions.CspDirective cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .BaseUris(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspChildren(this CspBuilder builder, CspOptions.CspDirective cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .ChildSources(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspForms(this CspBuilder builder, CspOptions.CspDirective cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .FormActions(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspManifests(this CspBuilder builder, CspOptions.CspDirective cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .ManifestSources(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspWorkers(this CspBuilder builder, CspOptions.CspDirective cspDirective = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirective == null)
        {
            return builder;
        }

        //builder
        //    .WorkerSources(x =>
        //    {
        //        if (cspDirective.IsNone)
        //        {
        //            x.None();
        //        }
        //        else
        //        {
        //            if (cspDirective.IsSelf)
        //            {
        //                x.Self();
        //            }

        //            if (cspDirective.Sources.Any())
        //            {
        //                x.CustomSources(cspDirective.Sources);
        //            }
        //        }
        //    });

        return builder;
    }

    internal static CspBuilder UseCspSandbox(this CspBuilder builder, CspOptions.CspDirectiveSandbox cspDirectiveSandbox = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (cspDirectiveSandbox == null)
        {
            return builder;
        }

        //builder
        //    .Sandbox(x =>
        //    {
        //        if (cspDirectiveSandbox.AllowForms)
        //        {
        //            x.AllowForms();
        //        }

        //        if (cspDirectiveSandbox.AllowModals)
        //        {
        //            x.AllowModals();
        //        }

        //        if (cspDirectiveSandbox.AllowOrientationLock)
        //        {
        //            x.AllowOrientationLock();
        //        }

        //        if (cspDirectiveSandbox.AllowPointerLock)
        //        {
        //            x.AllowPointerLock();
        //        }

        //        if (cspDirectiveSandbox.AllowPopups)
        //        {
        //            x.AllowPopups();
        //        }

        //        if (cspDirectiveSandbox.AllowPopupsToEscapeSandbox)
        //        {
        //            x.AllowPopupsToEscapeSandbox();
        //        }

        //        if (cspDirectiveSandbox.AllowPresentation)
        //        {
        //            x.AllowPresentation();
        //        }

        //        if (cspDirectiveSandbox.AllowSameOrigin)
        //        {
        //            x.AllowSameOrigin();
        //        }

        //        if (cspDirectiveSandbox.AllowScripts)
        //        {
        //            x.AllowScripts();
        //        }

        //        if (cspDirectiveSandbox.AllowTopNavigation)
        //        {
        //            x.AllowTopNavigation();
        //        }
        //    });

        return builder;
    }
}