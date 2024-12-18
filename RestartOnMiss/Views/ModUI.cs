using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.ViewControllers;
using RestartOnMiss.Configuration;
using Zenject;


namespace RestartOnMiss.Views
{
    public class ModUI : BSMLAutomaticViewController, IInitializable, IDisposable
    {
        public string ResourceName => "RestartOnMiss.UI.ModifiersUI.bsml";

        [UIValue("enabled")]
        public bool Enabled
        {
            get => PluginConfig.Instance.Enabled;
            set
            {
                if (PluginConfig.Instance.Enabled != value)
                {
                    PluginConfig.Instance.Enabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [UIValue("maxMiss")]
        public int maxMisses
        {
            get => PluginConfig.Instance.maxMisses;
            set
            {
                if (PluginConfig.Instance.maxMisses != value)
                {
                    PluginConfig.Instance.maxMisses = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        public void Initialize()
        {
            GameplaySetup.Instance.AddTab("RestartOnMiss", "RestartOnMiss.Views.ModifiersUI.bsml", this);
        }
        
        public void Dispose()
        {
            GameplaySetup.Instance.AddTab("RestartOnMiss", "RestartOnMiss.Views.ModifiersUI.bsml", this);
        }
    }
}
