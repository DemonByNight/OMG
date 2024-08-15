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
            Container.BindFactory<LevelConfigScriptableObject, GameField, GameField.Factory>().FromSubContainerResolve()
                .ByNewContextPrefab<GameFieldInstaller>(gameFieldPrefab);
        }
    }
}
