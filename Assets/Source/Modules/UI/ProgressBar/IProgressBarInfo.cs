using System;

namespace IJunior.UI
{
    public interface IProgressBarInfo
    {
        public event Action<float> ValueChanged;
    }
}