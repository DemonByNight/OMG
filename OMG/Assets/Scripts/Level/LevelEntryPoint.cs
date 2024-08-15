using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace OMG
{
    public class LevelEntryPoint : MonoBehaviour
    {
        [SerializeField] private SceneContext sceneContext;

        private void Awake()
        {
            sceneContext.Run();

        }
    }
}
