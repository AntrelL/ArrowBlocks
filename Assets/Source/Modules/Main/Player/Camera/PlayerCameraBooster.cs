using IJunior.CompositeRoot;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public class PlayerCameraBooster : Script
    {
        private const float BaseBoosterDistanceCoefficient = 1;

        [SerializeField][Range(0, 100)] private float _zoomSpeedCoefficient;
        [SerializeField][Range(0, 100)] private float _speedToTargetCoefficient;
        [SerializeField][Range(0, 1000)] private float _activationDistance;

        private float _boosterDistanceCoefficient;

        public void Initialize()
        {
            _boosterDistanceCoefficient = BaseBoosterDistanceCoefficient;
        }

        public bool IsActivated { get; private set; }

        public void ProcessSpeedValues(ref float speedToTarget, ref float zoomSpeed)
        {
            speedToTarget *= _speedToTargetCoefficient * _boosterDistanceCoefficient;
            zoomSpeed *= _zoomSpeedCoefficient * _boosterDistanceCoefficient;
        }

        public void UpdateState(Vector3 newTargetPosition, float offsetDistance,
            PlayerCameraCalculations calculations, Transform transform)
        {
            Vector3 realOffsetPosition = calculations.CalculateOffsetPosition(
                newTargetPosition, offsetDistance, transform);

            float distanceToRealOffsetPosition = Vector3.Distance(transform.position, realOffsetPosition);

            if (IsActivated)
            {
                IsActivated = transform.position != realOffsetPosition;

                if (IsActivated == false)
                    _boosterDistanceCoefficient = BaseBoosterDistanceCoefficient;
            }
            else
            {
                IsActivated = distanceToRealOffsetPosition > _activationDistance;

                if (IsActivated)
                    _boosterDistanceCoefficient = distanceToRealOffsetPosition;
            }
        }
    }
}