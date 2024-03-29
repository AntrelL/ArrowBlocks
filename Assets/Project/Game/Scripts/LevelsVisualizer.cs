using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelsVisualizer : MonoBehaviour
{
    private LevelIndicator[] _levelIndicators;

    private void Awake()
    {
        _levelIndicators = GetComponentsInChildren<LevelIndicator>();
    }

    public void UpdateLevelsIndicators(IReadOnlyList<IReadOnlyLevel> levels)
    {
        if (levels.Count != _levelIndicators.Length)
            throw new Exception("The number of levels and indicators for them do not match.");

        for (int i = 0; i < levels.Count; i++)
        {
            _levelIndicators[i].UpdateState(levels[i].State);
        }
    }
}