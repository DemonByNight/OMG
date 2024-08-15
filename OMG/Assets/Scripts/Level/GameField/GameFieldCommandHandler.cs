using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace OMG
{
    public interface IGameFieldCommandHandler
    {
        UniTask Move(Vector2 viewportStart, Vector2 viewportEnd);
    }

    public class GameFieldCommandHandler : IGameFieldCommandHandler
    {
        private enum Direction
        {
            None = 0,
            Top = 1,
            Right = 2,
            Bottom = 3,
            Left = 4
        }

        private readonly IGameUIArea _gameUIArea;
        private readonly IGameFieldStateManager _gameFieldStateManager;

        private HashSet<int> _indexesInProcess = new();
        private bool _isNormalizationStarted = false;

        public GameFieldCommandHandler(IGameUIArea gameUIArea, IGameFieldStateManager gameFieldStateManager)
        {
            _gameUIArea = gameUIArea;
            _gameFieldStateManager = gameFieldStateManager;
        }

        private async UniTask Move(FieldParseInfo fieldInfo, int indexStart, int indexEnd)
        {
            fieldInfo.Blocks.Swap(indexStart, indexEnd);
            await _gameUIArea.Move(indexStart, indexEnd);
        }

        public async UniTask Move(Vector2 viewportStart, Vector2 viewportEnd)
        {
            int indexStart = _gameUIArea.GetBlockIndexByViewport(viewportStart);
            int indexEnd = _gameUIArea.GetBlockIndexByViewport(viewportEnd);

            if (indexStart == -1 || indexEnd == -1)
                return;

            if (indexStart == indexEnd)
                return;

            if (_indexesInProcess.Contains(indexStart) || _indexesInProcess.Contains(indexEnd))
                return;

            FieldParseInfo fieldInfo = _gameFieldStateManager.GetFieldInfo();

            if (fieldInfo.Blocks[indexStart] == -1)
                return;

            Direction dir = CalculateDirection(fieldInfo, indexStart, indexEnd);

            if (dir == Direction.None)
                return;

            if (dir == Direction.Top && fieldInfo.Blocks[indexEnd] == -1)
                return;

            _indexesInProcess.Add(indexStart);
            _indexesInProcess.Add(indexEnd);

            await Move(fieldInfo, indexStart, indexEnd);

            _indexesInProcess.Remove(indexStart);
            _indexesInProcess.Remove(indexEnd);

            _gameFieldStateManager.Save(fieldInfo);

            NormalizeGameField().Forget();
        }

        private Direction CalculateDirection(FieldParseInfo fpi, int p1, int p2)
        {
            Direction result = Direction.None;

            if (p2 == p1 + 1)
                result = Direction.Right;
            else if (p2 == p1 - 1)
                result = Direction.Left;
            else if (p2 == p1 + fpi.Columns)
                result = Direction.Top;
            else if (p2 == p1 - fpi.Columns)
                result = Direction.Bottom;

            return result;
        }

        private async UniTask NormalizeGameField()
        {
            if (_isNormalizationStarted)
                return;

            await UniTask.WaitUntil(() => _indexesInProcess.Count == 0);

            _isNormalizationStarted = true;

            FieldParseInfo fieldInfo = _gameFieldStateManager.GetFieldInfo();

            //fly
            var flyingPairs = GetFlyingPairs(fieldInfo.Blocks, fieldInfo.Rows, fieldInfo.Columns);
            List<UniTask> waitFly = new();
            foreach (var pair in flyingPairs)
            {
                waitFly.Add(Move(fieldInfo, pair.Key, pair.Value));
            }

            await UniTask.WhenAll(waitFly);

            //adjacent
            var adjacentIndexesHorizontal = fieldInfo.Blocks.GetAdjacentIndexesHorizontal(3, fieldInfo.Rows, fieldInfo.Columns);
            var adjacentIndexesVertical = fieldInfo.Blocks.GetAdjacentIndexesVertical(3, fieldInfo.Rows, fieldInfo.Columns);

            adjacentIndexesHorizontal.UnionWith(adjacentIndexesVertical);
            await _gameUIArea.Destroy(adjacentIndexesHorizontal);

            foreach (var index in adjacentIndexesHorizontal)
            {
                fieldInfo[index] = -1;
            }

            _gameFieldStateManager.Save(fieldInfo);
            _isNormalizationStarted = false;
        }

        private Dictionary<int, int> GetFlyingPairs(IReadOnlyList<int> matrix, int rows, int columns)
        {
            //from -> to indexes
            Dictionary<int, int> flyingPairs = new();

            for (int i = 1; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (matrix[i.GetRowIndex(columns) + j] == -1)
                        continue;

                    (int, int) pair = (-1, -1);

                    int currentIndex = i.GetRowIndex(columns) + j;
                    pair.Item1 = currentIndex;
                    pair.Item2 = -1;

                    for(int k = currentIndex - columns; k >= 0 && matrix[k] == -1; k -= columns)
                    {
                        if (flyingPairs.ContainsKey(k))
                        {
                            pair.Item2 = flyingPairs[k] + columns;
                            break;
                        }

                        pair.Item2 = k;
                    }

                    if (pair.Item2 == -1)
                        continue;

                    flyingPairs.Add(pair.Item1, pair.Item2);
                }
            }

            return flyingPairs;
        }
    }
}
