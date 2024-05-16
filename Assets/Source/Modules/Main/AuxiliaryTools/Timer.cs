using IJunior.CompositeRoot;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public class Timer : IRootUpdateble
    {
        private float _value = 0f;
        private bool _isActiavted = false;

        public float Value => _value;

        public void RootUpdate()
        {
            if (_isActiavted)
            {
                _value += Time.deltaTime;
            }
        }

        public void StartCounting() => _isActiavted = true;

        public void StopCounting() => _isActiavted = false;

        public void ResetValue()
        {
            _value = 0f;
            _isActiavted = false;
        }
    }
}