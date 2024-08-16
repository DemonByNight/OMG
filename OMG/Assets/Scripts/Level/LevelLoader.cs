using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace OMG
{
    public interface ILevelInfoContainer
    {
        LevelConfigScriptableObject CurrentLevel { get; }
        UniTask LoadNextLevel();
        UniTask RestartLevel();
    }

    public class LevelLoader : ILevelInfoContainer
    {
        private const string LevelsPassedSaveKey = "-levels-passed-";

        private readonly ISaveService _saveService;
        private readonly LevelContainerScriptableObject _levelContainerScriptableObject;
        private readonly GameFieldInstanceProvider _gameFieldInstanceProvider;

        private int _passedLevels = 0;

        public LevelConfigScriptableObject CurrentLevel { get; private set; }

        public LevelLoader(ISaveService saveService,
            LevelContainerScriptableObject levelContainerScriptableObject,
            GameFieldInstanceProvider gameFieldInstanceProvider)
        {
            _saveService = saveService;
            _levelContainerScriptableObject = levelContainerScriptableObject;
            _gameFieldInstanceProvider = gameFieldInstanceProvider;

            _passedLevels = _saveService.Get<int>(LevelsPassedSaveKey, 0);
            IReadOnlyList<LevelConfigScriptableObject> levels = GetCurrentLevels(_levelContainerScriptableObject);
            CurrentLevel = levels[_passedLevels % levels.Count];
        }

        public async UniTask LoadNextLevel()
        {
            _passedLevels++;
            _saveService.Save<int>(LevelsPassedSaveKey, _passedLevels);

            IReadOnlyList<LevelConfigScriptableObject> levels = GetCurrentLevels(_levelContainerScriptableObject);
            var nextLevel = levels[_passedLevels % levels.Count];

            await StartLevelInternal(nextLevel);
        }

        private async UniTask StartLevelInternal(LevelConfigScriptableObject level)
        {
            CurrentLevel = level;
            _gameFieldInstanceProvider.Instance.ResetField();
        }

        public async UniTask RestartLevel()
        {
            await StartLevelInternal(CurrentLevel);
        }

        private IReadOnlyList<LevelConfigScriptableObject> GetCurrentLevels(LevelContainerScriptableObject levelConfigs)
            => levelConfigs.GetAreaLevels("Junggle");
    }
}
