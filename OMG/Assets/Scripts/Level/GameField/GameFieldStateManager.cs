using System;

namespace OMG
{
    public interface IGameFieldStateManager
    {
        FieldParseInfo GetFieldInfo();
        void Save(FieldParseInfo info);
    }

    [Serializable]
    public class FieldSaveData
    {
        public string LevelKey;
        public FieldParseInfo GameAreaState;

        public FieldSaveData(string levelKey, FieldParseInfo gameAreaState)
        {
            LevelKey = levelKey;
            GameAreaState = gameAreaState;
        }
    }

    public class GameFieldStateManager : IGameFieldStateManager
    {
        
        public const string SaveKey = "FieldSaveData";

        private string _levelKey;
        private FieldParseInfo _fieldParseInfo;
        private ISaveService _saveService;

        public GameFieldStateManager(string levelKey, FieldParseInfo fieldParseInfo, ISaveService saveService)
        {
            _levelKey = levelKey;
            _fieldParseInfo = fieldParseInfo;
            _saveService = saveService;
        }

        public FieldParseInfo GetFieldInfo() 
        {
            return _fieldParseInfo.GetCopy();
        }

        public void Save(FieldParseInfo info)
        {
            _fieldParseInfo = info;
            _saveService.Save(SaveKey, new FieldSaveData(_levelKey, info));
        }
    }
}
