using System.Linq;
using UnityEngine;
using Zenject;

namespace OMG
{
    public class LevelEntryPoint : MonoBehaviour
    {
        [SerializeField] private SceneContext sceneContext;
        [SerializeField] private GameField gameField;

        private void Awake()
        {
            sceneContext.Run();

            var levelsContainer = sceneContext.Container.Resolve<LevelContainerScriptableObject>();
            gameField.ConstructLevel(levelsContainer.GetAreaLevels("Junggle").FirstOrDefault());
        }
    }
}
