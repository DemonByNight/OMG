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
            bootstrapSceneContext.Run();
            SceneManager.LoadScene("Game");
        }
    }
}