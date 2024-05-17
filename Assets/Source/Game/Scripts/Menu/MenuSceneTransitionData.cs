using IJunior.ArrowBlocks.Main;

namespace IJunior.ArrowBlocks
{
    public class MenuSceneTransitionData
    {
        public MenuSceneTransitionData(
            PlayerData playerData,
            MenuScreenId menuScreenId,
            int levelNumber,
            BackgroundMusicPlayer backgroundMusicPlayer)
        {
            PlayerData = playerData;
            MenuScreenId = menuScreenId;
            LevelNumber = levelNumber;
            BackgroundMusicPlayer = backgroundMusicPlayer;
        }

        public PlayerData PlayerData { get; set; }

        public MenuScreenId MenuScreenId { get; set; }

        public int LevelNumber { get; set; }

        public BackgroundMusicPlayer BackgroundMusicPlayer { get; set; }
    }
}