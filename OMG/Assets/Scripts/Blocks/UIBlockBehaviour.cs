using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OMG
{
    public class UIBlockBehaviour : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private UIBlockAnimatorStateHandler animationStateHandler;

        public void SetOrder(int order)
        {
            sr.sortingOrder = order;
        }

        public async UniTask PlayDestroyEffectAsync()
        {
            await animationStateHandler.SetDestroyAsync().Task;
        }
    }
}