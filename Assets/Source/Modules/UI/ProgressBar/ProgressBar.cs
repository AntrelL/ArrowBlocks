using System;
using System.Collections.Generic;
using IJunior.CompositeRoot;
using UnityEngine;

namespace IJunior.UI
{
    public class ProgressBar : Script, IRootUpdateble
    {
        private const float MinValue = 0;
        private const float MaxValue = 1;

        [SerializeField] private Transform _fill;
        [SerializeField] private float _fillingSpeed;

        [SerializeField] private bool _isImmutableValue;
        [SerializeField][Range(MinValue, MaxValue)] private float _baseValue;

        [SerializeField] private List<ProgressBarIndicator> _indicators;

        private float _value;
        private float _targetValue;

        private IProgressBarInfo _progressBarInfo;

        public bool IsConnected => _progressBarInfo != null;

        public void Initialize()
        {
            foreach (var indicator in _indicators)
            {
                indicator.Initialize();
            }

            _targetValue = _value = _baseValue;

            SetFillScale(_value);
        }

        public void RootUpdate()
        {
            if (IsConnected == false || _isImmutableValue)
                return;

            _value = Mathf.MoveTowards(_value, _targetValue, _fillingSpeed * Time.deltaTime);

            SetFillScale(_value);
        }

        public void Connect(IProgressBarInfo progressBarInfo)
        {
            ProcessValueImmutabilityFlag();

            if (IsConnected)
                throw new Exception("The progress bar is already connected.");

            _progressBarInfo = progressBarInfo;
            _progressBarInfo.ValueChanged += OnValueChanged;
        }

        public void Disconnect()
        {
            ProcessValueImmutabilityFlag();

            if (IsConnected == false && _isImmutableValue == false)
                throw new Exception("The progress bar is not connected..");

            _progressBarInfo.ValueChanged -= OnValueChanged;
            _progressBarInfo = null;
        }

        private void OnValueChanged(float value)
        {
            if (value < MinValue)
                throw new Exception($"Value cannot be less then min value = {MinValue}.");

            if (value > MaxValue)
                throw new Exception($"Value cannot be greater then max value = {MaxValue}.");

            _targetValue = value;
        }

        private void SetFillScale(float value)
        {
            Vector3 scale = _fill.localScale;
            scale.x = value;

            _fill.localScale = scale;
            UpdateIndicatingIconStates();
        }

        private void UpdateIndicatingIconStates()
        {
            if (_indicators == null)
                return;

            foreach (var indicator in _indicators)
            {
                indicator.TryActivate(_value);
            }
        }

        private void ProcessValueImmutabilityFlag()
        {
            if (_isImmutableValue)
                throw new Exception("Immutable value mode enabled.");
        }
    }
}