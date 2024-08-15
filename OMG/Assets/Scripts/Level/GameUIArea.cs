using System.Collections.Generic;
using UnityEngine;

namespace OMG
{
    public class GameUIArea : MonoBehaviour
    {
        [SerializeField] private Camera renderCamera;

        private LevelParseInfo _levelParseInfo;
        private List<BlockCell> _cells = new();

        public void Setup(IReadOnlyList<Block> availableBlocks, LevelParseInfo levelParseInfo)
        {
            _levelParseInfo = levelParseInfo;

            renderCamera.aspect = (float)renderCamera.targetTexture.width / renderCamera.targetTexture.height;
            renderCamera.orthographicSize = _levelParseInfo.Columns / renderCamera.aspect;

            CalculateGrid(availableBlocks);
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

            List<int> localBlock = new(_levelParseInfo.Blocks);
            localBlock.Reverse();

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
