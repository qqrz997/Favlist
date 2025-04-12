using IPA.Loader;

namespace Favlist.Common;

internal static class InstalledMods
{
    public static bool SongDetails { get; } = PluginManager.GetPluginFromId("SongDetailsCache") != null;
    public static bool PlaylistManager { get; } = PluginManager.GetPluginFromId("PlaylistManager") != null;
}