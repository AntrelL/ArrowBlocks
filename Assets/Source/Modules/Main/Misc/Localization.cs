using Agava.YandexGames;
using Lean.Localization;

namespace IJunior.ArrowBlocks.Main
{
    public static class Localization
    {
        public static void SetLanguage(LeanLocalization leanLocalization)
        {
            string languageCode = string.Empty;

#if !UNITY_WEBGL || UNITY_EDITOR
            languageCode = "en";
#else
            languageCode = YandexGamesSdk.Environment.i18n.lang;
#endif

            string language = LeanLocalization.CurrentAliases[languageCode];
            leanLocalization.SetCurrentLanguage(language);
        }
    }
}