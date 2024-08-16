using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OMG
{
    [RequireComponent(typeof(Animator))]
    public class UIBlockAnimatorStateHandler : MonoBehaviour
    {
        private const string T_Destroy = "Destroy";

        private Animator _animator;

        private UniTaskCompletionSource _destroyCompletionSource;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public UniTaskCompletionSource SetDestroyAsync()
        {
            if (_destroyCompletionSource != null)
                return _destroyCompletionSource;

            _destroyCompletionSource = new();
            _animator.SetTrigger(T_Destroy);

            return _destroyCompletionSource;
        }

        //called via animation event
        public void DestroyAnimationFinished()
        {
            _destroyCompletionSource.TrySetResult();
        }
    }
}
