using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OMG
{
    public class BalloonsController : MonoBehaviour
    {
        [SerializeField] private List<PrefabPool> _ballonsPrefabPools;
        [SerializeField] private List<Transform> _pointsLeft;
        [SerializeField] private List<Transform> _pointsRight;
        [SerializeField] private int maxBalloonsAtTime;
        [SerializeField] private Transform parent;

        private Transform _cachePoint;
        private List<PoolObject> _activeBalloons = new();

        private void Update()
        {
            if (_activeBalloons.Count < maxBalloonsAtTime)
            {
                SpawnBalloon();
            }
        }

        private void SpawnBalloon()
        {
            var poolItem = _ballonsPrefabPools.Random().GetItem();
            if (!poolItem.TryGetComponent<BalloonBehavior>(out var bb))
            {
                return;
            }

            _activeBalloons.Add(bb);
            bb.transform.SetParent(parent);
            bb.OnReturnToPool += OnReturnToPool;

            Transform randomStartPoint, randomEndPoint;

            if(Random.value > 0.5f)
            {
                randomStartPoint = _pointsRight.Except(new List<Transform>() { _cachePoint }).ToList().Random();
                randomEndPoint = _pointsLeft.Random();
            }
            else
            {
                randomStartPoint = _pointsLeft.Except(new List<Transform>() { _cachePoint }).ToList().Random();
                randomEndPoint = _pointsRight.Random();
            }

            _cachePoint = randomStartPoint;

            bb.Move(randomStartPoint.position, randomEndPoint.position, 
                Random.Range(0.1f, 1f), Random.Range(0.1f, 0.2f), Random.Range(0.1f, 0.6f));
            bb.gameObject.SetActive(true);
        }

        private void OnReturnToPool(PoolObject po)
        {
            po.OnReturnToPool -= OnReturnToPool;
            _activeBalloons.Remove(po);
        }
    }
}
