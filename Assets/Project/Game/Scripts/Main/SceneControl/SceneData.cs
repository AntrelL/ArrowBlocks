using UnityEngine;

public class SceneData
{
    public SceneData(ScreenId baseScreenId, PlayerData playerData)
    {
        ScreenId = baseScreenId;
        PlayerData = playerData;
    }

    public ScreenId ScreenId { get; set; }
    public PlayerData PlayerData { get; private set; }

}