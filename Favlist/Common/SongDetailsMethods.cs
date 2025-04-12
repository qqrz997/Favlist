using System.Threading.Tasks;
using SongDetailsCache;

namespace Favlist.Common;

/// <summary>
/// Since SongDetailsCache is NOT a dependency, remember to check if it is installed using <see cref="InstalledMods"/>
/// before calling any method in this class. 
/// </summary>
internal static class SongDetailsMethods
{
    public static async Task<bool> ExistsOnBeatSaver(string hash)
    {
        var songDetails = await SongDetails.Init();
        return songDetails.songs.FindByHash(hash, out _);
    }
}