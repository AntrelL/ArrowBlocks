using UnityEngine;

public class ProgressBarIndicator : MonoBehaviour
{
    [SerializeField] private IndicatingIcon _indicatingIcon;
    [SerializeField] private float _activateValue;

    public void TryActivate(float value)
    {
        if (value >= _activateValue)
            _indicatingIcon.Activate();
        else
            _indicatingIcon.Deactivate();
    }
}
