using UnityEngine;

public class Timer : MonoBehaviour
{
    private float _value = 0f;
    private bool _isActiavted = false;

    public float Value => _value;

    private void Update()
    {
        if (_isActiavted)
        {
            _value += Time.deltaTime;
        }
    }

    public void StartCounting()
    {
        _isActiavted = true;
    }

    public void StopCounting()
    {
        _isActiavted = false;
    }

    public void ResetValue()
    {
        _value = 0f;
        _isActiavted = false;
    }
}
