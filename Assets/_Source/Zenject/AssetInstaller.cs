// _Source/Installers/AssetInstaller.cs

using AssetSystem;

namespace Zenject
{
    public class AssetInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AssetRepository>().AsSingle();

            Container.BindInterfacesAndSelfTo<AssetManager>().AsSingle().NonLazy();
        }
    }
}