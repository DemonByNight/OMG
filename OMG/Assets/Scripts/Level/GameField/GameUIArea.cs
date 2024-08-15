using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace OMG
{
    public interface IGameUIArea
    {
        int GetBlockIndexByViewport(Vector2 viewport);

        UniTask Move(int indexStart, int indexEnd);
    }

    public class GameUIArea : MonoBehaviour, IGameUIArea, IInitializable
    {
        [SerializeField] private Camera renderCamera;
        [SerializeField] private GameFieldIndexContainer gameFieldIndexContainerPrefab;

        private LevelConfigScriptableObject _levelConfigScriptableObject;
        private IGameFieldStateManager _gameFieldStateManager;

        private FieldParseInfo _levelParseInfo;
        private Dictionary<int, GameFieldIndexContainer> _indexContainers = new();
        private Dictionary<int, UIBlockBehaviour> _cells = new();

        [Inject]
        private void Construct(LevelConfigScriptableObject levelConfigScriptableObject, IGameFieldStateManager gameFieldStateManager)
        {
            _levelConfigScriptableObject = levelConfigScriptableObject;
            _gameFieldStateManager = gameFieldStateManager;
        }

        public void Initialize()
        {
            _levelParseInfo = _gameFieldStateManager.GetFieldInfo();

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

            int GetRowIndex(int index) => index * _levelParseInfo.Columns;

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
                    indexContainer.Index = GetRowIndex(i) + j;
                    _indexContainers.Add(indexContainer.Index, indexContainer);

                    if (_levelParseInfo[GetRowIndex(i) + j] != -1)
                    {
                        UIBlockBehaviour prefab = GetUIBlock(_levelParseInfo.Blocks, GetRowIndex(i) + j, availableBlocks);
                        UIBlockBehaviour uiBlock = Instantiate(prefab, spawnPoint, Quaternion.identity, transform);
                        uiBlock.transform.localScale = blockScale;

                        uiBlock.Index = GetRowIndex(i) + j;
                        uiBlock.SetOrder(GetRowIndex(i) + j);

                        _cells.Add(GetRowIndex(i) + j, uiBlock);
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

        public async UniTask Move(int indexStart, int indexEnd)
        {
            UniTask move1 = new(), move2 = new();

            bool cell1Exist = _cells.TryGetValue(indexStart, out var cell1);
            bool cell2Exist = _cells.TryGetValue(indexEnd, out var cell2);

            if (cell1Exist
            && _indexContainers.TryGetValue(indexEnd, out var container1))
            {
                cell1.SetOrder(indexEnd);
                move1 = cell1.transform.DOMove(container1.transform.position, 0.4f).ToUniTask();
            }

            if (cell2Exist
                 && _indexContainers.TryGetValue(indexStart, out var container2))
            {
                cell2.SetOrder(indexStart);
                move2 = cell2.transform.DOMove(container2.transform.position, 0.4f).ToUniTask();
            }

            await UniTask.WhenAll(move1, move2);

            var buf = _cells[indexStart];

            if (cell2Exist)
                _cells[indexStart] = _cells[indexEnd];
            else
                _cells.Remove(indexStart);

            if (cell1Exist)
                _cells[indexEnd] = buf;
        }
    }
}
