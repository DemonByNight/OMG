using System.Collections.Generic;
using UnityEngine;

namespace OMG
{
    public class PrefabPool : MonoBehaviour
    {
        [SerializeField] private PoolObject prefab;
        [SerializeField] private int startCount;

        private List<PoolObject> activePrefabs = new();
        private List<PoolObject> sparePrefabs = new();

        private void Awake()
        {
            for (int i = 0; i < startCount; i++)
            {
                ExpandSpares();
            }
        }

        private void ExpandSpares()
        {
            var item = Instantiate(prefab, transform);
            item.gameObject.SetActive(false);
            item.name = sparePrefabs.Count.ToString();

            item.SetPool(this);
            sparePrefabs.Add(item);
        }

        public void ReturnToPool(PoolObject item)
        {
            item.Disable();
            item.gameObject.SetActive(false);
            item.transform.SetParent(transform);
            activePrefabs.Remove(item);
            sparePrefabs.Add(item);
        }

        public PoolObject GetItem()
        {
            if (sparePrefabs.Count == 0)
                ExpandSpares();

            var candidate = sparePrefabs.Random();
            sparePrefabs.Remove(candidate);
            activePrefabs.Add(candidate);

            return candidate;
        }
    }
}
