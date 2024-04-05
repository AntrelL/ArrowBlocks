using IJunior.ArrowBlocks.Main;
using IJunior.TypedScenes;
using UnityEngine.Events;
using System;

namespace IJunior.ArrowBlocks
{
    public class LevelLoader
    {
        private UnityAction[] _levelActivators;

        public LevelLoader(PlayerData playerData)
        {
            _levelActivators = new UnityAction[]
            {
                () => LevelTemplate.Load(playerData),
                () => Level2.Load(playerData)
            };

            if (playerData.LevelsData.Count != _levelActivators.Length)
                throw new Exception("The number of levels and their activators does not match.");
        }

        public void LoadLevel(int number)
        {
            _levelActivators[number - 1].Invoke();
        }

        public UnityAction GetLevelActivator(int levelNumber)
        {
            return _levelActivators[levelNumber - 1];
        }
    }
}