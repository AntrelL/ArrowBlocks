using UnityEngine;
using IJunior.TypedScenes;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private LevelSceneEntry _levelSceneEntry;
    [SerializeField] private Game _game;

    public void LoadNextLevel()
    {
        if (_game.LevelNumber < _levelSceneEntry.SceneData.PlayerData.Levels.Count)
            new LevelLoaderPortable(_levelSceneEntry.SceneData).LoadLevel(_game.LevelNumber + 1);
    }

    public void LoadLevelsMenu()
    {
        LoadMenu(ScreenId.Levels);
    }

    public void LoadLeaderboardMenu()
    {
        LoadMenu(ScreenId.Leaderboard);
    }

    private void LoadMenu(ScreenId screenId)
    {
        _levelSceneEntry.SceneData.ScreenId = screenId;
        Menu.Load(_levelSceneEntry.SceneData);
    }
}