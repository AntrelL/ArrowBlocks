using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private VirtualBlockGrid _virtualBlockGrid;
    [SerializeField] private Player _player;
    [SerializeField] private Screen _mainScreen;
    [SerializeField] private Screen _victoryScreen;
    [SerializeField] private float _victoryDelay;

    private Coroutine _victoryRetarder;

    private void OnEnable()
    {
        _virtualBlockGrid.AllBlocksReleased += OnAllBlocksReleased;
    }

    private void OnDisable()
    {
        _virtualBlockGrid.AllBlocksReleased -= OnAllBlocksReleased;
    }

    private void OnAllBlocksReleased()
    {
        _victoryRetarder = StartCoroutine(Win(_victoryDelay));
    }

    public void Restart()
    {
        if (_victoryRetarder != null)
            StopCoroutine(_victoryRetarder);

        _virtualBlockGrid.ResetAllBlocks();
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
        yield return new WaitForSeconds(delay);

        Time.timeScale = 0;
        _mainScreen.Change(_victoryScreen);
        _player.Input.Disable();
    }
}