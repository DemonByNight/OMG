using Cysharp.Threading.Tasks;
using System;

namespace OMG
{
    public interface IGameFieldStateManager : IGameFieldComponent
    {
        void ResetField();
        FieldParseInfo GetFieldInfo();
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
            var checkSave = _saveService.Get<FieldSaveData>(SaveKey, new());
            if (string.IsNullOrEmpty(checkSave.LevelKey) ||
                !checkSave.LevelKey.Equals(_levelLoader.CurrentLevel.Name))
            {
                ResetField();
            }
            else
            {
                _fieldParseInfo = checkSave.GameAreaState;
            }

            Initialized = true;
        }

        public void ResetField()
        {
            _fieldParseInfo = _levelParser.Parse(_levelLoader.CurrentLevel);
            Save(_fieldParseInfo);
        }

        public FieldParseInfo GetFieldInfo()
            => _fieldParseInfo;

        private void Save(FieldParseInfo info)
        {
            _saveService.Save(SaveKey, new FieldSaveData(_levelLoader.CurrentLevel.Name, info));
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
