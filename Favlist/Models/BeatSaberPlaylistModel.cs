using System;
using Newtonsoft.Json;

namespace Favlist.Models;

[Serializable]
internal class BeatSaberPlaylistModel
{
    [JsonProperty("playlistTitle")]
    public string PlaylistTitle { get; }
    
    [JsonProperty("playlistAuthor")]
    public string PlaylistAuthor { get; }
    
    [JsonProperty("songs")]
    public PlaylistSongModel[] Songs { get; }
   
    [JsonProperty("image")]
    public string Image { get; }
    
    [JsonConstructor]
    public BeatSaberPlaylistModel(
        string playlistTitle, string playlistAuthor, PlaylistSongModel[] songs, string image) => 
        (PlaylistTitle, PlaylistAuthor, Songs, Image) = (playlistTitle, playlistAuthor, songs, image);
}