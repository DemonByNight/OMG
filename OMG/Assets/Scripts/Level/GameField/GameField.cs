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

        public IGameFieldCommandHandler GameFieldCommandHandler { get; private set; }

        public class Factory : PlaceholderFactory<LevelConfigScriptableObject, GameField> { }

        [Inject]
        private void Construct(IGameFieldCommandHandler gameFieldCommandHandler)
        {
            GameFieldCommandHandler = gameFieldCommandHandler;
        }
    }
}