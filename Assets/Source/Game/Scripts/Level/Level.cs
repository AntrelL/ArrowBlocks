using IJunior.ArrowBlocks.Main;
using IJunior.CompositeRoot;
using System.Collections;
using UnityEngine;
using IJunior.UI;

using Screen = IJunior.UI.Screen;

namespace IJunior.ArrowBlocks
{
    internal class Level : MonoBehaviour, IActivatable
    {
        [SerializeField][Range(0, 100)] private int _number;
        [SerializeField][Range(0, 100)] private float _victoryDelay;

        private Coroutine _victoryRetarder;
        private BlockConstruction _blockConstruction;
        private PlayerInput _playerInput;
        private PlayerData _playerData;
        private Screen _mainScreen;
        private Screen _victoryScreen;
        private Timer _timer;
        private TimeText _passingTimeText;
        private BombThrower _bombThrower;

        public int Number => _number;

        public void InitializeBaseInfo(BlockConstruction blockConstruction,
            TimeText passingTimeText, BombThrower bombThrower, Timer timer)
        {
            _blockConstruction = blockConstruction;
            _passingTimeText = passingTimeText;
            _bombThrower = bombThrower;
            _timer = timer;

            Time.timeScale = 1;
        }

        public void InitializePlayerInfo(PlayerInput playerInput, PlayerData playerData)
        {
            _playerInput = playerInput;
            _playerData = playerData;
        }

        public void InitializeScreensInfo(Screen mainScreen, Screen victoryScreen)
        {
            _mainScreen = mainScreen;
            _victoryScreen = victoryScreen;
        }

        public void OnActivate()
        {
            _blockConstruction.AllBlocksReleased += OnAllBlocksReleased;
            _blockConstruction.FirstBlockActivated += OnFirstBlockActivated;

            _playerInput.Enable();
        }

        public void OnDeactivate()
        {
            _blockConstruction.AllBlocksReleased -= OnAllBlocksReleased;
            _blockConstruction.FirstBlockActivated -= OnFirstBlockActivated;

            _playerInput.Disable();
        }

        public IEnumerator Win(float delay)
        {
            _timer.StopCounting();
            yield return new WaitForSeconds(delay);

            _passingTimeText.Value = _timer.Value;
            _mainScreen.SwitchTo(_victoryScreen);
            Time.timeScale = 0;

            _playerInput.Disable();
            _playerData.PassLevel(_number, _timer.Value);
        }

        public void Restart()
        {
            if (_victoryRetarder != null)
                StopCoroutine(_victoryRetarder);

            _blockConstruction.ResetAllBlocks();
            _timer.ResetValue();
            _bombThrower.ResetValues();
        }

        public void RestartFromMenu()
        {
            Time.timeScale = 1;
            _victoryScreen.SwitchTo(_mainScreen);
            _playerInput.Enable();

            Restart();
        }

        private void OnFirstBlockActivated()
        {
            _timer.StartCounting();
        }

        private void OnAllBlocksReleased()
        {
            _victoryRetarder = StartCoroutine(Win(_victoryDelay));
        }
    }
}