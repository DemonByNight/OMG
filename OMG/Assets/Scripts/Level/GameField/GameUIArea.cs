using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace OMG
{
    public interface IGameUIArea
    {

    }

    public class GameUIArea : MonoBehaviour, IGameUIArea, IInitializable
    {
        [SerializeField] private Camera renderCamera;

        private LevelConfigScriptableObject _levelConfigScriptableObject;
        private IGameFieldStateManager _gameFieldStateManager;

        private FieldParseInfo _levelParseInfo;
        private List<BlockCell> _cells = new();

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
            int GetRowIndex(int index) => index * _levelParseInfo.Columns;

            float scale = (renderCamera.orthographicSize * 2 * renderCamera.aspect) / _levelParseInfo.Columns;
            Vector3 blockScale = new(scale, scale, 1);

            float xOffset = (float)1 / _levelParseInfo.Columns;
            float yOffset = (float)1 / renderCamera.orthographicSize * 2 / scale;
            float groundYOffset = yOffset / 2;

            float z = renderCamera.farClipPlane - 1;

            List<int> localBlock = _levelParseInfo.Blocks.Split(_levelParseInfo.Columns).Reverse().SelectMany(g => g).ToList();

            for (int i = 0; i < _levelParseInfo.Rows; i++)
            {
                for (int j = 0; j < _levelParseInfo.Columns; j++)
                {
                    if (localBlock[GetRowIndex(i) + j] == -1)
                    {
                        _cells.Add(new(GetRowIndex(i) + j, null));
                        continue;
                    }

                    var spawnPoint = renderCamera.ViewportToWorldPoint(new Vector3((0.5f * xOffset) + (j * xOffset),
                                                                                   groundYOffset + (0.5f * yOffset) + (i * yOffset),
                                                                                   z));

                    UIBlockBehaviour prefab = GetUIBlock(localBlock, GetRowIndex(i) + j, availableBlocks);
                    UIBlockBehaviour uiBlock = Instantiate(prefab, spawnPoint, Quaternion.identity, transform);
                    uiBlock.transform.localScale = blockScale;

                    uiBlock.SetOrder(GetRowIndex(i) + j);

                    _cells.Add(new(GetRowIndex(i) + j, uiBlock));
                }
            }
        }

        private UIBlockBehaviour GetUIBlock(List<int> localBlocks, int matrixIndex, IReadOnlyList<Block> availableBlocks)
        {
            var availableBlockIndex = localBlocks[matrixIndex];
            return availableBlocks[availableBlockIndex].BlockPrefab;
        }
    }

    public class BlockCell
    {
        public int Index;
        public UIBlockBehaviour Block;

        public BlockCell(int index, UIBlockBehaviour block)
        {
            Index = index;
            Block = block;
        }
    }
}
