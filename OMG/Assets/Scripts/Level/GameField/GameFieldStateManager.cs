using Cysharp.Threading.Tasks;
using System;

namespace OMG
{
    public interface IGameFieldStateManager : IGameFieldComponent
    {
        void RestoreField(LevelConfigScriptableObject levelConfigs);
        void LoadNextField(LevelConfigScriptableObject levelConfigs);
        void ResetField();
        FieldParseInfo GetFieldInfo();
        LevelConfigScriptableObject CurrentLevelConfig { get; }
        void Set(params (int,int)[] savePairs);
    }

    [Serializable]
    public class FieldSaveData
    {
        public string LevelKey;
        public FieldParseInfo GameAreaState;

        public FieldSaveData()
        {

        }

        public FieldSaveData(string levelKey, FieldParseInfo gameAreaState)
        {
            LevelKey = levelKey;
            GameAreaState = gameAreaState;
        }
    }

    public class GameFieldStateManager : IGameFieldStateManager
    {
        public const string SaveKey = "FieldSaveData";

        private readonly ILevelInfoContainer _levelLoader;
        private readonly ISaveService _saveService;
        private readonly ILevelParser _levelParser;

        private FieldParseInfo _fieldParseInfo;

        public LevelConfigScriptableObject CurrentLevelConfig { get; private set; }
        public bool Initialized { get; private set; }

        public GameFieldStateManager(ILevelParser levelParser,
            ILevelInfoContainer levelLoader,
            ISaveService saveService)
        {
            _levelParser = levelParser;
            _levelLoader = levelLoader;
            _saveService = saveService;
        }

        public async UniTask InitializeComponent()
        {
            Initialized = true;
        }

        public void RestoreField(LevelConfigScriptableObject levelConfig)
        {
            CurrentLevelConfig = levelConfig;

            var checkSave = _saveService.Get<FieldSaveData>(SaveKey, new());
            if (string.IsNullOrEmpty(checkSave.LevelKey) ||
                !checkSave.LevelKey.Equals(CurrentLevelConfig.Name))
            {
                ResetField();
            }
            else
            {
                _fieldParseInfo = checkSave.GameAreaState;
            }
        }

        public void LoadNextField(LevelConfigScriptableObject levelConfig)
        {
            CurrentLevelConfig = levelConfig;
            ResetField();
        }

        public void ResetField()
        {
            _fieldParseInfo = _levelParser.Parse(CurrentLevelConfig);
            Save(_fieldParseInfo);
        }

        public FieldParseInfo GetFieldInfo()
            => _fieldParseInfo;

        private void Save(FieldParseInfo info)
        {
            _saveService.Save(SaveKey, new FieldSaveData(CurrentLevelConfig.Name, info));
        }
        
        //(index, value)
        public void Set(params (int, int)[] savePairs)
        {
            var consistentCopy = _fieldParseInfo.GetCopy();

            foreach (var pair in savePairs)
            {
                consistentCopy[pair.Item1] = pair.Item2;
            }

            _fieldParseInfo = consistentCopy;
            Save(_fieldParseInfo);
        }

        public void Clear()
        {
            Initialized = false;
        }
    }
}
