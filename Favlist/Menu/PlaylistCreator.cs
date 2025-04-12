using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Favlist.Common;
using Favlist.Models;
using Favlist.Resources;
using IPA.Utilities;
using Newtonsoft.Json;

namespace Favlist.Menu;

internal class PlaylistCreator
{
    private readonly PluginConfig config;
    private readonly IPlayerDataModel playerDataModel;

    public PlaylistCreator(PluginConfig config, IPlayerDataModel playerDataModel)
    {
        this.config = config;
        this.playerDataModel = playerDataModel;
    }
    
    private const string PlaylistName = "Favorites (Favlist).bplist";

    public async Task TryConvertFavourites(Action<Exception> onException, Action<PlaylistConversionResult> onSuccess)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var result = await ConvertFavourites();
            onSuccess(result);
        }
        catch (Exception exception)
        {
            onException(exception);
        }

        stopwatch.Stop();
        Plugin.Log.Info($"Created playlist in {stopwatch.ElapsedMilliseconds}ms");
    }
    
    private async Task<PlaylistConversionResult> ConvertFavourites()
    {
        var playlistSongs = await ConvertLevelIds(playerDataModel.playerData.favoritesLevelIds);

        var imageBytes = await ResourceLoader.GetResource("FavlistIcon.png");
        
        var playlist = new BeatSaberPlaylistModel(
            "Favorites (Created by Favlist)",
            "Favlist",
            playlistSongs.OrderBySongName().ToArray(),
            Convert.ToBase64String(imageBytes));
        
        var playlistJson = JsonConvert.SerializeObject(playlist, Formatting.Indented);
        
        var playlistsPath = Path.Combine(UnityGame.InstallPath, "Playlists");
        var playlistsDir = new DirectoryInfo(playlistsPath);
        if (!playlistsDir.Exists) playlistsDir.Create();

        Func<string, string, string> pathFunc = config.OverwriteExisting ? Path.Combine : PathExt.UniqueCombine;
        var filePath = pathFunc(playlistsDir.FullName, PlaylistName);
        
        await File.WriteAllTextAsync(filePath, playlistJson);
        
        return new(playlist, playerDataModel.playerData.favoritesLevelIds.Count);
    }
    
    private async Task<List<PlaylistSongModel>> ConvertLevelIds(IEnumerable<string> levelIds)
    {
        List<PlaylistSongModel> playlistSongs = [];
        foreach (var levelId in levelIds)
        {
            var playlistSong = TryGetSongCoreLevel(levelId, out var beatmapLevel) 
                ? await FromBeatmapLevel(beatmapLevel)
                : await FromLevelId(levelId);
            
            if (playlistSong is null) continue;

            if (!playlistSong.IsValid())
            {
                Plugin.Log.Warn($"Favorites contain an invalid ID: {levelId}");
                continue;
            }
            
            playlistSongs.Add(playlistSong);
        }
        return playlistSongs;
    }

    private async Task<PlaylistSongModel?> FromBeatmapLevel(BeatmapLevel beatmapLevel) =>
        beatmapLevel.hasPrecalculatedData ? PlaylistSongModel.FromBuiltinLevel(beatmapLevel)
        : !ShouldCheckBeatSaver() ? PlaylistSongModel.FromCustomLevel(beatmapLevel)
        : await ExistsOnBeatSaver(beatmapLevel) ? PlaylistSongModel.FromCustomLevel(beatmapLevel)
        : null;
    
    private async Task<PlaylistSongModel?> FromLevelId(string levelId) =>
        !TryGetHashFromId(levelId, out var hash) ? null
        : !config.ExcludeWip && IsWip(levelId) ? new(hash, levelId)
        : !ShouldCheckBeatSaver() ? new(hash, levelId)
        : await ExistsOnBeatSaver(hash) ? new(hash, levelId)
        : null;

    private static bool IsWip(string levelId) => levelId.EndsWith(" WIP", StringComparison.Ordinal);

    private bool ShouldCheckBeatSaver() => InstalledMods.SongDetails && config.CheckBeatSaver;
    
    private static bool TryGetSongCoreLevel(string levelId, [NotNullWhen(true)] out BeatmapLevel? level) => 
        (level = SongCore.Loader.GetLevelById(levelId)) != null;

    private static bool TryGetHashFromId(string levelId, [NotNullWhen(true)] out string? hash)
    {
        hash = null;
        
        var match = Regexes.SongIdHashRegex.Match(levelId);
        if (match.Success) hash = match.Value;
        
        return hash != null;
    }
    
    /// <summary>
    /// Do not call this method without checking <seealso cref="InstalledMods.SongDetails"/> is true
    /// </summary>
    private static async Task<bool> ExistsOnBeatSaver(string hash) => 
        await SongDetailsMethods.ExistsOnBeatSaver(hash);
    
    /// <summary>
    /// Do not call this method without checking <seealso cref="InstalledMods.SongDetails"/> is true
    /// </summary>
    private static async Task<bool> ExistsOnBeatSaver(BeatmapLevel level) =>
        SongCore.Collections.GetCustomLevelHash(level.levelID) is { Length: > 0 } hash 
        && await SongDetailsMethods.ExistsOnBeatSaver(hash);
}