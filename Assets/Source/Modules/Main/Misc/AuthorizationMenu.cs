using System;
using Agava.YandexGames;
using IJunior.CompositeRoot;
using UnityEngine;
using UnityEngine.UI;

using Screen = IJunior.UI.Screen;

namespace IJunior.ArrowBlocks.Main
{
    public class AuthorizationMenu : Script, IActivatable
    {
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private GameObject _leaderboardButton;

        private Screen _authorizationScreen;

        private Action _acceptCallback;
        private Action _cancelCallback;

        public void Initialize(Screen authorizationScreen)
        {
            _authorizationScreen = authorizationScreen;
        }

        public void OnActivate()
        {
            _acceptButton.onClick.AddListener(OnAcceptButtonClick);
            _cancelButton.onClick.AddListener(OnCancelButtonClick);
            _playButton.onClick.AddListener(OnPlayButtonClick);
        }

        public void OnDeactivate()
        {
            _acceptButton.onClick.RemoveListener(OnAcceptButtonClick);
            _cancelButton.onClick.RemoveListener(OnCancelButtonClick);
            _playButton.onClick.RemoveListener(OnPlayButtonClick);
        }

        public void Open(Action acceptCallback = null, Action cancelCallback = null)
        {
            _acceptCallback = acceptCallback;
            _cancelCallback = cancelCallback;

            _leaderboardButton.SetActive(false);
            _authorizationScreen.Open();
        }

        private void OnAcceptButtonClick()
        {
            Close();

            PlayerAccount.Authorize(() =>
            {
                _acceptCallback?.Invoke();
                _acceptCallback = null;
            });
        }

        private void OnCancelButtonClick()
        {
            Close();

            _cancelCallback?.Invoke();
            _cancelCallback = null;
        }

        private void OnPlayButtonClick() => OnCancelButtonClick();

        private void Close()
        {
            _leaderboardButton.SetActive(true);
            _authorizationScreen.Close();
        }
    }
}