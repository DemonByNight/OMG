using System;
using System.Collections.Generic;

namespace OMG
{
    [Serializable]
    public class FieldParseInfo
    {
        public List<int> Blocks = new();
        public int Rows;
        public int Columns;

        public int this[int index]
        {
            get => Blocks[index];
            set => Blocks[index] = value;
        }

        public FieldParseInfo GetCopy()
        {
            var copy = new FieldParseInfo();
            copy.Blocks = new(Blocks);
            copy.Rows = Rows;
            copy.Columns = Columns;
            return copy;
        }
    }

    public interface ILevelParser
    {
        FieldParseInfo Parse(LevelConfigScriptableObject levelConfig);
    }
}
