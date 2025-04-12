using Zenject;
using Favlist.Menu;

namespace Favlist.Installers;

internal class MenuInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<TabManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameplaySetupTab>().AsSingle();
    }
}