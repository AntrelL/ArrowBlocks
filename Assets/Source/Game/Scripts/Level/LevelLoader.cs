using System;
using System.Reflection;
using IJunior.ArrowBlocks.Main;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace IJunior.ArrowBlocks
{
    public class LevelLoader
    {
        private readonly UnityAction[] LevelActivators;
        private readonly PlayerData PlayerData;
        private readonly Tutorial Tutorial;

        public LevelLoader(PlayerData playerData, Tutorial tutorial = null)
        {
            PlayerData = playerData;
            Tutorial = tutorial;

            string assemblyName = "IJunior.TypedScenes";
            string levelNameTemplate = "Level";
            string levelLoadMethodName = "Load";

            LevelActivators = new UnityAction[PlayerData.LevelsData.Count];

            for (int i = 0; i < PlayerData.LevelsData.Count; i++)
            {
                Type levelType = Type.GetType($"{assemblyName}.{levelNameTemplate + (i + 1)}");
                dynamic level = Activator.CreateInstance(levelType);

                MethodInfo loadMethodInfo = levelType.GetMethod(levelLoadMethodName);
                Delegate loadDelegate = loadMethodInfo.CreateDelegate(typeof(Action<PlayerData, LoadSceneMode>));

                LevelActivators[i] = () => LoadLevel((Action<PlayerData, LoadSceneMode>)loadDelegate);
            }
        }

        public void LoadLevel(int number)
        {
            LevelActivators[number - 1].Invoke();
        }

        public UnityAction GetLevelActivator(int levelNumber)
        {
            return LevelActivators[levelNumber - 1];
        }

        private void LoadLevel(Action<PlayerData, LoadSceneMode> levelActivator)
        {
            if (Tutorial == null)
            {
                LoadLevelRaw(levelActivator);
                return;
            }

            Tutorial.TryShow(PlayerData, () =>
            {
                LoadLevelRaw(levelActivator);
            });
        }

        private void LoadLevelRaw(Action<PlayerData, LoadSceneMode> levelActivator)
        {
            levelActivator.Invoke(PlayerData, LoadSceneMode.Single);
        }
    }
}