using Cysharp.Threading.Tasks;
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

            StartLevel().Forget();
        }

        private async UniTask StartLevel()
        {
            Application.targetFrameRate = 60;
            await SceneManager.LoadSceneAsync("Level", LoadSceneMode.Additive);
        }
    }
}