using System;
using System.Collections.Generic;
using System.Linq;

namespace Nano.Models.Helpers;

/// <summary>
/// Types Helper
/// </summary>
public static class TypesHelper
{
    /// <summary>
    /// Get All Types.
    /// </summary>
    /// <returns>A collection of types.</returns>
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

                return x;
            })
            .Where(x => x != null)
            .SelectMany(x => x.GetTypes())
            .Where(x => x != null);
    }
}
