using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LevelText : MonoBehaviour
{
    private TMP_Text _tmpText;

    private void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();
    }

    public void Set(int number)
    {
        _tmpText.text = $"Level " + number;
    }
}