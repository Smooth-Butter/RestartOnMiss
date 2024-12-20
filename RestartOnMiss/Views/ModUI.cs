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
        
        [UIValue("countbombs")]
        public bool CountBombs
        {
            get => PluginConfig.Instance.CountBombs;
            set
            {
                if (PluginConfig.Instance.CountBombs != value)
                {
                    PluginConfig.Instance.CountBombs = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        [UIValue("countbadcuts")]
        public bool CountBadCuts
        {
            get => PluginConfig.Instance.CountBadCuts;
            set
            {
                if (PluginConfig.Instance.CountBadCuts != value)
                {
                    PluginConfig.Instance.CountBadCuts = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [UIValue("maxMiss")]
        public int MaxMisses
        {
            get => PluginConfig.Instance.MaxMisses;
            set
            {
                if (PluginConfig.Instance.MaxMisses != value)
                {
                    PluginConfig.Instance.MaxMisses = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        public void Initialize()
        {
            GameplaySetup.instance.AddTab("RestartOnMiss", "RestartOnMiss.Views.ModifiersUI.bsml", this);
        }
        
        public void Dispose()
        {
            GameplaySetup.instance.AddTab("RestartOnMiss", "RestartOnMiss.Views.ModifiersUI.bsml", this);
        }
    }
}
