using System;
using UnityEngine;

namespace OMG
{
    public class PoolObject : MonoBehaviour
    {
        private PrefabPool _parentPool;

        public event Action<PoolObject> OnReturnToPool;

        public void SetPool(PrefabPool parentPool)
        {
            _parentPool = parentPool;
        }

        public void ReturnToPool()
        {
            _parentPool.ReturnToPool(this);
        }

        public virtual void Disable()
        {
            OnReturnToPool?.Invoke(this);
        }
    }
}
