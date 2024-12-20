using BeatSaberMarkupLanguage.Settings;
using RestartOnMiss.Views;
using RestartOnMiss.Configuration;

namespace RestartOnMiss.Stuff
{
    public partial class StuffUtils
    {
        public static void BSMLUtilsAdd()
        {
            BeatSaberMarkupLanguage.Util.MainMenuAwaiter.MainMenuInitializing += OnMainMenuInit;
        }
        
        public static void OnMainMenuInit()
        {
            BSMLSettings.Instance.AddSettingsMenu("RestartOnMiss", "RestartOnMiss.Views.SettingsUI.bsml", new SettingsUI());
            Plugin.Log.Debug("RestartOnMiss: BSML settings menu registered.");
        }

        public static void BSMLUtilsRemove()
        {
            BeatSaberMarkupLanguage.Util.MainMenuAwaiter.MainMenuInitializing -= OnMainMenuInit;
            BSMLSettings.Instance.RemoveSettingsMenu(PluginConfig.Instance);
        }
    }
}