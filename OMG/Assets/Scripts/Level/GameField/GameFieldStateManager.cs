using System;
using System.Collections.Generic;

namespace OMG
{
    public interface IGameFieldStateManager
    {
        void ResetField();
        //[Obsolete]
        FieldParseInfo GetFieldInfo();
        void Set(params (int,int)[] savePairs);
        //void Save(FieldParseInfo info);
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
            _fieldParseInfo = _levelParser.Parse(_levelConfig);
            Save(_fieldParseInfo);
        }

        public FieldParseInfo GetFieldInfo()
            => _fieldParseInfo;

        private void Save(FieldParseInfo info)
        {
            _saveService.Save(SaveKey, new FieldSaveData(_levelConfig.Name, info));
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
    }
}
