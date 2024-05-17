#pragma warning disable CS0162

using Agava.YandexGames;
using IJunior.CompositeRoot;
using IJunior.UI;
using UnityEngine;

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

        public void TrySwitch() => TrySwitch(_leaderboard.MinLevelNumber);

        public void TrySwitch(int lastPlayedLevelNumber)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _authorizationMenu.Open();

            // TrySwitchRaw(lastPlayedLevelNumber); // Debug
            return;
#endif
            if (PlayerAccount.IsAuthorized == false)
            {
                _authorizationMenu.Open(() => TrySwitchRaw(lastPlayedLevelNumber));
                return;
            }

            TrySwitchRaw(lastPlayedLevelNumber);
        }

        private void TrySwitchRaw(int lastPlayedLevelNumber)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Switch(lastPlayedLevelNumber);
            return;
#endif

            if (PlayerAccount.IsAuthorized)
                PlayerAccount.RequestPersonalProfileDataPermission();
            else
                return;

            Switch(lastPlayedLevelNumber);
        }

        private void Switch(int lastPlayedLevelNumber)
        {
            _mainScreen.SwitchTo(_leaderboardScreen);
            _leaderboard.LevelNumber = lastPlayedLevelNumber;
        }
    }
}