using UnityEngine;
using IJunior.TypedScenes;

public class MenuSceneEntry : MonoBehaviour, ISceneLoadHandler<SceneData>
{
    [SerializeField] private Screen _mainScreen;
    [SerializeField] private Screen _levelsScreen;
    [SerializeField] private Screen _leaderboardScreen;
    [SerializeField] private Root _root;

    private SceneData _sceneData;

    private void Start()
    {
        if (_sceneData != null)
            ChangeScreen(_sceneData.ScreenId);
    }

    public void OnSceneLoaded(SceneData sceneData)
    {
        _sceneData = sceneData;
        _root.SetSceneData(sceneData);
    }

    public void ChangeScreen(ScreenId screenId)
    {
        Screen target = null;

        switch (screenId)
        {
            case ScreenId.Main:
                target = _mainScreen;
                break;
            case ScreenId.Levels:
                target = _levelsScreen;
                break;
            case ScreenId.Leaderboard:
                target = _leaderboardScreen;
                break;
        }

        _mainScreen.Change(target);
    }
}