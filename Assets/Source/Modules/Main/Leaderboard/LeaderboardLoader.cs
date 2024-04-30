#pragma warning disable

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

        public void Initialize(Leaderboard leaderboard, Screen leaderboardScreen, Screen mainScreen)
        {
            _leaderboard = leaderboard;
            _leaderboardScreen = leaderboardScreen;
            _mainScreen = mainScreen;
        }

        public void TrySwitch() => TrySwitch(_leaderboard.MinLevelNumber);

        public void TrySwitch(int lastPlayedLevelNumber)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _mainScreen.SwitchTo(_leaderboardScreen);
            Debug.Log(lastPlayedLevelNumber);
            _leaderboard.LevelNumber = lastPlayedLevelNumber;
            return;
#endif

            PlayerAccount.Authorize();

            if (PlayerAccount.IsAuthorized)
                PlayerAccount.RequestPersonalProfileDataPermission();

            if (PlayerAccount.IsAuthorized == false)
                return;

            _mainScreen.SwitchTo(_leaderboardScreen);
            _leaderboard.LevelNumber = lastPlayedLevelNumber;
        }
    }
}