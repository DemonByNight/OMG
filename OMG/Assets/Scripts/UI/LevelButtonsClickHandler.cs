using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using TMPro;

namespace OMG
{
    public class LevelButtonsClickHandler : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private TMP_Text text;

        private ILevelInfoContainer _levelLoader;

        private CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(ILevelInfoContainer levelLoader)
        {
            _levelLoader = levelLoader;

            Subscribe();
        }

        private void Update()
        {
            text.text = (1.0f / Time.deltaTime).ToString();
        }

        private void Subscribe()
        {
            restartButton.OnClickAsObservable().Subscribe(_ =>
            {
                _levelLoader.RestartLevel();
            }).AddTo(_disposables);

            nextLevelButton.OnClickAsObservable().Subscribe(_ => 
            {
                _levelLoader.LoadNextLevel();
            }).AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
