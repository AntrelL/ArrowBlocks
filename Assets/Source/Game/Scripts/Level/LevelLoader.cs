using UnityEngine.SceneManagement;
using IJunior.ArrowBlocks.Main;
using IJunior.TypedScenes;
using UnityEngine.Events;
using System;

namespace IJunior.ArrowBlocks
{
    public class LevelLoader
    {
        private UnityAction[] _levelActivators;
        private PlayerData _playerData;
        private Tutorial _tutorial;

        public LevelLoader(PlayerData playerData, Tutorial tutorial = null)
        {
            _playerData = playerData;
            _tutorial = tutorial;

            _levelActivators = new UnityAction[]
            {
                () => LoadLevel(LevelTemplate.Load),
                () => LoadLevel(Level2.Load)
            };

            if (_playerData.LevelsData.Count != _levelActivators.Length)
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

        private void LoadLevel(Action<PlayerData, LoadSceneMode> levelActivator)
        {
            if (_tutorial == null)
            {
                LoadLevelRaw(levelActivator);
                return;
            }

            _tutorial.TryShow(_playerData, () =>
            {
                LoadLevelRaw(levelActivator);
            });
        }

        private void LoadLevelRaw(Action<PlayerData, LoadSceneMode> levelActivator)
        {
            levelActivator.Invoke(_playerData, LoadSceneMode.Single);
        }
    }
}