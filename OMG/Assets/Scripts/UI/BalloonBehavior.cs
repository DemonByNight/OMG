using UnityEngine;

namespace OMG
{
    public class BalloonBehavior : PoolObject
    {
        private Vector3 _start;
        private Vector3 _end;
        private float _speed;
        private float _freqency;
        private float _magnitude;

        private bool _isEnabled = false;

        public void Move(Vector3 start, Vector3 end, float freqency = 1f, float magnitude = 1f, float speed = 0.3f)
        {
            _start = start;
            _end = end;
            _freqency = freqency;
            _magnitude = magnitude;
            _speed = speed;

            transform.position = _start;
            _isEnabled = true;
        }

        private void Update()
        {
            if (!_isEnabled)
                return;

            float speedMultiplier = _start.x > _end.x ? -1f : 1f;
            transform.position += new Vector3(_speed * speedMultiplier * Time.deltaTime, Time.deltaTime * Mathf.Sin(transform.position.x * _freqency) * _magnitude, 0);

            if (Mathf.Abs(transform.position.x - _end.x) <= 0.1f)
            {
                ReturnToPool();
            }
        }

        public override void Disable()
        {
            _isEnabled = false;
            base.Disable();
        }
    }
}