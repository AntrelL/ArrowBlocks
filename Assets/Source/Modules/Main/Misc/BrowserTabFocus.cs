using IJunior.CompositeRoot;
using Agava.WebUtility;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public class BrowserTabFocus : Script, IActivatable
    {
        private BackgroundMusicPlayer _backgroundMusicPlayer;
        private AdvertisingVisualizer _advertisingVisualizer;

        public void Initialize(BackgroundMusicPlayer backgroundMusicPlayer,
            AdvertisingVisualizer advertisingVisualizer = null)
        {
            _backgroundMusicPlayer = backgroundMusicPlayer;
            _advertisingVisualizer = advertisingVisualizer;
        }

        public void OnActivate()
        {
            Application.focusChanged += OnInBackgroundChangeApp;
            WebApplication.InBackgroundChangeEvent += OnInBackgroundChangeWeb;
        }

        public void OnDeactivate()
        {
            Application.focusChanged -= OnInBackgroundChangeApp;
            WebApplication.InBackgroundChangeEvent -= OnInBackgroundChangeWeb;
        }

        private void OnInBackgroundChangeApp(bool inApp)
        {
            OnInBackgroundChangeWeb(inApp == false);
        }

        private void OnInBackgroundChangeWeb(bool isBackground)
        {
            if (_advertisingVisualizer != null)
                isBackground = isBackground || _advertisingVisualizer.IsShowing;

            MuteAudio(isBackground);
            PauseGame(isBackground);
        }

        private void MuteAudio(bool value)
        {
            if (value)
                _backgroundMusicPlayer.Pause();
            else
                _backgroundMusicPlayer.UnPause();
        }

        private void PauseGame(bool value)
        {
            Time.timeScale = value ? 0 : 1;
        }
    }
}