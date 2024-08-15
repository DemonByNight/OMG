using System;
using Zenject;

namespace OMG
{
    public interface IGameFieldStateManager
    {
        void ResetField();
        FieldParseInfo GetFieldInfo();
        void Save(FieldParseInfo info);
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

        private readonly LevelConfigScriptableObject _levelConfig;
        private readonly ISaveService _saveService;
        private readonly ILevelParser _levelParser;

        private FieldParseInfo _fieldParseInfo;

        public GameFieldStateManager(ILevelParser levelParser, 
            LevelConfigScriptableObject levelConfig,
            ISaveService saveService)
        {
            _levelParser = levelParser;
            _levelConfig = levelConfig;
            _saveService = saveService;

            var checkSave = _saveService.Get<FieldSaveData>(SaveKey, new());
            if (string.IsNullOrEmpty(checkSave.LevelKey) ||
                !checkSave.LevelKey.Equals(_levelConfig.Name))
            {
                ResetField();
            }
            else
            {
                _fieldParseInfo = checkSave.GameAreaState;
            }
        }

        public void ResetField()
        {
            Save(_levelParser.Parse(_levelConfig));
        }

        public FieldParseInfo GetFieldInfo()
        {
            return _fieldParseInfo.GetCopy();
        }

        public void Save(FieldParseInfo info)
        {
            _fieldParseInfo = info;
            _saveService.Save(SaveKey, new FieldSaveData(_levelConfig.Name, info));
        }
    }
}
