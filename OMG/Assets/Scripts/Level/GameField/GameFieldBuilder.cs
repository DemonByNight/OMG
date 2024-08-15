using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

//TODO DELETE
namespace OMG
{
    public interface IGameFieldBuilder
    {

    }

    public class GameFieldBuilder : MonoBehaviour, IGameFieldBuilder, IInitializable
    {
        [SerializeField] private GameUIArea gameUIArea;

        private IGameFieldStateManager _gameFieldStateManager;

        [Inject]
        private void Construct(IGameFieldStateManager gameFieldStateManager)
        {
            _gameFieldStateManager = gameFieldStateManager;
        }

        public void Initialize()
        {
        }
    }
}
