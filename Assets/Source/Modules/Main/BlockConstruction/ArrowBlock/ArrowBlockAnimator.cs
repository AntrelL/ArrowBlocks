using IJunior.CompositeRoot;
using DG.Tweening;
using UnityEngine;
using System;

namespace IJunior.ArrowBlocks.Main
{
    public class ArrowBlockAnimator : Script
    {
        [SerializeField] private float _pushAnimationDistance;
        [SerializeField] private float _pushAnimationSpeed;

        private Transform _transform;

        public bool IsAnimated { get; private set; }

        public void Initialize(Transform transform)
        {
            _transform = transform;
        }

        public void PerformChainPushAnimation(Vector3 direction)
        {          
            PlayPushAnimation(direction);

            RaycastHit[] hits = Physics.RaycastAll(_transform.position, direction);

            if (hits.Length == 0)
                return;

            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.TryGetComponent(out ArrowBlockAnimator arrowBlockAnimator) == false)
                    throw new Exception("Arrow block detected an unknown object.");

                arrowBlockAnimator.PlayPushAnimation(direction);
            }
        }

        private void PlayPushAnimation(Vector3 direction)
        {
            int movementPartQuantity = 2;

            Vector3 basePosition = _transform.position;
            Vector3 shiftPosition = _transform.position + direction * _pushAnimationDistance;
            float time = _pushAnimationDistance * movementPartQuantity / _pushAnimationSpeed;

            IsAnimated = true;

            _transform.DOMove(shiftPosition, time);
            _transform.DOMove(basePosition, time).SetDelay(time).OnComplete(() => IsAnimated = false);
        }
    }
}