#pragma warning disable

using System.Collections.Generic;
using IJunior.ArrowBlocks.Main;
using IJunior.CompositeRoot;
using IJunior.TypedScenes;
using UnityEngine.Events;
using System.Collections;
using Agava.YandexGames;
using UnityEngine.UI;
using UnityEngine;
using IJunior.UI;
using TMPro;

using Screen = IJunior.UI.Screen;
using Leaderboard = IJunior.ArrowBlocks.Main.Leaderboard;

namespace IJunior.ArrowBlocks
{
    internal class MenuCompositeRoot : MonoBehaviour, 
        ISceneLoadHandler<(PlayerData PlayerData, MenuScreenId MenuScreenId)>
    {
        [Header("Screens")]
        [SerializeField] private Screen _mainScreen;
        [SerializeField] private Screen _levelsScreen;
        [SerializeField] private Screen _leaderboardScreen;
        [Space]
        [Header("User Interface")]
        [SerializeField] private LevelButtonsStorage _levelButtonsStorage;
        [SerializeField] private VersionText _versionText;
        [Space]
        [Header("Animators")]
        [SerializeField] private Rotator _cameraRotator;
        [Space]
        [Header("Audio")]
        [SerializeField] private BackgroundMusicPlayer _backgroundMusicPlayer;
        [SerializeField] private Slider _backgroundMusicVolumeSlider;
        [Space]
        [Header("Leaderboard")]
        [SerializeField] private Leaderboard _leaderboard;
        [SerializeField] private LeaderboardLine _leaderboardLineTemplate;
        [SerializeField] private TMP_InputField _leaderboardLevelNumberInput;
        [SerializeField] private TimeText _leaderboardPlayerTime;
        [SerializeField] private DigitalText _leaderboardPlayerPosition;
        [Space]
        [Header("Menu Control")]
        [SerializeField] private FlowControl _menuFlowControl;

        private PlayerData _playerData;
        private LevelLoader _levelLoader;
        private MenuScreenId _screenIdToSwitch = MenuScreenId.None;
        private UnityAction[] _levelActivators;
        private Button[] _levelButtons;

        private void Awake() => InitializeEarly();

        private void OnDisable() => UnsubscribeEvents();

        private IEnumerator Start()
        {
            yield return Initialize();

            SubscribeEvents();

            Time.timeScale = 1;
            _menuFlowControl.Run();

            yield return null;
        }

        public void OnSceneLoaded((PlayerData PlayerData, MenuScreenId MenuScreenId) sceneTransitionData)
        {
            _playerData = sceneTransitionData.PlayerData;
            _screenIdToSwitch = sceneTransitionData.MenuScreenId;
        }

        private void InitializeEarly()
        {
            InitializeScreens();

            if (_screenIdToSwitch != MenuScreenId.None)
                SwitchScreenById(_screenIdToSwitch);
        }

        private IEnumerator Initialize()
        {
            yield return InitializeYandexGamesSdk();

            var rootUpdatebleElements = new List<IRootUpdateble>();

            _levelButtons = _levelButtonsStorage.Initialize();

            if (_playerData == null)
            {
                _playerData = new PlayerData(_levelButtons.Length);
                yield return _playerData.TryGetFromCloud();
            }

            _levelLoader = new LevelLoader(_playerData);
            _levelActivators = new UnityAction[_levelButtons.Length];

            _levelButtonsStorage.UpdateLevelButtons(_playerData.LevelsData);
            _versionText.Initialize();

            InitializeLeaderboard(_playerData.LevelsData.Count);

            _cameraRotator.Initialize();
            rootUpdatebleElements.Add(_cameraRotator);

            _backgroundMusicPlayer = _backgroundMusicPlayer.Initialize(_backgroundMusicVolumeSlider);

            _menuFlowControl.Initialize(rootUpdatebleElements, new List<IRootFixedUpdateble>());

            yield return null;
        }

        private void SubscribeEvents()
        {
            for (int i = 0; i < _levelButtons.Length; i++)
            {
                _levelActivators[i] = _levelLoader.GetLevelActivator(i + 1);
                _levelButtons[i].onClick.AddListener(_levelActivators[i]);
            }

            _leaderboard.OnActivate();
            _backgroundMusicPlayer.OnActivate();
        }

        private void UnsubscribeEvents()
        {
            for (int i = 0; i < _levelButtons.Length; i++)
            {
                _levelButtons[i].onClick.RemoveListener(_levelActivators[i]);
            }

            _leaderboard.OnDeactivate();
            _backgroundMusicPlayer.OnDeactivate();
        }

        private void InitializeScreens()
        {
            _mainScreen.Initialize();
            _levelsScreen.Initialize();
            _leaderboardScreen.Initialize();
        }

        private void InitializeLeaderboard(int numberOfLevels)
        {
            _leaderboardPlayerTime.Initialize();
            _leaderboardPlayerPosition.Initialize();

            _leaderboard.Initialize(_leaderboardLineTemplate, numberOfLevels);
            _leaderboard.InitializeIOElements(_leaderboardLevelNumberInput,
                _leaderboardPlayerTime, _leaderboardPlayerPosition);

            _leaderboard.FinalInitialize();
        }

        private IEnumerator InitializeYandexGamesSdk()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            yield break;
#endif

            yield return YandexGamesSdk.Initialize();
        }

        private void SwitchScreenById(MenuScreenId screenId)
        {
            Screen target = null;

            switch (screenId)
            {
                case MenuScreenId.Main:
                    target = _mainScreen;
                    break;
                case MenuScreenId.Levels:
                    target = _levelsScreen;
                    break;
                case MenuScreenId.Leaderboard:
                    target = _leaderboardScreen;
                    break;
            }

            _mainScreen.SwitchTo(target);
        }
    }
}