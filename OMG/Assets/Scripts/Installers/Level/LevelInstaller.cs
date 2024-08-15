using Zenject;

namespace OMG
{
    public class LevelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ILevelParser>().To<TxtLevelParser>().AsSingle();
        }
    }
}
