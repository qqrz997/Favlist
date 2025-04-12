using System;
using Newtonsoft.Json;

namespace Favlist.Models;

[Serializable]
internal class PlaylistSongDifficultyModel
{
    [JsonProperty("characteristic")]
    public string Characteristic { get; }
    
    [JsonProperty("name")]
    public string Name { get; }
 
    [JsonConstructor]
    public PlaylistSongDifficultyModel(string characteristic, string name) =>
        (Characteristic, Name) = (characteristic, name);
}