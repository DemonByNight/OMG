using Zenject;
using UnityEngine;

namespace OMG
{
    public class GameFieldInstaller : MonoInstaller
    {
        [SerializeField] private GameField gameField;
        [SerializeField] private GameUIArea gameUIArea;

        [Inject] private LevelConfigScriptableObject _levelConfigScriptableObject;
        [Inject] private ILevelParser _levelParser;

        public override void InstallBindings()
        {
            Container.BindInstance(gameField).AsSingle().NonLazy();            
            Container.BindInterfacesTo<GameFieldStateManager>().AsSingle().WithArguments(_levelConfigScriptableObject);
            Container.BindInterfacesTo<GameUIArea>().FromInstance(gameUIArea).AsSingle();
            Container.BindInterfacesTo<GameFieldCommandHandler>().AsSingle();
            Container.Bind<LevelConfigScriptableObject>().FromInstance(_levelConfigScriptableObject);
        }
    }
}
