using UnityEngine;
using IJunior.TypedScenes;

public class LevelSceneEntry : MonoBehaviour, ISceneLoadHandler<SceneData>
{
    [SerializeField] private Game _game;

    public SceneData SceneData { get; private set; }

    private void Start()
    {
        _game.SetPlayerData(SceneData.PlayerData);
        _game.RestartFromMenu();
    }

    public void OnSceneLoaded(SceneData sceneData)
    {
        SceneData = sceneData;
    }
}