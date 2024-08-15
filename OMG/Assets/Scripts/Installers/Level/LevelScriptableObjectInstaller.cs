using UnityEngine;
using Zenject;

namespace OMG
{
    [CreateAssetMenu(fileName = "new LevelScriptableObjectInstaller", menuName = "[Level config]/Installers/LevelScriptableInstaller")]
    public class LevelScriptableObjectInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private LevelContainerScriptableObject levelContainer;

        public override void InstallBindings()
        {
            Container.BindInstances(levelContainer);
        }
    }
}
