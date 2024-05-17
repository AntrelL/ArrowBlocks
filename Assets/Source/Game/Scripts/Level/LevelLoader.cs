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
        private readonly BackgroundMusicPlayer BackgroundMusicPlayer;
        private readonly Tutorial Tutorial;

        public LevelLoader(
            PlayerData playerData,
            BackgroundMusicPlayer backgroundMusicPlayer,
            Tutorial tutorial = null)
        {
            PlayerData = playerData;
            BackgroundMusicPlayer = backgroundMusicPlayer;
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
                Type delegateType = typeof(Action<LevelSceneTransitionData, LoadSceneMode>);
                Delegate loadDelegate = loadMethodInfo.CreateDelegate(delegateType);

                LevelActivators[i] = () => LoadLevel((Action<LevelSceneTransitionData, LoadSceneMode>)loadDelegate);
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

        private void LoadLevel(Action<LevelSceneTransitionData, LoadSceneMode> levelActivator)
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

        private void LoadLevelRaw(Action<LevelSceneTransitionData, LoadSceneMode> levelActivator)
        {
            var levelSceneTransitionData = new LevelSceneTransitionData(PlayerData, BackgroundMusicPlayer);
            levelActivator.Invoke(levelSceneTransitionData, LoadSceneMode.Single);
        }
    }
}