using UnityEngine;
using UnityEngine.UI;

public class LevelIndicator : MonoBehaviour
{
    [SerializeField] private Sprite _passed;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void UpdateState(LevelState levelState)
    {
        switch (levelState)
        {
            case LevelState.Passed:
                _button.image.sprite = _passed;
                break;
            case LevelState.Opened:
                _button.interactable = true;
                break;
            case LevelState.Blocked:
                _button.interactable = false;
                break;
        }
    }
}