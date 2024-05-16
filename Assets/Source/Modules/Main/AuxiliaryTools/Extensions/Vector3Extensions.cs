using System;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public static class Vector3Extensions
    {
        public static Vector3 DivideByModulus(this Vector3 first, Vector3 second)
        {
            return PerformFunctionForCoordinates(first, second, (x1, x2) => x1 % x2);
        }

        public static Vector3 PerformFunctionForCoordinates(
            this Vector3 first, Vector3 second, Func<float, float, float> function)
        {
            return new Vector3(
                function.Invoke(first.x, second.x),
                function.Invoke(first.y, second.y),
                function.Invoke(first.z, second.z));
        }

        public static Vector3 CalculateAverageVector(this Vector3 first, params Vector3[] others)
        {
            foreach (var vector in others)
                first += vector;

            return first / (others.Length + 1);
        }
    }
}