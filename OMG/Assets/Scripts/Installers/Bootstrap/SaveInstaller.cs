using Zenject;

namespace OMG
{
    public class SaveInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<JSONSaveService>().AsSingle();
        }
    }
}
