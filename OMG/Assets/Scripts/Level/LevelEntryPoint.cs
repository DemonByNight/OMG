using System.Linq;
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
            gameFieldFactory.Create(levelsContainer.GetAreaLevels("Junggle").FirstOrDefault());
        }
    }
}
