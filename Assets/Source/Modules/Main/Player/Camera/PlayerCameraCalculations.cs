using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public class PlayerCameraCalculations
    {
        public float CalculateCurrentVerticalAngle(Vector3 targetPosition, Transform transform)
        {
            Vector3 horizontalPosition = transform.position;
            horizontalPosition.y = targetPosition.y;

            Vector3 horizontalDirection = (horizontalPosition - targetPosition).normalized;
            Vector3 directionFromTarget = (transform.position - targetPosition).normalized;

            Quaternion storedLocalRotation = transform.localRotation;

            transform.LookAt(targetPosition);
            Vector3 rotationAxis = transform.right;

            transform.localRotation = storedLocalRotation;

            return Vector3.SignedAngle(horizontalDirection, directionFromTarget, rotationAxis);
        }

        public Vector3 CalculateOffsetPosition(Vector3 targetPosition, float offsetDistance, Transform transform)
        {
            Vector3 direction = (transform.position - targetPosition).normalized;
            return (direction * offsetDistance) + targetPosition;
        }
    }
}