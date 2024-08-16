using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace OMG
{
    public class UIBlockBehaviour : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private UIBlockAnimatorStateHandler animationStateHandler;

        public int Index;

        public void SetOrder(int order)
        {
            sr.sortingOrder = order;
        }

        public async UniTask PlayDestroyEffectAsync(CancellationToken token)
        {
            await animationStateHandler.SetDestroyAsync().Task.AttachExternalCancellation(token);
        }
    }
}