using System.Collections.Generic;

namespace OMG
{
    public class LevelParseInfo
    {
        public List<int> Blocks = new();
        public int Rows;
        public int Columns;

        public int this[int index]
        {
            get => Blocks[index];
        }
    }

    public interface ILevelParser
    {
        LevelParseInfo Parse(LevelConfigScriptableObject levelConfig);
    }
}
