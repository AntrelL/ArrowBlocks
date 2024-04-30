#pragma warning disable

using System.Collections.Generic;
using IJunior.ArrowBlocks.Main;
using IJunior.CompositeRoot;
using IJunior.TypedScenes;
using UnityEngine.Events;
using System.Collections;
using Agava.YandexGames;
using Lean.Localization;
using UnityEngine.UI;
using UnityEngine;
using IJunior.UI;
using TMPro;

using Screen = IJunior.UI.Screen;
using Leaderboard = IJunior.ArrowBlocks.Main.Leaderboard;


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

            _mainScreen.Open();

            if (_screenIdToSwitch == MenuScreenId.Leaderboard)
                _leaderboardLoader.TrySwitch(_lastPlayedLevelNumber); 

            Time.timeScale = 1;
            AudioListener.volume = 1;

            _menuFlowControl.Run();

#if UNITY_WEBGL && !UNITY_EDITOR
            if (IsGameReadyMethodCalled == false)
            {
                YandexGamesSdk.GameReady();
                IsGameReadyMethodCalled = true;
            }
#endif
            yield return null;
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

            _leaderboardLoader.Initialize(_leaderboard, _leaderboardScreen, _mainScreen);
            InitializeLeaderboard(_playerData.LevelsData.Count);

            Localization.SetLanguage(_leanLocalization);

            FinalInitializeLeaderboard();

            _levelLoader = new LevelLoader(_playerData);
            _levelActivators = new UnityAction[_levelButtons.Length];

            _levelButtonsStorage.UpdateLevelButtons(_playerData.LevelsData);
            _versionText.Initialize();

            _cameraRotator.Initialize();
            rootUpdatebleElements.Add(_cameraRotator);

            _backgroundMusicPlayer = _backgroundMusicPlayer.Initialize(_backgroundMusicVolumeSlider);

            _browserTabFocus.Initialize(_backgroundMusicPlayer);

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
        }

        private void InitializeScreens()
        {
            _mainScreen.Initialize();
            _levelsScreen.Initialize();
            _leaderboardScreen.Initialize();
        }

        private void InitializeLeaderboard(int numberOfLevels)
        {
            _leaderboard.Initialize(_leaderboardLineTemplate, numberOfLevels);
            _leaderboard.InitializeIOElements(_leaderboardLevelNumberInput,
                _leaderboardPlayerTime, _leaderboardPlayerPosition);
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