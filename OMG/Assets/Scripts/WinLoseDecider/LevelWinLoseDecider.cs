using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UniRx;

namespace OMG
{
    public class LevelWinLoseDecider : IWinLoseDecider, IDisposable
    {
        private readonly GameFieldInstanceProvider _gameFieldInstanceProvider;

        private ReactiveProperty<bool> _win = new();
        public IReadOnlyReactiveProperty<bool> Win => _win;

        private ReactiveProperty<bool> _lose = new();
        public IReadOnlyReactiveProperty<bool> Lose => _lose;

        private CompositeDisposable _disposables = new();

        public LevelWinLoseDecider(GameFieldInstanceProvider gameFieldInstanceProvider)
        {
            _gameFieldInstanceProvider = gameFieldInstanceProvider;
        }

        public void StartWatching()
        {
            _gameFieldInstanceProvider.Instance.GameFieldStateProvider.StateChangedObservable.Subscribe(async _ =>
            {
                var currentState = _gameFieldInstanceProvider.Instance.GameFieldStateProvider.GetFieldCurrentState();
                if (currentState.Blocks.Any(g => g != -1))
                    return;

                await UniTask.WhenAll(
                    UniTask.WaitUntil(() => _gameFieldInstanceProvider.Instance.GameFieldCommandHandler.IsNormalizationInProcess == false),
                    UniTask.WaitUntil(() => _gameFieldInstanceProvider.Instance.GameFieldUIStateProvider.IsAnyAnimationInProgress == false));

                _win.Value = true;
            }).AddTo(_disposables);
        }

        public void Clear()
        {
            _disposables?.Clear();
            _win.Value = false;
            _lose.Value = false;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
