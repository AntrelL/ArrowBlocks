using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class VersionText : MonoBehaviour
{
    [SerializeField] private string _prefixText;

    private void Start()
    {
        GetComponent<TMP_Text>().text = _prefixText + Application.version;
    }
}