using System;
using System.Collections.Generic;
using System.Linq;

namespace Nano.Common.Helpers;

/// <summary>
/// Provides helper methods for working with <see cref="Type"/> objects across loaded assemblies.
/// </summary>
public static class TypesHelper
{
    /// <summary>
    /// Gets all types from all loaded assemblies in the current <see cref="AppDomain"/>,
    /// excluding assemblies whose names start with "Microsoft".
    /// </summary>
    /// <returns>A collection of <see cref="Type"/> objects.</returns>
    public static IEnumerable<Type> GetAllTypes()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(x =>
            {
                var name = x.GetName().Name;

                if (name == null)
                {
                    return null;
                }

                return name.StartsWith(nameof(Microsoft), StringComparison.Ordinal)
                    ? null
                    : x;
            })
            .Where(x => x != null)
            .SelectMany(x => x!.GetTypes());
    }
}