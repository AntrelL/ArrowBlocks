using System.Collections;
using IJunior.ArrowBlocks.Main;
using IJunior.CompositeRoot;
using IJunior.UI;
using UnityEngine;

using Screen = IJunior.UI.Screen;

namespace IJunior.ArrowBlocks
{
    internal class Level : MonoBehaviour, IActivatable
    {
        [SerializeField][Range(0, 100)] private int _number;
        [SerializeField][Range(0, 1000)] private int _coinsForCompleting;
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
        private BombSeller _bombSeller;
        private AudioSource _audioSource;
        private AudioClip _victorySound;

        private WaitForSeconds _victoryDelayObject;

        public int Number => _number;

        public void InitializeBaseInfo(
            BlockConstruction blockConstruction,
            TimeText passingTimeText,
            BombThrower bombThrower,
            BombSeller bombSeller,
            Timer timer)
        {
            _blockConstruction = blockConstruction;
            _passingTimeText = passingTimeText;
            _bombThrower = bombThrower;
            _bombSeller = bombSeller;
            _timer = timer;

            _victoryDelayObject = new WaitForSeconds(_victoryDelay);
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

        public void InitializeAudio(AudioSource audioSource, AudioClip victorySound)
        {
            _audioSource = audioSource;
            _victorySound = victorySound;
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

        public void StartGame() => StartPassageOfTime();

        public IEnumerator Win()
        {
            _timer.StopCounting();
            yield return _victoryDelayObject;

            _audioSource.PlayOneShot(_victorySound);

            _passingTimeText.SetValue(_timer.Value);
            _mainScreen.SwitchTo(_victoryScreen);
            StopPassageOfTime();

            _playerInput.Disable();
            _playerData.PassLevel(_number, _coinsForCompleting, _timer.Value);

            _bombSeller.UpdateAvailableBombsCount();
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
            StartPassageOfTime();
            _victoryScreen.SwitchTo(_mainScreen);
            _playerInput.Enable();

            Restart();
        }

        private void StartPassageOfTime() => Time.timeScale = 1;

        private void StopPassageOfTime() => Time.timeScale = 0;

        private void OnFirstBlockActivated()
        {
            _timer.StartCounting();
        }

        private void OnAllBlocksReleased()
        {
            _victoryRetarder = StartCoroutine(Win());
        }
    }
}