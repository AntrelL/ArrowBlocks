using System;

namespace IJunior.UI
{
    public interface ILinkedDigitalTextSource
    {
        public event Action<float> ValueChanged;

        public float Value { get; }
    }
}