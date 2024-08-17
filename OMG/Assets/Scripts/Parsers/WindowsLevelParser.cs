using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OMG
{
    public class WindowsLevelParser : ILevelParser
    {
        public FieldParseInfo Parse(LevelConfigScriptableObject levelConfig)
        {
            FieldParseInfo result = new();
            List<string> rows = new();

            rows = levelConfig.LevelAsset.text.Split(Environment.NewLine).ToList();

            List<string> valueCharacters = new();

            foreach (var row in rows)
            {
                //ensure only 1 space exists between characters
                var correctedString = Regex.Replace(row, @"\s+", " ");
                valueCharacters.AddRange(correctedString.Split(" "));
            }

            foreach (var c in valueCharacters)
            {
                int candidate = -1;

                switch (c)
                {
                    case "-": break;
                    default: int.TryParse(c, out candidate); break;
                }

                result.Blocks.Add(candidate);
            }

            result.Rows = rows.Count;
            result.Columns = result.Blocks.Count / result.Rows;
            result.Blocks = result.Blocks.Split(result.Columns).Reverse().SelectMany(g => g).ToList();

            return result;
        }
    }
}
