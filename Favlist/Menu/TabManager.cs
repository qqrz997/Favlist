using System;
using BeatSaberMarkupLanguage.GameplaySetup;
using Zenject;

namespace Favlist.Menu;

internal class TabManager : IInitializable, IDisposable
{
    private readonly GameplaySetup gameplaySetup;
    private readonly GameplaySetupTab gameplaySetupTab;

    private const string MenuName = "Favorites to Playlist";
    private const string ResourcePath = "Favlist.Menu.gameplaySetup.bsml";

    public TabManager(GameplaySetup gameplaySetup, GameplaySetupTab gameplaySetupTab)
    {
        this.gameplaySetup = gameplaySetup;
        this.gameplaySetupTab = gameplaySetupTab;
    }

    public void Initialize()
    {
        gameplaySetup.AddTab(MenuName, ResourcePath, gameplaySetupTab);
    }

    public void Dispose()
    {
        gameplaySetup.RemoveTab(MenuName);
    }
}