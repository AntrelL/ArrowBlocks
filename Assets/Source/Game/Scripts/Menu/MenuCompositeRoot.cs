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

using Screen = IJunior.UI.Screen;

namespace IJunior.ArrowBlocks
{
    internal class MenuCompositeRoot : MonoBehaviour, 
        ISceneLoadHandler<(PlayerData PlayerData, MenuScreenId MenuScreenId)>
    {
        [Header("Screens")]
        [SerializeField] private Screen _main;
        [SerializeField] private Screen _levels;
        [SerializeField] private Screen _leaderboard;
        [Space]
        [Header("User Interface")]
        [SerializeField] private LevelButtonsStorage _levelButtonsStorage;
        [SerializeField] private VersionText _versionText;
        [Space]
        [Header("Animators")]
        [SerializeField] private Rotator _cameraRotator;
        [Space]
        [Header("Menu Control")]
        [SerializeField] private FlowControl _menuFlowControl;

        private PlayerData _playerData;
        private LevelLoader _levelLoader;
        private MenuScreenId _screenIdToSwitch = MenuScreenId.None;
        private UnityAction[] _levelActivators;
        private Button[] _levelButtons;

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

        private IEnumerator Initialize()
        {
            yield return InitializeYandexGamesSdk();

            var rootUpdatebleElements = new List<IRootUpdateble>();

            InitializeScreens();
            _levelButtons = _levelButtonsStorage.Initialize();

            if (_playerData == null)
            {
                _playerData = new PlayerData(_levelButtons.Length);
                yield return _playerData.TryGetFromCloud();
            }

            if (_screenIdToSwitch != MenuScreenId.None)
                SwitchScreenById(_screenIdToSwitch);

            _levelLoader = new LevelLoader(_playerData);
            _levelActivators = new UnityAction[_levelButtons.Length];

            _levelButtonsStorage.UpdateLevelButtons(_playerData.LevelsData);
            _versionText.Initialize();

            _cameraRotator.Initialize();
            rootUpdatebleElements.Add(_cameraRotator);

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
        }

        private void UnsubscribeEvents()
        {
            for (int i = 0; i < _levelButtons.Length; i++)
            {
                _levelButtons[i].onClick.RemoveListener(_levelActivators[i]);
            }
        }

        private void InitializeScreens()
        {
            _main.Initialize();
            _levels.Initialize();
            _leaderboard.Initialize();
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
                    target = _main;
                    break;
                case MenuScreenId.Levels:
                    target = _levels;
                    break;
                case MenuScreenId.Leaderboard:
                    target = _leaderboard;
                    break;
            }

            _main.SwitchTo(target);
        }
    }
}