using UnityEngine;
using Zenject;

namespace OMG
{
    public interface IGameField
    {
        //UniTask Swipe(Vector2 viewportStart, Vector2 viewportEnd);
    }

    public class GameField : MonoBehaviour, IGameField
    {
        [SerializeField] private GameUIArea gameUIArea;

        public class Factory : PlaceholderFactory<LevelConfigScriptableObject, GameField> { }
    }
}