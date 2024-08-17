using System.Collections.Generic;
using System.Linq;

namespace OMG
{
    public static class MatrixExtensions
    {
        public static int GetRowIndex(this int index, int columns) => index * columns;

        public static HashSet<int> GetAdjacentIndexesHorizontal(this IList<int> matrix, int adjacentIndexesCount, int rows, int columns, ICollection<int> exceptIndex = null)
        {
            void AddValueToResult(HashSet<int> resultSet, int from, int to)
            {
                for (int i = from; i <= to; i++)
                {
                    if (resultSet.Contains(i))
                        continue;

                    resultSet.Add(i);
                }
            }

            ICollection<int> localExceptIndex = exceptIndex ?? new List<int>();

            if (columns < adjacentIndexesCount)
                return new();

            HashSet<int> adjacentIndexes = new();

            for (int i = 0; i < rows; i++)
            {
                int adjacentCounter = 0;
                int refValue = matrix[i.GetRowIndex(columns)];

                for (int j = 0; j < columns; j++)
                {
                    int currentIndex = i.GetRowIndex(columns) + j;
                    int candidate = matrix[currentIndex];

                    if (candidate == -1 || localExceptIndex.Contains(currentIndex))
                    {
                        adjacentCounter = 0;
                    }
                    else if (candidate == refValue)
                    {
                        adjacentCounter++;
                    }
                    else
                    {
                        adjacentCounter = 1;
                        refValue = candidate;
                    }

                    if (adjacentCounter >= 3)
                    {
                        AddValueToResult(adjacentIndexes, i.GetRowIndex(columns) + j - adjacentCounter + 1, i.GetRowIndex(columns) + j);
                    }
                }
            }

            return adjacentIndexes;
        }

        public static HashSet<int> GetAdjacentIndexesVertical(this IList<int> matrix, int adjacentIndexesCount, int rows, int columns, ICollection<int> exceptIndex = null)
        {
            void AddValueToResult(HashSet<int> resultSet, int from, int to)
            {
                for (int i = from; i <= to; i += columns)
                {
                    if (resultSet.Contains(i))
                        continue;

                    resultSet.Add(i);
                }
            }

            ICollection<int> localExceptIndex = exceptIndex ?? new List<int>();

            if (rows < adjacentIndexesCount)
                return new();

            HashSet<int> adjacentIndexes = new();

            for (int j = 0; j < columns; j++)
            {
                int adjacentCounter = 0;
                int refValue = matrix[j];

                for (int i = 0; i < rows; i++)
                {
                    int currentIndex = i.GetRowIndex(columns) + j;
                    int candidate = matrix[currentIndex];

                    if (candidate == -1 || localExceptIndex.Contains(currentIndex))
                    {
                        adjacentCounter = 0;
                    }
                    else if (candidate == refValue)
                    {
                        adjacentCounter++;
                    }
                    else
                    {
                        adjacentCounter = 1;
                        refValue = candidate;
                    }

                    if (adjacentCounter >= 3)
                    {
                        AddValueToResult(adjacentIndexes, i.GetRowIndex(columns) + j - columns * (adjacentCounter - 1), i.GetRowIndex(columns) + j);
                    }
                }
            }

            return adjacentIndexes;
        }

        public static ISet<int> GetAreaBlocks(this IList<int> matrix, int rows, int columns, ISet<int> areaBlocks, ICollection<int> exceptIndex = null)
        {
            void FindBlocksInArea(IList<int> matrix, int rows, int columns, int blockIndex, ISet<int> areaBlocks, ISet<int> newAreaBlocks, ICollection<int> exceptIndex = null)
            {
                List<int> adjacentIndexes = new() { blockIndex - 1, blockIndex + 1 };
                adjacentIndexes.RemoveAll(g => g / columns != blockIndex / columns);
                adjacentIndexes.Add(blockIndex - columns);
                adjacentIndexes.Add(blockIndex + columns);
                adjacentIndexes.RemoveAll(g => g < 0 || g >= matrix.Count);
                adjacentIndexes.RemoveAll(g => areaBlocks.Contains(g) || exceptIndex.Contains(g));
                int refValue = matrix[blockIndex];

                foreach (var index in adjacentIndexes)
                {
                    if (matrix[index] != refValue
                        || exceptIndex.Contains(index))
                        continue;

                    newAreaBlocks.Add(index);
                    FindBlocksInArea(matrix, rows, columns, index, areaBlocks, newAreaBlocks, exceptIndex);
                }
            }

            ICollection<int> localExceptIndex = exceptIndex ?? new List<int>();
            HashSet<int> result = new();

            foreach (var block in areaBlocks)
            {
                FindBlocksInArea(matrix, rows, columns, block, areaBlocks, result, exceptIndex);
            }

            return result;
        }

        public static Dictionary<int, int> GetFlyingPairs(this IList<int> matrix, int rows, int columns, ICollection<int> exceptIndex = null)
        {
            ICollection<int> localExceptIndex = exceptIndex ?? new List<int>();

            //from -> to indexes
            Dictionary<int, int> flyingPairs = new();

            for (int i = 1; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int currentIndex = i.GetRowIndex(columns) + j;

                    if (matrix[currentIndex] == -1
                        || localExceptIndex.Contains(currentIndex))
                        continue;

                    (int, int) pair = (-1, -1);

                    pair.Item1 = currentIndex;
                    pair.Item2 = -1;

                    for (int k = currentIndex - columns; k >= 0; k -= columns)
                    {
                        if (localExceptIndex.Contains(k))
                            break;

                        if (flyingPairs.ContainsKey(k))
                        {
                            pair.Item2 = flyingPairs[k] + columns;
                            break;
                        }

                        if (matrix[k] == -1)
                            pair.Item2 = k;
                    }

                    if (pair.Item2 == -1)
                        continue;

                    flyingPairs.Add(pair.Item1, pair.Item2);
                }
            }

            return flyingPairs;
        }

        public static bool HasAnyFlyingIndex(this IList<int> matrix, int rows, int columns, ICollection<int> exceptIndex = null)
        {
            ICollection<int> localExceptIndex = exceptIndex ?? new List<int>();

            for (int i = 1; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int currentIndex = i.GetRowIndex(columns) + j;

                    if (matrix[currentIndex] == -1
                        || localExceptIndex.Contains(currentIndex))
                        continue;

                    (int, int) pair = (-1, -1);

                    pair.Item1 = currentIndex;
                    pair.Item2 = -1;

                    for (int k = currentIndex - columns; k >= 0 && matrix[k] == -1; k -= columns)
                    {
                        if (localExceptIndex.Contains(k))
                            continue;

                        return true;
                    }
                }
            }

            return false;
        }

        public static IReadOnlyList<int> GetNumbersInBetween(this int startIndex, int endIndex, int step)
        {
            void Swap(ref int a, ref int b)
            {
                a += b;
                b = a - b;
                a -= b;
            }

            List<int> result = new();

            if (step > 0 && startIndex > endIndex)
            {
                Swap(ref startIndex, ref endIndex);
            }
            else if (step < 0 && endIndex > startIndex)
            {
                Swap(ref startIndex, ref endIndex);
            }
            else if (step == 0)
                return result;

            for (int i = startIndex; i <= endIndex; i += step)
            {
                result.Add(i);
            }

            return result;
        }
    }
}
