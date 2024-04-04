using UnityEngine;
using System;

namespace IJunior.ArrowBlocks.Main
{
    public class BlockConstructionCalculations
    {
        private Vector3 _cellSize;

        public BlockConstructionCalculations(Vector3 cellSize)
        {
            _cellSize = cellSize;
        }

        public Vector3 GetNeighboringCellPosition(Vector3 cellPosition, Vector3 directionToCell)
        {
            return cellPosition + Vector3.Scale(-directionToCell, _cellSize);
        }

        public (Vector3 Max, Vector3 Min) GetPositionExtremes(Vector3[] positions)
        {
            if (positions.Length == 0)
                throw new Exception("Number of positions cannot be zero.");

            (Vector3 Max, Vector3 Min) extremes = (positions[0], positions[0]);

            foreach (var position in positions)
            {
                extremes.Max = Vector3.Max(position, extremes.Max);
                extremes.Min = Vector3.Min(position, extremes.Min);
            }

            return extremes;
        }

        public Vector3 GetCenterPointPosition((Vector3 Max, Vector3 Min) extremes)
        {
            return extremes.Max.CalculateAverageVector(extremes.Min);
        }

        public float GetThroughLineLength((Vector3 Max, Vector3 Min) extremes)
        {
            return Vector3.Distance(extremes.Max, extremes.Min);
        }
    }
}