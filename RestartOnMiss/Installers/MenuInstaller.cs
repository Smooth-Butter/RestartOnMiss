using RestartOnMiss.Views;
using Zenject;

namespace RestartOnMiss.Installers
{
    internal class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ModUI>().FromNewComponentAsViewController().AsSingle().NonLazy();
        }
    }
}