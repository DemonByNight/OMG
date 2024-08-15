using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Zenject;

namespace OMG
{
    public interface ILevelLoader
    {
        LevelConfigScriptableObject CurrentLevel { get; }
        UniTask LoadNextLevel();
        UniTask RestartLevel();
    }

    public class LevelLoader : ILevelLoader
    {
        private const string LevelsPassedSaveKey = "-levels-passed-";
        private const string LevelSceneName = "Level";

        private readonly ISaveService _saveService;
        private readonly LevelContainerScriptableObject _levelContainerScriptableObject;

        private bool _isLevelSceneLoaded;
        private int _passedLevels = 0;
        public LevelConfigScriptableObject CurrentLevel { get; private set; }

        public LevelLoader(ISaveService saveService, LevelContainerScriptableObject levelContainerScriptableObject)
        {
            _saveService = saveService;
            _levelContainerScriptableObject = levelContainerScriptableObject;

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
            if (_isLevelSceneLoaded)
            {
                await SceneManager.UnloadSceneAsync(LevelSceneName);
                await UniTask.DelayFrame(1);
            }

            CurrentLevel = level;

            await SceneManager.LoadSceneAsync(LevelSceneName, LoadSceneMode.Additive);
            _isLevelSceneLoaded = true;
            await UniTask.DelayFrame(1);
        }

        public async UniTask RestartLevel()
        {
            await StartLevelInternal(CurrentLevel);
        }

        private IReadOnlyList<LevelConfigScriptableObject> GetCurrentLevels(LevelContainerScriptableObject levelConfigs)
            => levelConfigs.GetAreaLevels("Junggle");
    }
}
