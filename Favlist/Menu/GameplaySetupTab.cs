using System.Collections.Concurrent;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using Favlist.Common;
using HMUI;
using SongCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Favlist.Menu;

internal class GameplaySetupTab
{
    [Inject] private readonly PluginConfig config = null!;
    [Inject] private readonly PlaylistCreator playlistCreator = null!;
    [Inject] private readonly Loader loader = null!;
    
    [UIComponent("convert-button")] private readonly Button convertButton = null!;
    [UIComponent("message")] private readonly TextMeshProUGUI messageBoxText = null!;

    [UIComponent("beatsaver-toggle")] private readonly ToggleSetting beatsaverToggle = null!;
    [UIComponent("refresh-playlists-button")] private readonly Button refreshPlaylistsButton = null!;
    
    private readonly Color okColor = new(0.4f, 0.8f, 0.4f);
    private readonly Color warningColor = new(0.8f, 0.8f, 0.4f);
    private readonly Color errorColor = new(0.8f, 0.4f, 0.4f);
    
    [UIAction("#post-parse")]
    public void PostParse()
    {
        if (!InstalledMods.SongDetails)
        {
            beatsaverToggle.Interactable = false;
            beatsaverToggle.Value = false;
            var text = beatsaverToggle.transform.Find("NameText").GetComponent<CurvedTextMeshPro>();
            text.alpha = 0.5f;
            text.text += " (Requires SongDetailsCache)";
        }

        if (InstalledMods.PlaylistManager)
        {
            refreshPlaylistsButton.gameObject.SetActive(true);
            if (Loader.AreSongsLoading)
            {
                Loader.SongsLoadedEvent += LoaderOnSongsLoaded;
                refreshPlaylistsButton.interactable = false;
            }
        }
    }
    
    public bool OverwriteExisting
    {
        get => config.OverwriteExisting;
        set => config.OverwriteExisting = value;
    }

    public bool CheckBeatSaver
    {
        get => config.CheckBeatSaver;
        set => config.CheckBeatSaver = value;
    }

    public bool ExcludeWip
    {
        get => config.ExcludeWip;
        set => config.ExcludeWip = value;
    }
    
    // ReSharper disable once AsyncVoidMethod
    public async void ConvertButtonPressed()
    {
        convertButton.interactable = false;
        messageBoxText.text = string.Empty;

        await playlistCreator.TryConvertFavourites(
            onException: exception =>
            {
                messageBoxText.color = errorColor;
                messageBoxText.text = $"Encountered a problem while trying to create Favlist.\n" +
                                      $"{exception.GetType().Name}:\n" +
                                      $"\"{exception.Message}\"";
        
                Plugin.Log.Error(exception);
            },
            onSuccess: result =>
            {
                messageBoxText.color = okColor;
                messageBoxText.text = $"Found {result.FavoriteIdsCount} favorite song IDs in player data.\n" +
                                      $"Created playlist with {result.Playlist.Songs.Length} favorite maps.";
            });
        
        await CallbackAfterDelay.StartUnitySafe(1500, () => convertButton.interactable = true);
    }

    public void RefreshPlaylistsButtonPressed()
    {
        if (Loader.AreSongsLoading)
        {
            messageBoxText.color = warningColor;
            messageBoxText.text = "Refresh denied:\n" +
                                  "Songs are loading, try again later.";
            return;
        }

        Loader.SongsLoadedEvent -= LoaderOnSongsLoaded;
        Loader.SongsLoadedEvent += LoaderOnSongsLoaded; 
        loader.RefreshSongs(fullRefresh: false);

        refreshPlaylistsButton.interactable = false;
        
        messageBoxText.color = okColor;
        messageBoxText.text = "Playlists refreshed.";
    }

    private void LoaderOnSongsLoaded(Loader arg1, ConcurrentDictionary<string, BeatmapLevel> arg2)
    {
        Loader.SongsLoadedEvent -= LoaderOnSongsLoaded;
        CallbackAfterDelay.StartUnitySafe(1500, () => refreshPlaylistsButton.interactable = true);
    }
}