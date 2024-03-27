using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Screen : MonoBehaviour
{
    [SerializeField] private bool _isActiveByDefault;

    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_isActiveByDefault)
            Open();
        else
            Close();
    }

    public void Open()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Close()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void Change(Screen targetScreen)
    {
        Close();
        targetScreen.Open();
    }
}