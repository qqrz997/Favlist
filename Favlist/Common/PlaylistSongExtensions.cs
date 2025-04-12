using System.Collections.Generic;
using System.Linq;
using Favlist.Models;

namespace Favlist.Common;

internal static class PlaylistSongExtensions
{
    // A playlist song must specify either a level id or a custom level hash to be usable in a playlist
    public static bool IsValid(this PlaylistSongModel song) => 
        !string.IsNullOrWhiteSpace(song.Hash) || !string.IsNullOrWhiteSpace(song.LevelId);

    public static IEnumerable<PlaylistSongModel> OrderBySongName(this IEnumerable<PlaylistSongModel> source) =>
        source.OrderBy(x => x.SongName);
}