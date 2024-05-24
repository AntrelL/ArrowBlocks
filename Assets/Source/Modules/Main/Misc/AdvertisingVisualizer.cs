#pragma warning disable CS0162

using System;
using Agava.YandexGames;
using IJunior.CompositeRoot;
using UnityEngine;
using UnityEngine.UI;

namespace IJunior.ArrowBlocks.Main
{
    public class AdvertisingVisualizer : Script, IActivatable
    {
        public const int RewardForWatchingVideo = 200;

        private const int MinLevelToDisplayAds = 0;

        private Button _playRewardVideoButton;
        private PlayerData _playerData;
        private BackgroundMusicPlayer _backgroundMusicPlayer;
        private BombSeller _bombSeller;

        public bool IsShowing { get; private set; } = false;

        public void Initialize(
            Button playRewardVideoButton,
            PlayerData playerData,
            BackgroundMusicPlayer backgroundMusicPlayer,
            BombSeller bombSeller)
        {
            _playRewardVideoButton = playRewardVideoButton;
            _playerData = playerData;
            _backgroundMusicPlayer = backgroundMusicPlayer;
            _bombSeller = bombSeller;
        }

        public void OnActivate()
        {
            _playRewardVideoButton.onClick.AddListener(OnPlayRewardVideoButtonClick);
        }

        public void OnDeactivate()
        {
            _playRewardVideoButton.onClick.RemoveListener(OnPlayRewardVideoButtonClick);
        }

        public void ShowAdAfterLevel(int levelNumber, Action<bool> onCloseCallback = null)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            onCloseCallback?.Invoke(false);
            return;
#endif

            if (levelNumber < MinLevelToDisplayAds)
            {
                onCloseCallback?.Invoke(false);
                return;
            }

            InterstitialAd.Show(
                onOpenCallback: OnOpenAdCallback,
                onCloseCallback: (bool wasShown) =>
                {
                    OnCloseAdCallback();
                    onCloseCallback?.Invoke(wasShown);
                });
        }

        private void OnPlayRewardVideoButtonClick()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return;
#endif

            VideoAd.Show(
                onOpenCallback: OnOpenAdCallback,
                onCloseCallback: OnCloseAdCallback,
                onRewardedCallback: () =>
                {
                    _playerData.IncreaseMoney(RewardForWatchingVideo);
                    _bombSeller.UpdateAvailableBombsCount();
                });
        }

        private void OnOpenAdCallback()
        {
            _backgroundMusicPlayer.Pause();
            Time.timeScale = 0;

            IsShowing = true;
        }

        private void OnCloseAdCallback()
        {
            _backgroundMusicPlayer.UnPause();
            Time.timeScale = 1;

            IsShowing = false;
        }
    }
}