using IJunior.ArrowBlocks.Main;
using IJunior.CompositeRoot;
using IJunior.TypedScenes;

namespace IJunior.ArrowBlocks
{
    public class LevelSceneSwitcher : Script
    {
        private LevelLoader _levelLoader;
        private PlayerData _playerData;
        private int _levelNumber;
        private AdvertisingVisualizer _advertisingVisualizer;

        public void Initialize(LevelLoader levelLoader, PlayerData playerData,
            int levelNumber, AdvertisingVisualizer advertisingVisualizer)
        {
            _levelLoader = levelLoader;
            _playerData = playerData;
            _levelNumber = levelNumber;
            _advertisingVisualizer = advertisingVisualizer;
        }

        public void LoadNextLevel()
        {
            if (_levelNumber == _playerData.LevelsData.Count)
                return;

            _advertisingVisualizer.TryShowAdAfterLevel(_levelNumber, () =>
            {
                _levelLoader.LoadLevel(_levelNumber + 1);
            });
        }

        public void LoadLevelsMenu() => LoadMenu(MenuScreenId.Levels);

        public void LoadLeaderboardMenu() => LoadMenu(MenuScreenId.Leaderboard);

        private void LoadMenu(MenuScreenId menuScreenId) => Menu.Load((_playerData, menuScreenId));
    }
}