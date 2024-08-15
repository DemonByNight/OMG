using Zenject;

namespace OMG
{
    public class LevelLoaderInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<LevelLoader>().AsSingle();
        }
    }
}
