using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

namespace OMG
{
    public interface IGameFieldUIStateProvider
    {
        bool IsAnyAnimationInProgress { get; }
    }

    public interface IGameUIArea : IGameFieldComponent, IGameFieldUIStateProvider
    {
        int GetBlockIndexByViewport(Vector2 viewport);

        UniTask Move(int indexStart, int indexEnd, CancellationToken token);
        UniTask Destroy(HashSet<int> indexesToDestroy, CancellationToken token);
    }

    public class GameUIArea : MonoBehaviour, IGameUIArea
    {
        [SerializeField] private Camera renderCamera;
        [SerializeField] private GameFieldIndexContainer gameFieldIndexContainerPrefab;

        private LevelConfigScriptableObject _levelConfigScriptableObject;
        private IGameFieldStateManager _gameFieldStateManager;

        private FieldParseInfo _levelParseInfo;
        private Dictionary<int, GameFieldIndexContainer> _indexContainers = new();
        private Dictionary<int, UIBlockBehaviour> _cells = new();

        public bool Initialized { get; private set; }

        private int _animationsCount;
        public bool IsAnyAnimationInProgress => _animationsCount > 0;

        [Inject]
        private void Construct(IGameFieldStateManager gameFieldStateManager)
        {
            _gameFieldStateManager = gameFieldStateManager;
        }

        public async UniTask InitializeComponent()
        {
            _levelConfigScriptableObject = _gameFieldStateManager.CurrentLevelConfig;
            _levelParseInfo = _gameFieldStateManager.GetFieldCurrentState();

            renderCamera.aspect = (float)renderCamera.targetTexture.width / renderCamera.targetTexture.height;
            renderCamera.orthographicSize = _levelParseInfo.Columns / renderCamera.aspect;

            CalculateGrid(_levelConfigScriptableObject.BlockInUse);
        }

        private void CalculateGrid(IReadOnlyList<Block> availableBlocks)
        {
            UIBlockBehaviour GetUIBlock(List<int> localBlocks, int matrixIndex, IReadOnlyList<Block> availableBlocks)
            {
                var availableBlockIndex = localBlocks[matrixIndex];
                return availableBlocks[availableBlockIndex].BlockPrefab;
            }

            float scale = (renderCamera.orthographicSize * 2 * renderCamera.aspect) / _levelParseInfo.Columns;
            Vector3 blockScale = new(scale, scale, 1);

            float xOffset = (float)1 / _levelParseInfo.Columns;
            float yOffset = (float)1 / renderCamera.orthographicSize * 2 / scale;
            float groundYOffset = yOffset / 2;

            float z = renderCamera.farClipPlane - 1;

            for (int i = 0; i < _levelParseInfo.Rows; i++)
            {
                for (int j = 0; j < _levelParseInfo.Columns; j++)
                {
                    var spawnPoint = renderCamera.ViewportToWorldPoint(new Vector3((0.5f * xOffset) + (j * xOffset),
                                                                                   groundYOffset + (0.5f * yOffset) + (i * yOffset),
                                                                                   z));

                    GameFieldIndexContainer indexContainer = Instantiate(gameFieldIndexContainerPrefab, spawnPoint, Quaternion.identity, transform);
                    indexContainer.transform.localScale = blockScale;
                    indexContainer.Index = i.GetRowIndex(_levelParseInfo.Columns) + j;
                    _indexContainers.Add(indexContainer.Index, indexContainer);

                    if (_levelParseInfo[i.GetRowIndex(_levelParseInfo.Columns) + j] != -1)
                    {
                        UIBlockBehaviour prefab = GetUIBlock(_levelParseInfo.Blocks, i.GetRowIndex(_levelParseInfo.Columns) + j, availableBlocks);
                        UIBlockBehaviour uiBlock = Instantiate(prefab, spawnPoint, Quaternion.identity, transform);
                        uiBlock.transform.localScale = blockScale;
                    
                        uiBlock.Index = i.GetRowIndex(_levelParseInfo.Columns) + j;
                        uiBlock.SetOrder(i.GetRowIndex(_levelParseInfo.Columns) + j);
                    
                        _cells.Add(i.GetRowIndex(_levelParseInfo.Columns) + j, uiBlock);
                    }
                }
            }
        }

        public int GetBlockIndexByViewport(Vector2 viewport)
        {
            int result = -1;

            Ray ray = renderCamera.ViewportPointToRay(viewport);
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent<GameFieldIndexContainer>(out GameFieldIndexContainer indexContainer))
                {
                    result = indexContainer.Index;
                }
            }

            return result;
        }

        public async UniTask Move(int indexStart, int indexEnd, CancellationToken token)
        {
            UniTask move1 = new(), move2 = new();
            int localAnimationsCounter = 0;

            bool cell1Exist = _cells.TryGetValue(indexStart, out var cell1);
            bool cell2Exist = _cells.TryGetValue(indexEnd, out var cell2);

            if (cell1Exist
            && _indexContainers.TryGetValue(indexEnd, out var container1))
            {
                cell1.SetOrder(indexEnd);
                localAnimationsCounter++;
                move1 = cell1.transform.DOMove(container1.transform.position, 0.5f).ToUniTask(cancellationToken: token);
            }

            if (cell2Exist
                 && _indexContainers.TryGetValue(indexStart, out var container2))
            {
                cell2.SetOrder(indexStart);
                localAnimationsCounter++;
                move2 = cell2.transform.DOMove(container2.transform.position, 0.5f).ToUniTask(cancellationToken: token);
            }

            var buf = _cells[indexStart];

            if (cell2Exist)
                _cells[indexStart] = _cells[indexEnd];
            else
                _cells.Remove(indexStart);

            if (cell1Exist)
                _cells[indexEnd] = buf;

            if (token.IsCancellationRequested)
                return;

            _animationsCount += localAnimationsCounter;
            await UniTask.WhenAll(move1, move2);
            _animationsCount -= localAnimationsCounter;
        }

        public async UniTask Destroy(HashSet<int> indexesToDestroy, CancellationToken token)
        {
            List<UIBlockBehaviour> waitDestroy = new();

            foreach (var index in indexesToDestroy) 
            {
                if(_cells.TryGetValue(index, out UIBlockBehaviour cell))
                {
                    waitDestroy.Add(cell);
                }
            }

            await UniTask.WhenAll(waitDestroy.Select(g => g.PlayDestroyEffectAsync(token)));
            waitDestroy.ForEach(g => Destroy(g.gameObject));

            foreach (var index in indexesToDestroy)
                _cells.Remove(index);
        }

        public void Clear()
        {
            foreach (var item in _indexContainers)
                Destroy(item.Value.gameObject);
            _indexContainers.Clear();

            foreach (var item in _cells)
                Destroy(item.Value.gameObject);
            _cells.Clear();

            Initialized = false;
        }
    }
}
