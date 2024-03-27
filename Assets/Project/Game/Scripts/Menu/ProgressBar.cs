using System;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    private const float MinValue = 0;
    private const float MaxValue = 1;

    [SerializeField] private Transform _fill;
    [SerializeField] private float _fillingSpeed;

    [SerializeField] private bool _isImmutableValue;
    [SerializeField] [Range(MinValue, MaxValue)] private float _baseValue;

    [SerializeField] private List<ProgressBarIndicator> _indicators;

    private float _value;
    private float _targetValue;

    private IProgressBarInfo _progressBarInfo;

    public bool IsConnected => _progressBarInfo != null;

    private void Start()
    {
        _targetValue = _value = _baseValue;

        SetFillScale(_value);
        UpdateIndicatingIconStates(_value);
    }

    private void Update()
    {
        if (IsConnected == false)
            return;

        _value = Mathf.MoveTowards(_value, _targetValue, _fillingSpeed * Time.deltaTime);

        SetFillScale(_value);
        UpdateIndicatingIconStates(_value);
    }

    public void Connect(IProgressBarInfo progressBarInfo)
    {
        if (_isImmutableValue)
            throw new Exception("Immutable value mode enabled.");

        if (IsConnected)
            throw new Exception("The progress bar is already connected.");

        _progressBarInfo = progressBarInfo;
        _progressBarInfo.ValueChanged += OnValueChanged;
    }

    public void Disconnect()
    {
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
    }

    private void UpdateIndicatingIconStates(float value)
    {
        if (_indicators == null)
            return;

        foreach (var indicator in _indicators)
        {
            indicator.TryActivate(_value);
        }
    }
}