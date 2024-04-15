using IJunior.CompositeRoot;
using DG.Tweening;
using UnityEngine;
using System;
using System.Linq;

namespace IJunior.ArrowBlocks.Main
{
    public class ArrowBlockAnimator : Script
    {
        [SerializeField] private float _pushAnimationDistance;
        [SerializeField] private float _pushAnimationSpeed;
        [SerializeField] private float _maxSensingDistance;

        private Transform _transform;

        public Vector3 Position => _transform.position;
        public bool IsAnimated { get; private set; }

        public void Initialize(Transform transform)
        {
            _transform = transform;
        }

        public void PerformChainPushAnimation(Vector3 direction)
        {          
            RaycastHit[] hits = Physics.RaycastAll(_transform.position, direction);

            Vector3 previousBlockPosition = _transform.position;
            int numberOfDecimalPlaces = 4;

            ArrowBlockAnimator[] arrowBlockAnimators = ConvertToOrderedArrowBlockAnimators(hits, previousBlockPosition);

            if (arrowBlockAnimators.All(animator => animator.IsAnimated == false) == false)
                return;

            PlayPushAnimation(direction);

            foreach (var arrowBlockAnimator in arrowBlockAnimators)
            {
                float distanceBetweenBlocks = Vector3.Distance(arrowBlockAnimator.Position, previousBlockPosition);
                distanceBetweenBlocks = (float)Math.Round(distanceBetweenBlocks, numberOfDecimalPlaces);

                if (distanceBetweenBlocks > _maxSensingDistance)
                    break;

                arrowBlockAnimator.PlayPushAnimation(direction);
                previousBlockPosition = arrowBlockAnimator.Position;
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

        private ArrowBlockAnimator[] ConvertToOrderedArrowBlockAnimators(RaycastHit[] hits, Vector3 previousBlockPosition)
        {
            var arrowBlockAnimators = new ArrowBlockAnimator[hits.Length];

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.TryGetComponent(out ArrowBlockAnimator arrowBlockAnimator) == false)
                    throw new Exception("Arrow block detected an unknown object.");

                arrowBlockAnimators[i] = arrowBlockAnimator;
            }

            arrowBlockAnimators = arrowBlockAnimators
                .OrderBy(block => (block.Position - previousBlockPosition).sqrMagnitude)
                .ToArray();

            return arrowBlockAnimators;
        }
    }
}