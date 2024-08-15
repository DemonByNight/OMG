using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace OMG
{
    public class GameField : MonoBehaviour
    {
        [SerializeField] private GameUIArea gameUIArea;

        private ISaveService _saveService;
        private ILevelParser _levelParser;

        private LevelParseInfo _levelParseInfo;

        [Inject]
        private void Constructor(ISaveService saveService, ILevelParser levelParser)
        {
            _saveService = saveService;
            _levelParser = levelParser;
        }

        public void ConstructLevel(LevelConfigScriptableObject levelConfig)
        {
            //var levelSave = _saveService.Get<LevelSaveData>(LevelSaveData.SaveKey, new());
            //if (!string.IsNullOrEmpty(levelSave.LevelName) && !levelSave.LevelName.Equals(levelConfig.Name))
            //{
            //
            //}

            _levelParseInfo = _levelParser.Parse(levelConfig);
            gameUIArea.Setup(levelConfig.BlockInUse, _levelParseInfo);
        }

        public async UniTask Swipe(float swipeStart, float swipeEnd)
        {

        }
    }
}