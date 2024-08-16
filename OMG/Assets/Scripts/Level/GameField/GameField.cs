using Cysharp.Threading.Tasks;
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

        UniTask InitializeComponent();
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

            Initialize().Forget();
        }

        public async UniTask Initialize()
        {
            await _gameFieldStateManager.InitializeComponent();
            await gameUIArea.InitializeComponent();
            await GameFieldCommandHandler.InitializeComponent();
        }

        public async UniTask ResetField()
        {
            GameFieldCommandHandler.Clear();
            gameUIArea.Clear();
            _gameFieldStateManager.Clear();

            _gameFieldStateManager.ResetField();

            await _gameFieldStateManager.InitializeComponent();
            await gameUIArea.InitializeComponent();
            await GameFieldCommandHandler.InitializeComponent();
        }
    }
}