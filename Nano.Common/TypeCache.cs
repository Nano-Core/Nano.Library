using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Nano.Common;

/// <summary>
/// Incremental runtime type cache that tracks all loaded assemblies and exposes discovered types for reflection-based discovery.
/// </summary>
public static class TypeCache
{
    private static readonly Lock locker = new();

    private static readonly List<Type> types = [];
    private static readonly HashSet<Assembly> assemblies = [];

    static TypeCache()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            TryAddAssembly(assembly);
        }

        AppDomain.CurrentDomain.AssemblyLoad += (_, args) =>
        {
            TryAddAssembly(args.LoadedAssembly);
        };

        var entry = Assembly.GetEntryAssembly();

        if (entry != null)
        {
            LoadReferencedAssemblies(entry);
        }
    }

    /// <summary>
    /// Gets all discovered types across all tracked assemblies.
    /// </summary>
    public static IReadOnlyCollection<Type> GetAllTypes()
    {
        lock (locker)
        {
            return types
                .ToArray();
        }
    }


    private static void TryAddAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        if (!assemblies.Add(assembly))
        {
            return;
        }

        var name = assembly.GetName().Name;

        if (name is null || name.StartsWith(nameof(Microsoft), StringComparison.Ordinal) || name.StartsWith(nameof(System), StringComparison.Ordinal))
        {
            return;
        }

        lock (locker)
        {
            try
            {
                types
                    .AddRange(assembly.GetTypes());
            }
            catch (ReflectionTypeLoadException ex)
            {
                types
                    .AddRange(ex.Types.Where(t => t != null)!);
            }
        }
    }
    private static void LoadReferencedAssemblies(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (var reference in assembly.GetReferencedAssemblies())
        {
            try
            {
                var loaded = Assembly.Load(reference);

                if (assemblies.Add(loaded))
                {
                    TryAddAssembly(loaded);
                    LoadReferencedAssemblies(loaded);
                }
            }
            catch
            {
                // ignore load failures
            }
        }
    }
}
