using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace OMG
{
    public interface IGameField
    {
        IGameFieldCommandHandler GameFieldCommandHandler { get; }
        IGameFieldStateProvider GameFieldStateProvider { get; }
        IGameFieldUIStateProvider GameFieldUIStateProvider { get; }
    }

    public interface IGameFieldComponent
    {
        bool Initialized { get; }

        UniTask InitializeComponent();
        void Clear();
    }

    public class GameField : MonoBehaviour, IGameField
    {
        [SerializeField] private GameUIArea gameUIArea;

        private IGameFieldStateManager _gameFieldStateManager;

        public IGameFieldCommandHandler GameFieldCommandHandler { get; private set; }
        public IGameFieldStateProvider GameFieldStateProvider { get; private set; }
        public IGameFieldUIStateProvider GameFieldUIStateProvider => gameUIArea;

        public class Factory : PlaceholderFactory<GameField> { }

        [Inject]
        private void Construct(IGameFieldCommandHandler gameFieldCommandHandler, IGameFieldStateManager gameFieldStateManager)
        {
            GameFieldCommandHandler = gameFieldCommandHandler;
            GameFieldStateProvider = _gameFieldStateManager = gameFieldStateManager;
        }

        public async UniTask RestoreField(LevelConfigScriptableObject levelConfig)
        {
            ClearFieldState();

            await _gameFieldStateManager.InitializeComponent();
            _gameFieldStateManager.RestoreField(levelConfig);
            await gameUIArea.InitializeComponent();
            await GameFieldCommandHandler.InitializeComponent();
        }

        public async UniTask LoadNextField(LevelConfigScriptableObject levelConfig)
        {
            ClearFieldState();

            await _gameFieldStateManager.InitializeComponent();
            _gameFieldStateManager.LoadNextField(levelConfig);
            await gameUIArea.InitializeComponent();
            await GameFieldCommandHandler.InitializeComponent();
        }

        public async UniTask ResetField()
        {
            ClearFieldState();

            _gameFieldStateManager.ResetField();

            await _gameFieldStateManager.InitializeComponent();
            await gameUIArea.InitializeComponent();
            await GameFieldCommandHandler.InitializeComponent();
        }

        private void ClearFieldState()
        {
            GameFieldCommandHandler.Clear();
            gameUIArea.Clear();
            _gameFieldStateManager.Clear();
        }
    }
}