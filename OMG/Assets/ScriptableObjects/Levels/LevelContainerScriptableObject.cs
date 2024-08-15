using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OMG
{
    [CreateAssetMenu(fileName = "new LevelContainer", menuName = "[Level config]/ LevelContainer")]
    public class LevelContainerScriptableObject : ScriptableObject
    {
        public List<GameArea> Levels;

        public IReadOnlyList<LevelConfigScriptableObject> GetAreaLevels(string areaKey)
        {
            return Levels.FirstOrDefault(g => g.AreaKey.Equals(areaKey))?.Levels;
        }
    }

    [Serializable]
    public class GameArea
    {
        public string AreaKey;
        public List<LevelConfigScriptableObject> Levels;
    }
}
