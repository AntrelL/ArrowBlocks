using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IndicatingIcon : MonoBehaviour
{
    [SerializeField] private Sprite _activatedIcon;

    private Sprite _normalIcon;
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
        _normalIcon = _image.sprite;
    }

    public void Activate()
    {
        _image.sprite = _activatedIcon;
    }

    public void Deactivate()
    {
        _image.sprite = _normalIcon;
    }
}
