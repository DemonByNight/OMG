using UnityEngine;
using Zenject;

namespace OMG
{
    public interface IGameField
    {
        IGameFieldCommandHandler GameFieldCommandHandler { get; }
    }

    public interface IGameFieldComponent
    {
        bool Initialized { get; }

        void InitializeComponent();
        void Clear();
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

            Initialize();
        }

        public void Initialize()
        {
            _gameFieldStateManager.InitializeComponent();
            gameUIArea.InitializeComponent();
            GameFieldCommandHandler.InitializeComponent();
        }

        public void ResetField()
        {
            GameFieldCommandHandler.Clear();
            gameUIArea.Clear();
            _gameFieldStateManager.Clear();

            _gameFieldStateManager.ResetField();

            _gameFieldStateManager.InitializeComponent();
            gameUIArea.InitializeComponent();
            GameFieldCommandHandler.InitializeComponent();
        }
    }
}