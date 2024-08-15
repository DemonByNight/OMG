using UnityEngine;
using Zenject;

namespace OMG
{
    public interface IGameField
    {
        IGameFieldCommandHandler GameFieldCommandHandler { get; }
    }

    public class GameField : MonoBehaviour, IGameField
    {
        [SerializeField] private GameUIArea gameUIArea;

        private IGameFieldStateManager _gameFieldStateManager;
        public IGameFieldCommandHandler GameFieldCommandHandler { get; private set; }

        public class Factory : PlaceholderFactory<LevelConfigScriptableObject, GameField> { }

        [Inject]
        private void Construct(IGameFieldCommandHandler gameFieldCommandHandler, IGameFieldStateManager gameFieldStateManager)
        {
            GameFieldCommandHandler = gameFieldCommandHandler;
            _gameFieldStateManager = gameFieldStateManager;
        }

        public void ResetField()
        {
            _gameFieldStateManager.ResetField();
        }
    }
}