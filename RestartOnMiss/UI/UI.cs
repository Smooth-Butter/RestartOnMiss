using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RestartOnMiss.Configuration;

namespace RestartOnMiss.UI
{
    public class ModifierUI : BSMLResourceViewController, INotifyPropertyChanged
    {
        public override string ResourceName => "RestartOnMiss.UI.ModifiersUI.bsml";

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

        // Implement INotifyPropertyChanged to notify BSML of changes
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}