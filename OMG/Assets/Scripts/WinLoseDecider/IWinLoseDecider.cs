using UniRx;

namespace OMG
{
    public interface IWinLoseDecider
    {
        IReadOnlyReactiveProperty<bool> Win { get; }
        IReadOnlyReactiveProperty<bool> Lose { get; }
        void Clear();
        void StartWatching();

    }
}
