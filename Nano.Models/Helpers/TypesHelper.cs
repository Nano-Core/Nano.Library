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
    /// 
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetAllTypes()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(x => x.FullName != null && x.FullName.StartsWith(nameof(Microsoft)))
            .SelectMany(x => x.GetTypes());
    }
}
