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
        UniTask Move(Vector2 viewportStart, Vector2 viewportEnd);
    }

    public class GameFieldCommandHandler : IGameFieldCommandHandler, IDisposable
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

        private bool _isNormalizationInProcess = false;
        private CompositeDisposable _disposables = new();
        private HashSet<int> _uiBlockedIndexes = new();
        private HashSet<int> _normalizationBlockedIndexes = new();
        private CancellationTokenSource _cts;

        public bool Initialized { get; private set; }

        public GameFieldCommandHandler(IGameUIArea gameUIArea, IGameFieldStateManager gameFieldStateManager)
        {
            _gameUIArea = gameUIArea;
            _gameFieldStateManager = gameFieldStateManager;
        }

        public async UniTask InitializeComponent()
        {
            _cts = new();

            //start game normalize
            int processedCount = 0;
            do
            {
                processedCount = await NormalizeGameField();
            } while (processedCount > 0);

            Observable.EveryUpdate().Subscribe(async _ =>
            {
                if (_isNormalizationInProcess)
                    return;

                var fieldInfo = _gameFieldStateManager.GetFieldInfo();

                if (_normalizationBlockedIndexes.Count > 0
                    || fieldInfo.Blocks.HasAnyFlyingIndex(fieldInfo.Rows, fieldInfo.Columns, _uiBlockedIndexes))
                {
                    int processedCount = 0;
                    do
                    {
                        processedCount = await NormalizeGameField();
                    } while (processedCount > 0);
                }
            }).AddTo(_disposables);

            Initialized = true;
        }

        private async UniTask Move(int indexStart, int indexEnd, CancellationToken token)
        {
            var fieldInfo = _gameFieldStateManager.GetFieldInfo();

            _gameFieldStateManager.Set(
                (indexStart, fieldInfo[indexEnd]),
                (indexEnd, fieldInfo[indexStart]));

            await _gameUIArea.Move(indexStart, indexEnd, token);
        }

        public async UniTask Move(Vector2 viewportStart, Vector2 viewportEnd)
        {
            if (!Initialized)
                return;

            int indexStart = _gameUIArea.GetBlockIndexByViewport(viewportStart);
            int indexEnd = _gameUIArea.GetBlockIndexByViewport(viewportEnd);

            if (indexStart == -1 || indexEnd == -1)
                return;

            if (indexStart == indexEnd)
                return;

            if (_uiBlockedIndexes.Contains(indexStart) || _uiBlockedIndexes.Contains(indexEnd)
                || _normalizationBlockedIndexes.Contains(indexStart) || _normalizationBlockedIndexes.Contains(indexEnd))
                return;

            FieldParseInfo fieldInfo = _gameFieldStateManager.GetFieldInfo();

            if (fieldInfo.Blocks[indexStart] == -1)
                return;

            Direction dir = CalculateDirection(fieldInfo, indexStart, indexEnd);

            if (dir == Direction.None)
                return;

            if (dir == Direction.Top && fieldInfo.Blocks[indexEnd] == -1)
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

        private async UniTask<int> NormalizeGameField()
        {
            _isNormalizationInProcess = true;

            HashSet<int> localBlockedIndexes = new(_normalizationBlockedIndexes);
            FieldParseInfo fieldInfo = _gameFieldStateManager.GetFieldInfo();
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
            var adjacentIndexesHorizontal = fieldInfo.Blocks.GetAdjacentIndexesHorizontal(3, fieldInfo.Rows, fieldInfo.Columns, _uiBlockedIndexes);
            var adjacentIndexesVertical = fieldInfo.Blocks.GetAdjacentIndexesVertical(3, fieldInfo.Rows, fieldInfo.Columns, _uiBlockedIndexes);

            adjacentIndexesHorizontal.UnionWith(adjacentIndexesVertical);
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

            _isNormalizationInProcess = false;
            return waitList.Count;
        }

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            _cts.Cancel();
            _uiBlockedIndexes.Clear();
            _normalizationBlockedIndexes.Clear();
            _disposables?.Clear();
            Initialized = false;
        }
    }
}
