using System;
using System.Collections.Generic;
using UnityEngine;

namespace OMG
{
    [CreateAssetMenu(fileName = "new LevelConfig", menuName = "[Level config]/ LevelConfig")]
    public class LevelConfigScriptableObject : ScriptableObject
    {
        public string Name;
        public string Decription;
        public List<Block> BlockInUse;
        public TextAsset LevelAsset;
    }

    [Serializable]
    public class Block
    {
        public int Key;
        public UIBlockBehaviour BlockPrefab; 
    }
}
