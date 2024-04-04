using System;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public interface IPlayerCameraTarget
    {
        public event Action<float> ThroughLineLengthChanged;

        public Vector3 CenterPointPosition { get; }
    }
}