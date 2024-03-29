using UnityEngine;

public class Root : MonoBehaviour
{
    [SerializeField] private LevelLoader _levelLoader;
    [SerializeField] private LevelsVisualizer _levelsVisualizer;

    private SceneData _sceneData;

    private void Awake()
    {
        bool isSceneDataNull = _sceneData == null;

        if (isSceneDataNull)
        {
            PlayerData playerData = new PlayerData(150, 2);
            _sceneData = new SceneData(ScreenId.Main, playerData);
        }

        _levelLoader.Initialize(_sceneData);
    }

    private void Start()
    {
        _levelsVisualizer.UpdateLevelsIndicators(_sceneData.PlayerData.Levels);
    }

    public void SetSceneData(SceneData sceneData)
    {
        _sceneData = sceneData;
    }
}