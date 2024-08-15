using System;
using System.Collections.Generic;

namespace OMG
{
    [Serializable]
    public class LevelSaveData
    {
        [NonSerialized]
        public const string SaveKey = "LevelSaveData";

        public string LevelName;
        public List<int> GameAreaState = new();
    }
}
