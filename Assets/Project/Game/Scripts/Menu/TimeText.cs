using TMPro;
using UnityEngine;
using System;

[RequireComponent(typeof(TMP_Text))]
public class TimeText : MonoBehaviour
{
    private TMP_Text _tmpText;

    private void Start()
    {
        _tmpText = GetComponent<TMP_Text>();
    }

    public void SetTime(float time)
    {
        int wholePart = (int)Math.Truncate(time);
        string fractionalPartText = (time - wholePart).ToString().Substring(2, 3);

        _tmpText.text = $"Time: {wholePart}:{fractionalPartText}s";
    }
}