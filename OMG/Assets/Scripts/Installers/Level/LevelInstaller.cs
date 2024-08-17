using Zenject;
using UnityEngine;

namespace OMG
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private GameField gameFieldPrefab;

        public override void InstallBindings()
        {
            Container.Bind<ILevelParser>().To<TxtLevelParser>().AsSingle();
            Container.Bind<GameFieldInstanceProvider>().AsSingle();
            Container.BindInterfacesTo<LevelWinLoseDecider>().AsSingle();
            Container.BindFactory<GameField, GameField.Factory>().FromSubContainerResolve()
                .ByNewContextPrefab(gameFieldPrefab);
        }
    }
}
