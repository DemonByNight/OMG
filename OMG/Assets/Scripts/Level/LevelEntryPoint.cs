using UnityEngine;
using Zenject;

namespace OMG
{
    public class LevelEntryPoint : MonoBehaviour
    {
        [SerializeField] private SceneContext sceneContext;

        private void Awake()
        {
            sceneContext.Run();

            var gameFieldFactory = sceneContext.Container.Resolve<GameField.Factory>();
            var levelLoader = sceneContext.Container.Resolve<ILevelInfoContainer>();
            var gameFieldInstanceProvider = sceneContext.Container.Resolve<GameFieldInstanceProvider>();

            gameFieldInstanceProvider.Instance = gameFieldFactory.Create();
            levelLoader.RestoreLevel();
        }
    }
}
