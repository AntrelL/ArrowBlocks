using IJunior.CompositeRoot;
using UnityEngine;

namespace IJunior.UI
{
    [RequireComponent(typeof(IndicatingIcon))]
    public class ProgressBarIndicator : Script
    {
        [SerializeField] private float _activateValue;

        private IndicatingIcon _indicatingIcon;

        public void Initialize()
        {
            _indicatingIcon = GetComponent<IndicatingIcon>();
            _indicatingIcon.Initialize();
        }

        public bool TryActivate(float value)
        {
            if (value >= _activateValue)
            {
                _indicatingIcon.Activate();
                return true;
            }

            _indicatingIcon.Deactivate();
            return false;
        }
    }
}