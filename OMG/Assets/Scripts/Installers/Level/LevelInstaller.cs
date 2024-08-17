using Zenject;
using UnityEngine;

namespace OMG
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private GameField gameFieldPrefab;

        public override void InstallBindings()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Container.Bind<ILevelParser>().To<WindowsLevelParser>().AsSingle();
#elif UNITY_ANDROID
            Container.Bind<ILevelParser>().To<AndroidLevelParser>().AsSingle();
#endif
            Container.Bind<GameFieldInstanceProvider>().AsSingle();
            Container.BindInterfacesTo<LevelWinLoseDecider>().AsSingle();
            Container.BindFactory<GameField, GameField.Factory>().FromSubContainerResolve()
                .ByNewContextPrefab(gameFieldPrefab);
        }
    }
}
