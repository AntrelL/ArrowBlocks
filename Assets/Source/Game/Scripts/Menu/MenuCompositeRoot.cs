#pragma warning disable CS0162

using System.Collections;
using System.Collections.Generic;
using Agava.YandexGames;
using IJunior.ArrowBlocks.Main;
using IJunior.CompositeRoot;
using IJunior.TypedScenes;
using IJunior.UI;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Leaderboard = IJunior.ArrowBlocks.Main.Leaderboard;
using Screen = IJunior.UI.Screen;

namespace IJunior.ArrowBlocks
{
    internal class MenuCompositeRoot : MonoBehaviour,
        ISceneLoadHandler<(PlayerData PlayerData, MenuScreenId MenuScreenId, int LevelNumber)>
    {
        private static bool IsGameReadyMethodCalled = false;

        [Header("Screens")]
        [SerializeField] private Screen _mainScreen;
        [SerializeField] private Screen _levelsScreen;
        [SerializeField] private Screen _leaderboardScreen;
        [SerializeField] private Screen _tutorialScreen;
        [SerializeField] private Screen _authorizationScreen;
        [Space]
        [Header("User Interface")]
        [SerializeField] private GameObject _canvas;
        [SerializeField] private LeanLocalization _leanLocalization;
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
        [SerializeField] private LeaderboardLoader _leaderboardLoader;
        [SerializeField] private LeaderboardLine _leaderboardLineTemplate;
        [SerializeField] private TMP_Dropdown _leaderboardLevelNumberInput;
        [SerializeField] private TimeText _leaderboardPlayerTime;
        [SerializeField] private DigitalText _leaderboardPlayerPosition;
        [Space]
        [Header("Additional menus")]
        [SerializeField] private Tutorial _tutorial;
        [SerializeField] private AuthorizationMenu _authorizationMenu;
        [Space]
        [Header("Menu Control")]
        [SerializeField] private FlowControl _menuFlowControl;
        [SerializeField] private BrowserTabFocus _browserTabFocus;

        private PlayerData _playerData;
        private LevelLoader _levelLoader;
        private MenuScreenId _screenIdToSwitch = MenuScreenId.None;
        private UnityAction[] _levelActivators;
        private Button[] _levelButtons;

        private int _lastPlayedLevelNumber;

        private void Awake() => InitializeEarly();

        private void OnDisable() => UnsubscribeEvents();

        private IEnumerator Start()
        {
            yield return Initialize();

            SubscribeEvents();

            OpenSuitableScreen();

            Time.timeScale = 1;
            AudioListener.volume = 1;

            _menuFlowControl.Run();

#if !UNITY_WEBGL || UNITY_EDITOR
            yield break;
#endif

            if (IsGameReadyMethodCalled == false)
            {
                YandexGamesSdk.GameReady();
                Debug.Log("Game Ready");
                IsGameReadyMethodCalled = true;
            }
        }

        public void OnSceneLoaded((PlayerData PlayerData,
            MenuScreenId MenuScreenId, int LevelNumber) sceneTransitionData)
        {
            _playerData = sceneTransitionData.PlayerData;
            _screenIdToSwitch = sceneTransitionData.MenuScreenId;
            _lastPlayedLevelNumber = sceneTransitionData.LevelNumber;
        }

        private void InitializeEarly()
        {
            InitializeScreens();

#if !UNITY_WEBGL || UNITY_EDITOR
            return;
#endif

            YandexGamesSdk.CallbackLogging = true;
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

            _leaderboardLoader.Initialize(_leaderboard, _leaderboardScreen, _mainScreen, _authorizationMenu);
            InitializeLeaderboard(_playerData.LevelsData.Count);

            _tutorial.Initialize(_tutorialScreen, _levelsScreen);
            _authorizationMenu.Initialize(_authorizationScreen);

            Localization.SetLanguage(_leanLocalization);

            FinalInitializeLeaderboard();

            _levelLoader = new LevelLoader(_playerData, _tutorial);
            _levelActivators = new UnityAction[_levelButtons.Length];

            _levelButtonsStorage.UpdateLevelButtons(_playerData.LevelsData);
            _versionText.Initialize();

            _cameraRotator.Initialize();
            rootUpdatebleElements.Add(_cameraRotator);

            _backgroundMusicPlayer = _backgroundMusicPlayer.Initialize(_backgroundMusicVolumeSlider);

            _browserTabFocus.Initialize(_backgroundMusicPlayer);
            _tutorial.FinalInitialize();

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
            _browserTabFocus.OnActivate();
            _tutorial.OnActivate();
            _authorizationMenu.OnActivate();
        }

        private void UnsubscribeEvents()
        {
            for (int i = 0; i < _levelButtons.Length; i++)
            {
                _levelButtons[i].onClick.RemoveListener(_levelActivators[i]);
            }

            _leaderboard.OnDeactivate();
            _backgroundMusicPlayer.OnDeactivate();
            _browserTabFocus.OnDeactivate();
            _tutorial.OnDeactivate();
            _authorizationMenu.OnDeactivate();
        }

        private void InitializeScreens()
        {
            _mainScreen.Initialize();
            _levelsScreen.Initialize();
            _leaderboardScreen.Initialize();
            _tutorialScreen.Initialize();
            _authorizationScreen.Initialize();
        }

        private void InitializeLeaderboard(int numberOfLevels)
        {
            _leaderboard.Initialize(_leaderboardLineTemplate, numberOfLevels);
            _leaderboard.InitializeIOElements(
                _leaderboardLevelNumberInput, _leaderboardPlayerTime, _leaderboardPlayerPosition);
        }

        private void FinalInitializeLeaderboard()
        {
            _leaderboardPlayerTime.Initialize();
            _leaderboardPlayerPosition.Initialize();

            _leaderboard.FinalInitialize();
        }

        private IEnumerator InitializeYandexGamesSdk()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            yield break;
#endif

            yield return YandexGamesSdk.Initialize();
        }

        private void OpenSuitableScreen()
        {
            _mainScreen.Open();

            switch (_screenIdToSwitch)
            {
                case MenuScreenId.Levels:
                    _mainScreen.SwitchTo(_levelsScreen);
                    break;
                case MenuScreenId.Leaderboard:
                    _leaderboardLoader.TrySwitch(_lastPlayedLevelNumber);
                    break;
            }
        }
    }
}