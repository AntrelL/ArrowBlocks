#pragma warning disable

using IJunior.CompositeRoot;
using Agava.WebUtility;
using UnityEngine;
using Agava.YandexGames;
using UnityEngine.UI;
using System;

using Screen = IJunior.UI.Screen;

namespace IJunior.ArrowBlocks.Main
{
    public class Tutorial : Script, IActivatable
    {
        private static bool WasShown = false;

        [SerializeField] private GameObject _desktopText;
        [SerializeField] private GameObject _mobileText;
        [SerializeField] private Button _closeScreenButton;

        private Screen _tutorialScreen;
        private Screen _levelsScreen;
        private Action _endCallback;

        public void Initialize(Screen tutorialScreen, Screen levelsScreen)
        {
            _tutorialScreen = tutorialScreen;
            _levelsScreen = levelsScreen;

            _desktopText.SetActive(true);
            _mobileText.SetActive(true);
        }

        public void FinalInitialize()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _desktopText.SetActive(true);
            _mobileText.SetActive(false);
            return;
#endif

            _desktopText.SetActive(Device.IsMobile == false);
            _mobileText.SetActive(Device.IsMobile);
        }

        public void OnActivate()
        {
            _closeScreenButton.onClick.AddListener(OnCloseScreenButtonClick);
        }

        public void OnDeactivate()
        {
            _closeScreenButton.onClick.RemoveListener(OnCloseScreenButtonClick);
        }

        public void TryShow(PlayerData playerData, Action endCallback = null)
        {
            if (ShouldBeShown(playerData) == false)
            {
                endCallback?.Invoke();
                return;
            }

            _levelsScreen.SwitchTo(_tutorialScreen);
            _endCallback = endCallback;
        }

        private bool ShouldBeShown(PlayerData playerData)
        {
            if (WasShown)
                return false;

#if !UNITY_WEBGL || UNITY_EDITOR
            WasShown = true;
            return true;
#endif

            if (playerData.LevelsData[0].State == LevelState.Passed)
                return false;

            WasShown = true;
            return true;
        }

        private void OnCloseScreenButtonClick()
        {            
            _endCallback?.Invoke();
            _endCallback = null;
        }
    }
}