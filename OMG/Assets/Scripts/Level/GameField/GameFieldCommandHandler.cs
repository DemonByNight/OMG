using Cysharp.Threading.Tasks;
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

        public GameFieldCommandHandler(IGameUIArea gameUIArea, IGameFieldStateManager gameFieldStateManager)
        {
            _gameUIArea = gameUIArea;
            _gameFieldStateManager = gameFieldStateManager;
        }

        public async UniTask Move(Vector2 viewportStart, Vector2 viewportEnd)
        {
            int indexStart = _gameUIArea.GetBlockIndexByViewport(viewportStart);
            int indexEnd = _gameUIArea.GetBlockIndexByViewport(viewportEnd);

            if (indexStart == -1 || indexEnd == -1)
                return;

            if (indexStart == indexEnd)
                return;

            var fieldInfo = _gameFieldStateManager.GetFieldInfo();

            if (fieldInfo.Blocks[indexStart] == -1)
                return;

            Direction dir = CalculateDirection(fieldInfo, indexStart, indexEnd);

            if (dir == Direction.None)
                return;

            if (dir == Direction.Top && fieldInfo.Blocks[indexEnd] == -1)
                return;

            fieldInfo.Blocks.Swap(indexStart, indexEnd);
            await _gameUIArea.Move(indexStart, indexEnd);
            _gameFieldStateManager.Save(fieldInfo);
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
    }
}
