using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace OMG
{
    public class GameEntryPoint : MonoBehaviour
    {
        [SerializeField] private SceneContext bootstrapSceneContext;

        private void Awake()
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            bootstrapSceneContext.Run();
            bootstrapSceneContext.Container.Resolve<ILevelLoader>().RestartLevel();
        }
    }
}