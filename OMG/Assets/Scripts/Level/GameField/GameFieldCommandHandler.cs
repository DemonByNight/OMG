using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;

namespace OMG
{
    public interface IGameFieldCommandHandler : IGameFieldComponent
    {
        bool IsNormalizationInProcess { get; }
        UniTask Move(Vector2 viewportStart, Vector2 viewportEnd);
    }

    public enum Direction
    {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 3,
        Left = 4
    }

    public class GameFieldCommandHandler : IGameFieldCommandHandler, IDisposable
    {
        private readonly IGameUIArea _gameUIArea;
        private readonly IGameFieldStateManager _gameFieldStateManager;

        private CompositeDisposable _disposables = new();
        private HashSet<int> _uiBlockedIndexes = new();
        private HashSet<int> _normalizationBlockedIndexes = new();
        private CancellationTokenSource _cts;

        public bool IsNormalizationInProcess { get; private set; }
        public bool Initialized { get; private set; }

        public GameFieldCommandHandler(IGameUIArea gameUIArea, IGameFieldStateManager gameFieldStateManager)
        {
            _gameUIArea = gameUIArea;
            _gameFieldStateManager = gameFieldStateManager;
        }

        public async UniTask InitializeComponent()
        {
            _cts = new();

            Observable.EveryUpdate().Subscribe(async _ =>
            {
                if (IsNormalizationInProcess)
                    return;

                IsNormalizationInProcess = true;

                int processedCount = 0;
                do
                {
                    processedCount = await NormalizeGameField();
                } while (processedCount > 0);

                IsNormalizationInProcess = false;
            }).AddTo(_disposables);

            Initialized = true;
        }

        private async UniTask Move(int indexStart, int indexEnd, CancellationToken token)
        {
            var fieldInfo = _gameFieldStateManager.GetFieldCurrentState();

            _gameFieldStateManager.Set(
                (indexStart, fieldInfo[indexEnd]),
                (indexEnd, fieldInfo[indexStart]));

            await _gameUIArea.Move(indexStart, indexEnd, token);
        }

        public async UniTask Move(Vector2 viewportStart, Vector2 viewportEnd)
        {
            if (!Initialized)
                return;

            Direction dir = CalculateDirection(viewportStart, viewportEnd);

            int indexStart = _gameUIArea.GetBlockIndexByViewport(viewportStart);
            int indexEnd = _gameUIArea.GetBlockIndexByViewport(viewportStart, dir);

            if (indexStart == -1 || indexEnd == -1)
                return;

            if (indexStart == indexEnd)
                return;

            if (_uiBlockedIndexes.Contains(indexStart) || _uiBlockedIndexes.Contains(indexEnd)
                || _normalizationBlockedIndexes.Contains(indexStart) || _normalizationBlockedIndexes.Contains(indexEnd))
                return;

            FieldParseInfo fieldInfo = _gameFieldStateManager.GetFieldCurrentState();

            if (fieldInfo.Blocks[indexStart] == -1)
                return;

            if (dir == Direction.None)
                return;

            if (dir == Direction.Up && fieldInfo.Blocks[indexEnd] == -1)
                return;

            _uiBlockedIndexes.Add(indexStart);
            _uiBlockedIndexes.Add(indexEnd);

            await Move(indexStart, indexEnd, _cts.Token);

            if (_cts.IsCancellationRequested)
                return;

            _normalizationBlockedIndexes.Add(indexStart);
            _normalizationBlockedIndexes.Add(indexEnd);
            _uiBlockedIndexes.Remove(indexStart);
            _uiBlockedIndexes.Remove(indexEnd);
        }

        private Direction CalculateDirection(Vector2 p1, Vector2 p2)
        {
            Direction result = Direction.None;

            Vector3 dragVector = p2 - p1;

            float positiveX = Mathf.Abs(dragVector.x);
            float positiveY = Mathf.Abs(dragVector.y);
            if (positiveX > positiveY)
            {
                result = (dragVector.x > 0) ? Direction.Right : Direction.Left;
            }
            else
            {
                result = (dragVector.y > 0) ? Direction.Up : Direction.Down;
            }

            return result;

            //if (p1.x)
            //    result = Direction.Right;
            //else if (p2 == p1 - 1)
            //    result = Direction.Left;
            //else if (p2 == p1 + fpi.Columns)
            //    result = Direction.Top;
            //else if (p2 == p1 - fpi.Columns)
            //    result = Direction.Bottom;

            return result;
        }

        private async UniTask<int> NormalizeGameField()
        {
            HashSet<int> localBlockedIndexes = new(_normalizationBlockedIndexes);
            FieldParseInfo fieldInfo = _gameFieldStateManager.GetFieldCurrentState();
            List<UniTask> waitList = new();

            //fly
            var flyingPairs = fieldInfo.Blocks.GetFlyingPairs(fieldInfo.Rows, fieldInfo.Columns, localBlockedIndexes.Concat(_uiBlockedIndexes).ToList());

            foreach (var pair in flyingPairs)
            {
                var indexesToBlock = pair.Key.GetNumbersInBetween(pair.Value, fieldInfo.Columns);
                localBlockedIndexes.AddRange(indexesToBlock);
                _normalizationBlockedIndexes.AddRange(indexesToBlock);

                waitList.Add(Move(pair.Key, pair.Value, _cts.Token));
            }

            //adjacent
            var exceptCollection = localBlockedIndexes.Concat(_uiBlockedIndexes).ToList();
            var adjacentIndexesHorizontal = fieldInfo.Blocks.GetAdjacentIndexesHorizontal(3, fieldInfo.Rows, fieldInfo.Columns, exceptCollection);
            var adjacentIndexesVertical = fieldInfo.Blocks.GetAdjacentIndexesVertical(3, fieldInfo.Rows, fieldInfo.Columns, exceptCollection);

            adjacentIndexesHorizontal.UnionWith(adjacentIndexesVertical);
            adjacentIndexesHorizontal.UnionWith(fieldInfo.Blocks.GetAreaBlocks(fieldInfo.Rows, fieldInfo.Columns, adjacentIndexesHorizontal, exceptCollection));
            localBlockedIndexes.AddRange(adjacentIndexesHorizontal);
            _normalizationBlockedIndexes.AddRange(adjacentIndexesHorizontal);

            _gameFieldStateManager.Set(adjacentIndexesHorizontal.Select(g => (g, -1)).ToArray());

            if (adjacentIndexesHorizontal.Count > 0)
                waitList.Add(_gameUIArea.Destroy(adjacentIndexesHorizontal, _cts.Token));

            await waitList;

            if (_cts.IsCancellationRequested)
                return waitList.Count;

            foreach (var index in localBlockedIndexes)
                _normalizationBlockedIndexes.Remove(index);

            return waitList.Count;
        }

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            _cts?.Cancel();
            _uiBlockedIndexes.Clear();
            _normalizationBlockedIndexes.Clear();
            _disposables?.Clear();
            Initialized = false;
        }
    }
}
