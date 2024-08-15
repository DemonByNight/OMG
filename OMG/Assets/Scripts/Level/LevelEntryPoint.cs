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

            var levelsContainer = sceneContext.Container.Resolve<LevelContainerScriptableObject>();
            var gameFieldFactory = sceneContext.Container.Resolve<GameField.Factory>();
            var levelLoader = sceneContext.Container.Resolve<ILevelLoader>();
            var gameFieldInstanceProvider = sceneContext.Container.Resolve<GameFieldInstanceProvider>();

            gameFieldInstanceProvider.Instance = gameFieldFactory.Create(levelLoader.CurrentLevel);
        }
    }
}
