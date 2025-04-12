using System;
using Favlist.Common;
using Newtonsoft.Json;

namespace Favlist.Models;

[Serializable]
internal class PlaylistSongModel
{
    [JsonProperty("songName", NullValueHandling = NullValueHandling.Ignore)]
    public string? SongName { get; }
    
    [JsonProperty("levelAuthorName", NullValueHandling = NullValueHandling.Ignore)]
    public string? LevelAuthorName { get; }
    
    [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
    public string? Hash { get; }
    
    [JsonProperty("levelid", NullValueHandling = NullValueHandling.Ignore)]
    public string? LevelId { get; }
    
    [JsonProperty("difficulties", NullValueHandling = NullValueHandling.Ignore)]
    public PlaylistSongDifficultyModel[]? Difficulties { get; }

    public static PlaylistSongModel FromCustomLevel(BeatmapLevel l)
    {
        var hash = SongCore.Collections.GetCustomLevelHash(l.levelID) is { Length: > 0 } h ? h : null;
        var authors = l.GetLevelAuthors() is { Length: > 0 } a ? a : null;
        
        return new(l.songName, authors, hash, l.levelID);
    }
    
    public static PlaylistSongModel FromBuiltinLevel(BeatmapLevel l)
    {
        var authors = l.GetLevelAuthors() is { Length: > 0 } a ? a : null;
        
        return new(l.songName, authors, null, l.levelID);
    }

    [JsonConstructor]
    public PlaylistSongModel(
        string? songName,
        string? levelAuthorName,
        string? hash,
        string? levelid,
        PlaylistSongDifficultyModel[]? difficulties) =>
        (SongName, LevelAuthorName, Hash, LevelId, Difficulties) =
        (songName, levelAuthorName, hash, levelid, difficulties);

    public PlaylistSongModel(string levelId) => LevelId = levelId;
    
    public PlaylistSongModel(string hash, string levelId) =>
        (Hash, LevelId) = (hash, levelId);
    
    public PlaylistSongModel(string? songName, string? levelAuthorName, string? hash, string levelId) =>
        (SongName, LevelAuthorName, Hash, LevelId) = (songName, levelAuthorName, hash, levelId);
}