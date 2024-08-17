using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UniRx;

namespace OMG
{
    public interface ILevelInfoContainer
    {
        UniTask LoadNextLevel();
        UniTask RestoreLevel();
        UniTask RestartLevel();
    }

    public class LevelLoader : ILevelInfoContainer
    {
        private const string LevelsPassedSaveKey = "-levels-passed-";

        private readonly ISaveService _saveService;
        private readonly LevelContainerScriptableObject _levelContainerScriptableObject;
        private readonly GameFieldInstanceProvider _gameFieldInstanceProvider;
        private readonly IWinLoseDecider _winLoseDecider;

        private int _passedLevels = 0;
        private LevelConfigScriptableObject _currentLevel;
        private CompositeDisposable _disposables = new();

        public LevelLoader(ISaveService saveService,
            LevelContainerScriptableObject levelContainerScriptableObject,
            GameFieldInstanceProvider gameFieldInstanceProvider,
            IWinLoseDecider winLoseDecider)
        {
            _saveService = saveService;
            _winLoseDecider = winLoseDecider;
            _levelContainerScriptableObject = levelContainerScriptableObject;
            _gameFieldInstanceProvider = gameFieldInstanceProvider;

            _passedLevels = _saveService.GetInt(LevelsPassedSaveKey, 0);
            IReadOnlyList<LevelConfigScriptableObject> levels = GetCurrentLevels(_levelContainerScriptableObject);
            _currentLevel = levels[_passedLevels % levels.Count];
        }

        public async UniTask RestoreLevel()
        {
            ClearState();
            await _gameFieldInstanceProvider.Instance.RestoreField(_currentLevel);
            ObserverLevelCompletion();
        }

        public async UniTask LoadNextLevel()
        {
            ClearState();

            _passedLevels++;
            _saveService.SaveInt(LevelsPassedSaveKey, _passedLevels);

            IReadOnlyList<LevelConfigScriptableObject> levels = GetCurrentLevels(_levelContainerScriptableObject);
            var nextLevel = levels[_passedLevels % levels.Count];

            _currentLevel = nextLevel;

            await _gameFieldInstanceProvider.Instance.LoadNextField(_currentLevel);

            ObserverLevelCompletion();
        }

        public async UniTask RestartLevel()
        {
            ClearState();

            await _gameFieldInstanceProvider.Instance.ResetField();

            ObserverLevelCompletion();
        }

        private void ObserverLevelCompletion()
        {
            _winLoseDecider.Win.Where(x => x == true).Subscribe(async _ =>
            {
                await UniTask.Delay(500);
                await LoadNextLevel();
            }).AddTo(_disposables);

            _winLoseDecider.StartWatching();
        }

        private void ClearState()
        {
            _winLoseDecider?.Clear();
            _disposables?.Clear();
        }

        private IReadOnlyList<LevelConfigScriptableObject> GetCurrentLevels(LevelContainerScriptableObject levelConfigs)
            => levelConfigs.GetAreaLevels("Junggle");
    }
}
