#pragma warning disable CS0162

using System;
using Agava.YandexGames;
using IJunior.CompositeRoot;

using Screen = IJunior.UI.Screen;

namespace IJunior.ArrowBlocks.Main
{
    public class LeaderboardLoader : Script
    {
        private Leaderboard _leaderboard;
        private Screen _leaderboardScreen;
        private Screen _mainScreen;
        private AuthorizationMenu _authorizationMenu;

        public void Initialize(
            Leaderboard leaderboard,
            Screen leaderboardScreen,
            Screen mainScreen,
            AuthorizationMenu authorizationMenu)
        {
            _leaderboard = leaderboard;
            _leaderboardScreen = leaderboardScreen;
            _mainScreen = mainScreen;
            _authorizationMenu = authorizationMenu;
        }

        public void Switch(Action<bool> endCallback = null) =>
            Switch(_leaderboard.MinLevelNumber, endCallback);

        public void Switch(int lastPlayedLevelNumber, Action<bool> endCallback = null)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _authorizationMenu.Open();
            endCallback?.Invoke(false);
            return;
#endif
            Action tryingToSwitchWithCallback = () => endCallback?.Invoke(TrySwitchRaw(lastPlayedLevelNumber));

            if (PlayerAccount.IsAuthorized == false)
            {
                _authorizationMenu.Open(() => tryingToSwitchWithCallback.Invoke());
                return;
            }

            tryingToSwitchWithCallback.Invoke();
        }

        private bool TrySwitchRaw(int lastPlayedLevelNumber)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Switch(lastPlayedLevelNumber);
            return true;
#endif

            if (PlayerAccount.IsAuthorized)
                PlayerAccount.RequestPersonalProfileDataPermission();
            else
                return false;

            Switch(lastPlayedLevelNumber);
            return true;
        }

        private void Switch(int lastPlayedLevelNumber)
        {
            _mainScreen.SwitchTo(_leaderboardScreen);
            _leaderboard.SetLevelNumber(lastPlayedLevelNumber);
        }
    }
}