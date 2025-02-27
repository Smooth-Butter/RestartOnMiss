using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RestartOnMiss.Configuration;

namespace RestartOnMiss.Views
{
    public class SettingsUI : BSMLResourceViewController, INotifyPropertyChanged
    {
        public override string ResourceName => "RestartOnMiss.UI.SettingsUI.bsml";

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
        
        [UIValue("enabledinreplay")]
        public bool EnabledInReplay
        {
            get => PluginConfig.Instance.EnableInReplay;
            set
            {
                if (PluginConfig.Instance.EnableInReplay != value)
                {
                    PluginConfig.Instance.EnableInReplay = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        [UIValue("enabledinfpfc")]
        public bool EnabledInFPFC
        {
            get => PluginConfig.Instance.EnableInFPFC;
            set
            {
                if (PluginConfig.Instance.EnableInFPFC != value)
                {
                    PluginConfig.Instance.EnableInFPFC = value;
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

        // Implement INotifyPropertyChanged to notify BSML of changes
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}