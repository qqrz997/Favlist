using System.Text.RegularExpressions;

namespace Favlist.Common;

internal static class Regexes
{
    public static Regex SongIdHashRegex { get; } = new("[A-F0-9]{40}", RegexOptions.Compiled);
}