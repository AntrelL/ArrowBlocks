using System;
using UnityEngine;
using IJunior.TypedScenes;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Screen _levelsScreen;

    private SceneData _sceneData;
    private Action[] _levelsActivators;

    public Action[] LevelsActivators => _levelsActivators;

    public void Initialize(SceneData sceneData)
    {
        _sceneData = sceneData;
        InitializeLevelsActivators();
    }

    public void LoadLevel(int number)
    {
        _sceneData.ScreenId = ScreenId.Levels;
        _levelsActivators[--number].Invoke();
    }

    private void InitializeLevelsActivators()
    {
        _levelsActivators = new Action[]
        {
            () => TestLevel.Load(_sceneData),
            () => TestLevel2.Load(_sceneData),
        };
    }
}