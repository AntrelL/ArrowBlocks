using UnityEngine.SceneManagement;
using IJunior.ArrowBlocks.Main;
using IJunior.TypedScenes;
using UnityEngine.Events;
using System;
using System.Reflection;

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

            string assemblyName = "IJunior.TypedScenes";
            string levelNameTemplate = "Level";
            string levelLoadMethodName = "Load";

            _levelActivators = new UnityAction[_playerData.LevelsData.Count];

            for (int i = 0; i < _playerData.LevelsData.Count; i++)
            {
                Type levelType = Type.GetType($"{assemblyName}.{levelNameTemplate + (i + 1)}");
                dynamic level = Activator.CreateInstance(levelType);

                MethodInfo loadMethodInfo = levelType.GetMethod(levelLoadMethodName);
                Delegate loadDelegate = loadMethodInfo.CreateDelegate(typeof(Action<PlayerData, LoadSceneMode>));

                _levelActivators[i] = () => LoadLevel((Action<PlayerData, LoadSceneMode>)loadDelegate);
            }
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