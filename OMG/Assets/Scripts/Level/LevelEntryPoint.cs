using System.Linq;
using UnityEngine;
using Zenject;

namespace OMG
{
    public class LevelEntryPoint : MonoBehaviour
    {
        [SerializeField] private SceneContext sceneContext;
        [SerializeField] private GameFieldRawImageBehavior gameFieldImageBehaviour;

        private void Awake()
        {
            sceneContext.Run();

            var levelsContainer = sceneContext.Container.Resolve<LevelContainerScriptableObject>();
            var gameFieldFactory = sceneContext.Container.Resolve<GameField.Factory>();
            var item = gameFieldFactory.Create(levelsContainer.GetAreaLevels("Junggle").FirstOrDefault());
            gameFieldImageBehaviour.InjectGameField(item);
        }
    }
}
