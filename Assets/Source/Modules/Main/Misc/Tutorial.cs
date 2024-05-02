#pragma warning disable

using IJunior.CompositeRoot;
using Agava.WebUtility;
using UnityEngine;
using Agava.YandexGames;

namespace IJunior.ArrowBlocks.Main
{
    public class Tutorial : Script
    {
        private static bool WasShown = false;

        [SerializeField] private GameObject DesktopText;
        [SerializeField] private GameObject MobileText;

        public void Initialize()
        {
            DesktopText.SetActive(true);
            MobileText.SetActive(true);
        }

        public void FinalInitialize()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            DesktopText.SetActive(true);
            MobileText.SetActive(false);
            return;
#endif

            DesktopText.SetActive(Device.IsMobile == false);
            MobileText.SetActive(Device.IsMobile);
        }

        public bool ShouldBeShown(PlayerData playerData)
        {
            if (WasShown)
                return false;

#if !UNITY_WEBGL || UNITY_EDITOR
            WasShown = true;
            return true;
#endif

            if (playerData.LevelsData[0].State == LevelState.Passed)
                return false;

            WasShown = true;
            return true;
        }
    }
}