using Cysharp.Threading.Tasks;
using System.Collections.Generic;

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

        private int _passedLevels = 0;
        private LevelConfigScriptableObject _currentLevel;

        public LevelLoader(ISaveService saveService,
            LevelContainerScriptableObject levelContainerScriptableObject,
            GameFieldInstanceProvider gameFieldInstanceProvider)
        {
            _saveService = saveService;
            _levelContainerScriptableObject = levelContainerScriptableObject;
            _gameFieldInstanceProvider = gameFieldInstanceProvider;

            _passedLevels = _saveService.GetInt(LevelsPassedSaveKey, 0);
            IReadOnlyList<LevelConfigScriptableObject> levels = GetCurrentLevels(_levelContainerScriptableObject);
            _currentLevel = levels[_passedLevels % levels.Count];
        }

        public async UniTask RestoreLevel()
        {
            await _gameFieldInstanceProvider.Instance.RestoreField(_currentLevel);
        }

        public async UniTask LoadNextLevel()
        {
            _passedLevels++;
            _saveService.SaveInt(LevelsPassedSaveKey, _passedLevels);

            IReadOnlyList<LevelConfigScriptableObject> levels = GetCurrentLevels(_levelContainerScriptableObject);
            var nextLevel = levels[_passedLevels % levels.Count];

            _currentLevel = nextLevel;
            await _gameFieldInstanceProvider.Instance.LoadNextField(_currentLevel);
        }

        public async UniTask RestartLevel()
        {
            await _gameFieldInstanceProvider.Instance.ResetField();
        }

        private IReadOnlyList<LevelConfigScriptableObject> GetCurrentLevels(LevelContainerScriptableObject levelConfigs)
            => levelConfigs.GetAreaLevels("Junggle");
    }
}
