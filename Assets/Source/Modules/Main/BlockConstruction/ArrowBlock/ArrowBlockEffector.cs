using IJunior.CompositeRoot;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public class ArrowBlockEffector : Script
    {
        [SerializeField] private TrailRenderer _trail;
        [SerializeField] private float _minTrailDistance;

        private Transform _transform;

        public void Initialize(Transform transform)
        {
            _transform = transform;
        }

        public void TryActivateTrail(Vector3 targetPosition)
        {
            if (Vector3.Distance(_transform.position, targetPosition) >= _minTrailDistance)
                _trail.enabled = true;
        }

        public void DeactivateTrail() => _trail.enabled = false;

        public void ClearTrail() => _trail.Clear();
    }
}