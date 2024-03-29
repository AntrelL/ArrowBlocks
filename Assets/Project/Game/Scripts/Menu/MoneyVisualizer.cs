using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class MoneyVisualizer : MonoBehaviour
{
    [SerializeField] private LevelSceneEntry _levelSceneEntry;

    private PlayerData _playerData;
    private TMP_Text _tmpText;

    private void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();
        _playerData = _levelSceneEntry.SceneData.PlayerData;
    }

    private void OnEnable()
    {
        _playerData.MoneyQuantityChanged += OnMoneyQuantityChanged;
    }

    private void OnDisable()
    {
        _playerData.MoneyQuantityChanged -= OnMoneyQuantityChanged;
    }

    private void Start()
    {
        OnMoneyQuantityChanged(_playerData.Money);
    }

    private void OnMoneyQuantityChanged(int newMoneyQuantity)
    {
        _tmpText.text = newMoneyQuantity.ToString();
    }
}