using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OMG
{
    public static class MatrixExtensions
    {
        public static int GetRowIndex(this int index, int columns) => index * columns;

        public static HashSet<int> GetAdjacentIndexesHorizontal(this IList<int> matrix, int adjacentIndexesCount, int rows, int columns)
        {
            void AddValueToResult(HashSet<int> resultSet, int from, int to)
            {
                for (int i = from; i <= to; i++)
                {
                    if (resultSet.Contains(i))
                        return;

                    resultSet.Add(i);
                }
            }

            if (columns < adjacentIndexesCount)
                return new();

            HashSet<int> adjacentIndexes = new();

            for (int i = 0; i < rows; i++)
            {
                int adjacentCounter = 0;
                int refValue = matrix[i.GetRowIndex(columns)];

                for (int j = 0; j < columns; j++)
                {
                    int candidate = matrix[i.GetRowIndex(columns) + j];

                    if (candidate == -1)
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

        public static HashSet<int> GetAdjacentIndexesVertical(this IList<int> matrix, int adjacentIndexesCount, int rows, int columns)
        {
            void AddValueToResult(HashSet<int> resultSet, int from, int to)
            {
                for (int i = from; i <= to; i += columns)
                {
                    if (resultSet.Contains(i))
                        return;

                    resultSet.Add(i);
                }
            }

            if (rows < adjacentIndexesCount)
                return new();

            HashSet<int> adjacentIndexes = new();

            for (int j = 0; j < columns; j++)
            {
                int adjacentCounter = 0;
                int refValue = matrix[j];

                for (int i = 0; i < rows; i++)
                {
                    int candidate = matrix[i.GetRowIndex(columns) + j];

                    if (candidate == -1)
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
    }
}
