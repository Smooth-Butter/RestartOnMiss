using BeatSaberMarkupLanguage.Settings;
using RestartOnMiss.Views;
using RestartOnMiss.Configuration;

namespace RestartOnMiss.Stuff
{
    public partial class StuffUtils
    {
        public static void OnMainMenuInit()
        {
            BSMLSettings.instance.AddSettingsMenu("RestartOnMiss", "RestartOnMiss.Views.SettingsUI.bsml", new SettingsUI());
            Plugin.Log.Debug("RestartOnMiss: BSML settings menu registered.");
        }
    }
}