using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OMG
{
    public class TxtLevelParser : ILevelParser
    {
        public LevelParseInfo Parse(LevelConfigScriptableObject levelConfig)
        {
            LevelParseInfo result = new();

            var rows = levelConfig.LevelAsset.text.Split(Environment.NewLine);
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

            result.Rows = rows.Length;
            result.Columns = result.Blocks.Count / result.Rows;

            return result;
        }
    }
}
