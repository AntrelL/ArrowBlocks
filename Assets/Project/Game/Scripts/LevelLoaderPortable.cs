using IJunior.TypedScenes;
using System;

public class LevelLoaderPortable
{
    private Action[] _levelsActivators;

    public LevelLoaderPortable(SceneData sceneData)
    {
        InitializeLevelsActivators(sceneData);
    }

    public void LoadLevel(int number)
    {
        _levelsActivators[--number].Invoke();
    }

    private void InitializeLevelsActivators(SceneData sceneData)
    {
        _levelsActivators = new Action[]
        {
            () => TestLevel.Load(sceneData),
            () => TestLevel2.Load(sceneData),
        };
    }
}
