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
        [Inject] private ISaveService _saveService;

        public override void InstallBindings()
        {
            Container.BindInstance(gameField).AsSingle().NonLazy();            

            FieldParseInfo levelParseInfo = _levelParser.Parse(_levelConfigScriptableObject);
            Container.BindInterfacesTo<GameFieldStateManager>().AsSingle().WithArguments(_levelConfigScriptableObject.Name, levelParseInfo);

            Container.BindInterfacesTo<GameUIArea>().FromInstance(gameUIArea).AsSingle();
            Container.BindInterfacesTo<GameFieldCommandHandler>().AsSingle();
            Container.Bind<LevelConfigScriptableObject>().FromInstance(_levelConfigScriptableObject);
        }
    }
}
