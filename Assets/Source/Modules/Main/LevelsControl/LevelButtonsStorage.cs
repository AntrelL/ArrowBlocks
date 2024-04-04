using System.Collections.Generic;
using IJunior.CompositeRoot;
using UnityEngine.UI;
using System.Linq;
using System;

namespace IJunior.ArrowBlocks.Main
{
    public class LevelButtonsStorage : Script
    {
        private LevelButton[] _levelButtons;

        public Button[] Initialize()
        {
            _levelButtons = GetComponentsInChildren<LevelButton>();

            foreach (var levelButton in _levelButtons)
            {
                levelButton.Initialize();
            }

            return _levelButtons.Select(LevelButton => LevelButton.Button).ToArray();
        }

        public void UpdateLevelButtons(IReadOnlyList<IReadOnlyLevelData> levelsData)
        {
            if (levelsData.Count != _levelButtons.Length)
                throw new Exception("The number of levels and buttons for them do not match.");

            for (int i = 0; i < levelsData.Count; i++)
            {
                _levelButtons[i].UpdateState(levelsData[i].State);
            }
        }
    }
}