using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public class BombThrowerCalculations
    {
        public Vector3 CalculateThrowStartVelocity(Vector3 startPosition, Vector3 endPosition, float throwAngle)
        {
            Vector3 directionToTarget = endPosition - startPosition;

            Vector3 horizontalDirectionToTarget = new Vector3(directionToTarget.x, 0f, directionToTarget.z);
            Vector3 finalDirection = horizontalDirectionToTarget.normalized;

            Vector3 rotationAxis = Vector3.Cross(startPosition, Vector3.up);
            finalDirection = Quaternion.AngleAxis(-throwAngle, rotationAxis) * finalDirection;
            finalDirection.Normalize();

            float g = Physics.gravity.y;

            float x = horizontalDirectionToTarget.magnitude;
            float y = directionToTarget.y;

            float degreeToRadianConversionFactor = Mathf.PI / 180;
            float angleInRadians = throwAngle * degreeToRadianConversionFactor;

            float speedSquared = (g * x * x) / (2 * (y - Mathf.Tan(angleInRadians) * x)
                * Mathf.Pow(Mathf.Cos(angleInRadians), 2));

            float speed = Mathf.Sqrt(Mathf.Abs(speedSquared));

            return finalDirection * speed;
        }
    }
}
