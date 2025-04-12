using System.Collections.Generic;
using System.Linq;

namespace Favlist.Common;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) => source.OfType<T>();
}