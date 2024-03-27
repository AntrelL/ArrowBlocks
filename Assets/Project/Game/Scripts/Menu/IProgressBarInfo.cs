using System;

public interface IProgressBarInfo
{
    public event Action<float> ValueChanged;
}
