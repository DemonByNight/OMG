using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

namespace OMG
{
    public class LevelButtonsClickHandler : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextLevelButton;

        private ILevelInfoContainer _levelLoader;
        private GameFieldInstanceProvider _gameFieldInstanceProvider;

        private CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(ILevelInfoContainer levelLoader, GameFieldInstanceProvider gameFieldInstanceProvider)
        {
            _levelLoader = levelLoader;
            _gameFieldInstanceProvider = gameFieldInstanceProvider;

            Subscribe();
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
