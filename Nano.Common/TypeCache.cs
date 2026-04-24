using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Nano.Common;

/// <summary>
/// Provides a cached, lazy-loaded snapshot of all <see cref="Type"/> instances discovered across the application's loaded and referenced assemblies.
/// </summary>
public static class TypeCache
{
    private static readonly Lock locker = new();

    private static Type[]? types;
    private static bool initialized;

    /// <summary>
    /// Gets all discovered <see cref="Type"/> instances from the application's entry assembly and all recursively resolved referenced assemblies.
    /// </summary>
    /// <returns>A read-only collection of all discovered types available at initialization time.</returns>
    public static IReadOnlyCollection<Type> GetAllTypes()
    {
        if (initialized)
        {
            return types!;
        }

        lock (locker)
        {
            if (initialized)
            {
                return types!;
            }

            var assemblies = LoadAllReferencedAssemblies();

            var result = new List<Type>();

            foreach (var assembly in assemblies)
            {
                TryAddTypes(assembly, result);
            }

            types = result
                .ToArray();

            initialized = true;

            return types;
        }
    }


    private static HashSet<Assembly> LoadAllReferencedAssemblies()
    {
        var assemblies = new HashSet<Assembly>();

        var entry = Assembly.GetEntryAssembly();

        if (entry != null)
        {
            LoadRecursive(entry, assemblies);
        }

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            LoadRecursive(asm, assemblies);
        }

        return assemblies;
    }
    private static void LoadRecursive(Assembly assembly, ISet<Assembly> hashSet)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(hashSet);

        if (!hashSet.Add(assembly))
        {
            return;
        }

        AssemblyName[] references;

        try
        {
            references = assembly.GetReferencedAssemblies();
        }
        catch
        {
            return;
        }

        foreach (var reference in references)
        {
            try
            {
                var loaded = Assembly.Load(reference);

                LoadRecursive(loaded, hashSet);
            }
            catch
            {
                // ignore load failures (optional logging could go here)
            }
        }
    }
    private static void TryAddTypes(Assembly assembly, List<Type> results)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(results);

        try
        {
            results
                .AddRange(assembly.GetTypes());
        }
        catch (ReflectionTypeLoadException ex)
        {
            results
                .AddRange(ex.Types.Where(x => x != null)!);
        }
    }
}