using System.Linq;

namespace Favlist.Common;

internal static class BeatmapExtensions
{
    public static string GetLevelAuthors(this BeatmapLevel beatmapLevel) =>
        string.Join(", ", beatmapLevel.allMappers.Concat(beatmapLevel.allLighters).Distinct());
}