using System;
using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private VirtualBlockGrid _virtualBlockGrid;
    [SerializeField] private Player _player;
    [SerializeField] private Screen _mainScreen;
    [SerializeField] private Screen _victoryScreen;
    [SerializeField] private float _victoryDelay;
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private int _levelNumber;
    [SerializeField] private GameObject _nextLevelButtonSwitch;
    [SerializeField] private Timer _timer;
    [SerializeField] private TimeText _timeText;
    [SerializeField] private LevelText _levelText;

    private Coroutine _victoryRetarder;

    public int LevelNumber => _levelNumber;

    private void OnEnable()
    {
        _virtualBlockGrid.AllBlocksReleased += OnAllBlocksReleased;
        _virtualBlockGrid.FirstBlockActivated += OnFirstBlockActivated;
    }

    private void OnDisable()
    {
        _virtualBlockGrid.AllBlocksReleased -= OnAllBlocksReleased;
        _virtualBlockGrid.FirstBlockActivated += OnFirstBlockActivated;
    }

    private void Start()
    {
        _levelText.Set(_levelNumber);
    }

    private void OnFirstBlockActivated()
    {
        _timer.StartCounting();
    }

    public void SetPlayerData(PlayerData playerData)
    {
        _playerData = playerData;
        _nextLevelButtonSwitch.SetActive(_levelNumber != playerData.Levels.Count);
    }

    public void Restart()
    {
        if (_victoryRetarder != null)
            StopCoroutine(_victoryRetarder);

        _virtualBlockGrid.ResetAllBlocks();
        _timer.ResetValue();
    }

    public void RestartFromMenu()
    {
        Time.timeScale = 1;
        _victoryScreen.Change(_mainScreen);
        _player.Input.Enable();

        Restart();
    }

    public IEnumerator Win(float delay)
    {
        _timer.StopCounting();
        yield return new WaitForSeconds(delay);

        _timeText.SetTime(_timer.Value);
        _mainScreen.Change(_victoryScreen);
        Time.timeScale = 0;

        _player.Input.Disable();
        _playerData.PassLevel(_levelNumber, _timer.Value);
    }

    private void OnAllBlocksReleased()
    {
        _victoryRetarder = StartCoroutine(Win(_victoryDelay));
    }
}