using Zenject;
using UnityEngine;

namespace OMG
{
    public class GameFieldInstaller : MonoInstaller
    {
        [SerializeField] private GameField gameField;
        [SerializeField] private GameUIArea gameUIArea;

        public override void InstallBindings()
        {
            Container.BindInstance(gameField).AsSingle().NonLazy();            
            Container.BindInterfacesTo<GameFieldStateManager>().AsSingle();
            Container.BindInterfacesTo<GameUIArea>().FromInstance(gameUIArea).AsSingle();
            Container.BindInterfacesTo<GameFieldCommandHandler>().AsSingle();
        }
    }
}
