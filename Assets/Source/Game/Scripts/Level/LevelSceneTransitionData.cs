using IJunior.ArrowBlocks.Main;

namespace IJunior.ArrowBlocks
{
    public class LevelSceneTransitionData
    {
        public LevelSceneTransitionData(PlayerData playerData, BackgroundMusicPlayer backgroundMusicPlayer)
        {
            PlayerData = playerData;
            BackgroundMusicPlayer = backgroundMusicPlayer;
        }

        public PlayerData PlayerData { get; set; }

        public BackgroundMusicPlayer BackgroundMusicPlayer { get; set; }
    }
}